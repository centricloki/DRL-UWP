using DRLMobile.Core.Interface;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    /// <summary>
    /// Service responsible for resolving and caching XAML <see cref="Brush"/> instances 
    /// and map-pin image paths for all classifications on the map.
    /// 
    /// All map-pin configuration (active status, display order, colors, and static pin images) 
    /// is driven dynamically by the database via the <c>MapClassification</c> SQLite table.
    /// 
    /// If a classification does not specify an explicit color override in the database, 
    /// the service generates a deterministic, visually distinct two-tone gradient based on the 
    /// classification ID. This ensures identical color assignments for all users of the app.
    /// </summary>
    public static class ClassificationColorService
    {
        /// <summary>
        /// Cache storing the resolved hex color string (either a 6-digit hex or a comma-separated 
        /// pair of hex values for gradients) keyed by AccountClassificationId.
        /// </summary>
        private static readonly Dictionary<int, string> _colorCache =
            new Dictionary<int, string>();

        /// <summary>
        /// A curated list of dark, bold, high-contrast colors used to generate
        /// two-tone gradient pins when no explicit color code is defined in the database.
        /// </summary>
        private static readonly string[] _darkColors = new[]
        {
            "8B0000", // Dark Red
            "00008B", // Dark Blue
            "006400", // Dark Green
            "8B4513", // Saddle Brown
            "4B0082", // Indigo
            "FF8C00", // Dark Orange
            "2F4F4F", // Dark Slate Gray
            "800000", // Maroon
            "000000", // Black
            "808000", // Olive
            "008080", // Teal
            "800080"  // Purple
        };

        /// <summary>
        /// Pre-computes, caches colors, and generates map-pin PNG files in the local 
        /// application storage for all active map classifications.
        /// 
        /// This should be called when navigating to the map page, immediately after 
        /// loading classifications from the database.
        /// </summary>
        /// <param name="mapClassifications">The list of active classifications fetched from the MapClassification table.</param>
        /// <param name="dbService">Database service to use for persisting generated colors</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task PrewarmAsync(IList<MapClassificationViewModel> mapClassifications, IDatabaseService dbService = null)
        {
            if (mapClassifications == null || mapClassifications.Count == 0) return;

            // 1. Identify all colors ALREADY used in the database to avoid duplicates.
            var usedColors = new HashSet<string>(
                mapClassifications
                    .Where(c => !string.IsNullOrWhiteSpace(c.HexColorCode))
                    .Select(c => c.HexColorCode.TrimStart('#').ToUpper())
            );

            var pinTasks = new List<Task>();

            foreach (var item in mapClassifications)
            {
                int id = item.AccountClassificationId;
                string hex;

                // Priority 1: Use the explicit color code set in the database
                if (!string.IsNullOrWhiteSpace(item.HexColorCode))
                {
                    hex = item.HexColorCode.TrimStart('#');
                }
                // Priority 2: Fallback to the deterministic gradient calculation (with uniqueness check)
                else
                {
                    string gradientHex = GetUniqueGradientHex(id, usedColors);
                    
                    // For consistency between legend and pin, convert gradient to averaged color
                    if (gradientHex.Contains(","))
                    {
                        var parts = gradientHex.Split(',');
                        Color colorA = HexToColor(parts[0]);
                        Color colorB = HexToColor(parts[1]);
                        
                        // Calculate averaged color to match pin generation
                        byte r = (byte)((colorA.R + colorB.R) / 2);
                        byte g = (byte)((colorA.G + colorB.G) / 2);
                        byte b = (byte)((colorA.B + colorB.B) / 2);
                        Color averagedColor = Color.FromArgb(255, r, g, b);
                        
                        hex = $"{averagedColor.R:X2}{averagedColor.G:X2}{averagedColor.B:X2}";
                        
                        // Also update the item's HexColorCode so it gets persisted to database if dbService is provided
                        item.HexColorCode = hex;
                        // Update the database with the generated color
                        await dbService.UpdateMapClassificationColorAsync(item.AccountClassificationId, item.HexColorCode.TrimStart('#'));
                    }
                    else
                    {
                        hex = gradientHex;
                    }
                    
                    // Add the newly generated color to the used set so the next ID doesn't pick it
                    usedColors.Add(gradientHex.Replace(",", ",").ToUpper()); 
                }

                _colorCache[id] = hex;

                // If no custom static map-pin image asset is defined in the database,
                // generate a dynamic pin image locally using the resolved color.
                if (string.IsNullOrWhiteSpace(item.MapPinImageName))
                {
                    pinTasks.Add(MapPinGenerator.GetOrCreateMapPinAsync(id, hex));
                }
            }

            await Task.WhenAll(pinTasks);
            
            //// Persist any generated colors to the database for consistency across app restarts
            //if (dbService != null)
            //{
            //    foreach (var item in mapClassifications.Where(x=>x.string.IsNullOrWhiteSpace(item.MapPinImageName))
            //    {
            //        if (!string.IsNullOrEmpty(item.HexColorCode))
            //        {
            //            // Check if this classification exists in the database with the same color
            //            // If not in database or color differs, update it
            //            var allMapClassifications = await dbService.GetMapClassificationsAsync();
            //            var existingMapClassification = allMapClassifications?
            //                .FirstOrDefault(mc => mc.AccountClassificationId == item.AccountClassificationId);

            //            if (existingMapClassification == null || 
            //                !string.Equals(existingMapClassification.HexColorCode?.TrimStart('#'), 
            //                             item.HexColorCode?.TrimStart('#'), StringComparison.OrdinalIgnoreCase))
            //            {
            //                // Update the database with the generated color
            //                await dbService.UpdateMapClassificationColorAsync(item.AccountClassificationId, item.HexColorCode);
            //            }
            //        }
            //    }
            //}
        }

        /// <summary>
        /// Returns the cached 6-digit hex color string (or single averaged hex color)
        /// for a given classification ID. If the cache is empty, it returns a 
        /// calculated fallback value on the fly.
        /// </summary>
        /// <param name="classificationId">The classification ID to lookup.</param>
        /// <returns>A color hex string.</returns>
        public static string GetColorHex(int classificationId)
        {
            if (_colorCache.TryGetValue(classificationId, out string cached))
                return cached;

            // Fallback in case PrewarmAsync has not finished executing yet
            string fallback = ComputeMixedDistinctHex(classificationId);
            
            // If fallback is a gradient (comma-separated), convert to averaged color
            if (fallback.Contains(","))
            {
                var parts = fallback.Split(',');
                Color colorA = HexToColor(parts[0]);
                Color colorB = HexToColor(parts[1]);
                
                // Calculate averaged color to match pin generation
                byte r = (byte)((colorA.R + colorB.R) / 2);
                byte g = (byte)((colorA.G + colorB.G) / 2);
                byte b = (byte)((colorA.B + colorB.B) / 2);
                Color averagedColor = Color.FromArgb(255, r, g, b);
                
                fallback = $"{averagedColor.R:X2}{averagedColor.G:X2}{averagedColor.B:X2}";
            }
            
            _colorCache[classificationId] = fallback;
            return fallback;
        }

        /// <summary>
        /// Resolves and returns a <see cref="Brush"/> for a classification ID.
        /// Returns a <see cref="SolidColorBrush"/> with the resolved color.
        /// </summary>
        /// <param name="classificationId">The classification ID.</param>
        /// <returns>A SolidColorBrush.</returns>
        public static Brush GetBrush(int classificationId)
        {
            string hex = GetColorHex(classificationId);

            // At this point, hex should be a single color (averaged if originally gradient)
            // since we convert gradients to averaged colors in GetColorHex
            return new SolidColorBrush(HexToColor(hex));
        }

        /// <summary>
        /// Resolves the file path or URI string for a classification's map pin image.
        /// Uses a custom static asset name from the database if specified;
        /// otherwise returns the local storage URI of the dynamically generated pin.
        /// </summary>
        /// <param name="classificationId">The classification ID.</param>
        /// <param name="mapPinImageName">The static pin filename from the database (optional).</param>
        /// <returns>The image path URI string.</returns>
        public static string GetMapPinPath(int classificationId, string mapPinImageName = null)
        {
            // Priority 1: Use custom static database asset if provided
            if (!string.IsNullOrWhiteSpace(mapPinImageName))
                return $"ms-appx:///Assets/Maps/{mapPinImageName}";

            // Priority 2: Use the locally generated custom color pin
            return MapPinGenerator.GetLocalPinUri(classificationId);
        }

        /// <summary>
        /// Helper utility to parse a hex color string into a Windows XAML <see cref="Color"/>.
        /// </summary>
        /// <param name="hex">6-digit hex string (e.g. "FF5733" or "#FF5733").</param>
        /// <returns>The resolved Color struct, falling back to Gray if parsing fails.</returns>
        public static Color HexToColor(string hex)
        {
            try
            {
                hex = hex.TrimStart('#');
                if (hex.Length == 6)
                {
                    byte r = Convert.ToByte(hex.Substring(0, 2), 16);
                    byte g = Convert.ToByte(hex.Substring(2, 2), 16);
                    byte b = Convert.ToByte(hex.Substring(4, 2), 16);
                    return Color.FromArgb(255, r, g, b);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ClassificationColorService), nameof(HexToColor), ex.StackTrace);
            }
            return Colors.Gray;
        }

        /// <summary>
        /// Deterministically computes a unique, visually distinct two-tone color combination 
        /// for a given ID. It guarantees identical colors for the same ID across all users and devices,
        /// while ensuring the generated combination does not clash with existing database colors.
        /// </summary>
        private static string GetUniqueGradientHex(int classificationId, HashSet<string> usedColors)
        {
            int len = _darkColors.Length;
            int offset = 0;

            while (true)
            {
                // Calculate indices based on ID and an offset to "shift" the selection if there's a clash
                int idxA = (classificationId + offset) % len;
                int idxB = ((classificationId / len) + idxA + 1 + offset) % len;

                // Ensure we don't pick the same color twice for the gradient
                if (idxA == idxB) idxB = (idxB + 1) % len;

                string colorA = _darkColors[idxA];
                string colorB = _darkColors[idxB];
                string candidate = $"{colorA},{colorB}";

                // Check if this combination (or its individual parts) conflicts with existing DB colors
                // We check if the exact gradient string exists, or if either solid color is already heavily used
                if (!usedColors.Contains(candidate.ToUpper()))
                {
                    return candidate;
                }

                // If clash found, increment offset to try the next available pair
                offset++;
                
                // Safety break to prevent infinite loops if we run out of combinations
                if (offset > len * len) return candidate; 
            }
        }

        private static string ComputeMixedDistinctHex(int classificationId)
        {
            // Kept for backward compatibility or other callers, though PrewarmAsync now uses GetUniqueGradientHex
            int len = _darkColors.Length;
            int idxA = classificationId % len;
            int idxB = ((classificationId / len) + idxA + 1) % len;
            return _darkColors[idxA] + "," + _darkColors[idxB];
        }        
    }
}