using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;

using System;
using System.Globalization;
using System.Threading.Tasks;

using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Core.Models.UIModels
{
    public class SRCProductUIModel : BaseModel
    {
        private readonly object thisLock = new object();

        private OrderDetail _orderDetailMasterData;
        public OrderDetail OrderDetailMasterData
        {
            get { return _orderDetailMasterData; }
            set { SetProperty(ref _orderDetailMasterData, value); }
        }

        private ProductDetailUiModel _productDetailUiModel;
        public ProductDetailUiModel ProductDetailMasterData
        {
            get { return _productDetailUiModel; }
            set { SetProperty(ref _productDetailUiModel, value); }
        }

        private Favorite _favoriteMasterData;
        public Favorite FavoriteMasterData
        {
            get { return _favoriteMasterData; }
            set { SetProperty(ref _favoriteMasterData, value); }
        }
        private int _productID;
        public int ProductID
        {
            get { return _productID; }
            set { SetProperty(ref _productID, value); }
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

        private string _pdf;
        public string Pdf
        {
            get { return _pdf; }
            set { SetProperty(ref _pdf, value); }
        }

        private string _link;
        public string Link
        {
            get { return _link; }
            set { SetProperty(ref _link, value); }
        }

        private bool _isAddedToCart;
        public bool IsAddedToCart
        {
            get { return _isAddedToCart; }
            set { SetProperty(ref _isAddedToCart, value); }
        }

        private int _isDistributed;
        public int IsDistributed
        {
            get { return _isDistributed; }
            set
            {
                SetProperty(ref _isDistributed, value);
                OnPropertyChanged("DistributionImageSource");
            }
        }

        private string _distributionRecordedDate;
        public string DistributionRecordedDate
        {
            get { return _distributionRecordedDate; }
            set { SetProperty(ref _distributionRecordedDate, value); GetDistributionDate(); }
        }

        public DateTime? DistributionDate { get; set; }
        public string DistributionDateFormat { get; set; }

        private bool _isFavorite;
        public bool IsFavorite
        {
            get { return _isFavorite; }
            set { SetProperty(ref _isFavorite, value); OnPropertyChanged("FavoriteImageSource"); }
        }

        private ImageSource pdfImage;
        public ImageSource PdfImage
        {
            get { return new BitmapImage(new Uri("ms-appx:///Assets/SRCProduct/pdf.png", UriKind.Absolute)); }
            set
            {
                if (pdfImage != value)
                {
                    pdfImage = value;
                    OnPropertyChanged("PdfImage");
                }
            }
        }

        private string cartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";
        public string CartImage
        {
            get { return cartImage; }
            set { SetProperty(ref cartImage, value); }

        }
        private string favoriteImage = "ms-appx:///Assets/SRCProduct/favorite_normal.png";
        public string FavoriteImage
        {
            get { return favoriteImage; }
            set { SetProperty(ref favoriteImage, value); }
        }
        private string favoriteSalesDocs = "ms-appx:///Assets/SRCProduct/favorite_normal.png";
        public string FavoriteSalesDocs
        {
            get { return favoriteSalesDocs; }
            set { SetProperty(ref favoriteSalesDocs, value); }
        }
        private string distributionImage = "ms-appx:///Assets/SRCProduct/distribution_normal.png";
        public string DistributionImage
        {
            get { return distributionImage; }
            set { SetProperty(ref distributionImage, value); }
        }
        public int CatId { get; set; }
        public int StyleId { get; set; }
        public int BrandId { get; set; }
        public int ProductType { get; set; }
        public string SearchDisplayPath
        {
            get { return (ItemNumber + " " + ItemDescription); }
        }

        private string _uom;
        public string UOM
        {
            get { return _uom; }
            set { SetProperty(ref _uom, value); }
        }

        private int _isDeleted;
        public int IsDeleted
        {
            get { return _isDeleted; }
            set { SetProperty(ref _isDeleted, value); }
        }

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
        private string leftArrowImage = "ms-appx:///Assets/ProductDetail/left_arrow_normal.png";
        public string LeftArrowImage
        {
            get { return leftArrowImage; }
            set { SetProperty(ref leftArrowImage, value); }

        }
        private string rightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_hover.png";
        public string RightArrowImage
        {
            get { return rightArrowImage; }
            set { SetProperty(ref rightArrowImage, value); }

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

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set { SetProperty(ref _quantity, value); }
        }

        public string DistributionFormattedDate => !string.IsNullOrWhiteSpace(DistributionRecordedDate) && (DistributionRecordedDate != "01-01-0001") ? DistributionRecordedDate : "";
        public ImageSource DistributionImageSource
=> new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/distribution_{(IsDistributed > 0 ? "selected" : "normal")}.png"));

        public ImageSource FavoriteImageSource
=> new BitmapImage(new Uri($"ms-appx:///Assets/SRCProduct/favorite_{(IsFavorite ? "selected" : "normal")}.png"));

        //private void GetDistributionDate()
        //{
        //    Parallel.Invoke(() =>
        //    {
        //        lock (thisLock)
        //        {
        //            if (!string.IsNullOrEmpty(DistributionRecordedDate))
        //            {
        //                bool isValidDate = DateTime.TryParse(DistributionRecordedDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);

        //                if (isValidDate)
        //                {
        //                    DistributionDate = date;
        //                }
        //            }
        //            else DistributionDate = null;
        //        }
        //    });
        //}

        private void GetDistributionDate()
        {
            Parallel.Invoke(() =>
            {
                lock (thisLock)
                {
                    if (!string.IsNullOrEmpty(DistributionRecordedDate))
                    {
                        bool isValidDate = DateTime.TryParse(DistributionRecordedDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);

                        if (isValidDate)
                        {
                            DistributionDate = date;
                            DistributionDateFormat = string.Format("{0:MM-dd-yyyy}", date);
                        }
                    }
                    else { DistributionDate = null; DistributionDateFormat = ""; }
                    //OnPropertyChanged("DistributionDate");
                    OnPropertyChanged("DistributionDateFormat");
                }
            });
        }

        public void OrderDetailUiToDataModel()
        {
            OrderDetailMasterData = new OrderDetail();
            OrderDetailMasterData.ProductName = ItemNumber;
            OrderDetailMasterData.ProductDescription = ItemDescription;
            OrderDetailMasterData.CategoryId = CatId;
            OrderDetailMasterData.BrandId = BrandId;
            OrderDetailMasterData.StyleId = StyleId;
            OrderDetailMasterData.BrandId = BrandId;
            OrderDetailMasterData.Unit = UOM;
            OrderDetailMasterData.ProductId = ProductID;
            OrderDetailMasterData.isTobbaco = isTobbaco;
            OrderDetailMasterData.DeviceOrderID = DeviceOrderID;
            OrderDetailMasterData.OrderId = OrderId;
            OrderDetailMasterData.OrderDetailId = OrderDetailId;
            OrderDetailMasterData.Quantity = Quantity;
        }

        public void SrcProductToProductDetail()
        {
            ProductDetailMasterData = new ProductDetailUiModel();
            ProductDetailMasterData.ItemNumber = ItemNumber;
            ProductDetailMasterData.ItemDescription = ItemDescription;
            ProductDetailMasterData.Quantity = Quantity;
            ProductDetailMasterData.ProductId = ProductID;
            ProductDetailMasterData.QuantityDisplay = Convert.ToString(Quantity);

        }
        public void FavoriteUiToDataModel()
        {
            if (FavoriteMasterData == null)
            {
                FavoriteMasterData = new Favorite();
                FavoriteMasterData.ProductName = ItemNumber;
                FavoriteMasterData.ProductDescription = ItemDescription;
                FavoriteMasterData.CategoryId = CatId;
                FavoriteMasterData.BrandId = BrandId;
                FavoriteMasterData.StyleId = StyleId;
                FavoriteMasterData.BrandId = BrandId;
                FavoriteMasterData.ProductId = ProductID;
                FavoriteMasterData.isTobbaco = isTobbaco;
            }
        }
    }
}
