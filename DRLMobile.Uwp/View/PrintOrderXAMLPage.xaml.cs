using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.DataSyncRequestModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.EmailAndPrintOrder;
using DRLMobile.ExceptionHandler;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PrintOrderXAMLPage : Page
    {
        //  public RichTextBlock TextContentBlock { get; set; }
        public RichTextBlock TextContentBlock = new RichTextBlock();
        private List<EmailAndPrintOrder.OrderDetails> orderData;
        private EmailAndPrintOrder.CustomerInfo customerInfo;
        private RetailTransactionUIModel retailTransactionUIModel;
        string salesType;
        private readonly double defaultFontSize = 7.5d;
        private readonly double dFontSizeSeven = 7d;
        Paragraph paragraph = new Paragraph();
        Run run = new Run();
        private readonly App AppReference = (App)Application.Current;

        public PrintOrderXAMLPage(List<EmailAndPrintOrder.OrderDetails> orderData,
            CustomerInfo customerInfo,
            string salesType, BitmapImage bitmap)
        {
            this.InitializeComponent();
            this.TextContentBlock = TextContent;
            this.orderData = orderData;
            this.customerInfo = customerInfo;
            // this.retailTransactionUIModel= retailTransactionUIModel;
            //salesType = retailTransactionUIModel.SelectedSalesType;
            this.salesType = salesType;
            AddAllContent();
            if (bitmap != null)
            {
                if (bitmap.UriSource != null)
                {
                    AddSignName(bitmap.UriSource);
                }
            }
        }
        public PrintOrderXAMLPage()
        {
            this.InitializeComponent();
        }
        public PrintOrderXAMLPage(List<EmailAndPrintOrder.OrderDetails> orderData,
            EmailAndPrintOrder.CustomerInfo customerInfo,
            RetailTransactionUIModel retailTransactionUIModel)
        {
            this.InitializeComponent();
            this.TextContentBlock = TextContent;
            this.orderData = orderData;
            this.customerInfo = customerInfo;
            this.retailTransactionUIModel = retailTransactionUIModel;
            salesType = retailTransactionUIModel.SelectedSalesType;
            AddAllContent();

            if (retailTransactionUIModel != null)
            {
                if (!string.IsNullOrWhiteSpace(retailTransactionUIModel.CustomerSignatureFileName))
                {
                    AddSignNameAsync(retailTransactionUIModel.CustomerSignatureFileName);
                }

            }
        }

        private void AddAllContent()
        {

            GenerateHeader();
            AddCompanyDetails();
            AddPhoneandDate();
            AddCustomerDetails();
            AddDistributorAndBookDate();
            AddOrderDetails();
            AddInvoiceTotal();
            AddTaxStatement();
            AddPrintName();
        }

        private void AddParagraph(string text, double txtSize = 7.5d, double topMargin = 0, double bottomMargin = 0)
        {
            paragraph = new Paragraph();
            run = new Run();
            run.Text = text;
            run.FontSize = txtSize;
            paragraph.Inlines.Add(run);
            paragraph.Margin = new Thickness(0, topMargin, 0, bottomMargin);
            paragraph.FontWeight = FontWeights.Bold;
            TextContentBlock.TextWrapping = TextWrapping.WrapWholeWords;
            TextContentBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
            TextContentBlock.Blocks.Add(paragraph);
        }

        private void AddLineSeparator()
        {
            var paragraph = new Paragraph();
            paragraph.Inlines.Add(new InlineUIContainer
            {
                Child = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 2,
                    X2 = this.Width
                }
            });
            this.TextContentBlock.Blocks.Add(paragraph);
            TextContentBlock.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private void GenerateHeader()
        {
            if (!string.IsNullOrEmpty(salesType) && salesType == "8")
            {
                AddParagraph("Credit Request", 14);
            }
        }

        private void AddCompanyDetails()
        {
            try
            {
                AddParagraph("Republic Brands L.P.", 12);
                AddParagraph("2301 Ravine Way, Glenview,IL 60025");
            }
            catch (Exception ex)
            {
                var errDescription = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException;
                ErrorLogger.WriteToErrorLog(GetType().Name, "AddCompanyDetails", errDescription);
            }
        }

        private void AddPhoneandDate()
        {
            AddParagraph("Phone: 847-832-9700");
            AddParagraph("Fax: 847-832-9710");
            AddParagraph("Date :" + DateTime.Now.ToString("d"));
            AddParagraph("Republic Sales Rep: " + customerInfo.SalesRepresentative, defaultFontSize, 2);
            AddLineSeparator();
        }

        private void AddCustomerDetails()
        {
            string trxnumber = string.IsNullOrEmpty(customerInfo.PurchaseNumber) ? "-" : customerInfo.PurchaseNumber;
            AddParagraph("Transaction # " + trxnumber, defaultFontSize, 2);
            //AddParagraph(string.IsNullOrEmpty(customerInfo.PurchaseNumber) ? "-" : customerInfo.PurchaseNumber, 0,defaultFontSize);
            AddParagraph("Customer Name: " + customerInfo.CustomerName);

            string permitno = string.IsNullOrEmpty(customerInfo.PermitNumber) ? "-" : customerInfo.PermitNumber;
            AddParagraph(String.Format("Sellers Rep. Tobacco Permit # :${0}", permitno), dFontSizeSeven, 0, 2);

            AddParagraph("Address: " + customerInfo.Address, defaultFontSize, 2);
            AddParagraph("City: " + customerInfo.City);
            AddParagraph("State: " + customerInfo.State + " Zip: " + customerInfo.Zip, defaultFontSize, 0, 2);

        }

        private void AddDistributorAndBookDate()
        {
            // sales type 4 = Suggested order
            if (salesType == "4")
            {
                if (orderData != null)
                {
                    if (!string.IsNullOrWhiteSpace(orderData[0].DistributorID))
                        AddParagraph("Distributor:" + orderData[0].DistributorID, defaultFontSize, 2);
                    if (!string.IsNullOrWhiteSpace(orderData[0].DistributorName))
                        AddParagraph("Distributor Name:" + orderData[0].DistributorName, defaultFontSize, 2);
                }
            }

            //  sales type 2 = Prebook
            if (salesType == "2")
            {
                if (orderData != null)
                {
                    if (!string.IsNullOrWhiteSpace(orderData[0].RetailDistributorNumber))
                        AddParagraph("Retailer’s Distributor Customer Number:" + orderData[0].RetailDistributorNumber, defaultFontSize, 2);
                    if (!string.IsNullOrWhiteSpace(orderData[0].DistributorID))
                        AddParagraph("Distributor:" + orderData[0].DistributorID, defaultFontSize, 2);
                    if (!string.IsNullOrWhiteSpace(orderData[0].DistributorName))
                        AddParagraph("Distributor Name:" + orderData[0].DistributorName, defaultFontSize, 2);
                    AddParagraph("Prebook Ship Date:" + customerInfo.ShipDate);
                }
            }

            //  sales type 13 = Credit Card sale
            if (salesType == "13")
            {
                AddParagraph("State Tobacco License:" + orderData[0].TobaccoLicense, defaultFontSize, 2);
                AddParagraph("Retailer Licence:" + orderData[0].RetailerLicense, defaultFontSize);
            }

        }
        private void AddOrderDetails()
        {
            if (orderData.Any())
            {
                AddProductItems(orderData.Where((prd) => prd.IsTobaccoProduct == "0"), false);

                AddProductItems(orderData.Where((prd) => prd.IsTobaccoProduct == "1"));
            }
        }

        private void AddProductItems(IEnumerable<OrderDetails> productList, bool IsTobaccoProduct = true)
        {
            if (productList.Any())
            {
                AddLineSeparator();
                if (IsTobaccoProduct)
                {
                    // 8 is for Credit request
                    if (salesType == "8")
                    {
                        AddParagraph("RTN", defaultFontSize, 2);
                    }
                    else
                    {

                        AddParagraph("Tobacco Products", defaultFontSize, 2);
                        AddLineSeparator();
                    }
                }
                else
                {
                    // 8 is for Credit request
                    if (salesType == "8")
                    {
                        AddParagraph("DIF", defaultFontSize, 2);
                    }
                    else
                    {
                        AddParagraph("Non-Tobacco Products", defaultFontSize, 2);
                        AddLineSeparator();
                    }
                }

                AddOrderItem(productList, IsTobaccoProduct);
            }
        }

        private void AddOrderItem(IEnumerable<OrderDetails> orderData, bool IsTobaccoProduct)
        {
            decimal tobSubTotal = 0;
            foreach (OrderDetails details in orderData)
            {
                decimal subtotal = decimal.Multiply(details.Quantity, details.Price);
                AddParagraph(string.Format("{0} - {1}", details.ProductName, details.Description), defaultFontSize, 2);
                AddParagraph(string.Format("{0,10}{1,10}{2,17}", "Qty", "Price", "SubTotal"));
                AddParagraph(string.Format("{0,10}{1,10}{2,15}", details.Quantity,
                    string.Format("${0:0.00}", details.Price), string.Format("${0:0.00}", subtotal)));

                tobSubTotal += subtotal;
            }
            AddLineSeparator();
            AddParagraph(String.Format("Total {0} Products: {1,6}", (IsTobaccoProduct ? "Tobacco" : "Non Tobacco"),
                string.Format("${0:0.00}", tobSubTotal)), defaultFontSize, 5);
        }
        private void AddInvoiceTotal()
        {
            AddLineSeparator();
            if (orderData.Count > 0)
            {
                AddParagraph(string.Format("Total Invoice Amount: {0,6}",
                    string.Format("${0:0.00}", orderData[0].GrandTotal)), defaultFontSize, 2);
            }
            AddLineSeparator();
        }
        private void AddTaxStatement()
        {
            AddParagraph("Tax Statement: " + customerInfo.CustomStatement, dFontSizeSeven, 2);
        }

        private async void AddSignNameAsync(string fileName)
        {
            Paragraph sigParagraph = new Paragraph();
            InlineUIContainer signContainer = new InlineUIContainer();
            Image signatureImage = new Image();
            signatureImage.Stretch = Stretch.Uniform;
            BitmapImage signbitimage = await GetBitmapAsync(fileName);
            {
                if (signbitimage != null)
                {
                    signatureImage.Source = signbitimage;
                    signContainer.Child = signatureImage;
                    sigParagraph.Inlines.Add(signContainer);
                    TextContent.Blocks.Add(sigParagraph);
                }
            }
            AddLineSeparator();
        }

        private void AddSignName(Uri bitmap)
        {
            Paragraph sigParagraph = new Paragraph();
            InlineUIContainer signContainer = new InlineUIContainer();
            Image signatureImage = new Image();
            signatureImage.Stretch = Stretch.Uniform;
            BitmapImage signbitimage = new BitmapImage();

            //  if (retailTransactionUIModel != null)
            {
                if (bitmap != null)
                {
                    signbitimage.UriSource = bitmap;
                    signatureImage.Source = signbitimage;
                    signContainer.Child = signatureImage;
                    sigParagraph.Inlines.Add(signContainer);
                    TextContent.Blocks.Add(sigParagraph);

                }
            }

            AddLineSeparator();

        }


        private void AddPrintName()
        {
            AddParagraph("Print Name :" + customerInfo.PrintName);
            AddParagraph("Please Sign Below \n");


            //if (retailTransactionUIModel != null)
            //{
            //    if (retailTransactionUIModel.CustomerSignPath.UriSource != null)
            //    {
            //      BitmapImage sign = new BitmapImage(retailTransactionUIModel.CustomerSignPath.UriSource);
            //        ScenarioImage.Source = sign;
            //    }
            //}

        }

        private async Task<BitmapImage> GetBitmapAsync(string fileName)
        {
            BitmapImage bitmapImage = null;
            // Open a stream for the selected file
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            fileName = fileName.Substring(fileName.IndexOf("LocalState") + 11);
            StorageFile file = await storageFolder.GetFileAsync(fileName);

            // Ensure a file was selected
            if (file != null)
            {
                // Ensure the stream is disposed once the image is loaded
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap
                    bitmapImage = new BitmapImage();
                    await bitmapImage.SetSourceAsync(fileStream);
                }
            }
            return bitmapImage;
        }
    }
}
