using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Data.Pdf;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReceiptPrintPage : Page
    {
        private PdfDocument pdfDocument;

        private readonly string FilePath;

        public ObservableCollection<BitmapImage> PdfPages{ get; set; }

        public ReceiptPrintPage(string documentPath)
        {
            this.InitializeComponent();

            PdfPages = new ObservableCollection<BitmapImage>();

            FilePath = documentPath;

            LoadDocumentFromSource();
        }

        private async void LoadDocumentFromSource()
        {
            pdfDocument = null;

            //amol commented the code on 21 - dec to load test file
            //   var file = await StorageFile.GetFileFromPathAsync(FilePath);
            var file = await StorageFile.GetFileFromPathAsync("PrintOrderXAMLPage.xaml");
            if (file != null)
            {
                pdfDocument = await PdfDocument.LoadFromFileAsync(file);

               await LoadPdfDocumentForPrint(pdfDocument);
            }
        }

        private async Task LoadPdfDocumentForPrint(PdfDocument pdfDoc)
        {
            PdfPages.Clear();

            for (uint i = 0; i < pdfDoc.PageCount; i++)
            {
                BitmapImage image = new BitmapImage();

                var page = pdfDoc.GetPage(i);

                using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                {
                    await page.RenderToStreamAsync(stream);
                    await image.SetSourceAsync(stream);
                }

                PdfPages.Add(image);
            }
        }
    }
}