using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class AddEditProductOrderHistoryUIModel : BaseModel
    {
        private bool _isCreditRequest;

        public bool IsCreditRequest
        {
            get { return _isCreditRequest; }
            set { SetProperty(ref _isCreditRequest, value); }
        }

        private ObservableCollection<ProductMaster> _productList;
        public ObservableCollection<ProductMaster> ProductList
        {
            get { return _productList; }
            set { SetProperty(ref _productList, value); }
        }



        public BrandData SelectedBrand
        {
            get;
            set;
        }

        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set { SetProperty(ref _brandName, value); }
        }

        public CategoryMaster SelectedCategory
        {
            get;
            set;
        }

        private string _styleName;
        public string StyleName
        {
            get { return _styleName; }
            set { SetProperty(ref _styleName, value); }
        }

        public StyleData SelectedStyle
        {
            get;
            set;
        }

        public ProductMaster SelectedProduct
        {
            get;
            set;
        }

        public OrderDetail EditedOrderDetail { get; set; }

        private string _selectedUom;
        public string SelectedUom
        {
            get { return _selectedUom; }
            set { SetProperty(ref _selectedUom, value); }
        }

        private string _selectedCreditRequest;
        public string SelectedCreditRequest
        {
            get { return _selectedCreditRequest; }
            set { SetProperty(ref _selectedCreditRequest, value); }
        }

        private string _quantity;
        public string Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }


        private string _price;
        public string Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private string _priceToSave;
        public string PriceToSave
        {
            get { return _priceToSave; }
            set { SetProperty(ref _priceToSave, value); }
        }


        private string _subTotal;
        public string SubTotal
        {
            get { return _subTotal; }
            set { SetProperty(ref _subTotal, value); }
        }


        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }

        private string _productDesc;
        public string ProductDesc
        {
            get { return _productDesc; }
            set { SetProperty(ref _productDesc, value); }
        }

        private string _autoSuggestionText;
        public string AutoSuggestionText
        {
            get { return _autoSuggestionText; }
            set { SetProperty(ref _autoSuggestionText, value); }
        }

        public AddEditProductOrderHistoryUIModel()
        {
            ProductList = new ObservableCollection<ProductMaster>();
            SelectedBrand = null;
            SelectedCategory = null;
            SelectedProduct = null;
            SelectedStyle = null;
            SelectedUom = "EA";
            Quantity = "1";
            Price = "$0.00";
            SubTotal = "$0.00";
            StyleName = string.Empty;
            BrandName = string.Empty;
            ProductName = string.Empty;
            ProductDesc = string.Empty;
        }


    }
}
