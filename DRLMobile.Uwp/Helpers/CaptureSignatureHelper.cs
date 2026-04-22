using DRLMobile.Core.Helpers;
using DRLMobile.ExceptionHandler;
using Microsoft.Graphics.Canvas;
using Microsoft.Toolkit.Uwp.Helpers;

using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Uwp.Helpers
{
    public static class CaptureSignatureHelper
    {           
        private static byte[] ConvertSingatureCanvasToByteArray(InkCanvas canvas)
        {
            var canvasStroke = canvas.InkPresenter.StrokeContainer.GetStrokes();

            if (canvasStroke.Count > 0)
            {
                var width = (int)canvas.ActualWidth;
                var height = (int)canvas.ActualHeight;
                var device = CanvasDevice.GetSharedDevice();

                var renderTarget = new CanvasRenderTarget(device, width, height, 96);

                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.Clear(Windows.UI.Colors.White);
                    ds.DrawInk(canvas.InkPresenter.StrokeContainer.GetStrokes());
                }

                return renderTarget.GetPixelBytes();
            }

            return null;
        }

        private static async Task<WriteableBitmap> ConvertInkCanvasToWriteableBitmap(InkCanvas inkCanvas)
        {
            var bytes = ConvertSingatureCanvasToByteArray(inkCanvas);

            if (bytes != null)
            {
                var width = (int)inkCanvas.ActualWidth;
                var height = (int)inkCanvas.ActualHeight;

                var bmp = new WriteableBitmap(width, height);

                using (var stream = bmp.PixelBuffer.AsStream())
                {
                    await stream.WriteAsync(bytes, 0, bytes.Length);

                    return bmp;
                }
            }

            return null;
        }

        public static async Task<StorageFile> SaveSignatureToStorageFile(InkCanvas inkCanvas, string docFileName)
        {
            StorageFile file = null;
                      
            // get the writable bitmap from main ink canvas
            WriteableBitmap writeableBitmap = await ConvertInkCanvasToWriteableBitmap(inkCanvas);

            if (writeableBitmap != null)
            {
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(docFileName, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
                    Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
                    byte[] pixels = new byte[pixelStream.Length];
                    await pixelStream.ReadAsync(pixels, 0, pixels.Length);
                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight,
                        96.0,
                        96.0,
                        pixels);
                    await encoder.FlushAsync();
                }
            }
            return file;
        }

        public static bool SaveSignatureImage(byte[] fileByteArray, string docFileName)
        {
            MemoryStream memoryStream = new MemoryStream();

            string basePath = ApplicationConstants.APP_PATH + @"\Signatures\";

            try
            {
                if (fileByteArray != null)
                {
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }

                    string signatureFilePath = Path.Combine(basePath, docFileName);

                    using (FileStream file = new FileStream(signatureFilePath, FileMode.Create, FileAccess.Write))
                    {
                        memoryStream.Read(fileByteArray, 0, (int)memoryStream.Length);

                        file.Write(fileByteArray, 0, fileByteArray.Length);

                        memoryStream.Close();

                        memoryStream.Dispose();

                        file.Close();

                        file.Dispose();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog("CaptureSignatureHelper", "SaveSignatureImage", ex.StackTrace);

                return false;
            }
        }

        public static async Task<InMemoryRandomAccessStream> ConvertWriteableBitmapToRandomAccessStream(WriteableBitmap writeableBitmap)
        {
            var stream = new InMemoryRandomAccessStream();

            BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream);
            Stream pixelStream = writeableBitmap.PixelBuffer.AsStream();
            byte[] pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)writeableBitmap.PixelWidth, (uint)writeableBitmap.PixelHeight, 96.0, 96.0, pixels);

            await encoder.FlushAsync();

            return stream;

        }

    }
}
