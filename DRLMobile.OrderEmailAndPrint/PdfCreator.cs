using DRLMobile.ExceptionHandler;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.OrderEmailAndPrint
{
    public class PdfCreator
    {
        private static PdfCreator instance = null;
        private static readonly object _lock = new object();

        private PdfCreator()
        {

        }
        public static PdfCreator Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new PdfCreator();
                    }
                    return instance;
                }
            }
        }



        public bool CreatePdfFromImages(string path, List<string> imagesFilePath)
        {
            try
            {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(path));
                Document document = new Document(pdfDocument);

                foreach (var item in imagesFilePath)
                {
                    ImageData imageData = ImageDataFactory.Create(item);
                    Image image = new Image(imageData);
                    image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth() - 50);
                    image.SetAutoScaleHeight(true);
                    document.Add(image);
                }

                pdfDocument.Close();
                return true;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(PdfCreator), nameof(CreatePdfFromImages), ex.StackTrace);
                return false;
            }
        }



    }
}
