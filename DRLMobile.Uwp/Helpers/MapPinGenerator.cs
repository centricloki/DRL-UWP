using DRLMobile.ExceptionHandler;

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;

using System;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI;

namespace DRLMobile.Uwp.Helpers
{
    /// <summary>
    /// Generates map-pin PNG images at runtime using Win2D that exactly match
    /// the style and dimensions of the existing hand-crafted pins in Assets/Maps/.
    ///
    /// Measured pixel anatomy of the existing 25 × 40 px pins:
    ///   Canvas   : 25 × 40 px
    ///   Circle   : centre at (12, 11), radius ≈ 10.5 px  (rows 1–21, max width 21 px)
    ///   Stem     : x=11–13 (3 px wide), rows 22–38  (centred at x=12)
    ///   Stem color: dark charcoal  RGB(50,50,50)
    ///
    /// Generated PNGs are cached in
    ///   ApplicationData.Current.LocalFolder\MapPins\MapPin-{id}_v{PinVersion}.png
    /// and served as ms-appdata:///local/MapPins/MapPin-{id}_v{PinVersion}.png URIs.
    ///
    /// Increment <see cref="PinVersion"/> whenever the rendering changes so that
    /// stale cached files are automatically discarded on next launch.
    /// </summary>
    public static class MapPinGenerator
    {
        private const string FolderName = "MapPins";

        /// <summary>
        /// Bump this when the rendering logic changes.
        /// Old cached files (with a different version suffix) are silently ignored
        /// and regenerated — no manual cache-clearing or reinstall needed.
        /// </summary>
        private const int PinVersion = 6;

        // ── Canvas ────────────────────────────────────────────────────────────────
        // Exactly matches the existing 25 × 40 px assets.
        private const float CanvasWidth = 25f;
        private const float CanvasHeight = 40f;

        // ── Circle head ───────────────────────────────────────────────────────────
        // Centre at (12, 11), radius 10.5 → diameter 21 px, sitting 1 px from top.
        private const float CircleCX = 12f;
        private const float CircleCY = 11f;
        private const float CircleR = 10.5f;

        // ── Stem ──────────────────────────────────────────────────────────────────
        // 3 px wide, x=11–13, rows 22–38 (1 px gap between circle bottom and stem).
        private const float StemLeft = 11f;
        private const float StemWidth = 3f;
        private const float StemTop = CircleCY + CircleR + 0.5f; // just below circle
        private const float StemBottom = 38f;

        // Dark charcoal — identical to the stem/outline colour in the existing PNGs.
        private static readonly Color StemColor = Color.FromArgb(255, 50, 50, 50);

        // ──────────────────────────────────────────────────────────────────────────
        // Public API
        // ──────────────────────────────────────────────────────────────────────────

        /// <summary>Returns the ms-appdata URI for a classification ID (file may not exist yet).</summary>
        public static string GetLocalPinUri(int classificationId)
            => $"ms-appdata:///local/{FolderName}/MapPin-{classificationId}_v{PinVersion}.png";

        /// <summary>
        /// Returns the cached local URI if the PNG already exists; otherwise
        /// generates the pin, saves it, and returns the URI.
        /// Falls back to the existing white pin on any error.
        /// </summary>
        public static async Task<string> GetOrCreateMapPinAsync(int classificationId, string hexColor)
        {
            try
            {
                string fileName = $"MapPin-{classificationId}_v{PinVersion}.png";
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                StorageFolder pinFolder = await localFolder
                    .CreateFolderAsync(FolderName, CreationCollisionOption.OpenIfExists);

                // Cache hit.
                StorageFile existing = await TryGetFileAsync(pinFolder, fileName);
                if (existing != null)
                    return GetLocalPinUri(classificationId);

                // Generate and save.
                await RenderAndSaveAsync(pinFolder, fileName, hexColor);

                return GetLocalPinUri(classificationId);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(
                    nameof(MapPinGenerator),
                    nameof(GetOrCreateMapPinAsync),
                    ex.StackTrace);

                return "ms-appx:///Assets/Maps/MapPin-White.png";
            }
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Rendering
        // ──────────────────────────────────────────────────────────────────────────

        private static async Task RenderAndSaveAsync(
            StorageFolder folder, string fileName, string hexColor)
        {
            var device = CanvasDevice.GetSharedDevice();

            Color fillA = Colors.Gray;
            Color fillB = Colors.Gray;
            bool isSplit = false;

            if (hexColor.Contains(","))
            {
                var parts = hexColor.Split(',');
                fillA = ClassificationColorService.HexToColor(parts[0]);
                fillB = ClassificationColorService.HexToColor(parts[1]);
                isSplit = true;
            }
            else
            {
                fillA = ClassificationColorService.HexToColor(hexColor);
            }

            // Use 96 DPI — same as the source PNG assets.
            using (var rt = new CanvasRenderTarget(device, CanvasWidth, CanvasHeight, 96f))
            {
                using (var ds = rt.CreateDrawingSession())
                {
                    ds.Clear(Colors.Transparent);

                    // ── 1. Stem — drawn first so circle paints over the overlap ──
                    ds.FillRoundedRectangle(
                        StemLeft,
                        StemTop,
                        StemWidth,
                        StemBottom - StemTop,
                        radiusX: 1f,
                        radiusY: 1f,
                        color: StemColor);

                    // ── 2. Circle head — supports smooth linear gradient blend or solid ──
                    // LEGEND MATCHING: Using averaged color to match legend display
                    if (isSplit)
                    {
                        // Calculate averaged color to match legend
                        byte r = (byte)((fillA.R + fillB.R) / 2);
                        byte g = (byte)((fillA.G + fillB.G) / 2);
                        byte b = (byte)((fillA.B + fillB.B) / 2);
                        Color averagedColor = Color.FromArgb(255, r, g, b);

                        ds.FillCircle(CircleCX, CircleCY, CircleR, averagedColor);
                    }
                    else
                    {
                        ds.FillCircle(CircleCX, CircleCY, CircleR, fillA);
                    }

                    /*
                    // ── ORIGINAL GRADIENT IMPLEMENTATION (commented for legend matching) ─────────────
                    // This section is commented out to maintain consistency with legend
                    // Uncomment this section and comment out the averaged color code above
                    // to revert to gradient behavior

                    if (isSplit)
                    {
                        var stops = new[]
                        {
                            new CanvasGradientStop { Position = 0.0f, Color = fillA },
                            new CanvasGradientStop { Position = 1.0f, Color = fillB }
                        };

                        using (var brush = new CanvasLinearGradientBrush(device, stops))
                        {
                            brush.StartPoint = new Vector2(CircleCX - CircleR, CircleCY);
                            brush.EndPoint = new Vector2(CircleCX + CircleR, CircleCY);

                            ds.FillCircle(CircleCX, CircleCY, CircleR, brush);
                        }
                    }
                    else
                    {
                        ds.FillCircle(CircleCX, CircleCY, CircleR, fillA);
                    }
                    // ── END ORIGINAL IMPLEMENTATION ──────────────────────────────────
                    */

                    // ── 3. Dark outline ring — matches the subtle border visible
                    //        on all existing pins (antialiasing gives the clean edge) ──
                    ds.DrawCircle(CircleCX, CircleCY, CircleR, StemColor, strokeWidth: 0.8f);
                }

                StorageFile outFile = await folder.CreateFileAsync(
                    fileName, CreationCollisionOption.ReplaceExisting);

                using (var stream = await outFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    await rt.SaveAsync(stream, CanvasBitmapFileFormat.Png);
                }
            }
        }

        // ──────────────────────────────────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────────────────────────────────

        private static async Task<StorageFile> TryGetFileAsync(StorageFolder folder, string name)
        {
            try { return await folder.GetFileAsync(name); }
            catch (FileNotFoundException) { return null; }
        }
    }
}
