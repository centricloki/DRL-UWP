using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;
using System;
using System.Collections.Generic;

namespace DRLMobile.Core.Models.UIModels
{
    public class OrderHistoryUIModel : BaseModel
    {
        private string _invoice;
        public string Invoice
        {
            get { return _invoice; }
            set { SetProperty(ref _invoice, value); }
        }

        private string _orderType;
        public string OrderType
        {
            get { return _orderType; }
            set { SetProperty(ref _orderType, value); }
        }

        private string _distributorName;
        public string DistributorName
        {
            get { return _distributorName; }
            set { SetProperty(ref _distributorName, value); }
        }


        private string _totalQuantity;
        public string TotalQuantity
        {
            get { return _totalQuantity; }
            set { SetProperty(ref _totalQuantity, value); }
        }

        private string _totalAmount;
        public string TotalAmount
        {
            get { return _totalAmount; }
            set { SetProperty(ref _totalAmount, value); }
        }

        private string _orderPlacedOn;
        public string OrderPlacedOn
        {
            get { return _orderPlacedOn; }
            set { SetProperty(ref _orderPlacedOn, value); }
        }

        private string _shippingCompany;
        public string ShippingCompany
        {
            get { return _shippingCompany; }
            set { SetProperty(ref _shippingCompany, value); }
        }

        private string _trackingNumber;
        public string TrackingNumber
        {
            get { return _trackingNumber; }
            set { SetProperty(ref _trackingNumber, value); }
        }

        private string _trackingUrl;
        public string TrackingUrl
        {
            get { return _trackingUrl; }
            set { SetProperty(ref _trackingUrl, value); }
        }

        public int OrderId { get; set; }
        
        public string DeviceOrderID { get; set; }
        
        public int IsOrderConfirmed { get; set; }
        public int IsExported { get; set; }
        public string SignaturePath { get; set; }

        private DateTime _OrderOnDate;
        public DateTime OrderOnDate
        {
            get { return _OrderOnDate; }
            set { SetProperty(ref _OrderOnDate, value); }
        }

        private string _salesType;
        public string SalesType
        {
            get { return _salesType; }
            set { _salesType = value; SetOrderType(); }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }


        private string _productDescription;
        public string ProductDescription
        {
            get { return _productDescription; }
            set { SetProperty(ref _productDescription, value); }
        }

        public string SearchDisplayPath
        {
            get
            {
                return Invoice + " " + OrderType + " " + DistributorName + " " + TotalAmount + " " +
                    OrderPlacedOn + " " + ShippingCompany + " " + TrackingNumber + " " + TrackingUrl;
            }
        }

        private void SetOrderType()
        {
            try
            {
                OrderType = Helpers.HelperMethods.GetSalesTypeString(SalesType);
            }
            catch (Exception e)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryUIModel), nameof(SetOrderType), e.StackTrace);
            }
        }
    }
}
