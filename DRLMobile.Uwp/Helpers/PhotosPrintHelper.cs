using DRLMobile.Uwp.Enums;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Graphics.Printing;
using Windows.Graphics.Printing.OptionDetails;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Printing;

namespace DRLMobile.Uwp.Helpers
{
    public class PhotosPrintHelper : ReceiptPrintHelper
    {
        private const int NumberOfPhotos = 1;

        private const int DPI96 = 96;

        private PhotoSize photoSize;

        private Scaling photoScale;

        private Dictionary<int, UIElement> pageCollection = new Dictionary<int, UIElement>();

        private static object printSync = new object();

        private PhotosPageDescription currentPageDescription;

        private long requestCount;

        private string FilePath;

        public PhotosPrintHelper(Page scenarioPage, string imageFilePath) : base(scenarioPage)
        {
            photoSize = PhotoSize.SizeFullPage;
            photoScale = Scaling.ShrinkToFit;
            FilePath = imageFilePath;
            
        }


        protected override void PrintTaskRequested(PrintManager sender, PrintTaskRequestedEventArgs e)
        {
            PrintTask printTask = null;

            printTask = e.Request.CreatePrintTask("Honey App - Preview Photo Print", sourceRequestedArgs =>
            {
                PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(printTask.Options);

                printDetailedOptions.DisplayedOptions.Clear();

                printDetailedOptions.DisplayedOptions.Add(StandardPrintTaskOptions.MediaSize);
                printDetailedOptions.DisplayedOptions.Add(StandardPrintTaskOptions.Copies);

                PrintCustomItemListOptionDetails photosSizeOption = printDetailedOptions.CreateItemListOption("photoSize", "Photo Size");

                photosSizeOption.AddItem("SizeFullPage", "Full Page");
                photosSizeOption.AddItem("Size4x6", "4 x 6 in");
                photosSizeOption.AddItem("Size5x7", "5 x 7 in");
                photosSizeOption.AddItem("Size8x10", "8 x 10 in");

                printDetailedOptions.DisplayedOptions.Add("photoSize");

                PrintCustomItemListOptionDetails scaling = printDetailedOptions.CreateItemListOption("scaling", "Scaling");
                scaling.AddItem("ShrinkToFit", "Shrink To Fit");
                scaling.AddItem("Crop", "Crop");

                printDetailedOptions.DisplayedOptions.Add("scaling");

                printTask.Options.Orientation = PrintOrientation.Landscape;

                printDetailedOptions.OptionChanged += PrintDetailedOptionsOptionChanged;

                printTask.Completed += async (s, args) =>
                {
                    await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        ClearPageCollection();

                        photoScale = Scaling.ShrinkToFit;
                        photoSize = PhotoSize.SizeFullPage;

                        currentPageDescription = null;

                        // Notify the user when the print operation fails.
                        if (args.Completion == PrintTaskCompletion.Failed)
                        {
                           //"Failed to print message.
                        }
                    });
                };

                sourceRequestedArgs.SetSource(printDocumentSource);

            });
        }

        private async void PrintDetailedOptionsOptionChanged(PrintTaskOptionDetails sender, PrintTaskOptionChangedEventArgs args)
        {
            bool invalidatePreview = false;

            if (args.OptionId == null)
            {
                return;
            }

            string optionId = args.OptionId.ToString();

            if (optionId == "photoSize")
            {
                IPrintOptionDetails photoSizeOption = sender.Options[optionId];

                string photoSizeValue = photoSizeOption.Value as string;

                if (!string.IsNullOrEmpty(photoSizeValue))
                {
                    switch (photoSizeValue)
                    {
                        case "SizeFullPage":
                            photoSize = PhotoSize.SizeFullPage;
                            break;
                        case "Size4x6":
                            photoSize = PhotoSize.Size4x6;
                            break;
                        case "Size5x7":
                            photoSize = PhotoSize.Size5x7;
                            break;
                        case "Size8x10":
                            photoSize = PhotoSize.Size8x10;
                            break;
                    }

                    invalidatePreview = true;
                }

                if (optionId == "scaling")
                {
                    IPrintOptionDetails scalingOption = sender.Options[optionId];
                    string scalingValue = scalingOption.Value as string;

                    if (!string.IsNullOrEmpty(scalingValue))
                    {
                        switch (scalingValue)
                        {
                            case "Crop":
                                photoScale = Scaling.Crop;
                                break;
                            case "ShrinkToFit":
                                photoScale = Scaling.ShrinkToFit;
                                break;
                        }
                        invalidatePreview = true;
                    }
                }

                if (invalidatePreview)
                {
                    await printPage.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, printDocument.InvalidatePreview);
                }
            }
        }

        protected override void CreatePrintPreviewPages(object sender, PaginateEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;

            Interlocked.Increment(ref requestCount);

            PhotosPageDescription pageDescription = new PhotosPageDescription();

            PrintTaskOptionDetails printDetailedOptions = PrintTaskOptionDetails.GetFromPrintTaskOptions(e.PrintTaskOptions);
            PrintPageDescription printPageDescription = e.PrintTaskOptions.GetPageDescription(0);

            printDetailedOptions.Options["photoSize"].ErrorText = string.Empty;

            pageDescription.PageSize = printPageDescription.PageSize;

            pageDescription.Margin.Width = Math.Max(
                printPageDescription.ImageableRect.Left,
                printPageDescription.ImageableRect.Right - printPageDescription.PageSize.Width);

            pageDescription.Margin.Height = Math.Max(
                printPageDescription.ImageableRect.Top,
                printPageDescription.ImageableRect.Bottom - printPageDescription.PageSize.Height);

            pageDescription.ViewablePageSize.Width = printPageDescription.PageSize.Width - pageDescription.Margin.Width * 2;
            pageDescription.ViewablePageSize.Height = printPageDescription.PageSize.Height - pageDescription.Margin.Height * 2;

            switch (photoSize)
            {
                case PhotoSize.Size4x6:
                    pageDescription.PictureViewSize.Width = 4 * DPI96;
                    pageDescription.PictureViewSize.Height = 6 * DPI96;
                    break;
                case PhotoSize.Size5x7:
                    pageDescription.PictureViewSize.Width = 5 * DPI96;
                    pageDescription.PictureViewSize.Height = 7 * DPI96;
                    break;
                case PhotoSize.Size8x10:
                    pageDescription.PictureViewSize.Width = 8 * DPI96;
                    pageDescription.PictureViewSize.Height = 10 * DPI96;
                    break;
                case PhotoSize.SizeFullPage:
                    pageDescription.PictureViewSize.Width = pageDescription.ViewablePageSize.Width;
                    pageDescription.PictureViewSize.Height = pageDescription.ViewablePageSize.Height;
                    break;
            }

            if ((pageDescription.ViewablePageSize.Width > pageDescription.ViewablePageSize.Height) && (photoSize != PhotoSize.SizeFullPage))
            {
                var swap = pageDescription.PictureViewSize.Width;
                pageDescription.PictureViewSize.Width = pageDescription.PictureViewSize.Height;
                pageDescription.PictureViewSize.Height = swap;
            }

            pageDescription.IsContentCropped = photoScale == Scaling.Crop;

            if (currentPageDescription == null || !currentPageDescription.Equals(pageDescription))
            {
                ClearPageCollection();

                if (pageDescription.PictureViewSize.Width > pageDescription.ViewablePageSize.Width ||
                    pageDescription.PictureViewSize.Height > pageDescription.ViewablePageSize.Height)
                {
                    printDetailedOptions.Options["photoSize"].ErrorText = "Photo doesn’t fit on the selected paper";

                    printDoc.SetPreviewPageCount(1, PreviewPageCountType.Intermediate);

                    lock (printSync)
                    {
                        ///pageCollection[0] = new PreviewUnavailable(pageDescription.PageSize, pageDescription.ViewablePageSize);
                    }
                }
                else
                {
                    // Inform preview that is has #NumberOfPhotos pages to show.
                    printDoc.SetPreviewPageCount(1, PreviewPageCountType.Intermediate);
                }

                currentPageDescription = pageDescription;
            }
        }

        protected async override void GetPrintPreviewPage(object sender, GetPreviewPageEventArgs e)
        {
            try
            {
                long requestNumber = 0;
                Interlocked.Exchange(ref requestNumber, requestCount);
                int pageNumber = e.PageNumber;

                UIElement page;
                bool pageReady = false;

                lock (printSync)
                {
                    pageReady = pageCollection.TryGetValue(pageNumber - 1, out page);
                }

                if (!pageReady)
                {
                    page = await GeneratePageAsync(pageNumber, currentPageDescription);

                    if (Interlocked.CompareExchange(ref requestNumber, requestNumber, requestCount) != requestCount)
                    {
                        return;
                    }

                    lock (printSync)
                    {
                        pageCollection[pageNumber - 1] = page;

                        PrintCanvas.Children.Add(page);
                        PrintCanvas.InvalidateMeasure();
                        PrintCanvas.UpdateLayout();
                    }
                }

                PrintDocument printDoc = (PrintDocument)sender;

                // Send the page to preview.
                printDoc.SetPreviewPage(pageNumber, page);
            }
            catch (Exception ex)
            {
                throw ex;
            }        
        }

        protected async override void AddPrintPages(object sender, AddPagesEventArgs e)
        {
            PrintDocument printDoc = (PrintDocument)sender;

            // Loop over all of the preview pages
            for (int i = 0; i < NumberOfPhotos; i++)
            {
                UIElement page = null;
                bool pageReady = false;

                lock (printSync)
                {
                    pageReady = pageCollection.TryGetValue(i, out page);
                }

                if (!pageReady)
                {
                    // If the page is not ready create a task that will generate its content.
                    page = await GeneratePageAsync(i + 1, currentPageDescription);
                }

                printDoc.AddPage(page);
            }

            printDoc.AddPagesComplete();

            // Reset the current page description as soon as possible since the PrintTask.Completed event might fire later (long running job)
            currentPageDescription = null;
        }

        private void ClearPageCollection()
        {
            lock (printSync)
            {
                pageCollection.Clear();
                PrintCanvas.Children.Clear();
            }
        }

        private static void Swap<T>(ref T v1, ref T v2)
        {
            T swap = v1;
            v1 = v2;
            v2 = swap;
        }

        private async Task<UIElement> GeneratePageAsync(int photoNumber, PhotosPageDescription pageDescription)
        {
            Canvas page = new Canvas
            {
                Width = pageDescription.PageSize.Width,
                Height = pageDescription.PageSize.Height
            };

            Canvas viewablePage = new Canvas()
            {
                Width = pageDescription.ViewablePageSize.Width,
                Height = pageDescription.ViewablePageSize.Height
            };

            viewablePage.SetValue(Canvas.LeftProperty, pageDescription.Margin.Width);
            viewablePage.SetValue(Canvas.TopProperty, pageDescription.Margin.Height);

            // The image "frame" which also acts as a viewport
            Grid photoView = new Grid
            {
                Width = pageDescription.PictureViewSize.Width,
                Height = pageDescription.PictureViewSize.Height
            };

            // Center the frame.
            photoView.SetValue(Canvas.LeftProperty, (viewablePage.Width - photoView.Width) / 2);
            photoView.SetValue(Canvas.TopProperty, (viewablePage.Height - photoView.Height) / 2);

            // Return an async task that will complete when the image is fully loaded.
            WriteableBitmap bitmap = await LoadBitmapAsync(pageDescription.PageSize.Width > pageDescription.PageSize.Height);

            if (bitmap != null)
            {
                Image image = new Image
                {
                    Source = bitmap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                // Use the real image size when croping or if the image is smaller then the target area (prevent a scale-up).
                if (photoScale == Scaling.Crop ||
                    (bitmap.PixelWidth <= pageDescription.PictureViewSize.Width &&
                    bitmap.PixelHeight <= pageDescription.PictureViewSize.Height))
                {
                    image.Stretch = Stretch.None;
                    image.Width = bitmap.PixelWidth;
                    image.Height = bitmap.PixelHeight;
                }

                // Add the newly created image to the visual root.
                photoView.Children.Add(image);
                viewablePage.Children.Add(photoView);
                page.Children.Add(viewablePage);
            }

            // Return the page with the image centered.
            return page;
        }

        private async Task<WriteableBitmap> LoadBitmapAsync(bool landscape)
        {
            var file = await StorageFile.GetFileFromPathAsync(FilePath);

            using (var stream = await file.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                BitmapTransform transform = new BitmapTransform();
                transform.Rotation = BitmapRotation.None;
                uint width = decoder.PixelWidth;
                uint height = decoder.PixelHeight;

                if (landscape && width < height)
                {
                    transform.Rotation = BitmapRotation.Clockwise270Degrees;
                    Swap(ref width, ref height);
                }
                else if (!landscape && width > height)
                {
                    transform.Rotation = BitmapRotation.Clockwise90Degrees;
                    Swap(ref width, ref height);
                }

                PixelDataProvider pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,    
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,    
                    ColorManagementMode.DoNotColorManage);

                WriteableBitmap bitmap = new WriteableBitmap((int)width, (int)height);

                var pixelBuffer = pixelData.DetachPixelData();
                
                using (var pixelStream = bitmap.PixelBuffer.AsStream())
                {
                    pixelStream.Write(pixelBuffer, 0, (int)pixelStream.Length);
                }

                return bitmap;
            }
        }

        public async Task<WriteableBitmap> ProcessImageAsync(string filepath)
        {
            try
            {
                var uri = new Uri(new Uri("ms-appx://"), filepath).ToString();

                BitmapImage image = new BitmapImage(new Uri(uri));

                RandomAccessStreamReference random = RandomAccessStreamReference.CreateFromUri(image.UriSour‌​ce);

                using (IRandomAccessStream stream = await random.OpenReadAsync())
                {
                    var memStream = new InMemoryRandomAccessStream();

                    //Create a decoder for the image
                    var decoder = await BitmapDecoder.CreateAsync(stream);

                    uint width = decoder.PixelWidth;
                    uint height = decoder.PixelHeight;

                    BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(memStream, decoder);

                    encoder.BitmapTransform.ScaledWidth = width;
                    encoder.BitmapTransform.ScaledHeight = height;
                    encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Cubic;

                    encoder.BitmapTransform.Rotation = BitmapRotation.Clockwise180Degrees;

                    await encoder.FlushAsync();
                    memStream.Seek(0);

                    //Initialize writable bitmap.
                    var wBitmap = new WriteableBitmap((int)encoder.BitmapTransform.ScaledWidth, (int)encoder.BitmapTransform.ScaledHeight);
                    await wBitmap.SetSourceAsync(memStream);

                    return wBitmap;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }

            return null;
        }
    }
}