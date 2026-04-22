using Newtonsoft.Json;

using SQLite;

using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class OrderMaster
    {
        [PrimaryKey]
        [JsonProperty("orderid")]
        public int OrderID { get; set; }

        [JsonProperty("deviceorderid")]
        public string DeviceOrderID { get; set; }

        [JsonProperty("customerid")]
        public int CustomerID { get; set; }

        public string DeviceCustomerID { get; set; }
        public int CustomerDistributorID { get; set; }
        public string SalesType { get; set; }

        public int ImportedFrom { get; set; }

        [JsonProperty("isexported")]
        public int IsExported { get; set; }
        public string CustomerName { get; set; }

        public string DeviceDistId { get; set; }
        [JsonProperty("statetobaccolicense")]
        public string StateTobaccoLicence { get; set; }
        public string OrderAddress { get; set; }

        [JsonProperty("ordercityid")]
        public string OrderCityId { get; set; }
        public string OrderZipCode { get; set; }
        public string RetailerLicense { get; set; }
        public string RetailerSalesTaxCertificate { get; set; }
        public string RepublicSalesRepository { get; set; }
        public string CustomStatement { get; set; }

        [JsonProperty("customershippingcityid")]
        public string CustomerShippingCityID { get; set; }
        public string CustomerShippingZipCode { get; set; }

        [JsonProperty("prebookshipdate")]
        public string PrebookShipDate { get; set; }
        public string CustomerSignatureFileName { get; set; }
        public string CustomerComment { get; set; }
        public string EmailRecipients { get; set; }

        public int IsOrderEmailSent { get; set; }
        public int IsOrderConfirmed { get; set; }

        public string ShippingCompany { get; set; }
        public string TrackingNumber { get; set; }
        public string TrackingURL { get; set; }


        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [JsonProperty("zoneid")]
        public int ZoneId { get; set; }

        [JsonProperty("regionid")]
        public int RegionId { get; set; }

        [JsonProperty("territoryid")]
        public int TerritoryId { get; set; }
        [JsonProperty("sellerreptobaccopermitno")]
        public string OrderMasterSellerRepTobacco { get; set; }
        [JsonProperty("sellername")]
        public string OrderMasterSellerName { get; set; }
        public int OpenOrderstatus { get; set; }

        [JsonProperty("invoicenumber")]
        public string InvoiceNumber { get; set; }
        public int CustomerParentID { get; set; }

        [JsonProperty("orderstateid")]
        public int OrderStateId { get; set; }

        [JsonProperty("customershippingstateid")]
        public int CustomerShippingStateID { get; set; }

        [JsonProperty("grandtotal")]
        public string GrandTotal { get; set; }

        [JsonProperty("orderdate")]
        public string OrderDate { get; set; }

        [JsonProperty("purchaseordernumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("printname")]
        public string PrintName { get; set; }
        public int IsVoided { get; set; }
        public bool IsDownload { get; set; }

        [JsonProperty("transactionid")]
        public string TransactionId { get; set; }

        [JsonProperty("transactionstatus")]
        public string TransactionStatus { get; set; }

        [JsonProperty("tenderID")]
        public string TenderID { get; set; }

        [JsonProperty("authresultcode")]
        public string AuthResultCode { get; set; }

        [JsonProperty("receiptnumber")]
        public string ReceiptNumber { get; set; }

        [JsonProperty("createdat")]
        public string CreatedAt { get; set; }

        [JsonProperty("cardbrand")]
        public string CardBrand { get; set; }

        [JsonProperty("cclastfourdigit")]
        public string CCLastFourDigit { get; set; }

        [JsonProperty("retaildistributornumber")]
        public string RetailDistributorNumber { get; set; }

        private int _salestypeFromServer;
        [Ignore]
        [JsonProperty("salestype")]
        public int SalestypeFromServer
        {
            get { return _salestypeFromServer; }
            set
            {
                _salestypeFromServer = value;

                SalesType = Convert.ToString(value);
            }
        }

        private bool _IsEmailSentFromServer;
        [Ignore]
        [JsonProperty("isorderemailsent")]
        public bool IsEmailSentFromServer
        {
            get { return _IsEmailSentFromServer; }
            set
            {
                _IsEmailSentFromServer = value;

                IsOrderEmailSent = value ? 1 : 0;
            }
        }


        private bool _IsOrderConfirmedFromServer;
        [Ignore]
        [JsonProperty("isoderconfirmed")]
        public bool IsOrderConfirmedFromServer
        {
            get { return _IsOrderConfirmedFromServer; }
            set
            {
                _IsOrderConfirmedFromServer = value;

                IsOrderConfirmed = value ? 1 : 0;
            }
        }

        private int _CreatedByFromServer;
        [Ignore]
        [JsonProperty("createdby")]
        public int CreatedByFromServer
        {
            get { return _CreatedByFromServer; }
            set
            {
                _CreatedByFromServer = value;

                CreatedBy = Convert.ToString(value);
            }
        }

        private int _UpdatedByFromServer;
        [Ignore]
        [JsonProperty("updatedby")]
        public int UpdatedByFromServer
        {
            get { return _UpdatedByFromServer; }
            set
            {
                _UpdatedByFromServer = value;

                UpdatedBy = Convert.ToString(value);
            }
        }

        private bool _openorderstatusFromServer;
        [Ignore]
        [JsonProperty("openorderstatus")]
        public bool OpenorderstatusFromServer
        {
            get { return _openorderstatusFromServer; }
            set
            {
                _openorderstatusFromServer = value;

                OpenOrderstatus = value ? 1 : 0;
            }
        }
    }
}