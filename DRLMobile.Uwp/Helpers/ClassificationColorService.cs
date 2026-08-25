using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
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
        /// A curated list of light, pastel, high-contrast colors used to generate
        /// two-tone gradient pins when no explicit color code is defined in the database.
        /// </summary>
        private static readonly string[] _lightColors = new[]
        {
            "FFB3B3", // Light Pink/Red
            "B3D1FF", // Light Sky Blue
            "B3FFB3", // Light Lime Green
            "FFFFB3", // Light Pastel Yellow
            "FFD1B3", // Light Apricot/Peach
            "FFB3FF", // Light Orchid/Lavender
            "B3FFFF", // Light Cyan/Turquoise
            "D1B3FF", // Light Lilac/Violet
            "E6E6E6", // Light Silver/Gray
            "FFE0B3", // Pale Orange
            "C2F0C2", // Pale Mint Green
            "F0D8A8"  // Pale Sand/Khaki
        };

        /// <summary>
        /// Pre-computes, caches colors, and generates map-pin PNG files in the local 
        /// application storage for all active map classifications.
        /// 
        /// This should be called when navigating to the map page, immediately after 
        /// loading classifications from the database.
        /// </summary>
        /// <param name="mapClassifications">The list of active classifications fetched from the MapClassification table.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public static async Task PrewarmAsync(IList<MapClassificationViewModel> mapClassifications)
        {
            if (mapClassifications == null || mapClassifications.Count == 0) return;

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
                // Priority 2: Fallback to the deterministic gradient calculation
                else
                {
                    hex = ComputeMixedDistinctHex(id);
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
        }

        /// <summary>
        /// Returns the cached 6-digit hex color string (or comma-separated hex colors)
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
            _colorCache[classificationId] = fallback;
            return fallback;
        }

        /// <summary>
        /// Resolves and returns a <see cref="Brush"/> for a classification ID.
        /// Returns a <see cref="LinearGradientBrush"/> for gradient colors (comma-separated),
        /// or a <see cref="SolidColorBrush"/> for single solid colors.
        /// </summary>
        /// <param name="classificationId">The classification ID.</param>
        /// <returns>A SolidColorBrush or LinearGradientBrush.</returns>
        public static Brush GetBrush(int classificationId)
        {
            string hex = GetColorHex(classificationId);

            // Blended colors are stored as "HexA,HexB" in the cache
            if (hex.Contains(","))
            {
                var parts = hex.Split(',');
                var brush = new LinearGradientBrush();
                brush.StartPoint = new Windows.Foundation.Point(0, 0);
                brush.EndPoint = new Windows.Foundation.Point(1, 0);
                brush.GradientStops.Add(new GradientStop { Color = HexToColor(parts[0]), Offset = 0.0 });
                brush.GradientStops.Add(new GradientStop { Color = HexToColor(parts[1]), Offset = 1.0 });
                return brush;
            }

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
        /// for a given ID. It guarantees identical colors for the same ID across all users and devices.
        /// </summary>
        private static string ComputeMixedDistinctHex(int classificationId)
        {
            int len = _lightColors.Length;
            int idxA = classificationId % len;
            int idxB = ((classificationId / len) + idxA + 1) % len;
            return _lightColors[idxA] + "," + _lightColors[idxB];
        }
    }
}
