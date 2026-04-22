using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class ProductDetailUiModel : BaseModel
    {
        private string _itemNumber;
        public string ItemNumber
        {
            get { return _itemNumber; }
            set { SetProperty(ref _itemNumber, value); }
        }
        private string _itemDescription;
        public string ItemDescription
        {
            get { return _itemDescription; }
            set { SetProperty(ref _itemDescription, value); }
        }
        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }
        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }
        private string _quantityDisplay;
        public string QuantityDisplay
        {
            get { return _quantityDisplay; }
            set { SetProperty(ref _quantityDisplay, value); }
        }
        private int _selectedProductDetailIndex;
        public int SelectedProductDetailIndex
        {
            get { return _selectedProductDetailIndex; }
            set { SetProperty(ref _selectedProductDetailIndex, value); }
        }
        private string productDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_black.png";
        public string ProductDetailCartImage
        {
            get { return productDetailCartImage; }
            set { SetProperty(ref productDetailCartImage, value); }

        }
        private string productDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/CartBgNormal.png";
        public string ProductDetailCartBackgroundImage
        {
            get { return productDetailCartBackgroundImage; }
            set { SetProperty(ref productDetailCartBackgroundImage, value); }

        }
        private string productImage;
        public string ProductImage
        {
            get { return productImage; }
            set
            {
                if (productImage != value)
                {
                    productImage = value;
                    OnPropertyChanged("ProductImage");
                }
            }
        }
        private string factsheet;
        public string Factsheet
        {
            get { return factsheet; }
            set
            {
                if (factsheet != value)
                {
                    factsheet = value;
                    OnPropertyChanged("Factsheet");
                }
            }
        }
        private string retailImage;
        public string RetailImage
        {
            get { return retailImage; }
            set
            {
                if (retailImage != value)
                {
                    retailImage = value;
                    OnPropertyChanged("RetailImage");
                }
            }
        }
        private string favoriteImage = "ms-appx:///Assets/ProductDetail/favorite_normal.png";
        public string FavoriteImage
        {
            get { return favoriteImage; }
            set
            {
                if (favoriteImage != value)
                {
                    favoriteImage = value;
                    OnPropertyChanged("FavoriteImage");
                }
            }
        }
        private string leftArrowImage= "ms-appx:///Assets/ProductDetail/left_arrow_normal.png";
        public string LeftArrowImage
        {
            get { return leftArrowImage; }
            set
            {
                if (leftArrowImage != value)
                {
                    leftArrowImage = value;
                    OnPropertyChanged("LeftArrowImage");
                }
            }
        }
        private string rightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_normal.png";
        public string RightArrowImage
        {
            get { return rightArrowImage; }
            set
            {
                if (rightArrowImage != value)
                {
                    rightArrowImage = value;
                    OnPropertyChanged("RightArrowImage");
                }
            }
        }
        private string productImagePath;
        public string ProductImagePath
        {
            get { return productImagePath; }
            set { SetProperty(ref productImagePath, value); }
        }
        private string factsheetPath;
        public string FactsheetPath
        {
            get { return factsheetPath; }
            set
            {
                if (factsheetPath != value)
                {
                    factsheetPath = value;
                    OnPropertyChanged("FactsheetPath");
                }
            }
        }
        private string retailImagePath;
        public string RetailImagePath
        {
            get { return retailImagePath; }
            set
            {
                if (retailImagePath != value)
                {
                    retailImagePath = value;
                    OnPropertyChanged("RetailImagePath");
                }
            }
        }


        public string IpImage { get; set; }
        public string SalesDocs { get; set; }

    }
}
