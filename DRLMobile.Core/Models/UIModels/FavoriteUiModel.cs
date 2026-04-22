using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace DRLMobile.Core.Models.UIModels
{
    public class FavoriteUiModel : BaseModel
    {
        private Favorite _favoriteData;
        public Favorite FavoriteData
        {
            get { return _favoriteData; }
            set { SetProperty(ref _favoriteData, value); }
        }

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

        private int _categoryId;
        public int CategoryId
        {
            get { return _categoryId; }
            set { SetProperty(ref _categoryId, value); }
        }

        private string _categoryName;
        public string CategoryName
        {
            get { return _categoryName; }
            set { SetProperty(ref _categoryName, value); }
        }

        private int _brandId;
        public int BrandId
        {
            get { return _brandId; }
            set { SetProperty(ref _brandId, value); }
        }

        private string _brandName;
        public string BrandName
        {
            get { return _brandName; }
            set { SetProperty(ref _brandName, value); }
        }

        private int _styleId;
        public int StyleId
        {
            get { return _styleId; }
            set { SetProperty(ref _styleId, value); }
        }

        private string _styleName;
        public string StyleName
        {
            get { return _styleName; }
            set { SetProperty(ref _styleName, value); }
        }

        private int _productId;
        public int ProductId
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        //private string _productName;
        //public string ProductName
        //{
        //    get { return _productName; }
        //    set { SetProperty(ref _productName, value); }
        //}

        private int _isTobbaco;
        public int isTobbaco
        {
            get { return _isTobbaco; }
            set { SetProperty(ref _isTobbaco, value); }
        }

        private int _isDeleted;
        public int IsDeleted
        {
            get { return _isDeleted; }
            set { SetProperty(ref _isDeleted, value); }
        }

        //private string _productDescription;
        //public string ProductDescription
        //{
        //    get { return _productDescription; }
        //    set { SetProperty(ref _productDescription, value); }
        //}

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        private int _price;
        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }

        private string _updatedDate;
        public string UpdatedDate
        {
            get { return _updatedDate; }
            set { SetProperty(ref _updatedDate, value); }
        }

        private string _deviceOrderID;
        public string DeviceOrderID
        {
            get { return _deviceOrderID; }
            set { SetProperty(ref _deviceOrderID, value); }
        }

        private int? _orderId;
        public int? OrderId
        {
            get { return _orderId; }
            set { SetProperty(ref _orderId, value); }
        }

        private int? _orderDetailId;
        public int? OrderDetailId
        {
            get { return _orderDetailId; }
            set { SetProperty(ref _orderDetailId, value); }
        }

        private string _creditRequest;
        public string CreditRequest
        {
            get { return _creditRequest; }
            set { SetProperty(ref _creditRequest, value); }
        }

        private bool _isAllRowsSelected;
        public bool IsAllRowsSelected
        {
            get { return _isAllRowsSelected; }
            set { SetProperty(ref _isAllRowsSelected, value); }
        }  
        
        public string SearchDisplayPath
        {
            get { return ItemNumber + " " + ItemDescription + " "+BrandName + " "+StyleName; }
        }

        private OrderDetail _orderDetailUiModel;
        public OrderDetail OrderDetailMasterData
        {
            get { return _orderDetailUiModel; }
            set { SetProperty(ref _orderDetailUiModel, value); }
        }

        public void FavoriteUiToDataModel()
        {
            //FavoriteData = new Favorite();
            FavoriteData.ProductName = ItemNumber;
            FavoriteData.ProductDescription = ItemDescription;
            FavoriteData.CategoryId = CategoryId;
            FavoriteData.BrandId = BrandId;
            FavoriteData.StyleId = StyleId;
            FavoriteData.ProductId = ProductId;
            FavoriteData.isTobbaco = isTobbaco;
        }
        public void FavoriteToOrderDetailModel()
        {
            OrderDetailMasterData = new OrderDetail();
            OrderDetailMasterData.ProductName = ItemNumber;
            OrderDetailMasterData.ProductDescription = ItemDescription;
            OrderDetailMasterData.Quantity = Quantity;
            OrderDetailMasterData.ProductId = ProductId;
            OrderDetailMasterData.BrandName = BrandName;
            OrderDetailMasterData.CategoryId = CategoryId;
            OrderDetailMasterData.BrandId = BrandId;
            OrderDetailMasterData.StyleId = StyleId;
            OrderDetailMasterData.StyleName = StyleName;
            OrderDetailMasterData.isTobbaco = isTobbaco;

        }
        private Visibility _isCheckBoxVisible=Visibility.Visible;
        public Visibility IsCheckBoxVisible
        {
            get { return _isCheckBoxVisible; }
            set { SetProperty(ref _isCheckBoxVisible, value); }
        }
    }
}
