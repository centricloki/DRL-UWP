using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;

namespace DRLMobile.Core.Models.UIModels
{
    public class PopOrderCartUiModel : BaseModel
    {
        ///private readonly string PlaceholderImage = "ms-appx:///Assets/RackOrder/rack_item_placeholder.png";
        public PopOrderCartUiModel()
        {
            ProductImagePath = Application.Current.Resources["PlaceholderImage"] as string;
        }
        private OrderDetail _orderDetailMasterData;
        public OrderDetail OrderDetailMasterData
        {
            get { return _orderDetailMasterData; }
            set { SetProperty(ref _orderDetailMasterData, value); }
        }
        private string _productAdditionalDocumentID;
        public string ProductAdditionalDocumentID
        {
            get { return _productAdditionalDocumentID; }
            set { SetProperty(ref _productAdditionalDocumentID, value); }
        }
        private int _productId;
        public int ProductID
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private string _documentFileName;
        public string DocumentFileName
        {
            get { return _documentFileName; }
            set { SetProperty(ref _documentFileName, value); }
        }
        private string _documentType;
        public string DocumentType
        {
            get { return _documentType; }
            set { SetProperty(ref _documentType, value); }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private int _price;
        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }
        public int ProductType { get; set; }
        public string ImportedFrom { get; set; }
        public string DistributionRecordedDate { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int Status { get; set; }
        public int LangID { get; set; }
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
        public int isDistributed { get; set; }
        public int CatId { get; set; }
        public int BrandId { get; set; }
        public int StyleId { get; set; }
        public string UOM { get; set; }
        public string Total { get; set; }
        public int IsDeleted { get; set; }
        private int _sRCHoneySellable;
        public int SRCHoneySellable
        {
            get { return _sRCHoneySellable; }
            set { SetProperty(ref _sRCHoneySellable, value); }
        }

        private int _sRCHoneyReturnable;
        public int SRCHoneyReturnable
        {
            get { return _sRCHoneyReturnable; }
            set { SetProperty(ref _sRCHoneyReturnable, value); }
        }

        private int _sRCCanIOrder;
        public int SRCCanIOrder
        {
            get { return _sRCCanIOrder; }
            set { SetProperty(ref _sRCCanIOrder, value); }
        }

        private int _isTobbaco;
        public int isTobbaco
        {
            get { return _isTobbaco; }
            set { SetProperty(ref _isTobbaco, value); }
        }

        private string _productImage;
        public string ProductImage
        {
            get { return _productImage; }
            set
            {
                SetProperty(ref _productImage, value);
            }
        }
        private string _productImagePath;
        public string ProductImagePath
        {
            get { return _productImagePath; }
            set { SetProperty(ref _productImagePath, value); }
        }
        private string cartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";
        public string CartImage
        {
            get { return cartImage; }
            set { SetProperty(ref cartImage, value); }

        }
        private bool _isAddedInGrid;
        public bool IsAddedInGrid
        {
            get { return _isAddedInGrid; }
            set { SetProperty(ref _isAddedInGrid, value); }
        }
        private bool _isAddedToCart;
        public bool IsAddedToCart
        {
            get { return _isAddedToCart; }
            set { SetProperty(ref _isAddedToCart, value); }
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
        public void OrderDetailUiToDataModel()
        {
            OrderDetailMasterData = new OrderDetail();
            OrderDetailMasterData.ProductName = ProductName;
            OrderDetailMasterData.ProductDescription = Description;
            OrderDetailMasterData.CategoryId = CatId;
            OrderDetailMasterData.BrandId = BrandId;
            OrderDetailMasterData.StyleId = StyleId;
            OrderDetailMasterData.BrandId = BrandId;
            OrderDetailMasterData.Unit = UOM;
            OrderDetailMasterData.Total = Total;
            OrderDetailMasterData.ProductId = ProductID;
            OrderDetailMasterData.isTobbaco = isTobbaco;
            OrderDetailMasterData.DeviceOrderID = DeviceOrderID;
            OrderDetailMasterData.OrderId = OrderId;
            OrderDetailMasterData.OrderDetailId = OrderDetailId;
            OrderDetailMasterData.Quantity = Quantity;
        }
    }
}
