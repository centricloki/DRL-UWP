using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using iText.Kernel.Pdf;
using iText.Html2pdf;
using iText.Kernel.Geom;

namespace DRLMobile.EmailAndPrintOrder
{
    public class BuildOrderEmail
    {
        private static string rackpoporder = "NO";
        string FilledTemplate = string.Empty;
        readonly string xmlpath = @"C:\orderdataxml";
        private readonly string autoitem = @"TUOLTR";
        private readonly string autoitemqty = @"1";

        /// <summary>
        /// Load the required base template
        /// </summary>
        /// <param name="SalesType">sales type for email to be generated</param>
        /// <param name="CustomerState">customer state</param>
        /// <returns></returns>
        
        /// <summary>
        /// truncates as per the length 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private string Truncate(string value, int maxLength)
        {
            if (value == null)
            {
                return "";
            }
            if (maxLength < 0)
            {
                return "";
            }
            return value?.Substring(0, Math.Min(value.Length, maxLength));
        }

        /// <summary>
        /// splits word by length to adjust the template.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static List<string> SplitWordsByLength(string str, int maxLength)
        {
            List<string> chunks = new List<string>();
            while (str.Length > 0)
            {
                if (str.Length <= maxLength)                    //if remaining string is less than length, add to list and break out of loop
                {
                    chunks.Add(str);
                    break;
                }

                string chunk = str.Substring(0, maxLength);     //Get maxLength chunk from string.

                if (char.IsWhiteSpace(str[maxLength]))          //if next char is a space, we can use the whole chunk and remove the space for the next line
                {
                    chunks.Add(chunk);
                    str = str.Substring(chunk.Length + 1);
                }
                else
                {
                    int splitIndex = chunk.LastIndexOf(' ');    //Find last space in chunk.
                    if (splitIndex != -1)                       //If space exists in string,
                        chunk = chunk.Substring(0, splitIndex); //  remove chars after space.
                    str = str.Substring(chunk.Length + (splitIndex == -1 ? 0 : 1));
                    chunks.Add(chunk);                          //Add to list
                }
            }
            return chunks;
        }

        /// <summary>
        /// Build email body
        /// </summary>
        /// <param name="Body">Body</param>
        /// <param name="customerInfo">Customer info</param>
        /// <param name="SalesType">Sales type for the order</param>
        /// <param name="orderID">order id</param>
        /// <param name="xmlbuild"></param>
        /// <returns></returns>
        public string BuildEmailBody(List<OrderDetails> Body, CustomerInfo customerInfo, string SalesType, string BrandImagePath, List<string> TemplateToLoad)
        {
            List<string> Templates = TemplateToLoad;
            StringBuilder xmlbuild = new StringBuilder();
            string Template = string.Empty;

            bool OrderXMLExist = false;
            StringBuilder TobaccoProduct = new StringBuilder();
            StringBuilder NotTobaccoProduct = new StringBuilder();
            StringBuilder PromoProduct = new StringBuilder();
            StringBuilder PriceListProduct = new StringBuilder();
            StringBuilder TopPromoProduct = new StringBuilder();
            StringBuilder TopPriceListProduct = new StringBuilder();
            StringBuilder RTFProduct = new StringBuilder();
            StringBuilder DIFProduct = new StringBuilder();
            string ItemDetails = string.Empty;
            decimal SubTotalTP = 0;
            decimal SubTotalNTP = 0;
            decimal GrandTotal = 0;
            string heading = string.Empty;
            string Distributor = string.Empty;
            string DistributorName = string.Empty;
            string DistributorAddress = string.Empty;
            string RetailerLicense = string.Empty;
            string SalesTaxCertificate = string.Empty;
            string TobaccoLicense = string.Empty;
            string Units = string.Empty;
            string ReturnReason = string.Empty;

            //Added by Senthil Ramadoss on 01/28/2020 - Rack pop order xml build
            StringBuilder xmlbuildDetails = new StringBuilder();
            if (SalesType == "5" || SalesType == "10")//  salestype 10 added by senthil ramadoss on 02-21-2020
            {
                if (!OrderXMLExist)
                    xmlbuildDetails.AppendLine("<Detail>");
            }

            foreach (OrderDetails item in Body)
            {
                if (SalesType == "2")
                    ItemDetails = "<tr><td style ='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + item.ProductName +
                         "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Description +
                         "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.UOM +
                         "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>&nbsp;&nbsp;<br><br>" +
                         "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Quantity + "</td></tr>";
                if (SalesType == "3")
                    ItemDetails = "<tr><td style ='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + item.ProductName +
                       "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Description +
                       "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.UOM +
                       "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Quantity + "</td></tr>";

                if (SalesType == "5" || SalesType == "10")//  salestype 10 added by senthil ramadoss on 10-01-2018
                {
                    if (!OrderXMLExist)
                    {
                        xmlbuildDetails.AppendLine("<LineItem>");
                        if (item.ProductName != null)
                            xmlbuildDetails.AppendLine("<ItemNumber>" + Truncate(item.ProductName.Trim(), 15) + "</ItemNumber>");
                        else
                            xmlbuildDetails.AppendLine("<ItemNumber></ItemNumber>");
                        xmlbuildDetails.AppendLine("<OrderQuantity>" + item.Quantity + "</OrderQuantity>");
                        xmlbuildDetails.AppendLine("</LineItem>");
                    }
                    //Added by Senthil Ramadoss on 01/28/2020 - Rack pop order xml build


                    ItemDetails = "<tr><td style ='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + item.ProductName +
                        "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Description +
                        "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.UOM +
                        "</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-left:1px solid black;'>" + item.Quantity + "</td></tr>";

                }

                if (SalesType == "4" || SalesType == "1" || SalesType == "9" || SalesType == "11" || SalesType == "12" || SalesType == "13")
                {
                    ItemDetails = "<tr><td style ='text-align: center;border-left:1px solid black;border-bottom:1px solid black;'>" + item.ProductName +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Description +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.UOM +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Price +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Quantity +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + ((item.Price * item.Quantity).ToString("C2")) +
                         "</td></tr>";
                }
                if (SalesType == "6")
                {
                    ItemDetails = "<tr><td style ='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + item.ProductName +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Description +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.UOM +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Quantity + "</td></tr>";

                }
                //Lisa - added 3/23/16 for new return sales type
                if (SalesType == "8")
                {
                    ItemDetails = "<tr><td style='text-align: center;border-bottom:1px solid black;'>" + item.ProductName +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Description +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.UOM +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.ReturnReason +
                         "</td><td style='text-align: center; padding: 5px;border-left:1px solid black;border-bottom:1px solid black;'>" + item.Quantity + "</td></tr>";

                }
                //checking if the product is a tobacco or non tobacco

                if (SalesType == "8")
                {
                    NotTobaccoProduct.Append(ItemDetails);
                }
                else
                {
                    if (item.IsTobaccoProduct == "True")
                    {
                        if (SalesType == "6" && customerInfo.State == "AZ")
                        {
                            if (item.IsPromotional == "True")
                            {
                                TopPromoProduct.Append(ItemDetails);
                            }
                            else
                            {
                                TopPriceListProduct.Append(ItemDetails);
                            }
                        }
                        else
                        {

                            TobaccoProduct.Append(ItemDetails);
                        }
                        SubTotalTP += (item.Price * item.Quantity);
                    }
                    else
                    {
                        if (SalesType == "6" && customerInfo.State == "AZ")
                        {
                            if (item.IsPromotional == "True")
                            {
                                PromoProduct.Append(ItemDetails);
                            }
                            else
                            {
                                PriceListProduct.Append(ItemDetails);
                            }
                        }
                        else
                            NotTobaccoProduct.Append(ItemDetails);
                        SubTotalNTP += (item.Price * item.Quantity);
                    }
                    if (SalesType == "6" && customerInfo.State != "AZ")
                    {
                        if (item.IsPromotional == "True")
                        {
                            PromoProduct.Append(ItemDetails);
                        }
                        if (item.IsPromotional == "False")
                        {
                            PriceListProduct.Append(ItemDetails);
                        }
                    }
                }

                GrandTotal = item.GrandTotal;
                Distributor = item.DistributorID;
                DistributorName = item.DistributorName;
                DistributorAddress = item.DistributorCity + ", " + item.DistributorState + ", " + item.DistributorZip;
                RetailerLicense = item.RetailerLicense;
                SalesTaxCertificate = item.SalesTaxCertificate;
                TobaccoLicense = item.TobaccoLicense;

            }
            StringBuilder xmlbuildHeader = new StringBuilder();
            if (SalesType == "5" || SalesType == "10")//  salestype 10 added by senthil ramadoss on 10-01-2018
            {
                if (!OrderXMLExist)
                {
                    xmlbuildDetails.AppendLine("<LineItem>");
                    xmlbuildDetails.AppendLine("<ItemNumber>" + autoitem + "</ItemNumber>");
                    xmlbuildDetails.AppendLine("<OrderQuantity>" + autoitemqty + "</OrderQuantity>");
                    xmlbuildDetails.AppendLine("</LineItem>");
                    xmlbuildDetails.Append("</Detail>"); //Added by Senthil Ramadoss on 01/28/2020 - Rack pop order xml build
                    xmlbuildHeader.Append("<Header>");
                }
            }

            StringBuilder tableTP = new StringBuilder();
            StringBuilder tableDIF = new StringBuilder();
            StringBuilder tableRTF = new StringBuilder();
            StringBuilder tableNTP = new StringBuilder();
            StringBuilder tablePromoPO = new StringBuilder();
            StringBuilder tablePriceListPO = new StringBuilder();
            StringBuilder ToptablePromoPO = new StringBuilder();
            StringBuilder ToptablePriceListPO = new StringBuilder();
            string header = string.Empty;
            string headerTP = string.Empty;
            string headerReturn = string.Empty;
            string headerNTP = string.Empty;
            string headerPO = string.Empty;
            string headerPL = string.Empty;
            string headerTPO = string.Empty;
            string headerTPL = string.Empty;
            string subtotalRowTP = string.Empty;
            string subtotalRowNTP = string.Empty;
            string GrandtotalRow = string.Empty;
            //Added by Senthil Ramadoss on 01/28/2020 - Rack pop order xml build


            // checking if there is data for table.If present then show table.
            if (SalesType == "6")
            {

                header = "<table width=800 style ='border-top:1px solid black; border-right:1px solid black;border-left:1px solid black;width:100%'><tr><td style='border-right:1px solid black; background-color:black; color: white; text-align: left' colspan=100 %>" +
                          "H" + "</td></tr>" +
                          "<tr><th style ='border-bottom:1px solid black;'>" +
                          "RT Item#</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                          "Description</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                          "UOM</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Quantity" +
                          "</th></tr>";

            }
            if (SalesType == "2")
            {
                header = "<table width=800 style ='border-top:1px solid black;border-right:1px solid black;border-left:1px solid black; width:100%'><tr><td style='border:1px solid black; background-color:black; color: white; text-align: left' colspan=100 %>" +
                          "H" + "</td></tr>" +
                          "<tr><th style ='border-bottom:1px solid black;'>" +
                          "RT Item#</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                          "Description</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                          "Unit of Measure</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                          "Distributor UIN</th><th style ='border-bottom:1px solid black;'>Quantity" +
                          "</th></tr>";
            }
            //Lisa - new sales type for returns 3/24/16
            if (SalesType == "8")
            {
                header = "<table width=800 style ='border-left:1px solid black;border-top:1px solid black; border-right:1px solid black;width:100%'><tr><td style='border-left:1px solid black;border-bottom:1px solid black; background-color:black; color: white; text-align: left' colspan=100 %>" +
                          "H" + "</td></tr>" +
                         "<tr><th style ='border-bottom:1px solid black;border-bottom:1px solid black;'>" +
                          "RT Item#</th><th style ='border-left:1px solid black;border-bottom:1px solid black;border-bottom:1px solid black;'>" +
                          "Description</th><th style ='border-left:1px solid black;border-bottom:1px solid black;border-bottom:1px solid black;'>" +
                          "UOM</th><th style ='border-left:1px solid black;border-bottom:1px solid black;border-bottom:1px solid black;'>" +
                          "Return Reason</th><th style ='border-left:1px solid black;border-bottom:1px solid black;'>Quantity" +
                          "</th></tr>";
            }
            if (SalesType == "3" || SalesType == "5" || SalesType == "10")//  salestype 10 added by senthil ramadoss on 10-01-2018
            {


                header = "<table width=800 style ='border-top:1px solid black;border-right:1px solid black;border-left:1px solid black; width:100%'><tr><td style='border:1px solid black; background-color:black; color: white; text-align: left' colspan=100 %>" +
                           "H" + "</td></tr>" +
                           "<tr><th style ='border-bottom:1px solid black;'>" +
                           "RT Item#</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>" +
                           "Description</th><th style ='border-bottom:1px solid black;border-left:1px solid black;;'>" +
                           "UOM</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Quantity" +
                           "</th></tr>";
            }

            if (SalesType == "4" || SalesType == "1" || SalesType == "9" || SalesType == "11" || SalesType == "12" || SalesType == "13")
            {
                header = "<table width='" + 800 + "' style ='border-top:1px solid black; border-right:1px solid black;width:100%'><tr><td style='border:1px solid black; background-color:black; color: white; text-align: left' colspan=100 %>" + "H" + "</td></tr>" +
                        "<tr><th style ='border-bottom:1px solid black;border-left:1px solid black;'>RT Item#</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Description</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Unit</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Unit Price" +
                        "</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Quantity</th><th style ='border-bottom:1px solid black;border-left:1px solid black;'>Net Price</th></tr>";
                subtotalRowTP = "<tr><td></td><td></td><td></td><td></td><td style ='background-color:black; color: white; text-align: left'>Sub Total</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + SubTotalTP.ToString("C2") + "</td></tr>";
                subtotalRowNTP = "<tr><td></td><td></td><td></td><td></td><td style ='background-color:black; color: white; text-align: left'>Sub Total</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;'>" + SubTotalNTP.ToString("C2") + "</td></tr>";

                if (SalesType == "13")
                {
                    GrandtotalRow = @"<table width = '" + 800 + "' style = 'margin-right: 0px;margin-left: auto;' >" +
                                        "<tr ><td style = 'width:125px' ></td ><td style = 'width:130px' ></td><td style = 'width:175px' ></td ><td style = 'width:115px' ></td ><td style = 'width:110px;border-bottom:1px solid black;background-color:black; color: white; text-align: left' > Invoice Total </td><td style = 'text-align: center; padding: 5px;border-bottom:1px solid black;border-right:1px solid black;border-top:1px solid black;' > " + GrandTotal.ToString("C2") + " </td ></tr ></table> " +
                                      "<table width = '" + 800 + "'  ><tr ><td style = 'width:125px' ></td ><td style = 'width:130px' ></td><td style = 'width:175px' ></td ><td style = 'width:115px' ></td ><td  colspan=2 style = ' padding: 5px;border-bottom:1px solid black;border-right:1px solid black;border-top:1px solid black;border-left:1px solid black;'><font face = 'arial' >     " + customerInfo.CreatedAt + "<br />Receipt number:# " + customerInfo.ReceiptNo + " <br />Auth Code: " + customerInfo.AuthCode + "<br />Card: " + customerInfo.CCBrand + " " + customerInfo.CCLFDigit + "<br /></td></tr></table>";

                }
                else
                {
                    GrandtotalRow = "<table width='" + 800 + "' style ='margin-right: 0px;margin-left: auto;'><tr><td style='width:125px'></td><td style='width:130px'></td><td style='width:175px'></td><td style='width:115px'></td><td style ='width:110px;border-bottom:1px solid black;background-color:black; color: white; text-align: left'>Invoice Total</td><td style='text-align: center; padding: 5px;border-bottom:1px solid black;border-right:1px solid black;border-top:1px solid black;'>" + GrandTotal.ToString("C2") + "</td></tr></table>";
                }

            }
            if (SalesType == "8")
            {
                heading = "Products";
                headerNTP = header.Replace("H", heading);
                tableNTP.Append(headerNTP);
                tableNTP.Append(NotTobaccoProduct);
                tableNTP.Append("</table>");
            }
            else
            {
                if (TobaccoProduct.Length != 0 && SalesType != "6")
                {
                    heading = "Tobacco Products";
                    headerTP = header.Replace("H", heading);
                    tableTP.Append(headerTP);
                    tableTP.Append(TobaccoProduct);
                    tableTP.Append(subtotalRowTP + "</table>");
                }
                if (NotTobaccoProduct.Length != 0 && SalesType != "6")
                {
                    heading = " Non Tobacco Products";
                    headerNTP = header.Replace("H", heading);
                    tableNTP.Append(headerNTP);
                    tableNTP.Append(NotTobaccoProduct);
                    tableNTP.Append(subtotalRowNTP + "</table>");
                }
                if (PromoProduct.Length != 0)
                {
                    heading = "Promotional Products";
                    headerPO = header.Replace("H", heading);
                    tablePromoPO.Append(headerPO);
                    tablePromoPO.Append(PromoProduct + "</table>");
                }
                if (PriceListProduct.Length != 0)
                {
                    heading = "Price List Products";
                    headerPL = header.Replace("H", heading);
                    tablePriceListPO.Append(headerPL);
                    tablePriceListPO.Append(PriceListProduct + "</table>");
                }
                if (TopPromoProduct.Length != 0)
                {
                    heading = "Promotional Products";
                    headerTPO = header.Replace("H", heading);
                    ToptablePromoPO.Append(headerTPO);
                    ToptablePromoPO.Append(TopPromoProduct + "</table>");
                }

                if (TopPriceListProduct.Length != 0)
                {
                    heading = "Price List Products";
                    headerTPL = header.Replace("H", heading);
                    ToptablePriceListPO.Append(headerTPL);
                    ToptablePriceListPO.Append(TopPriceListProduct + "</table>");
                }
            }

            if (SalesType == "6" && PromoProduct.Length == 0 && PriceListProduct.Length == 0)
                Templates.Remove(Template);
            if (SalesType == "6" && TopPromoProduct.Length == 0 && TopPriceListProduct.Length == 0)
                Templates.Remove(Template);
            if (SalesType == "8" && customerInfo.State == "AZ" && NotTobaccoProduct.Length == 0)
                Templates.Remove(Template);
            //replacing tokens with data in the template
            foreach (var t in Templates)
            {
                string username = customerInfo.UserName;
                string custno = customerInfo.CustomerNumber;

                if (SalesType == "5" || SalesType == "10")//  salestype 10 added by senthil ramadoss on 10-01-2018
                {
                    if (!OrderXMLExist)
                    {
                        xmlbuildHeader.AppendLine("<Company>10</Company>");
                        xmlbuildHeader.AppendLine("<CustomerNumber>" + Truncate(custno, 10) + "</CustomerNumber>");
                        xmlbuildHeader.AppendLine("<OrderType>007</OrderType>");
                        xmlbuildHeader.AppendLine("<PurchaseOrderNumber>" + Truncate(customerInfo.PurchaseNumber, 20) + "</PurchaseOrderNumber>");
                        xmlbuildHeader.AppendLine("<OurReferenceNumber></OurReferenceNumber>");
                        xmlbuildHeader.AppendLine("</Header>");

                        //Added by Senthil Ramadoss on 01/28/2020 - Rack pop order xml build
                        StringBuilder xmlbuildAdds = new StringBuilder();
                        xmlbuildAdds.AppendLine("<Address>");
                        xmlbuildAdds.AppendLine("<AddressType>1</AddressType>");
                        xmlbuildAdds.AppendLine("<AddressID></AddressID>");
                        if (customerInfo.CustomerName.Trim().ToUpper().Length > 0)
                        {
                            customerInfo.CustomerName = customerInfo.CustomerName.Trim().ToUpper().Replace("'", "");
                        }
                        xmlbuildAdds.AppendLine("<CustomerName>" + Truncate(SecurityElement.Escape(customerInfo.CustomerName.Trim().ToUpper()), 36) + "</CustomerName>");

                        List<string> addslist = SplitWordsByLength(customerInfo.Address.Trim(), 30);
                        int j = 1;
                        foreach (string adds in addslist)
                        {
                            string newtext = adds.Replace("\r\n", "");
                            newtext = newtext.Replace("\n", "");
                            xmlbuildAdds.Append("<AddressLine" + j + ">" + SecurityElement.Escape(newtext) + "</AddressLine" + j + ">");
                            j++;
                        }
                        if (j == 2)
                        {
                            xmlbuildAdds.Append("<AddressLine2></AddressLine2>");
                            xmlbuildAdds.Append("<AddressLine3></AddressLine3>");
                        }
                        else if (j == 3)
                        {
                            xmlbuildAdds.Append("<AddressLine3></AddressLine3>");
                        }

                        xmlbuildAdds.Append("<City>" + Truncate(customerInfo.City, 36) + "</City>");
                        xmlbuildAdds.Append("<State>" + Truncate(customerInfo.State, 2) + "</State>");
                        xmlbuildAdds.Append("<PostalCode>" + Truncate(customerInfo.Zip, 10) + "</PostalCode>");
                        xmlbuildAdds.Append("<Country>USA</Country>");
                        xmlbuildAdds.Append("</Address>");

                        StringBuilder xmlbuildComments = new StringBuilder();
                        xmlbuildComments.AppendLine("<Comments>");
                        if (customerInfo.CustomerComment.Trim() != "")
                        {
                            string[] coments = customerInfo.CustomerComment.Trim().Split(new[] { "\n" }, StringSplitOptions.None);
                            foreach (string com in coments)
                            {
                                string newcomtext = com.Trim();
                                newcomtext = newcomtext.Replace("\n", "");
                                if (newcomtext != "")
                                    xmlbuildComments.Append("<Comment>" + Truncate(SecurityElement.Escape(newcomtext), 240) + "</Comment>");
                                else
                                    xmlbuildComments.Append("<Comment></Comment>");
                            }
                        }
                        else
                        {
                            xmlbuildComments.Append("<Comment></Comment>");
                        }

                        xmlbuildComments.AppendLine("</Comments>");
                        rackpoporder = "YES";

                        xmlbuild.AppendLine(xmlbuildHeader.ToString());

                        xmlbuild.AppendLine(xmlbuildAdds.ToString());
                        xmlbuild.AppendLine(xmlbuildComments.ToString());
                        xmlbuild.AppendLine(xmlbuildDetails.ToString());
                        xmlbuild.AppendLine("</Order>");


                    }
                }

                FilledTemplate = String.Format(t, customerInfo.PermitNumber, customerInfo.SalesRepresentative, customerInfo.SalesRepPhone, customerInfo.SalesRepEmail,
                                                         customerInfo.OrderDate, customerInfo.PurchaseNumber, customerInfo.CustomerNumber, customerInfo.CustomerName,
                                                         customerInfo.Address, customerInfo.City + ", " + customerInfo.State + ", " + customerInfo.Zip,
                                                         "", "", tableNTP, tableTP,
                                                         customerInfo.CustomerComment, customerInfo.PrintName, "", Distributor, DistributorName, DistributorAddress,
                                                         customerInfo.ShipDate, customerInfo.RTAccountRep, TobaccoLicense, RetailerLicense, SalesTaxCertificate,
                                                         customerInfo.CustomStatement, customerInfo.PONumber, tablePriceListPO, tablePromoPO, ToptablePriceListPO, ToptablePromoPO, GrandtotalRow,
                                                         customerInfo.ContactEmail, customerInfo.ContactPhone);

                string finalTemplate = FilledTemplate.Replace("cid:MyPic", BrandImagePath);
            }

            return FilledTemplate;
        }

        /// <summary>
        /// Generates the pdf file on the given path
        /// </summary>
        /// <param name="destinationPath">full path of the file including the extension.pdf</param>
        /// <param name="htmltoconvert">html string to convert</param>
        /// <returns></returns>
        public bool GenerateOrderPDF(string destinationPath, string htmltoconvert)
        {
            PdfWriter writer = new PdfWriter(destinationPath);

            try
            {

                PdfDocument document = new PdfDocument(writer);
                PageSize size = new PageSize(675, 850);
                document.SetDefaultPageSize(size);
                ConverterProperties properties = new ConverterProperties();
                HtmlConverter.ConvertToPdf(htmltoconvert, document, properties);
                writer.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }

            return true;


        }
    }
}