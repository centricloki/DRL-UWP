using DevExpress.UI.Xaml.Grid;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.CustomControls;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.ViewModels
{
    public class SrcProductPageViewModel : ObservableObject
    {
        #region Properties

        private readonly ResourceLoader resourceLoader;

        private readonly List<int> SelectedCategoryIdList;
        private readonly List<int> SelectedBrandIdList;
        private readonly List<int> SelectedStyleIdList;

        private readonly App AppRef = (App)Application.Current;

        private bool isCartCommandFired = false;
        private bool isFavoriteCommandFired = false;
        private bool isFavoriteSalesDocsCommandFired = false;
        private bool isDistributionCommandFired = false;
        private bool isPdfCommandFired = false;

        private bool isSalesDocClicked = false;
        private bool isFromProductList = true;
        private string filetype = "1";

        private ObservableCollection<CategoryUIModel> _categoryList;
        public ObservableCollection<CategoryUIModel> CategoryList
        {
            get => _categoryList;
            set { _categoryList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BrandUIModel> _brandList;
        public ObservableCollection<BrandUIModel> BrandList
        {
            get => _brandList;
            set { _brandList = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StyleUIModel> _styleList;
        public ObservableCollection<StyleUIModel> StyleList
        {
            get => _styleList;
            set { _styleList = value; OnPropertyChanged(); }
        }

        private CategoryUIModel _selectedProduct;
        public CategoryUIModel SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                _selectedProduct = value;
                OnPropertyChanged();
            }
        }

        private ProductDetailUiModel _productDetailUiModel;
        public ProductDetailUiModel ProductDetailModel
        {
            get => _productDetailUiModel;
            set
            {
                _productDetailUiModel = value;
                OnPropertyChanged();
            }
        }
        private bool _isCustomerPanelVisible;
        public bool IsCustomerPanelVisible
        {
            get => _isCustomerPanelVisible;
            set
            {
                _isCustomerPanelVisible = value;
                OnPropertyChanged();
            }
        }

        private ICommand _itemClickCommand;
        public ICommand ItemClickCommand
        {
            get
            {
                if (_itemClickCommand == null)
                {
                    _itemClickCommand = new RelayCommand<ItemClickEventArgs>(OnItemClick);
                }

                return _itemClickCommand;
            }
        }

        private List<SRCProductUIModel> DbProductDataSource;
        public ObservableCollection<SRCProductUIModel> ProductGridDataSource { get; set; }

        private ObservableCollection<SRCProductUIModel> _headerSearchItemSource;
        public ObservableCollection<SRCProductUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        private Visibility _noBrandVisibility = Visibility.Collapsed;
        public Visibility NoBrandVisibility
        {
            get { return _noBrandVisibility; }
            set { SetProperty(ref _noBrandVisibility, value); }
        }

        private Visibility _noStyleVisibility = Visibility.Collapsed;
        public Visibility NoStyleVisibility
        {
            get { return _noStyleVisibility; }
            set { SetProperty(ref _noStyleVisibility, value); }
        }

        private Visibility _noProductVisibility = Visibility.Collapsed;
        public Visibility NoProductVisibility
        {
            get { return _noProductVisibility; }
            set { SetProperty(ref _noProductVisibility, value); }
        }
        private Visibility _isBadgeVisible = Visibility.Collapsed;
        public Visibility IsBadgeVisible
        {
            get { return _isBadgeVisible; }
            set { SetProperty(ref _isBadgeVisible, value); }
        }

        private string _badgeText = string.Empty;
        public string BadgeText
        {
            get { return _badgeText; }
            set { SetProperty(ref _badgeText, value); }
        }

        private int _badgeCount = 0;
        public int BadgeCount
        {
            get { return _badgeCount; }
            set { SetProperty(ref _badgeCount, value); }
        }

        private string _customerNameNumber = string.Empty;
        public string CustomerNameNumber
        {
            get { return _customerNameNumber; }
            set { SetProperty(ref _customerNameNumber, value); }
        }

        private string _customerAddresss = string.Empty;
        public string CustomerAddresss
        {
            get { return _customerAddresss; }
            set { SetProperty(ref _customerAddresss, value); }
        }

        private string _customerCityState = string.Empty;
        public string CustomerCityState
        {
            get { return _customerCityState; }
            set { SetProperty(ref _customerCityState, value); }
        }

        private bool _isDirectCustomer;
        public bool IsDirectCustomer
        {
            get { return _isDirectCustomer; }
            set { SetProperty(ref _isDirectCustomer, value); }
        }

        private Visibility _productDetailVisibility = Visibility.Collapsed;
        public Visibility ProductDetailVisibility
        {
            get { return _productDetailVisibility; }
            set { SetProperty(ref _productDetailVisibility, value); }
        }

        private Visibility _productListVisibility;
        public Visibility ProductListVisibility
        {
            get { return _productListVisibility; }
            set { SetProperty(ref _productListVisibility, value); }
        }

        private bool _isProductDetailsVisible;
        public bool IsProductDetailsVisible
        {
            get { return _isProductDetailsVisible; }
            set { SetProperty(ref _isProductDetailsVisible, value); }
        }

        private SRCProductUIModel _selectedProductDetail;
        public SRCProductUIModel SelectedProductDetail
        {
            get { return _selectedProductDetail; }
            set { SetProperty(ref _selectedProductDetail, value); }
        }

        private Visibility _customerTitlePanelVisibility = Visibility.Collapsed;
        public Visibility CustomerTitlePanelVisibility
        {
            get { return _customerTitlePanelVisibility; }
            set { SetProperty(ref _customerTitlePanelVisibility, value); }
        }

        private bool _previewDocumentVisibility;
        public bool PreviewDocumentVisibility
        {
            get { return _previewDocumentVisibility; }
            set { SetProperty(ref _previewDocumentVisibility, value); }
        }

        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }
        #endregion

        #region Commands

        public ICommand PageLoadedCommand { private set; get; }
        public ICommand CartImageCommand { private set; get; }
        public ICommand FavoriteImageCommand { private set; get; }
        public ICommand CategoryClickedCommand { private set; get; }
        public ICommand StyleClickedCommand { private set; get; }
        public ICommand BrandClickedCommand { private set; get; }
        public ICommand SalesDocsClickedCommand { private set; get; }
        public ICommand OtherCategoryClickedCommand { private set; get; }
        public ICommand HeaderComboBoxClickedCommand { private set; get; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        private ICommand _cartButtonCommand;
        public ICommand CartButtonCommand => _cartButtonCommand ?? (_cartButtonCommand = new RelayCommand(NavigateToCartPage));
        public ICommand DistributionImageCommand { private set; get; }
        public ICommand ItemSelectedCommand { private set; get; }
        public ICommand ListIconClickedCommand { private set; get; }
        public ICommand DetailAddToCartCommand { private set; get; }
        public ICommand FactSheetClickCommand { private set; get; }
        public ICommand RetailImageClickCommand { private set; get; }
        public ICommand ProductImageClickCommand { private set; get; }
        public ICommand FavoriteDetailImageClickCommand { private set; get; }
        public ICommand FavoriteSalesDocsImageCommand { private set; get; }
        public ICommand LeftArrowClickCommand { private set; get; }
        public ICommand RightArrowClickCommand { private set; get; }
        public ICommand CustomerPanelClickCommand { get; private set; }
        public ICommand PreviewDocumentCommand { get; private set; }
        public ICommand PdfFactsheetCommand { get; private set; }
        public ICommand NumPadButtonClickCommand { get; private set; }


        private GridControl gridControlObject;
        #endregion

        #region Constructor

        public SrcProductPageViewModel()
        {
            LoadingVisibility = Visibility.Visible;

            IsProductDetailsVisible = false;

            resourceLoader = ResourceLoader.GetForCurrentView();
   
            DbProductDataSource = new List<SRCProductUIModel>();
            ProductGridDataSource = new ObservableCollection<SRCProductUIModel>();
            HeaderSearchItemSource = new ObservableCollection<SRCProductUIModel>();

            ProductDetailModel = new ProductDetailUiModel();
            SelectedProductDetail = new SRCProductUIModel();

            PageLoadedCommand = new AsyncRelayCommand<GridControl>(LoadInitialPageData);

            InitializeCommands();

            SelectedCategoryIdList = new List<int>();
            SelectedBrandIdList = new List<int>();
            SelectedStyleIdList = new List<int>();            
            PreviewDocumentVisibility = false;
            gridControlObject = new GridControl();
        }
        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            CartImageCommand = new AsyncRelayCommand<SRCProductUIModel>(CartImageClicked);
            FavoriteImageCommand = new RelayCommand<SRCProductUIModel>(FavoriteImageClicked);
            FavoriteSalesDocsImageCommand = new RelayCommand<SRCProductUIModel>(FavoriteSalesDocsImageClicked);
            CustomerPanelClickCommand = new AsyncRelayCommand(CustomerPanelClicked);
            CategoryClickedCommand = new AsyncRelayCommand<CategoryUIModel>(CategoryClicked);
            StyleClickedCommand = new AsyncRelayCommand<StyleUIModel>(StyleClicked);
            BrandClickedCommand = new AsyncRelayCommand<BrandUIModel>(BrandClicked);
            SalesDocsClickedCommand = new RelayCommand<GridControl>(SalesDocsClicked);
            OtherCategoryClickedCommand = new RelayCommand<GridControl>(OtherCategoryClicked);
            HeaderComboBoxClickedCommand = new RelayCommand<int>(HandleHeaderComboboxChanged);
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HandleTextChangeHeaderCommand);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<SRCProductUIModel>(SuggestionChoosen);
            DistributionImageCommand = new RelayCommand<SRCProductUIModel>(DistributionImageClicked);
            ItemSelectedCommand = new RelayCommand<SRCProductUIModel>(ProductSelected);
            ListIconClickedCommand = new RelayCommand(ListIconClicked);
            DetailAddToCartCommand = new AsyncRelayCommand(ProductDetailAddToCart);
            FactSheetClickCommand = new AsyncRelayCommand(FactsheetClicked);
            RetailImageClickCommand = new AsyncRelayCommand(RetailImageClicked);
            ProductImageClickCommand = new AsyncRelayCommand(ProductImageClicked);
            FavoriteDetailImageClickCommand = new RelayCommand<SRCProductUIModel>(FavoriteDetailImageClicked);
            LeftArrowClickCommand = new RelayCommand(LeftArrowClicked);
            RightArrowClickCommand = new RelayCommand(RightArrowClicked);
            PdfFactsheetCommand = new AsyncRelayCommand<SRCProductUIModel>(PdfFactsheetCommandHandler);
            PreviewDocumentCommand = new RelayCommand<string>(PreviewDocumentCommandHandler);
            NumPadButtonClickCommand = new RelayCommand<string>(NumPadButtonClicked);
        }

        private async Task CustomerPanelClicked()
        {
            if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
            {
                await ShowRemoveCustomerWarning();
            }
        }
        private async Task ShowRemoveCustomerWarning()
        {
            CustomContentDialog dialog = new CustomContentDialog();
            dialog.Title = resourceLoader.GetString("WouldYouLikeTo");
            dialog.FirstButtonText = resourceLoader.GetString("CustomerKeepCart");
            dialog.SecondButtonText = resourceLoader.GetString("CustomerEmptyCart");
            dialog.CancelButtonText = resourceLoader.GetString("Cancel");

            await dialog.ShowAsync();

            if (dialog.Result == Result.Yes)
            {
                ChangeCustomerKeepCart();
                SetDefaultGridVisibility(gridControlObject);

            }
            else if (dialog.Result == Result.No)
            {
                ChangeCustomerEmptyCart();
                SetDefaultGridVisibility(gridControlObject);
            }
        }

        private void ChangeCustomerKeepCart()
        {
            AppRef.SelectedCustomerId = string.Empty;
            CustomerAddresss = string.Empty;
            CustomerCityState = string.Empty;
            CustomerNameNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;

        }
        private async void ChangeCustomerEmptyCart()
        {
            AppRef.SelectedCustomerId = string.Empty;
            CustomerAddresss = string.Empty;
            CustomerCityState = string.Empty;
            CustomerNameNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;
            await AppRef.QueryService.DeleteAllCartItems(AppRef.CurrentOrderId);
            foreach (var item in ProductGridDataSource)
            {

                item.IsAddedToCart = false;
            }
            ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_black.png";
            ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_inactive.png";
            BadgeCount = 0;
            AppRef.CartItemCount = 0;
            AppRef.CurrentOrderId = 0;
            IsBadgeVisible = Visibility.Collapsed;
            BadgeText = string.Empty;
        }

        private async Task GetCategoryListData()
        {
            if (CategoryList == null)
            {
                CategoryList = new ObservableCollection<CategoryUIModel>();
            }

            ObservableCollection<CategoryUIModel> CategoryListCopy;

            var categoryUIModelList = await ((App)Application.Current).QueryService.GetCategoryFilterDataForProductsList();

            CategoryListCopy = new ObservableCollection<CategoryUIModel>(categoryUIModelList);

            List<CategoryUIModel> unOrderedCategoryList = new List<CategoryUIModel>(CategoryListCopy);

            List<CategoryUIModel> orderedCategoryList = unOrderedCategoryList?.OrderBy(x => x.CategoryId).ToList();

            CategoryList = new ObservableCollection<CategoryUIModel>(orderedCategoryList);
        }

        private async Task GetBrandListData()
        {
            BrandList = new ObservableCollection<BrandUIModel>();

            ObservableCollection<BrandUIModel> BrandListCopy = new ObservableCollection<BrandUIModel>();

            List<BrandUIModel> brandUIModelList = await ((App)Application.Current).QueryService.GetBrandFilterDataForProductsList();

            foreach (var brandmodel in brandUIModelList)
            {
                BrandUIModel brandUIModel = new BrandUIModel()
                {
                    BrandId = brandmodel.BrandId,
                    BrandName = brandmodel.BrandName,
                    IsSelected = false,
                    BrandImage = brandmodel.BrandImage,
                    BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH, ApplicationConstants.BrandImageBaseFolder,
                    HelperMethods.GetNameFromURL(brandmodel.BrandImage)),
                    SelectedImage = brandmodel.SelectedImage
                };

                BrandListCopy.Add(brandUIModel);
            }

            BrandList = new ObservableCollection<BrandUIModel>(BrandListCopy);

            NoBrandVisibility = BrandList?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task GetStyleListData()
        {
            StyleList = new ObservableCollection<StyleUIModel>();

            ObservableCollection<StyleUIModel> StyleListCopy = new ObservableCollection<StyleUIModel>();

            List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetStyleFilterDataForProductsList();

            foreach (var stylemodel in styleUIModelList)
            {
                StyleUIModel styleUIModel = new StyleUIModel()
                {
                    StyleId = stylemodel.StyleId,
                    StyleName = stylemodel.StyleName,
                    IsSelected = false,
                    StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png"
                };

                StyleListCopy.Add(styleUIModel);
            }

            StyleList = new ObservableCollection<StyleUIModel>(StyleListCopy);

            NoStyleVisibility = StyleList?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private async Task LoadInitialPageData(GridControl gridControl)
        {
            try
            {
                gridControlObject = gridControl;

                await GetCategoryListData();
                await GetBrandListData();
                await GetStyleListData();

                DbProductDataSource = await ((App)Application.Current).QueryService.GetSRCProductsListDataOnInitialLoad(AppRef.CurrentOrderId);

                DbProductDataSource?.ToList().ForEach(x => ProductGridDataSource.Add(x));

                BadgeCount = AppRef.CartItemCount;

                if (BadgeCount > 0)
                {
                    IsBadgeVisible = Visibility.Visible;
                    BadgeText = BadgeCount.ToString();
                }

                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
                {
                    gridControl.Columns["ItemNumber"].Visible = true;
                    gridControl.Columns["ItemDescription"].Visible = true;
                    gridControl.Columns["Link"].Visible = false;
                    gridControl.Columns["PdfImage"].Visible = true;
                    gridControl.Columns["CartImage"].Visible = true;
                    gridControl.Columns["DistributionImage"].Visible = true;
                    gridControl.Columns["DistributionRecordedDate"].Visible = true;
                    gridControl.Columns["FavoriteImage"].Visible = true;
                    gridControl.Columns["FavoriteSalesDocs"].Visible = false;
                }
                else
                {
                    SetDefaultGridVisibility(gridControl);
                }

                LoadingVisibility = Visibility.Collapsed;

                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

                    var selectedCustomer = await ((App)Application.Current).QueryService.GetSavedCustomerInformation(customerId);

                    if (selectedCustomer != null)
                    {
                        CustomerNameNumber = selectedCustomer.CustomerName + " " + selectedCustomer.CustomerNumber;
                        CustomerAddresss = selectedCustomer.PhysicalAddress;
                        CustomerCityState = selectedCustomer.SubAddressText;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                        IsDirectCustomer = selectedCustomer.AccountType != 2;
                    }
                }

                DbProductDataSource = await ((App)Application.Current).QueryService.GetSRCProductsPageGridData(AppRef.CurrentOrderId);

                DbProductDataSource?.ToList().ForEach(x => ProductGridDataSource.Add(x));

                if (IsDirectCustomer)
                {
                    foreach (var item in ProductGridDataSource)
                    {
                        item.IsDistributed = 1;
                        item.DistributionImage = "ms-appx:///Assets/SRCProduct/distribution_selected.png";
                        item.DistributionRecordedDate = DateTimeHelper.ConvertStringDateToMM_DD_YYYY(DateTime.Now.ToString());
                    }
                }

                if (AppRef.CurrentOrderId != 0 && (bool)AppRef.IsOrderTypeChanged)
                {
                    List<OrderDetailUIModel> orderDetailsData = await AppRef.QueryService.GetCartProductDetailsData(AppRef.CurrentOrderId);

                    foreach (var item in orderDetailsData)
                    {
                        item.OrderDetailUiToDataModel();

                        CheckForOrderTypeAndSetOrderDetailsData(item.OrderDetailData);
                    }
                }
            }
            catch (Exception ex)
            {
                LoadingVisibility = Visibility.Collapsed;

                ErrorLogger.WriteToErrorLog(GetType().Name, "LoadInitialPageData", ex.StackTrace);
            }
        }

        /// <summary>
        /// Cart image click event
        /// </summary>
        /// <param name="sRCProductUIModel"></param>
        private async Task CartImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            try
            {
                isCartCommandFired = isFromProductList;

                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    ShowNoCustomerSelected();
                }
                else
                {
                    if (sRCProductUIModel.SRCHoneySellable != 0 && sRCProductUIModel.SRCHoneyReturnable != 0 && sRCProductUIModel.SRCCanIOrder != 0)
                    {
                        await VerifyCartItems(sRCProductUIModel);
                    }
                    else
                    {
                        ShowProductCannotBeAddedToCart();
                    }
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CartImageClicked", ex.StackTrace);
            }
        }
        private async Task VerifyCartItems(SRCProductUIModel sRCProductUIModel)
        {
            if (AppRef.CartItemCount > 0)
            {
                if (AppRef.CartDataFromScreen == 0)
                {
                    SelectDeselectCart(sRCProductUIModel);
                }
                else
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", ResourceExtensions.GetLocalized("EmptyRackOrPopCart"), "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentOrderId);
                        if (isSuccess)
                        {
                            AppRef.CartItemCount = 0;
                            BadgeCount = AppRef.CartItemCount;
                            if (BadgeCount <= 0)
                            {
                                IsBadgeVisible = Visibility.Collapsed;
                            }
                            else
                            {
                                IsBadgeVisible = Visibility.Visible;
                                BadgeText = BadgeCount.ToString();
                            }
                            SelectDeselectCart(sRCProductUIModel);
                        }
                    }
                }
            }
            else
            {
                SelectDeselectCart(sRCProductUIModel);
            }
        }

        /// <summary>
        /// Cart select deselect
        /// </summary>
        /// <param name="sRCProductUIModel"></param>
        private async void SelectDeselectCart(SRCProductUIModel sRCProductUIModel)
        {
            if (!sRCProductUIModel.IsAddedToCart)
            {
                sRCProductUIModel.IsAddedToCart = true;

                BadgeCount++;
                AppRef.CartDataFromScreen = 0;

                sRCProductUIModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_selected.png";

                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_white.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_active.png";

                if (AppRef.CurrentOrderId == 0)
                {
                    await InsertEntryIntoOrderMaster();
                }

                await InsertOrderDetailData(sRCProductUIModel);
            }
            else
            {
                sRCProductUIModel.IsAddedToCart = false;

                BadgeCount--;

                sRCProductUIModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";

                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_black.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_inactive.png";

                await DeleteCartItemFromDb(sRCProductUIModel.ProductID, AppRef.CurrentOrderId);
            }

            if (BadgeCount <= 0)
            {
                IsBadgeVisible = Visibility.Collapsed;
            }
            else
            {
                IsBadgeVisible = Visibility.Visible;
                BadgeText = BadgeCount.ToString();
            }

            ((App)Application.Current).CartItemCount = BadgeCount;
        }

        private void FavoriteImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            isFavoriteCommandFired = isFromProductList;
            SelectDeselectFavorite(sRCProductUIModel);
        }

        private void FavoriteSalesDocsImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            isFavoriteSalesDocsCommandFired = isFromProductList;
            SelectDeselectFavoriteSalesDocs(sRCProductUIModel);
        }

        private async void SelectDeselectFavorite(SRCProductUIModel sRCProductUIModel)
        {
            if (!sRCProductUIModel.IsFavorite)
            {
                sRCProductUIModel.IsFavorite = true;
                sRCProductUIModel.FavoriteImage = "ms-appx:///Assets/SRCProduct/favorite_selected.png";
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_selected.png";
                await InsertFavoriteData(sRCProductUIModel);
            }
            else
            {
                sRCProductUIModel.IsFavorite = false;
                sRCProductUIModel.FavoriteImage = "ms-appx:///Assets/SRCProduct/favorite_normal.png";
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_normal.png";

                await InsertFavoriteData(sRCProductUIModel);
                ///await DeleteFavoriteItemFromDb(sRCProductUIModel.ProductID);
            }

        }

        private async void SelectDeselectFavoriteSalesDocs(SRCProductUIModel sRCProductUIModel)
        {
            if (!sRCProductUIModel.IsFavorite)
            {
                sRCProductUIModel.IsFavorite = true;

                sRCProductUIModel.FavoriteSalesDocs = "ms-appx:///Assets/SRCProduct/favorite_selected.png";
                await InsertFavoriteData(sRCProductUIModel);
            }
            else
            {
                sRCProductUIModel.IsFavorite = false;
                sRCProductUIModel.FavoriteSalesDocs = "ms-appx:///Assets/SRCProduct/favorite_normal.png";

                await InsertFavoriteData(sRCProductUIModel);
               /// await DeleteFavoriteItemFromDb(sRCProductUIModel.ProductID);
            }
        }

        private async Task InsertFavoriteData(SRCProductUIModel sRCProductUIModel)
        {
            List<BrandUIModel> brandUiModelList = await ((App)Application.Current).QueryService.GetBrandFilterDataForProductsList();
            List<StyleUIModel> styleUiModelList = await ((App)Application.Current).QueryService.GetStyleFilterDataForProductsList();

            var brandInfo = brandUiModelList.FirstOrDefault(x => x.BrandId == sRCProductUIModel.BrandId);
            var styleInfo = styleUiModelList.FirstOrDefault(x => x.StyleId == sRCProductUIModel.StyleId);

            sRCProductUIModel.FavoriteUiToDataModel();

            var _list = new List<Favorite>() { sRCProductUIModel.FavoriteMasterData };

            _list[0].BrandName = brandInfo?.BrandName;
            _list[0].StyleName = styleInfo?.StyleName;
            _list[0].UserId = Convert.ToInt32(AppRef.LoginUserIdProperty);

            if (sRCProductUIModel.IsFavorite)
            {
                _list[0].isDeleted = 0;
                _list[0].IsExported = 0;
                _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            }
            else
            {
                _list[0].isDeleted = 1;
                _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            }

            await ((App)Application.Current).QueryService.InsertFavorite(_list);
        }

        private async Task InsertEntryIntoOrderMaster()
        {
            var orderMasterRecord = new OrderMaster();

            orderMasterRecord.IsExported = 1;

            var newOrderAdded = await ((App)Application.Current).QueryService.InsertOrUpdateOrderMaster(orderMasterRecord);

            if (newOrderAdded != null)
            {
                ((App)Application.Current).CurrentOrderId = newOrderAdded.OrderID;
            }
        }

        /// <summary>
        /// Insert Cart Item to database
        /// </summary>
        /// <param name="sRCProductUIModel"></param>
        /// <returns></returns>
        private async Task InsertOrderDetailData(SRCProductUIModel sRCProductUIModel)
        {
            List<BrandUIModel> brandUiModelList = await ((App)Application.Current).QueryService.GetBrandFilterDataForProductsList();
            List<StyleUIModel> styleUiModelList = await ((App)Application.Current).QueryService.GetStyleFilterDataForProductsList();
            List<CategoryUIModel> categoryUiModelList = await ((App)Application.Current).QueryService.GetCategoryFilterDataForProductsList();

            var brandInfo = brandUiModelList.FirstOrDefault(x => x.BrandId == sRCProductUIModel.BrandId);
            var styleInfo = styleUiModelList.FirstOrDefault(x => x.StyleId == sRCProductUIModel.StyleId);
            var categoryInfo = categoryUiModelList.FirstOrDefault(x => x.CategoryId == sRCProductUIModel.CatId);

            sRCProductUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { sRCProductUIModel.OrderDetailMasterData };

            _list[0].BrandName = brandInfo?.BrandName;
            _list[0].StyleName = styleInfo?.StyleName;
            _list[0].Total = "0";
            _list[0].OrderId = ((App)Application.Current).CurrentOrderId;
            _list[0].DeviceOrderID = Convert.ToString(((App)Application.Current).CurrentOrderId);
            _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

            if ((bool)AppRef.IsCreditRequestOrder)
            {
                if (categoryInfo?.ERPCategoryId == 1010 || categoryInfo?.ERPCategoryId == 1030)
                {
                    _list[0].Unit = "BX";
                }
                else
                {
                    _list[0].Unit = "EA";
                }

                if (_list[0].Quantity > 0)
                {
                    _list[0].Quantity = _list[0].Quantity * -1;
                }
                else if (_list[0].Quantity == 0)
                {
                    _list[0].Quantity = -1;
                }

                _list[0].Price = 0;
                _list[0].Total = "0";

                SetCreditRequestType(categoryInfo, _list);
            }
            else
            {
                SetUomForCategoryId(categoryInfo, _list);
            }

            await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        private async void CheckForOrderTypeAndSetOrderDetailsData(OrderDetail orderDetailItem)
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                orderDetailItem.Quantity = orderDetailItem.Quantity * -1;
                orderDetailItem.Price = 0;
                orderDetailItem.Total = "0";
                orderDetailItem.Unit = orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ? "BX" : "EA";
                orderDetailItem.CreditRequest = orderDetailItem.CategoryId == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";
                orderDetailItem.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            }
            else
            {
                if (orderDetailItem.Quantity > 0)
                {
                    orderDetailItem.Quantity *= -1;
                }
                else if (orderDetailItem.Quantity == 0)
                {
                    orderDetailItem.Quantity = -1;
                }

                orderDetailItem.Price = 0;
                orderDetailItem.Total = "0";
                orderDetailItem.CreditRequest = "";
                orderDetailItem.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                if (orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ||
                    orderDetailItem.CategoryId == 13 || orderDetailItem.CategoryId == 14)
                {
                    orderDetailItem.Unit = "BX";
                }
                else if (orderDetailItem.CategoryId == 2 || orderDetailItem.CategoryId == 3 ||
                    orderDetailItem.CategoryId == 7 || orderDetailItem.CategoryId == 17 ||
                    orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ||
                    orderDetailItem.CategoryId == 15 || orderDetailItem.CategoryId == 12)
                {
                    orderDetailItem.Unit = "EA";
                }
                else
                {
                    orderDetailItem.Unit = "CA";
                }
            }

            await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(orderDetailItem);
        }

        private void SetUomForCategoryId(CategoryUIModel categoryInfo, List<OrderDetail> _list)
        {
            switch (categoryInfo?.ERPCategoryId)
            {
                case 1010:
                case 1030:
                case 1031:
                case 1042:
                    _list[0].Unit = "BX";
                    break;
                case 1020:
                case 1021:
                case 1040:
                case 1041:
                case 1043:
                case 1070:
                    _list[0].Unit = "EA";
                    break;
                default:
                    _list[0].Unit = "CA";
                    break;
            }
        }

        private void SetCreditRequestType(CategoryUIModel categoryInfo, List<OrderDetail> _list)
        {
            switch (categoryInfo?.ERPCategoryId)
            {
                case 1030:
                    _list[0].CreditRequest = "DIF-Destroyed";
                    break;
                default:
                    _list[0].CreditRequest = "RTN-Retail Returns";
                    break;
            }
        }

        /// <summary>
        /// Delete cart item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteCartItemFromDb(int productId, int deviceOrderId)
        {
            await ((App)Application.Current).QueryService.DeleteOrderDetail(productId, deviceOrderId);
        }

        private async void ShowProductCannotBeAddedToCart()
        {

            ContentDialog productCannotBeAddedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("ProductCannotBeAddedText"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await productCannotBeAddedDialog.ShowAsync();
        }

        private async void ShowNoCustomerSelected()
        {
            ContentDialog noCustomerSelectedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("NoCustomerSelectedText"),
                PrimaryButtonText = resourceLoader.GetString("SelectCustomer"),
                SecondaryButtonText = resourceLoader.GetString("Cancel")
            };

            ContentDialogResult result = await noCustomerSelectedDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                NavigationService.Navigate<CustomerPage>();
            }
            else
            {
                noCustomerSelectedDialog.Hide();
            }
        }

        private int GetSalesDocsIndex()
        {
            int index = 0;

            for (int i = 0; i < CategoryList.Count; i++)
            {
                if (CategoryList[i].CategoryId == -99)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        private async Task FilterProductsListOnFilterSelectUnSelect()
        {
            ProductGridDataSource.Clear();

            if (SelectedCategoryIdList != null && SelectedBrandIdList != null && SelectedStyleIdList != null)
            {
                if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList.Count == 0)
                {
                    DbProductDataSource = await ((App)Application.Current).QueryService.GetSRCProductsPageGridData(AppRef.CurrentOrderId);

                    DbProductDataSource?.ForEach(x => ProductGridDataSource.Add(x));
                }
                else
                {
                    DbProductDataSource = await ((App)Application.Current).QueryService.GetSRCProductsDataOnAllSelectedFilters(SelectedCategoryIdList, SelectedBrandIdList, SelectedStyleIdList, AppRef.CurrentOrderId);

                    DbProductDataSource?.ForEach(x => ProductGridDataSource.Add(x));

                    if (DbProductDataSource?.Count == 0)
                    {
                        NoProductVisibility = Visibility.Visible;
                    }
                }
                if (DbProductDataSource?.Count == 0)
                {
                    NoProductVisibility = Visibility.Visible;
                }
                else
                {
                    NoProductVisibility = Visibility.Collapsed;
                }
            }
        }

        private async Task GetFilteredCategoryBrandStyleData()
        {
            if (SelectedCategoryIdList != null && SelectedBrandIdList != null && SelectedStyleIdList != null)
            {
                if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList.Count == 0)
                {
                    await GetCategoryListData();
                    await GetStyleListData();
                    await GetBrandListData();
                }
                else if (SelectedCategoryIdList.Count > 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList.Count == 0)
                {
                    await GetFilteredBrandListOnCategoryClick(SelectedCategoryIdList);

                    await GetFilteredStyleListOnCategoryClick(SelectedCategoryIdList);
                }
                else if (SelectedBrandIdList.Count > 0 && SelectedCategoryIdList.Count == 0 && SelectedStyleIdList.Count == 0)
                {
                    await GetFilteredCategoryListOnBrandClick(SelectedBrandIdList);

                    await GetFilteredStyleListOnBrandClick(SelectedBrandIdList);
                }
                else if (SelectedStyleIdList.Count > 0 && SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count == 0)
                {
                    await GetFilteredCategoryListOnSytleClick(SelectedStyleIdList);

                    await GetFilteredBrandListOnSytleClick(SelectedStyleIdList);
                }
                else if (SelectedCategoryIdList.Count > 0 && SelectedBrandIdList.Count > 0 && SelectedStyleIdList.Count == 0)
                {
                    await GetFilteredStyleListOnCategoryAndBrandClick(SelectedCategoryIdList, SelectedBrandIdList);
                }
                else if (SelectedCategoryIdList.Count > 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList.Count > 0)
                {
                    await GetFilteredBrandListOnCategoryAndStyleClick(SelectedCategoryIdList, SelectedStyleIdList);
                }
                else if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count > 0 && SelectedStyleIdList.Count > 0)
                {
                    await GetFilteredCategoryListOnBrandAndStyleClick(SelectedBrandIdList, SelectedStyleIdList);
                }
            }
        }

        /// <summary>
        /// Selection of Category filter item
        /// </summary>
        /// <param name="categoryUIModel"></param>
        private async Task CategoryClicked(CategoryUIModel categoryUIModel)
        {
            try
            {
                if (categoryUIModel != null)
                {
                    SelectedCategoryIdList.Clear();

                    CategorySelectUnselectUI(categoryUIModel);

                    foreach (var category in CategoryList)
                    {
                        if (category.IsSelected)
                        {
                            SelectedCategoryIdList.Add(category.CategoryId);
                        }
                    }

                    //if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList.Count == 0)
                    //{
                    //    await GetCategoryListData();
                    //    await GetStyleListData();
                    //    await GetBrandListData();
                    //}
                    //else if (SelectedCategoryIdList.Count > 0)
                    //{
                    //    await GetFilteredBrandListOnCategoryClick(SelectedCategoryIdList);

                    //    await GetFilteredStyleListOnCategoryClick(SelectedCategoryIdList);
                    //}

                    await GetFilteredCategoryBrandStyleData();

                    await FilterProductsListOnFilterSelectUnSelect();

                    GetFilteredProductDetailValues();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CategoryClicked", ex.StackTrace);
            }
        }

        private void GetFilteredProductDetailValues()
        {
            if (ProductGridDataSource?.Count > 0)
            {
                ProductDetailModel.ItemNumber = ProductGridDataSource[0]?.ItemNumber;
                ProductDetailModel.ItemDescription = ProductGridDataSource[0]?.ItemDescription;
                ProductDetailModel.ProductId = ProductGridDataSource[0].ProductID;
                ProductDetailModel.Quantity = ProductGridDataSource[0].Quantity;
                ProductDetailModel.SelectedProductDetailIndex = 0;
                GetSelectedProductValues(ProductGridDataSource[0]);
            }
        }

        private async Task GetFilteredBrandListOnCategoryClick(List<int> selectedCategoryList)
        {
            ObservableCollection<BrandUIModel> BrandListCopy = new ObservableCollection<BrandUIModel>();

            List<BrandUIModel> brandUIModelList = await ((App)Application.Current).QueryService.GetFilteredBrandDataWhenCategoryIsSelected(selectedCategoryList);

            if (brandUIModelList != null)
            {
                BrandList.Clear();

                foreach (var brandmodel in brandUIModelList)
                {
                    if (SelectedBrandIdList.Count > 0)
                    {
                        foreach (var brandIdItem in SelectedBrandIdList)
                        {
                            if (brandmodel.BrandId == brandIdItem)
                            {
                                brandmodel.IsSelected = true;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                brandmodel.IsSelected = false;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = " ";
                            }
                        }
                    }
                    else
                    {
                        brandmodel.IsSelected = false;
                        brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                        brandmodel.SelectedImage = " ";
                    }

                    BrandUIModel brandUIModel = new BrandUIModel()
                    {
                        BrandId = brandmodel.BrandId,
                        BrandName = brandmodel.BrandName,
                        IsSelected = brandmodel.IsSelected,
                        BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage)),
                        BrandImage = brandmodel.BrandImage,
                        SelectedImage = brandmodel.SelectedImage
                    };

                    BrandListCopy.Add(brandUIModel);
                }

                //List<BrandUIModel> unOrderedBrandList = new List<BrandUIModel>(BrandListCopy);
                //List<BrandUIModel> orderedBrandList = unOrderedBrandList?.OrderBy(x => x.BrandId).ToList();
                BrandList = new ObservableCollection<BrandUIModel>(BrandListCopy.OrderBy(x => x.BrandName));

                if (BrandList?.Count == 0)
                {
                    NoBrandVisibility = Visibility.Visible;
                }
                else
                {
                    NoBrandVisibility = Visibility.Collapsed;
                }
            }
        }

        private async Task GetFilteredStyleListOnCategoryClick(List<int> selectedCategoryList)
        {
            ObservableCollection<StyleUIModel> StyleListCopy = new ObservableCollection<StyleUIModel>();

            List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetFilteredStyleDataWhenCategoryIsSelected(selectedCategoryList);

            if (styleUIModelList != null)
            {
                StyleList.Clear();

                if (styleUIModelList.Count > 0)
                {
                    foreach (var stylemodel in styleUIModelList)
                    {
                        if (SelectedStyleIdList.Count > 0)
                        {
                            foreach (var styleIdItem in SelectedStyleIdList)
                            {
                                if (stylemodel.StyleId == styleIdItem)
                                {
                                    stylemodel.IsSelected = true;
                                    stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_selected.png";
                                    break;
                                }
                                else
                                {
                                    stylemodel.IsSelected = false;
                                    stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                                }
                            }
                        }
                        else
                        {
                            stylemodel.IsSelected = false;
                            stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                        }

                        StyleUIModel styleUIModel = new StyleUIModel()
                        {
                            StyleId = stylemodel.StyleId,
                            StyleName = stylemodel.StyleName,
                            IsSelected = stylemodel.IsSelected,
                            StyleImage = stylemodel.StyleImage
                        };

                        StyleListCopy.Add(styleUIModel);
                    }
                }

                //List<StyleUIModel> unOrderedStyleList = new List<StyleUIModel>(StyleListCopy);
                //List<StyleUIModel> orderedStyleList = unOrderedStyleList?.OrderBy(x => x.StyleId).ToList();

                StyleList = new ObservableCollection<StyleUIModel>(StyleListCopy.OrderBy(x => x.StyleName));

                if (StyleList?.Count == 0)
                {
                    NoStyleVisibility = Visibility.Visible;
                }
                else
                {
                    NoStyleVisibility = Visibility.Collapsed;
                }
            }
        }

        private void CategorySelectUnselectUI(CategoryUIModel categoryUIModel)
        {
            categoryUIModel.IsSelected = !categoryUIModel.IsSelected;

            bool selected = categoryUIModel.IsSelected;

            int catId = categoryUIModel.CategoryId;

            int index = GetSalesDocsIndex();

            if (catId == -99)
            {
                CategoryList[index] = categoryUIModel;
            }

            if (CategoryList[index].IsSelected)
            {
                foreach (var category in CategoryList)
                {
                    category.IsSelected = false;
                    category.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";

                }

                if (catId == -99)
                {
                    if (selected)
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                    }
                    else
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    CategoryList[index] = categoryUIModel;
                    CategoryList[index].IsSelected = selected;
                }
                else
                {
                    var categoryIndex = CategoryList.FirstOrDefault(x => x.CategoryId == categoryUIModel.CategoryId);

                    int indexFound = CategoryList.IndexOf(categoryIndex);

                    if (selected)
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                    }
                    else
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    CategoryList[indexFound] = categoryUIModel;
                    CategoryList[indexFound].IsSelected = selected;

                    if (CategoryList.Count > 1)
                    {
                        if (!CategoryList[index].IsSelected)
                        {
                            CategoryList[index].IsSelected = false;

                            CategoryList[index].CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                        }
                    }
                }
            }
            else
            {
                if (!CategoryList[index].IsSelected)
                {
                    var categoryIndex = CategoryList.FirstOrDefault(x => x.CategoryId == categoryUIModel.CategoryId);

                    int indexFound = CategoryList.IndexOf(categoryIndex);

                    if (selected)
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                    }
                    else
                    {
                        categoryUIModel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    //if (CategoryList.Count > 1)
                    //{
                    //    CategoryList[index].IsSelected = false;

                    //    CategoryList[index].CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    //}

                    CategoryList[indexFound] = categoryUIModel;
                }
            }
        }

        /// <summary>
        /// Selection of Style Filter Item
        /// </summary>
        /// <param name="styleUIModel"></param>
        private async Task StyleClicked(StyleUIModel styleUIModel)
        {
            try
            {
                if (styleUIModel != null)
                {
                    var styleIndex = StyleList.FirstOrDefault(x => x.StyleId == styleUIModel.StyleId);
                    int indexFound = StyleList.IndexOf(styleIndex);

                    styleUIModel.IsSelected = !styleUIModel.IsSelected;

                    bool alreadyExists = false;

                    if (SelectedStyleIdList != null)
                    {
                        alreadyExists = SelectedStyleIdList.Contains(styleUIModel.StyleId);
                    }

                    if (styleUIModel.IsSelected)
                    {
                        styleUIModel.StyleImage = "ms-appx:///Assets/SRCProduct/style_selected.png";

                        if (!alreadyExists)
                        {
                            SelectedStyleIdList?.Add(styleUIModel.StyleId);
                        }
                    }
                    else
                    {
                        styleUIModel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";

                        if (alreadyExists)
                        {
                            SelectedStyleIdList?.Remove(styleUIModel.StyleId);
                        }
                    }

                    StyleList[indexFound] = styleUIModel;

                    SelectedStyleIdList?.Clear();

                    foreach (var style in StyleList)
                    {
                        if (style.IsSelected)
                        {
                            SelectedStyleIdList?.Add(style.StyleId);
                        }
                    }

                    //if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList.Count == 0 && SelectedStyleIdList?.Count == 0)
                    //{
                    //    await GetCategoryListData();
                    //    await GetStyleListData();
                    //    await GetBrandListData();
                    //}
                    //else if (SelectedStyleIdList?.Count > 0)
                    //{
                    //    await GetFilteredCategoryListOnSytleClick(SelectedStyleIdList);

                    //    await GetFilteredBrandListOnSytleClick(SelectedStyleIdList);
                    //}

                    await GetFilteredCategoryBrandStyleData();

                    await FilterProductsListOnFilterSelectUnSelect();

                    GetFilteredProductDetailValues();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "StyleClicked", ex.StackTrace);
            }
        }

        private async Task GetFilteredCategoryListOnSytleClick(List<int> selectedStyleList)
        {
            ObservableCollection<CategoryUIModel> CategoryListCopy = new ObservableCollection<CategoryUIModel>();

            List<CategoryUIModel> categoryUIModelList = await ((App)Application.Current).QueryService.GetFilteredCategoryDataWhenStyleIsSelected(selectedStyleList);

            if (categoryUIModelList != null)
            {
                CategoryList.Clear();

                foreach (var categorymodel in categoryUIModelList)
                {
                    if (SelectedCategoryIdList.Count > 0)
                    {
                        foreach (var categoryIdItem in SelectedCategoryIdList)
                        {
                            if (categorymodel.CategoryId == categoryIdItem)
                            {
                                categorymodel.IsSelected = true;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                categorymodel.IsSelected = false;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                            }
                        }
                    }
                    else
                    {
                        categorymodel.IsSelected = false;
                        categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    CategoryUIModel categoryUIModel = new CategoryUIModel()
                    {
                        CategoryId = categorymodel.CategoryId,
                        CategoryName = categorymodel.CategoryName,
                        IsSelected = categorymodel.IsSelected,
                        CategoryImage = categorymodel.CategoryImage
                    };

                    CategoryListCopy.Add(categoryUIModel);
                }

                List<CategoryUIModel> unOrderedCategoryList = new List<CategoryUIModel>(CategoryListCopy);

                List<CategoryUIModel> orderedCategoryList = unOrderedCategoryList?.OrderBy(x => x.CategoryId).ToList();

                CategoryList = new ObservableCollection<CategoryUIModel>(orderedCategoryList);
            }
        }

        private async Task GetFilteredBrandListOnSytleClick(List<int> selectedStyleList)
        {
            ObservableCollection<BrandUIModel> BrandListCopy = new ObservableCollection<BrandUIModel>();

            List<BrandUIModel> brandUIModelList = await ((App)Application.Current).QueryService.GetFilteredBrandDataWhenStyleIsSelected(selectedStyleList);

            if (brandUIModelList != null)
            {
                BrandList.Clear();

                foreach (var brandmodel in brandUIModelList)
                {
                    if (SelectedBrandIdList.Count > 0)
                    {
                        foreach (var brandIdItem in SelectedBrandIdList)
                        {
                            if (brandmodel.BrandId == brandIdItem)
                            {
                                brandmodel.IsSelected = true;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                brandmodel.IsSelected = false;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = " ";
                            }
                        }
                    }
                    else
                    {
                        brandmodel.IsSelected = false;
                        brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                        brandmodel.SelectedImage = " ";
                    }

                    BrandUIModel brandUIModel = new BrandUIModel()
                    {
                        BrandId = brandmodel.BrandId,
                        BrandName = brandmodel.BrandName,
                        IsSelected = false,
                        BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage)),
                        BrandImage = brandmodel.BrandImage,
                        SelectedImage = brandmodel.SelectedImage
                    };

                    BrandListCopy.Add(brandUIModel);
                }

                //List<BrandUIModel> unOrderedBrandList = new List<BrandUIModel>(BrandListCopy);
                //List<BrandUIModel> orderedBrandList = unOrderedBrandList?.OrderBy(x => x.BrandId).ToList();
                BrandList = new ObservableCollection<BrandUIModel>(BrandListCopy.OrderBy(x => x.BrandName));

                if (BrandList?.Count == 0)
                {
                    NoBrandVisibility = Visibility.Visible;
                }
                else
                {
                    NoBrandVisibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// Selection of Brand Filter Item
        /// </summary>
        /// <param name="brandUIModel"></param>
        private async Task BrandClicked(BrandUIModel brandUIModel)
        {
            try
            {
                if (brandUIModel != null)
                {
                    var brandIndex = BrandList.FirstOrDefault(x => x.BrandId == brandUIModel.BrandId);
                    int indexFound = BrandList.IndexOf(brandIndex);
                    bool alreadyExists = false;

                    brandUIModel.IsSelected = !brandUIModel.IsSelected;

                    if (SelectedBrandIdList != null)
                    {
                        alreadyExists = SelectedBrandIdList.Contains(brandUIModel.BrandId);
                    }

                    if (brandUIModel.IsSelected)
                    {
                        brandUIModel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandUIModel.BrandImage));

                        brandUIModel.SelectedImage = "ms-appx:///Assets/SRCProduct/category_selected.png";

                        if (!alreadyExists)
                        {
                            SelectedBrandIdList?.Add(brandUIModel.BrandId);
                        }
                    }
                    else
                    {
                        brandUIModel.SelectedImage = " ";

                        brandUIModel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandUIModel.BrandImage));

                        if (alreadyExists)
                        {
                            SelectedBrandIdList.Remove(brandUIModel.BrandId);
                        }
                    }

                    BrandList[indexFound] = brandUIModel;

                    SelectedBrandIdList?.Clear();

                    foreach (var brand in BrandList)
                    {
                        if (brand.IsSelected)
                        {
                            SelectedBrandIdList?.Add(brand.BrandId);
                        }
                    }

                    //if (SelectedCategoryIdList.Count == 0 && SelectedBrandIdList?.Count == 0 && SelectedStyleIdList.Count == 0)
                    //{
                    //    await GetCategoryListData();
                    //    await GetStyleListData();
                    //    await GetBrandListData();
                    //}
                    //else if (SelectedBrandIdList?.Count > 0)
                    //{
                    //    await GetFilteredCategoryListOnBrandClick(SelectedBrandIdList);

                    //    await GetFilteredStyleListOnBrandClick(SelectedBrandIdList);
                    //}

                    await GetFilteredCategoryBrandStyleData();

                    await FilterProductsListOnFilterSelectUnSelect();

                    GetFilteredProductDetailValues();

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "BrandClicked", ex.StackTrace);
            }
        }

        private async Task GetFilteredCategoryListOnBrandClick(List<int> selectedBrandList)
        {
            ObservableCollection<CategoryUIModel> CategoryListCopy = new ObservableCollection<CategoryUIModel>();

            List<CategoryUIModel> categoryUIModelList = await ((App)Application.Current).QueryService.GetFilteredCategoryDataWhenBrandIsSelected(selectedBrandList);

            if (categoryUIModelList != null)
            {
                CategoryList.Clear();

                foreach (var categorymodel in categoryUIModelList)
                {
                    if (SelectedCategoryIdList.Count > 0)
                    {
                        foreach (var categoryIdItem in SelectedCategoryIdList)
                        {
                            if (categorymodel.CategoryId == categoryIdItem)
                            {
                                categorymodel.IsSelected = true;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                categorymodel.IsSelected = false;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                            }
                        }
                    }
                    else
                    {
                        categorymodel.IsSelected = false;
                        categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    CategoryUIModel categoryUIModel = new CategoryUIModel()
                    {
                        CategoryId = categorymodel.CategoryId,
                        CategoryName = categorymodel.CategoryName,
                        IsSelected = categorymodel.IsSelected,
                        CategoryImage = categorymodel.CategoryImage
                    };

                    CategoryListCopy.Add(categoryUIModel);
                }

                List<CategoryUIModel> unOrderedCategoryList = new List<CategoryUIModel>(CategoryListCopy);

                List<CategoryUIModel> orderedCategoryList = unOrderedCategoryList?.OrderBy(x => x.CategoryId).ToList();

                CategoryList = new ObservableCollection<CategoryUIModel>(orderedCategoryList);
            }
        }

        private async Task GetFilteredStyleListOnBrandClick(List<int> selectedBrandList)
        {
            ObservableCollection<StyleUIModel> StyleListCopy = new ObservableCollection<StyleUIModel>();

            List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetFilteredStyleDataWhenBrandIsSelected(selectedBrandList);

            if (styleUIModelList != null)
            {
                StyleList.Clear();

                foreach (var stylemodel in styleUIModelList)
                {
                    if (SelectedStyleIdList.Count > 0)
                    {
                        foreach (var styleIdItem in SelectedStyleIdList)
                        {
                            if (stylemodel.StyleId == styleIdItem)
                            {
                                stylemodel.IsSelected = true;
                                stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_selected.png";
                                break;
                            }
                            else
                            {
                                stylemodel.IsSelected = false;
                                stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                            }
                        }
                    }
                    else
                    {
                        stylemodel.IsSelected = false;
                        stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                    }

                    StyleUIModel styleUIModel = new StyleUIModel()
                    {
                        StyleId = stylemodel.StyleId,
                        StyleName = stylemodel.StyleName,
                        IsSelected = stylemodel.IsSelected,
                        StyleImage = stylemodel.StyleImage
                    };

                    StyleListCopy.Add(styleUIModel);
                }

                //List<StyleUIModel> unOrderedStyleList = new List<StyleUIModel>(StyleListCopy);
                //List<StyleUIModel> orderedStyleList = unOrderedStyleList?.OrderBy(x => x.StyleId).ToList();
                StyleList = new ObservableCollection<StyleUIModel>(StyleListCopy.OrderBy(x => x.StyleName));

                if (StyleList?.Count == 0)
                {
                    NoBrandVisibility = Visibility.Visible;
                }
                else
                {
                    NoBrandVisibility = Visibility.Collapsed;
                }
            }
        }

        private async Task GetFilteredStyleListOnCategoryAndBrandClick(List<int> selectedCategoryList, List<int> selectedBrandList)
        {
            ObservableCollection<StyleUIModel> StyleListCopy = new ObservableCollection<StyleUIModel>();

            List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetFilteredStyleDataWhenCategoryAndBrandSelected(selectedCategoryList, selectedBrandList);

            if (styleUIModelList != null)
            {
                StyleList.Clear();

                foreach (var stylemodel in styleUIModelList)
                {
                    if (SelectedStyleIdList.Count > 0)
                    {
                        foreach (var styleIdItem in SelectedStyleIdList)
                        {
                            if (stylemodel.StyleId == styleIdItem)
                            {
                                stylemodel.IsSelected = true;
                                stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_selected.png";
                                break;
                            }
                            else
                            {
                                stylemodel.IsSelected = false;
                                stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                            }
                        }
                    }
                    else
                    {
                        stylemodel.IsSelected = false;
                        stylemodel.StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png";
                    }

                    StyleUIModel styleUIModel = new StyleUIModel()
                    {
                        StyleId = stylemodel.StyleId,
                        StyleName = stylemodel.StyleName,
                        IsSelected = stylemodel.IsSelected,
                        StyleImage = stylemodel.StyleImage
                    };

                    StyleListCopy.Add(styleUIModel);
                }

                //List<StyleUIModel> unOrderedStyleList = new List<StyleUIModel>(StyleListCopy);
                //List<StyleUIModel> orderedStyleList = unOrderedStyleList?.OrderBy(x => x.StyleId).ToList();
                StyleList = new ObservableCollection<StyleUIModel>(StyleListCopy.OrderBy(x => x.StyleName));

                if (StyleList?.Count == 0)
                {
                    NoBrandVisibility = Visibility.Visible;
                }
                else
                {
                    NoBrandVisibility = Visibility.Collapsed;
                }
            }
        }

        private async Task GetFilteredBrandListOnCategoryAndStyleClick(List<int> selectedCategoryList, List<int> selectedStyleList)
        {
            ObservableCollection<BrandUIModel> BrandListCopy = new ObservableCollection<BrandUIModel>();

            List<BrandUIModel> brandUIModelList = await ((App)Application.Current).QueryService.GetFilteredBrandDataWhenCategoryAndStyleSelected(selectedCategoryList, selectedStyleList);

            if (brandUIModelList != null)
            {
                BrandList.Clear();

                foreach (var brandmodel in brandUIModelList)
                {
                    if (SelectedBrandIdList.Count > 0)
                    {
                        foreach (var brandIdItem in SelectedBrandIdList)
                        {
                            if (brandmodel.BrandId == brandIdItem)
                            {
                                brandmodel.IsSelected = true;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                brandmodel.IsSelected = false;
                                brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                                brandmodel.SelectedImage = " ";
                            }
                        }
                    }
                    else
                    {
                        brandmodel.IsSelected = false;
                        brandmodel.BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                ApplicationConstants.BrandImageBaseFolder,
                                HelperMethods.GetNameFromURL(brandmodel.BrandImage));
                        brandmodel.SelectedImage = " ";
                    }

                    BrandUIModel brandUIModel = new BrandUIModel()
                    {
                        BrandId = brandmodel.BrandId,
                        BrandName = brandmodel.BrandName,
                        IsSelected = false,
                        BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH,
                                    ApplicationConstants.BrandImageBaseFolder,
                                    HelperMethods.GetNameFromURL(brandmodel.BrandImage)),
                        BrandImage = brandmodel.BrandImage,
                        SelectedImage = brandmodel.SelectedImage
                    };

                    BrandListCopy.Add(brandUIModel);
                }

                //List<BrandUIModel> unOrderedBrandList = new List<BrandUIModel>(BrandListCopy);
                //List<BrandUIModel> orderedBrandList = unOrderedBrandList?.OrderBy(x => x.BrandId).ToList();

                BrandList = new ObservableCollection<BrandUIModel>(BrandListCopy.OrderBy(x => x.BrandName));

                if (BrandList?.Count == 0)
                {
                    NoBrandVisibility = Visibility.Visible;
                }
                else
                {
                    NoBrandVisibility = Visibility.Collapsed;
                }
            }
        }

        private async Task GetFilteredCategoryListOnBrandAndStyleClick(List<int> selectedBrandList, List<int> selectedStyleList)
        {
            ObservableCollection<CategoryUIModel> CategoryListCopy = new ObservableCollection<CategoryUIModel>();

            List<CategoryUIModel> categoryUIModelList = await ((App)Application.Current).QueryService.GetFilteredCategoryDataWhenBrandAndStyleSelected(selectedBrandList, selectedStyleList);

            if (categoryUIModelList != null)
            {
                CategoryList.Clear();

                foreach (var categorymodel in categoryUIModelList)
                {
                    if (SelectedCategoryIdList.Count > 0)
                    {
                        foreach (var categoryIdItem in SelectedCategoryIdList)
                        {
                            if (categorymodel.CategoryId == categoryIdItem)
                            {
                                categorymodel.IsSelected = true;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_selected.png";
                                break;
                            }
                            else
                            {
                                categorymodel.IsSelected = false;
                                categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                            }
                        }
                    }
                    else
                    {
                        categorymodel.IsSelected = false;
                        categorymodel.CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png";
                    }

                    CategoryUIModel categoryUIModel = new CategoryUIModel()
                    {
                        CategoryId = categorymodel.CategoryId,
                        CategoryName = categorymodel.CategoryName,
                        IsSelected = categorymodel.IsSelected,
                        CategoryImage = categorymodel.CategoryImage
                    };

                    CategoryListCopy.Add(categoryUIModel);
                }

                List<CategoryUIModel> unOrderedCategoryList = new List<CategoryUIModel>(CategoryListCopy);

                List<CategoryUIModel> orderedCategoryList = unOrderedCategoryList?.OrderBy(x => x.CategoryId).ToList();

                CategoryList = new ObservableCollection<CategoryUIModel>(orderedCategoryList);
            }
        }

        private void SalesDocsClicked(GridControl gridControl)
        {
            if (gridControl != null)
            {          
                int index = GetSalesDocsIndex();

                if (CategoryList[index].IsSelected)
                {
                    gridControl.Columns["ItemNumber"].Visible = true;
                    gridControl.Columns["ItemDescription"].Visible = false;
                    gridControl.Columns["Link"].Visible = true;
                    gridControl.Columns["PdfImage"].Visible = true;
                    gridControl.Columns["CartImage"].Visible = false;
                    gridControl.Columns["DistributionImage"].Visible = false;
                    gridControl.Columns["DistributionRecordedDate"].Visible = false;
                    gridControl.Columns["FavoriteImage"].Visible = false;
                    gridControl.Columns["FavoriteSalesDocs"].Visible = true;

                    isSalesDocClicked = true;
                    filetype = "3";
                }
                else
                {
                    isSalesDocClicked = false;
                    filetype = "1";
                    SetDefaultGridVisibility(gridControl);
                }
            }
        }

        private void SetDefaultGridVisibility(GridControl gridControl)
        {       
            if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
            {
                if (gridControl != null)
                {
                    gridControl.Columns["ItemNumber"].Visible = true;
                    gridControl.Columns["ItemDescription"].Visible = true;
                    gridControl.Columns["Link"].Visible = false;
                    gridControl.Columns["PdfImage"].Visible = true;
                    gridControl.Columns["CartImage"].Visible = true;
                    gridControl.Columns["DistributionImage"].Visible = true;
                    gridControl.Columns["DistributionRecordedDate"].Visible = true;
                    gridControl.Columns["FavoriteImage"].Visible = true;
                    gridControl.Columns["FavoriteSalesDocs"].Visible = false;
                }
            }
            else
            {
                if (gridControl != null)
                {
                    gridControl.Columns["ItemNumber"].Visible = true;
                    gridControl.Columns["ItemDescription"].Visible = true;
                    gridControl.Columns["Link"].Visible = false;
                    gridControl.Columns["PdfImage"].Visible = true;
                    gridControl.Columns["CartImage"].Visible = true;
                    gridControl.Columns["DistributionImage"].Visible = false;
                    gridControl.Columns["DistributionRecordedDate"].Visible = false;
                    gridControl.Columns["FavoriteImage"].Visible = true;
                    gridControl.Columns["FavoriteSalesDocs"].Visible = false;
                }
            }
        }

        private void OtherCategoryClicked(GridControl gridControl)
        {
            if (gridControl != null)
            {
                isSalesDocClicked = false;
                filetype = "1";
               SetDefaultGridVisibility(gridControl);
            }
        }

        /// <summary>
        /// All, Open Stock and promotional filtering
        /// </summary>
        /// <param name="selectedIndex"></param>
        private void HandleHeaderComboboxChanged(int selectedIndex)
        {
            //0-All 1-Open stock 2-Promotional
            int index = selectedIndex;

            ProductGridDataSource.Clear();

            if (index == 0)
            {
                DbProductDataSource?.ForEach(x => ProductGridDataSource.Add(x));
            }
            else if (index == 1)
            {
                List<SRCProductUIModel> list = DbProductDataSource.Where(x => x.ProductType == 0).ToList();
                list?.ForEach(x => ProductGridDataSource.Add(x));
            }
            else if (index == 2)
            {
                List<SRCProductUIModel> list = DbProductDataSource.Where(x => x.ProductType == 1).ToList();
                list?.ForEach(x => ProductGridDataSource.Add(x));
            }
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            _ = args?.ClickedItem as ListBoxItem;
        }

        private void SuggestionChoosen(SRCProductUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }

            ProductGridDataSource.Clear();

            var _filterItem = DbProductDataSource.FirstOrDefault(x => x.ItemNumber.Equals(selectedItem.ItemNumber));
            if (_filterItem != null)
            {
                ListIconClicked();
            }
            ProductGridDataSource.Add(_filterItem);
        }

        private async Task HandleTextChangeHeaderCommand(string text)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(text))
            {
                var ifDataGridHasAlreadyData = DbProductDataSource?.Count == ProductGridDataSource?.Count;

                if (ifDataGridHasAlreadyData)
                {
                    LoadHeaderSearchWithInitialData();
                }
                else
                {
                    await LoadDataGridAndHeaderSearchWithInitialData();
                }
            }
            else
            {
                var tempList = DbProductDataSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();

                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new SRCProductUIModel() { ItemNumber = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(x => HeaderSearchItemSource.Add(x));
                }
            }
        }

        private void LoadHeaderSearchWithInitialData()
        {
            DbProductDataSource.ForEach(x => { HeaderSearchItemSource.Add(x); });
        }

        private async Task LoadDataGridAndHeaderSearchWithInitialData()
        {
            LoadingVisibilityHandler(isLoading: true);

            await Task.Delay(200);

            ProductGridDataSource.Clear();

            DbProductDataSource.ForEach(x =>
            {
                ProductGridDataSource.Add(x);
            });
            HeaderSearchItemSource.Clear();

            LoadingVisibilityHandler(isLoading: false);
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }
        private void NavigateToCartPage()
        {
            if (AppRef.CartItemCount > 0)
            {
                if (AppRef.CartDataFromScreen == 0)
                {
                    NavigationService.Navigate<CartPage>();
                }
                else if (AppRef.CartDataFromScreen == 1)
                {
                    NavigationService.Navigate<RetailTransactionPage>();
                }
            }
            else if (AppRef.CartItemCount == 0)
            {
                NavigationService.Navigate<CartPage>();
            }
        }
        private void DistributionImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            try
            {
                if (!IsDirectCustomer)
                {
                    if (sRCProductUIModel.IsDistributed == 0)
                    {
                        sRCProductUIModel.IsDistributed = 1;
                        sRCProductUIModel.DistributionImage = "ms-appx:///Assets/SRCProduct/distribution_selected.png";
                        sRCProductUIModel.DistributionRecordedDate = DateTimeHelper.ConvertStringDateToMM_DD_YYYY(DateTime.Now.ToString());

                    }
                    else
                    {
                        sRCProductUIModel.IsDistributed = 0;
                        sRCProductUIModel.DistributionImage = "ms-appx:///Assets/SRCProduct/distribution_normal.png";
                        sRCProductUIModel.DistributionRecordedDate = string.Empty;
                    }
                    isDistributionCommandFired = isFromProductList;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "DistributionImageClicked", ex.StackTrace);
            }
        }

        #region Product Detail screen Methods
        /// <summary>
        /// Add to Cart click on product Detail screen
        /// </summary>
        /// <param name="srcProductUiModel"></param>
        private async Task ProductDetailAddToCart()
        {
            isFromProductList = false;

            if (SelectedProductDetail != null && ProductDetailModel != null)
            {
                if (string.IsNullOrEmpty(ProductDetailModel.QuantityDisplay))
                {
                    SelectedProductDetail.Quantity = 1;
                    ProductDetailModel.QuantityDisplay = "1";
                }
                else
                {
                    SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay);
                    await CartImageClicked(SelectedProductDetail);
                }
            }
        }

        /// <summary>
        /// List icon click on product Detail screen
        /// </summary>
        /// <param name="srcProductUiModel"></param>
        private void ListIconClicked()
        {
            ProductDetailVisibility = Visibility.Collapsed;
            IsProductDetailsVisible = false;
            ProductListVisibility = Visibility.Visible;
            NoProductVisibility = Visibility.Collapsed;
            LoadingVisibility = Visibility.Collapsed;
            isFromProductList = true;

        }

        private void ProductSelected(SRCProductUIModel selectedProduct)
        {
            if (!isCartCommandFired && !isFavoriteCommandFired && !isFavoriteSalesDocsCommandFired && !isDistributionCommandFired && !isPdfCommandFired)
            {
                if (!isSalesDocClicked)
                {
                    ShowProductDetailScreen();

                    if (selectedProduct != null)
                    {
                        GetSelectedProductValues(selectedProduct);
                    }
                }
            }
            else
            {
                isCartCommandFired = false;
                isFavoriteCommandFired = false;
                isFavoriteSalesDocsCommandFired = false;
                isDistributionCommandFired = false;
                isPdfCommandFired = false;
            }
        }

        private async void GetSelectedProductValues(SRCProductUIModel selectedProduct)
        {
            selectedProduct?.SrcProductToProductDetail();

            var product = new List<ProductDetailUiModel>() { selectedProduct?.ProductDetailMasterData };

            if (product?.Count > 0)
            {
                ProductDetailModel = product[0];
            }

            await GetAdditionalDocuments(selectedProduct);

            SelectedProductDetail = ProductGridDataSource?.FirstOrDefault(X => X.ProductID == ProductDetailModel.ProductId);

            if (SelectedProductDetail != null)
            {
                ProductDetailModel.SelectedProductDetailIndex = ProductGridDataSource.IndexOf(SelectedProductDetail);
            }

            SetLeftRightArrowImages();
            SetAddToCartDetailImage(selectedProduct);
            SetFavoriteDetailImage(selectedProduct);

            SetProductMainImage();
        }

        private void SetProductMainImage()
        {
            if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
            {
                var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.ProductImage);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    ProductDetailModel.ProductImagePath = filePath;
                }
                else
                {
                    ProductDetailModel.ProductImagePath = "ms-appx:///Assets/ProductDetail/default_product.png";
                }
            }
            else
            {
                ProductDetailModel.ProductImagePath = "ms-appx:///Assets/ProductDetail/default_product.png";
            }
        }

        private async Task GetAdditionalDocuments(SRCProductUIModel selectedProduct)
        {
            if (selectedProduct != null)
            {
                var productDetailUiModel = await ((App)Application.Current).QueryService.GetProductAdditionalDocumentData(selectedProduct.ProductID);
                ProductDetailModel.Factsheet = productDetailUiModel?.Factsheet;
                ProductDetailModel.RetailImage = productDetailUiModel?.RetailImage;
                ProductDetailModel.ProductImage = productDetailUiModel?.ProductImage;
                ProductDetailModel.SalesDocs = productDetailUiModel?.SalesDocs;
            }
        }

        private void SetLeftRightArrowImages()
        {
            if (ProductGridDataSource?.Count > 1)
            {
                if (ProductDetailModel?.SelectedProductDetailIndex == 0)
                {
                    ProductDetailModel.LeftArrowImage = "ms-appx:///Assets/ProductDetail/left_arrow_normal.png";
                    ProductDetailModel.RightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_hover.png";
                }
                else if (ProductDetailModel?.SelectedProductDetailIndex == ProductGridDataSource?.Count - 1)
                {
                    ProductDetailModel.LeftArrowImage = "ms-appx:///Assets/ProductDetail/left_arrow_hover.png";
                    ProductDetailModel.RightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_normal.png";
                }
                else
                {
                    ProductDetailModel.LeftArrowImage = "ms-appx:///Assets/ProductDetail/left_arrow_hover.png";
                    ProductDetailModel.RightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_hover.png";
                }
            }
            else
            {
                ProductDetailModel.LeftArrowImage = "ms-appx:///Assets/ProductDetail/left_arrow_normal.png";
                ProductDetailModel.RightArrowImage = "ms-appx:///Assets/ProductDetail/right_arrow_normal.png";
            }
        }

        private void SetAddToCartDetailImage(SRCProductUIModel selectedProduct)
        {
            if (selectedProduct.IsAddedToCart)
            {
                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_white.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_active.png";
            }
            else
            {
                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_black.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_inactive.png";
            }
        }
        private void SetFavoriteDetailImage(SRCProductUIModel selectedProduct)
        {
            if (selectedProduct.IsFavorite)
            {
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_selected.png";
            }
            else
            {
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_normal.png";
            }
        }

        private void ShowProductDetailScreen()
        {
            IsProductDetailsVisible = true;
            ProductDetailVisibility = Visibility.Visible;
            ProductListVisibility = Visibility.Collapsed;
            NoProductVisibility = Visibility.Collapsed;
            LoadingVisibility = Visibility.Collapsed;
        }

        private void RightArrowClicked()
        {
            if (ProductDetailModel != null)
            {
                int index = ProductDetailModel.SelectedProductDetailIndex;
                if (index < ProductGridDataSource?.Count - 1)
                {
                    int nextIndex = index + 1;
                    var nextProduct = ProductGridDataSource[nextIndex];
                    if (nextProduct != null)
                    {
                        GetSelectedProductValues(nextProduct);
                    }
                }
            }
        }

        private void LeftArrowClicked()
        {
            if (ProductDetailModel != null)
            {
                int index = ProductDetailModel.SelectedProductDetailIndex;
                if (index > 0)
                {
                    int prevIndex = index - 1;
                    if (ProductGridDataSource != null)
                    {
                        var previousProduct = ProductGridDataSource[prevIndex];
                        if (previousProduct != null)
                        {
                            GetSelectedProductValues(previousProduct);
                        }
                    }
                }
            }
        }

        private async Task ShowFileDoesNotExist()
        {

            ContentDialog productCannotBeAddedDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("FileDoesNotExist"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await productCannotBeAddedDialog.ShowAsync();
        }
        private void FavoriteDetailImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            isFromProductList = false;
            if (SelectedProductDetail != null)
            {
                FavoriteImageClicked(SelectedProductDetail);
            }
        }

        private async Task ProductImageClicked()
        {
            if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
            {
                var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.ProductImage);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    ProductDetailModel.ProductImagePath = filePath;
                    PreviewDocumentCommandHandler(ProductDetailModel.ProductImagePath);
                }
                else
                {
                    await ShowFileDoesNotExist();
                }
            }
            else
            {
               await ShowFileDoesNotExist();
            }           
        }

        private async Task RetailImageClicked()
        {
            if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
            {
                var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.RetailImage);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    ProductDetailModel.ProductImagePath = filePath;
                    PreviewDocumentCommandHandler(ProductDetailModel.ProductImagePath);
                }
                else
                {
                    await ShowFileDoesNotExist();
                }
            }
            else
            {
                await ShowFileDoesNotExist();
            }
        }

        private async Task FactsheetClicked()
        {
            if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
            {
                var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.Factsheet);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    ProductDetailModel.ProductImagePath = filePath;
                    PreviewDocumentCommandHandler(ProductDetailModel.ProductImagePath);
                }
                else
                {
                    await ShowFileDoesNotExist();
                }
            }
            else
            {
                await ShowFileDoesNotExist();
            }
        }

        private void PreviewDocumentCommandHandler(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                PreviewUrl = null;
                PreviewUrl = url;
                PreviewDocumentVisibility = true;
            }
            else
            {
                PreviewDocumentVisibility = false;
            }
        }

        private async Task PdfFactsheetCommandHandler(SRCProductUIModel selectedObj)
        {
            try
            {
                isPdfCommandFired = true;
                LoadingVisibility = Visibility.Visible;
                await GetAdditionalDocuments(selectedObj);
                string filePath = string.Empty;
                if (!isSalesDocClicked)
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel.Factsheet);
                else
                    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.SalesDocs, ProductDetailModel.SalesDocs);

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    PreviewUrl = filePath;
                    PreviewDocumentVisibility = true;
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("FileDoesNotExist"), ResourceExtensions.GetLocalized("OK"));

                }

                LoadingVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                LoadingVisibility = Visibility.Collapsed;
                ErrorLogger.WriteToErrorLog(nameof(SrcProductPageViewModel), nameof(PdfFactsheetCommandHandler), ex.StackTrace);
            }

        }
        private void NumPadButtonClicked(string value)
        {
            UpdateQuantityFieldValue(ProductDetailModel, value);            
        }
        private void UpdateQuantityFieldValue(ProductDetailUiModel item, string value)
        {
                   
                    if (value == "C")
                    {
                        if (item?.QuantityDisplay.Length > 0)
                        {
                            item.QuantityDisplay = item?.QuantityDisplay.Remove(item.QuantityDisplay.Length - 1);
                        }
                    }
                    else
                    {
                        if (item?.QuantityDisplay.Length == 0)
                        {
                            if (value != "0" && value != "-")
                            {
                                item.QuantityDisplay = item?.QuantityDisplay + value;
                            }
                            else
                            {
                                item.QuantityDisplay = string.Empty;
                            }
                        }
                        else if (item?.QuantityDisplay?.Length < 4 && (value != "-"))
                        {
                            item.QuantityDisplay = item?.QuantityDisplay + value;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
        }

        #endregion

        #endregion
    }
}
