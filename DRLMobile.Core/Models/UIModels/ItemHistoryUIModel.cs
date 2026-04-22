using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class ItemHistoryUIModel : BaseModel
    {
        private int _productId;
        public int ProductID
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private string _invoice;
        public string Invoice
        {
            get { return _invoice; }
            set { SetProperty(ref _invoice, value); }
        }

        private string _orderDate;
        public string OrderDate
        {
            get { return _orderDate; }
            set { SetProperty(ref _orderDate, value); }
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


        private ItemHistoryProductModel _product;
        public ItemHistoryProductModel Product
        {
            get { return _product; }
            set { _product = value; }
        }

        private string _uom;
        public string UOM
        {
            get { return _uom; }
            set { SetProperty(ref _uom, value); }
        }

        private OrderMaster _order;
        public OrderMaster Order
        {
            get { return _order; }
            set { _order = value; SetOrderDetails(); }
        }

        private DateTime _OrderOnDate;
        public DateTime OrderOnDate
        {
            get { return _OrderOnDate; }
            set { SetProperty(ref _OrderOnDate, value); }
        }

        public string SearchDisplayPath { get { return Invoice + " " + ProductName + " " + ProductDescription + " " + Price + " " + UOM + " " + OrderDate; } }

        public string SearchDisplayPathForOrderScreen { get { return ProductName + " " + ProductDescription; } }


        private void SetOrderDetails()
        {
            Invoice = Order?.InvoiceNumber;
            OrderDate = Order?.OrderDate.Split(' ')[0];
        }

        
    }
}
