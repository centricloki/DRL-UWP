using DRLMobile.EmailAndPrintOrder;

using System;
using System.Collections.Generic;

using Windows.ApplicationModel.Resources;

namespace DRLMobile.Uwp.Helpers
{
    public static class LoadEmailTemplate
    {
        static List<string> Templates = new List<string>();
        static string Template = string.Empty;

        public static List<string> LoadTemplate(string SalesTypeValue, string CustomerState)
        {
            int SalesType = Convert.ToInt32(SalesTypeValue);
            string Template1;

            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();

            if (SalesType.Equals((int)Helper.SalesType.Cash_Sales) || SalesType.Equals((int)Helper.SalesType.Chain_Distribution))
            {
                Template = resourceLoader.GetString("TemplateRetailSale");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Prebook))
            {
                Template = resourceLoader.GetString("TemplateRetailPrebook");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Bill_Through))
            {
                Template = resourceLoader.GetString("TemplateRetailBillThrough");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Suggested_Order))
            {
                Template = resourceLoader.GetString("TemplateSuggestedOrder");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Rack_POS) || SalesType.Equals((int)Helper.SalesType.POP))
            {
                Template = resourceLoader.GetString("TemplateRackPop");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Distributor_Order))
            {
                Template = resourceLoader.GetString("TemplatePurchaseOrder");
                Template1 = resourceLoader.GetString("TemplateTopPurchaseOrder");

                if (CustomerState == "AZ")
                {
                    Templates.Add(Template);
                    Templates.Add(Template1);
                }
                else
                {
                    Templates.Add(Template);
                }
            }
            else if (SalesType.Equals((int)Helper.SalesType.Credit_Request))
            {
                Template = resourceLoader.GetString("TemplateCreditRequest");
                Template1 = resourceLoader.GetString("TemplateTopCreditRequest");

                if (CustomerState == "AZ")
                {
                    Templates.Add(Template);
                    Templates.Add(Template1);
                }
                else
                {
                    Templates.Add(Template);
                }
            }
            else if (SalesType.Equals((int)Helper.SalesType.Cash_Sales_Initiative))
            {
                Template = resourceLoader.GetString("TemplateCashSalesInitiative");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Sample_Order))
            {
                Template = resourceLoader.GetString("TemplateSampleOrder");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Credit_Card_Sales))
            {
                Template = resourceLoader.GetString("TemplateCardSale");
                Templates.Add(Template);
            }
            else if (SalesType.Equals((int)Helper.SalesType.Car_Stock_Order))
            {
                Template = resourceLoader.GetString("TemplateCarStockOrder");
                Templates.Add(Template);
            }

            return Templates;
        }

        /// <summary>
        /// Loads print template
        /// </summary>
        /// <returns></returns>
        public static string LoadPrintRecieptTemplate()
        {
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            string Template = resourceLoader.GetString("TemplateRecieptPrint");
            return Template;
        }
    }


}