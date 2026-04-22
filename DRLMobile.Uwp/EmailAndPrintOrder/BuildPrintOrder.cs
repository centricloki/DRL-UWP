using DRLMobile.ExceptionHandler;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace DRLMobile.EmailAndPrintOrder
{
    public class BuildPrintOrder
    {
        // Define high level components
        PdfWriter recieptWriter;
        PdfDocument recieptDocument;
        Document recieptLayout;
        PageSize pageSize;
        LineSeparator lineSeperator;

        double headerFontSize = 8.0;
        double dataFontSize = 4.0;
        double subTotalFontSize = 6.0;    
        double margin = 0.5;
        double lineSepWidth = 0.5;

        private string DestinationPath;
        private string SalesType;

        private List<OrderDetails> OrderDetailsData;
        private CustomerInfo CustomerOrderInfo;
       
        public BuildPrintOrder(string destinationPath, List<OrderDetails> orderData, CustomerInfo customerInfo, string salestype)
        {
            this.DestinationPath = destinationPath;
            this.OrderDetailsData = orderData;
            this.CustomerOrderInfo = customerInfo;
            this.SalesType = salestype;

        }

        public void GeneratePrintForm()
        {
            try
            {
                if (!File.Exists(this.DestinationPath))
                {
                    CreateObjectAndProperties();
                    GenerateHeader();
                    AddCompanyDetails();
                    AddPhoneandDate();
                    AddCustomerDetails();
                    AddDistributorAndBookDate();
                    AddTobaccoProductDetails();
                    AddNonTobaccoProductDetails();
                    AddInvoiceTotal();
                    AddTaxStatement();
                    AddPrintName();
                    WriteFileToDisk();
                }
            }
            catch (Exception ex)
            {
                var errDescription = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException;

                ErrorLogger.WriteToErrorLog(GetType().Name, "GeneratePrintForm", errDescription);

                recieptWriter?.Close();
                recieptDocument?.Close();
            }
        }

        private void CreateObjectAndProperties()
        {
            recieptWriter = new PdfWriter(DestinationPath);

            recieptDocument = new PdfDocument(recieptWriter);
            recieptLayout = new Document(recieptDocument);
            recieptLayout.SetMargins((float)margin, (float)margin, (float)margin, (float)margin);

            PageSize pageSize = new PageSize(PageSize.A8);

            this.recieptDocument.SetDefaultPageSize(pageSize);
            this.lineSeperator = new LineSeparator(new SolidLine((float)lineSepWidth)).SetMargin((float)margin);

        }

        private void GenerateHeader()
        {
            if (!string.IsNullOrEmpty(SalesType) && SalesType == "8")
            {
                recieptLayout.Add(new Paragraph("Credit Request")
                    .SetFontSize((float)dataFontSize)
                    .SetHorizontalAlignment(HorizontalAlignment.CENTER)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetVerticalAlignment(VerticalAlignment.BOTTOM));

                recieptLayout.Add(lineSeperator);
            }
        }

        private void AddCompanyDetails()
        {
            try
            {                
                Paragraph companyName = new Paragraph("Republic Brands LLC").SetVerticalAlignment(VerticalAlignment.TOP).SetBold().SetFontSize((float)headerFontSize).SetMargin((float)margin);

                recieptLayout.Add(companyName);

                Paragraph companyAddress = new Paragraph("2301 Ravine Way,Glenview,IL 60026").SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

                recieptLayout.Add(companyAddress);
            }
            catch (Exception ex)
            {
                var errDescription = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException;

                ErrorLogger.WriteToErrorLog(GetType().Name, "AddCompanyDetails", errDescription);
            }        
        }

        private void AddPhoneandDate()
        {
            Paragraph phoneandDate = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            phoneandDate.Add("Phone: 847-832-9700");
            phoneandDate.Add(new Text("\n"));
            phoneandDate.Add("Fax: 847-832-9710");
            phoneandDate.Add(new Text("\n"));
            phoneandDate.Add("Date :" + DateTime.Now.ToString("d"));
            phoneandDate.Add(new Text("\n"));
            phoneandDate.Add(new Text("\n"));
            phoneandDate.Add("Republic Sales Rep:" + this.CustomerOrderInfo.SalesRepresentative);

            recieptLayout.Add(phoneandDate);

            recieptLayout.Add(this.lineSeperator);
        }

        private void AddCustomerDetails()
        {
            Paragraph customerDetails = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            customerDetails.Add("Transaction #: " + CustomerOrderInfo.PurchaseNumber);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("Purchase Order Number: " + CustomerOrderInfo.PONumber);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("Customer Name: " + CustomerOrderInfo.CustomerName);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("Sellers Representative Tobacco Permit #:" + "\n" + CustomerOrderInfo.PermitNumber);
            customerDetails.Add(new Text("\n"));

            customerDetails.Add("Address: " + this.CustomerOrderInfo.Address);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("City: " + this.CustomerOrderInfo.City);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("State: " + this.CustomerOrderInfo.State);
            customerDetails.Add(new Text("\n"));
            customerDetails.Add("Zip: " + this.CustomerOrderInfo.Zip);

            this.recieptLayout.Add(customerDetails);
        }

        private void AddDistributorAndBookDate()
        {
            Paragraph DistributorAndBookDate = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            // sales type 4 = Suggested order
            if (SalesType == "4")
            {
                if (OrderDetailsData != null)
                {
                    DistributorAndBookDate.Add("Distributor:" + OrderDetailsData[0].DistributorName);
                }
            }

            //  sales type 2 = Prebook
            if (SalesType == "2")
            {
                if (this.OrderDetailsData != null)
                {
                    DistributorAndBookDate.Add("Retailer’s Distributor Customer Number:" + OrderDetailsData[0].RetailDistributorNumber);
                    DistributorAndBookDate.Add("Distributor:" + OrderDetailsData[0].DistributorName);
                    DistributorAndBookDate.Add(new Text("\n"));
                    DistributorAndBookDate.Add("Prebook Ship Date:" + String.Format("d", CustomerOrderInfo.ShipDate));
                }
            }

            //  sales type 13 = Credit Card sale
            if (SalesType == "13")
            {
                DistributorAndBookDate.Add("State Tobacco License:" + OrderDetailsData[0].TobaccoLicense);
                DistributorAndBookDate.Add(new Text("\n"));
                DistributorAndBookDate.Add("Retailer Licence:" + OrderDetailsData[0].RetailerLicense);
            }

            this.recieptLayout.Add(DistributorAndBookDate);
            this.recieptLayout.Add(this.lineSeperator);
            //.SetMargins(0f, 0f, 0f, 0f);
        }


        private void AddTobaccoProductDetails()
        {
            Paragraph orderType = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            // 8 is for Credit request
            if (SalesType == "8")
            {
                orderType.Add("RTN").SetBold();
                recieptLayout.Add(orderType);
            }
            else
            {
                orderType.Add("Tobacco Products").SetBold();
                recieptLayout.Add(orderType);
            }

            recieptLayout.Add(lineSeperator);

            Table productsTable = new Table(4).SetBorder(Border.NO_BORDER).SetFontSize((float)dataFontSize).SetMargin(3f).SetWidth(UnitValue.CreatePercentValue(90));

            productsTable.AddCell("Item No");
            productsTable.AddCell("Qty");
            productsTable.AddCell("Price");
            productsTable.AddCell("Sub Total");

            decimal tobSubTotal = 0;

            foreach (OrderDetails details in this.OrderDetailsData)
            {
                if (details.IsTobaccoProduct == "1")
                {
                    string productNameandNo = string.Concat(details.ProductName, "\n", details.Description);
                    productsTable.AddCell(productNameandNo);
                    productsTable.AddCell(details.Quantity.ToString());
                    productsTable.AddCell("$ " + details.Price.ToString());
                    decimal subtotal = decimal.Multiply(details.Quantity, details.Price);
                    productsTable.AddCell("$ " + subtotal.ToString());
                    tobSubTotal += subtotal;
                }

                //if (String.IsNullOrEmpty(details.IsTobaccoProduct))
                //{
                //    string productNameandNo = String.Concat(details.ProductName == null ? "": details.ProductName, "\n", details.Description == null ? "" : details.Description);
                //    productsTable.AddCell(productNameandNo);
                //    productsTable.AddCell(details.Quantity.ToString());
                //    productsTable.AddCell("$ " + details.Price.ToString());
                //    Decimal subtotal = Decimal.Multiply(details.Quantity, details.Price);
                //    productsTable.AddCell("$ " + subtotal.ToString());
                //    tobSubTotal += subtotal;

                //}
            }

            Paragraph totalTobProducts = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)subTotalFontSize).SetMargin((float)margin).SetBold();

            totalTobProducts.Add("Total Tobacco Products :-");
            totalTobProducts.Add(new Tab());
            totalTobProducts.AddTabStops(new TabStop(50, TabAlignment.RIGHT));
            totalTobProducts.Add("$" + tobSubTotal.ToString());

            Cell tobSubTotalCell = new Cell(1, 4);

            tobSubTotalCell.Add(totalTobProducts);
            productsTable.AddCell(tobSubTotalCell);

            foreach (IElement element in productsTable.GetChildren())
            {
                ((Cell)element).SetBorder(Border.NO_BORDER);
            }

            recieptLayout.Add(productsTable);
        }

        private void AddNonTobaccoProductDetails()
        {
            recieptLayout.Add(lineSeperator);

            Paragraph orderType = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            // 8 is for Credit request
            if (SalesType == "8")
            {
                orderType.Add("DIF").SetBold();
                recieptLayout.Add(orderType);
            }
            else
            {
                orderType.Add("Non - Tobacco Products").SetBold();
                recieptLayout.Add(orderType);
            }

            recieptLayout.Add(this.lineSeperator);
            Border tableBorder = Border.NO_BORDER;

            Table productsTable = new Table(4).SetBorder(tableBorder).SetFontSize((float)dataFontSize).SetMargin((float)margin).SetWidth(UnitValue.CreatePercentValue(90));

            productsTable.AddCell("Item No");
            productsTable.AddCell("Qty");
            productsTable.AddCell("Price");
            productsTable.AddCell("Sub Total");
            decimal nontobProductTotal = 0;
            foreach (OrderDetails details in OrderDetailsData)
            {
                if (details.IsTobaccoProduct == "0")
                {
                    string productNameandNo = string.Concat(details.ProductName, "\n", details.Description);
                    productsTable.AddCell(productNameandNo);
                    productsTable.AddCell(details.Quantity.ToString());
                    productsTable.AddCell("$ " + details.Price.ToString());
                    decimal subtotal = decimal.Multiply(details.Quantity, details.Price);
                    productsTable.AddCell("$ " + subtotal.ToString());
                    nontobProductTotal += (int)subtotal;
                }
            }

            Paragraph totalnonTobProducts = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)subTotalFontSize).SetMargin((float)margin).SetBold();

            totalnonTobProducts.Add("Total Non Tobacco Products :-");
            totalnonTobProducts.Add(new Tab());
            totalnonTobProducts.AddTabStops(new TabStop(50, TabAlignment.RIGHT));
            totalnonTobProducts.Add("$ " + nontobProductTotal.ToString());

            Cell TobGrandTotalCell = new Cell(1, 4);
            TobGrandTotalCell.Add(totalnonTobProducts);
            productsTable.AddCell(TobGrandTotalCell);

            foreach (IElement element in productsTable.GetChildren())
            {
                ((Cell)element).SetBorder(Border.NO_BORDER);
            }

            recieptLayout.Add(productsTable);
        }

        private void AddInvoiceTotal()
        {
            recieptLayout.Add(lineSeperator);

            Paragraph invoiceAmount = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)subTotalFontSize).SetMargin((float)margin).SetBold();

            invoiceAmount.Add("Total Invoice Amount:");
            invoiceAmount.Add(new Tab());
            invoiceAmount.AddTabStops(new TabStop(20, TabAlignment.RIGHT));
            invoiceAmount.Add("$ " + OrderDetailsData[0].GrandTotal.ToString());
            recieptLayout.Add(invoiceAmount);
            recieptLayout.Add(lineSeperator);
        }

        private void AddTaxStatement()
        {
            Paragraph taxStatement = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            taxStatement.Add(this.CustomerOrderInfo.CustomStatement);

            recieptLayout.Add(taxStatement);
        }

        private void AddPrintName()
        {
            Paragraph printName = new Paragraph().SetVerticalAlignment(VerticalAlignment.TOP).SetFontSize((float)dataFontSize).SetMargin((float)margin);

            printName.Add("Print Name :");
            printName.Add(this.CustomerOrderInfo.PrintName);
            printName.Add(new Text("\n"));
            printName.Add("Please Sign Below");
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            printName.Add(new Text("\n"));
            recieptLayout.Add(printName);
            recieptLayout.Add(lineSeperator);
            //.SetMargins(2f, 2f, 2f, 2f);
        }

        private void WriteFileToDisk()
        {
            recieptLayout.Close();
            recieptWriter.Close();
        }
    }
}
