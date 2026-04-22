using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class OrderHistoryDetailsGridUIModel : BaseModel
    {
        public BrandData Brand { get; set; }
        public StyleData Style { get; set; }
        public CategoryMaster Category { get; set; }
        public ProductMaster Product { get; set; }
        public bool IsUpdated { get; set; }

        public OrderDetail OrderDetailObject { get; set; }

        private int _isTobbaco;
        public int IsTobbaco
        {
            get { return _isTobbaco; }
            set { SetProperty(ref _isTobbaco, value); }
        }

        private string _displayBrandName;
        public string DisplayBrandName
        {
            get { return _displayBrandName; }
            set { SetProperty(ref _displayBrandName, value); }
        }


        private string _displayStyleName;
        public string DisplayStyleName
        {
            get { return _displayStyleName; }
            set { SetProperty(ref _displayStyleName, value); }
        }


        private string _displayProductName;
        public string DisplayProductName
        {
            get { return _displayProductName; }
            set { SetProperty(ref _displayProductName, value); }
        }


        private string _displayProductDesc;
        public string DisplayProductDesc
        {
            get { return _displayProductDesc; }
            set { SetProperty(ref _displayProductDesc, value); }
        }

        private string _displayProductQty;
        public string DisplayProductQty
        {
            get { return _displayProductQty; }
            set { SetProperty(ref _displayProductQty, value); }
        }

        private int _productQty;
        public int ProductQty
        {
            get { return _productQty; }
            set { SetProperty(ref _productQty, value); }
        }


        private string _displayProductUnit;
        public string DisplayProductUnit
        {
            get { return _displayProductUnit; }
            set { SetProperty(ref _displayProductUnit, value); }
        }

        private string _displayProductUnitPrice;
        public string DisplayProductUnitPrice
        {
            get { return _displayProductUnitPrice; }
            set { SetProperty(ref _displayProductUnitPrice, value); }
        }

        private decimal _productUnitPrice;
        public decimal ProductUnitPrice
        {
            get { return _productUnitPrice; }
            set { SetProperty(ref _productUnitPrice, value); }
        }

        private string _displayProductSubtotal;
        public string DisplayProductSubtotal
        {
            get { return _displayProductSubtotal; }
            set { SetProperty(ref _displayProductSubtotal, value); }
        }

        private string _creditRequestType;
        public string CreditRequestType
        {
            get { return _creditRequestType; }
            set { SetProperty(ref _creditRequestType, value); }
        }

        public void PropulateUI()
        {
            DisplayBrandName = Brand?.BrandName;
            DisplayProductDesc = Product?.Description;
            DisplayProductName = Product?.ProductName;
            
            DisplayStyleName = Style?.StyleName;
        }
    }
}
