using AutoMapper;
using AutoMapper.Internal;

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.UI.Xaml.Grid;

using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class SrcProductPageViewModel : BaseModel
    {
        #region Properties
        IEnumerable<int> intialExcludedCategories = new List<int> { -88, -99, 10, 11 };
        private readonly Windows.UI.Core.CoreDispatcher coreDispatcher;
        private Stopwatch stopWatch = null;
        private bool isExecutionLocked = false;
        private const long timeGap = 300;
        private long currentMilliseconds = timeGap;

        private ObservableCollection<CategoryUIModel> _categoryCollection;
        public ObservableCollection<CategoryUIModel> CategoryCollection
        {
            get => _categoryCollection;
            set { _categoryCollection = value; OnPropertyChanged(); }
        }

        private ObservableCollection<StyleUIModel> _styleCollection;
        public ObservableCollection<StyleUIModel> StyleCollection
        {
            get => _styleCollection;
            set { _styleCollection = value; OnPropertyChanged(); }
        }

        private ObservableCollection<BrandUIModel> _brandCollection;
        public ObservableCollection<BrandUIModel> BrandCollection
        {
            get => _brandCollection;
            set { _brandCollection = value; OnPropertyChanged(); }
        }

        private bool _isProductGridLoad = false;
        public bool IsProductGridLoad
        {
            get { return _isProductGridLoad; }
            set { SetProperty(ref _isProductGridLoad, value); OnPropertyChanged("IsProductWithSaleCategory"); }
        }

        private bool _isNoProductLoad = false;
        public bool IsNoProductLoad
        {
            get { return _isNoProductLoad; }
            set { SetProperty(ref _isNoProductLoad, value); }
        }

        private bool _isProductDetailLoad = false;
        public bool IsProductDetailLoad
        {
            get { return _isProductDetailLoad; }
            set { SetProperty(ref _isProductDetailLoad, value); }
        }

        private Visibility _isSaleCategoryVisible = Visibility.Collapsed;
        public Visibility IsSaleCategoryVisible
        {
            get { return _isSaleCategoryVisible; }
            set
            {
                SetProperty(ref _isSaleCategoryVisible, value);
                OnPropertyChanged("IsProductWithSaleCategory");
                OnPropertyChanged("IsNoSelectedCustomerGrid");
                OnPropertyChanged("IsSelectedCustomerGrid");
            }
        }

        public bool IsSelectedCustomerGrid => !string.IsNullOrEmpty(((App)Application.Current).SelectedCustomerId.Trim()) && !IsProductWithSaleCategory;
        public bool IsNoSelectedCustomerGrid => string.IsNullOrEmpty(((App)Application.Current).SelectedCustomerId.Trim()) && !IsProductWithSaleCategory;

        private bool _isBrandGridLoad = false;
        public bool IsBrandGridLoad
        {
            get { return _isBrandGridLoad; }
            set { SetProperty(ref _isBrandGridLoad, value); }
        }

        private bool _isStyleGridLoad = false;
        public bool IsStyleGridLoad
        {
            get { return _isStyleGridLoad; }
            set { SetProperty(ref _isStyleGridLoad, value); }
        }
        private CategoryUIModel _selectedCategory;
        public CategoryUIModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged();
            }
        }

        public bool IsProductWithSaleCategory => (IsProductGridLoad
          && (IsSaleCategoryVisible == Visibility.Visible));

        private IEnumerable<CategoryUIModel> SelectedCategories { get { return CategoryCollection.Where(x => x.IsSelected); } }
        private IEnumerable<BrandUIModel> SelectedBrands { get { return BrandCollection.Where(x => x.IsSelected); } }
        private IEnumerable<StyleUIModel> SelectedStyles { get { return StyleCollection.Where(x => x.IsSelected); } }

        private string headerSearchTextSelected;



        #region Product Grid Column Properties

        private bool _isItemNumberVisible = false;
        public bool IsItemNumberVisible
        {
            get { return _isItemNumberVisible; }
            set { SetProperty(ref _isItemNumberVisible, value); }
        }
        private int _itemNumberVisibleIndex;
        public int ItemNumberVisibleIndex
        {
            get { return _itemNumberVisibleIndex; }
            set { SetProperty(ref _itemNumberVisibleIndex, value); }
        }
        private string _sortItemNumberText;
        public string SortItemNumberText
        {
            get { return _sortItemNumberText; }
            set { SetProperty(ref _sortItemNumberText, value); }
        }
        private string _sortFavoriteText;
        public string SortFavoriteText
        {
            get { return _sortFavoriteText; }
            set { SetProperty(ref _sortFavoriteText, value); }
        }
        private string _filterItemNumberText;
        public string FilterItemNumberText
        {
            get { return _filterItemNumberText; }
            set { SetProperty(ref _filterItemNumberText, value); }
        }

        private bool _isItemDescriptionVisible = false;
        public bool IsItemDescriptionVisible
        {
            get { return _isItemDescriptionVisible; }
            set { SetProperty(ref _isItemDescriptionVisible, value); }
        }
        private int _itemDescriptionVisibleIndex;
        public int ItemDescriptionVisibleIndex
        {
            get { return _itemDescriptionVisibleIndex; }
            set { SetProperty(ref _itemDescriptionVisibleIndex, value); }
        }
        private string _sortItemDescriptionText;
        public string SortItemDescriptionText
        {
            get { return _sortItemDescriptionText; }
            set { SetProperty(ref _sortItemDescriptionText, value); }
        }
        private string _filterItemDescriptionText;
        public string FilterItemDescriptionText
        {
            get { return _filterItemDescriptionText; }
            set { SetProperty(ref _filterItemDescriptionText, value); }
        }

        private bool _isLinkVisible = false;
        public bool IsLinkVisible
        {
            get { return _isLinkVisible; }
            set { SetProperty(ref _isLinkVisible, value); }
        }
        private int _linkVisibleIndex = 3;
        public int LinkVisibleIndex
        {
            get { return _linkVisibleIndex; }
            set { SetProperty(ref _linkVisibleIndex, value); }
        }
        private string _sortLinkText;
        public string SortLinkText
        {
            get { return _sortLinkText; }
            set { SetProperty(ref _sortLinkText, value); }
        }
        private string _filterLinkText;
        public string FilterLinkText
        {
            get { return _filterLinkText; }
            set { SetProperty(ref _filterLinkText, value); }
        }

        private bool _isPdfImgVisible = false;
        public bool IsPdfImgVisible
        {
            get { return _isPdfImgVisible; }
            set { SetProperty(ref _isPdfImgVisible, value); }
        }
        private int _pdfImgVisibleIndex = 4;
        public int PdfImgVisibleIndex
        {
            get { return _pdfImgVisibleIndex; }
            set { SetProperty(ref _pdfImgVisibleIndex, value); }
        }
        private string _sortPdfImageText;
        public string SortPdfImageText
        {
            get { return _sortPdfImageText; }
            set { SetProperty(ref _sortPdfImageText, value); }
        }

        private bool _isAddToCartVisible = false;
        public bool IsAddToCartVisible
        {
            get { return _isAddToCartVisible; }
            set { SetProperty(ref _isAddToCartVisible, value); }
        }
        private int _addToCartVisibleIndex;
        public int AddToCartVisibleIndex
        {
            get { return _addToCartVisibleIndex; }
            set { SetProperty(ref _addToCartVisibleIndex, value); }
        }
        private string _sortIsAddedToCartText;
        public string SortIsAddedToCartText
        {
            get { return _sortIsAddedToCartText; }
            set { SetProperty(ref _sortIsAddedToCartText, value); }
        }

        private bool _isDistImgVisible = false;
        public bool IsDistImgVisible
        {
            get { return _isDistImgVisible; }
            set { SetProperty(ref _isDistImgVisible, value); }
        }
        private int _distImgVisibleIndex;
        public int DistImgVisibleIndex
        {
            get { return _distImgVisibleIndex; }
            set { SetProperty(ref _distImgVisibleIndex, value); }
        }
        private string _sortDistributionImageText;
        public string SortDistributionImageText
        {
            get { return _sortDistributionImageText; }
            set { SetProperty(ref _sortDistributionImageText, value); }
        }

        private bool _isDistDateVisible = false;
        public bool IsDistDateVisible
        {
            get { return _isDistDateVisible; }
            set { SetProperty(ref _isDistDateVisible, value); }
        }
        private int _distDateVisibleIndex;
        public int DistDateVisibleIndex
        {
            get { return _distDateVisibleIndex; }
            set { SetProperty(ref _distDateVisibleIndex, value); }
        }
        private string _sortDistributionDateText;
        public string SortDistributionDateText
        {
            get { return _sortDistributionDateText; }
            set { SetProperty(ref _sortDistributionDateText, value); }
        }

        private string _filterDistributionDateText;
        public string FilterDistributionDateText
        {
            get { return _filterDistributionDateText; }
            set { SetProperty(ref _filterDistributionDateText, value); }
        }

        private bool _IsFavoriteVisible = false;
        public bool IsFavoriteVisible
        {
            get { return _IsFavoriteVisible; }
            set { SetProperty(ref _IsFavoriteVisible, value); }
        }
        private int _favoriteVisibleIndex;
        public int FavoriteVisibleIndex
        {
            get { return _favoriteVisibleIndex; }
            set { SetProperty(ref _favoriteVisibleIndex, value); }
        }
        private string _sortIsFavoriteText;
        public string SortIsFavoriteText
        {
            get { return _sortIsFavoriteText; }
            set { SetProperty(ref _sortIsFavoriteText, value); }
        }

        private bool _isFavoriteSalesDocVisible = false;
        public bool IsFavoriteSalesDocVisible
        {
            get { return _isFavoriteSalesDocVisible; }
            set { SetProperty(ref _isFavoriteSalesDocVisible, value); }
        }
        private int _favoriteSalesDocVisibleIndex;
        public int FavoriteSalesDocVisibleIndex
        {
            get { return _favoriteSalesDocVisibleIndex; }
            set { SetProperty(ref _favoriteSalesDocVisibleIndex, value); }
        }
        private string _sortFavoriteSalesDocsText;
        public string SortFavoriteSalesDocsText
        {
            get { return _sortFavoriteSalesDocsText; }
            set { SetProperty(ref _sortFavoriteSalesDocsText, value); }
        }


        #endregion



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
        private IEnumerable<SRCProductUIModel> DbProductDataSource2;
        private IEnumerable<SRCProductUIModel> InitialProductDataSource;
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
        private string _customerNumber = string.Empty;
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { SetProperty(ref _customerNumber, value); }
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

        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
        }

        private string _quantityString = string.Empty;
        public string quantityString
        {
            get { return _quantityString; }
            set { SetProperty(ref _quantityString, value); }
        }

        private PhotosPrintHelper _printHelper;
        public PhotosPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }
        #endregion

        #region Commands

        //public ICommand SortColumnClickCommand { get; private set; }        
        public IAsyncRelayCommand PageLoadedCommand { private set; get; }
        public IAsyncRelayCommand CategoryClickedCommand { private set; get; }
        public IAsyncRelayCommand BrandClickedCommand { private set; get; }
        public IAsyncRelayCommand StyleClickedCommand { private set; get; }
        public IAsyncRelayCommand ProductImageClickCommand { private set; get; }
        public IAsyncRelayCommand RetailImageClickCommand { private set; get; }
        public IAsyncRelayCommand FactSheetClickCommand { private set; get; }
        public IAsyncRelayCommand DetailAddToCartCommand { private set; get; }
        public IAsyncRelayCommand QuantityChangedCommand { get; private set; }
        public IAsyncRelayCommand NumPadButtonClickCommand { get; private set; }
        public IAsyncRelayCommand PdfFactsheetCommand { get; private set; }
        public IAsyncRelayCommand FavoriteSalesDocsImageCommand { private set; get; }
        public IAsyncRelayCommand CartImageCommand { private set; get; }
        public IAsyncRelayCommand LeftArrowClickCommand { private set; get; }
        public IAsyncRelayCommand RightArrowClickCommand { private set; get; }
        public IAsyncRelayCommand DistributionImageCommand { private set; get; }
        public IAsyncRelayCommand ListIconClickedCommand { private set; get; }
        public ICommand HeaderComboBoxClickedCommand { private set; get; }
        public ICommand CustomerPanelClickCommand { get; private set; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }

        public IAsyncRelayCommand SortColumnClickCommandAsync { get; private set; }
        public IAsyncRelayCommand FilterColumnClickCommandAsync { get; private set; }

        //public ICommand PageLoadedCommand { private set; get; }
        // public ICommand CartImageCommand { private set; get; }
        public ICommand FavoriteImageCommand { private set; get; }
        //public ICommand CategoryClickedCommand { private set; get; }

        //public ICommand ProductImageClickCommand { private set; get; }

        public ICommand SalesDocsClickedCommand { private set; get; }
        public ICommand OtherCategoryClickedCommand { private set; get; }
        //public ICommand HeaderComboBoxClickedCommand { private set; get; }
        //public ICommand HeaderSearchTextChangeCommand { private set; get; }
        // public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        private ICommand _cartButtonCommand;
        public ICommand CartButtonCommand => _cartButtonCommand ?? (_cartButtonCommand = new AsyncRelayCommand(NavigateToCartPageAsync));
        //public ICommand DistributionImageCommand { private set; get; }
        public ICommand ItemSelectedCommand { private set; get; }
        //public ICommand ListIconClickedCommand { private set; get; }
        //public ICommand DetailAddToCartCommand { private set; get; }
        //public ICommand FactSheetClickCommand { private set; get; }
        //public ICommand RetailImageClickCommand { private set; get; }

        public ICommand FavoriteDetailImageClickCommand { private set; get; }
        //public ICommand FavoriteSalesDocsImageCommand { private set; get; }
        //public ICommand LeftArrowClickCommand { private set; get; }
        //public ICommand RightArrowClickCommand { private set; get; }
        //public ICommand CustomerPanelClickCommand { get; private set; }
        public ICommand PreviewDocumentCommand { get; private set; }
        //public ICommand PdfFactsheetCommand { get; private set; }
        // public ICommand NumPadButtonClickCommand { get; private set; }
        //public ICommand QuantityChangedCommand { get; private set; }
        public ICommand PrintProductImageCommand { get; private set; }

        #endregion

        #region Constructor

        public SrcProductPageViewModel()
        {
            LoadingVisibility = Visibility.Visible;

            coreDispatcher = Windows.ApplicationModel.Core.CoreApplication
       .MainView.CoreWindow.Dispatcher;

            IsProductDetailsVisible = false;

            resourceLoader = ResourceLoader.GetForCurrentView();

            DbProductDataSource = new List<SRCProductUIModel>();
            ProductGridDataSource = new ObservableCollection<SRCProductUIModel>();
            HeaderSearchItemSource = new ObservableCollection<SRCProductUIModel>();

            ProductDetailModel = new ProductDetailUiModel();
            SelectedProductDetail = new SRCProductUIModel();

            PageLoadedCommand = new AsyncRelayCommand(LoadInitialPageData);

            InitializeCommands();

            SelectedCategoryIdList = new List<int>();
            SelectedBrandIdList = new List<int>();
            SelectedStyleIdList = new List<int>();
            PreviewDocumentVisibility = false;
        }

        #endregion

        #region Private Methods

        private void InitializeCommands()
        {
            CartImageCommand = new AsyncRelayCommand<SRCProductUIModel>(CartImageClicked);
            FavoriteImageCommand = new AsyncRelayCommand<SRCProductUIModel>(FavoriteImageClicked);
            FavoriteSalesDocsImageCommand = new AsyncRelayCommand<SRCProductUIModel>(FavoriteSalesDocsImageClicked);
            CustomerPanelClickCommand = new AsyncRelayCommand(CustomerPanelClicked);
            CategoryClickedCommand = new AsyncRelayCommand<CategoryUIModel>(CategoryClicked);
            StyleClickedCommand = new AsyncRelayCommand<StyleUIModel>(StyleClicked);
            BrandClickedCommand = new AsyncRelayCommand<BrandUIModel>(BrandClicked);
            //SalesDocsClickedCommand = new RelayCommand<GridControl>(SalesDocsClicked);
            //OtherCategoryClickedCommand = new RelayCommand<GridControl>(OtherCategoryClicked);
            HeaderComboBoxClickedCommand = new AsyncRelayCommand<int>(HandleHeaderComboboxChanged);
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HandleTextChangeHeaderCommand);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<SRCProductUIModel>(SuggestionChoosen);
            DistributionImageCommand = new AsyncRelayCommand<SRCProductUIModel>(DistributionImageClicked);
            ItemSelectedCommand = new RelayCommand<SRCProductUIModel>(ProductSelected);
            ListIconClickedCommand = new AsyncRelayCommand<SRCProductUIModel>(ListIconClicked);
            DetailAddToCartCommand = new AsyncRelayCommand(ProductDetailAddToCartAsync);
            FactSheetClickCommand = new AsyncRelayCommand(FactsheetClickedAsync);
            RetailImageClickCommand = new AsyncRelayCommand(RetailImageClickedAsync);
            ProductImageClickCommand = new AsyncRelayCommand(ProductImageClickedAsync);
            FavoriteDetailImageClickCommand = new RelayCommand<SRCProductUIModel>(FavoriteDetailImageClicked);
            LeftArrowClickCommand = new AsyncRelayCommand(LeftArrowClicked);
            RightArrowClickCommand = new AsyncRelayCommand(RightArrowClicked);
            PdfFactsheetCommand = new AsyncRelayCommand<SRCProductUIModel>(PdfFactsheetCommandHandler);
            PreviewDocumentCommand = new AsyncRelayCommand<string>(PreviewDocumentCommandHandlerAsync);
            NumPadButtonClickCommand = new AsyncRelayCommand<string>(NumPadButtonClickedAsync);
            QuantityChangedCommand = new AsyncRelayCommand(QuantityChangedAsync);
            PrintProductImageCommand = new AsyncRelayCommand(PrintProductImageCommandHandlerAsync);
            SortColumnClickCommandAsync = new AsyncRelayCommand<string>(SortColumnClickCommandHandlerAsync);
            FilterColumnClickCommandAsync = new AsyncRelayCommand<string>(FilterColumnClickCommandHandlerAsync);
        }

        private async Task LoadInitialPageData()
        {
            try
            {
                await GetCategoryListData();
                await GetBrandListData();
                await GetStyleListData();

                InitialProductDataSource = await ((App)Application.Current).QueryService.GetSRCProductsAsync(AppRef.CurrentDeviceOrderId);

                DbProductDataSource2 = InitialProductDataSource.Where(x =>
                !intialExcludedCategories.Any(y => y == x.CatId));

                if ((bool)AppRef.IsDistributionOptionClicked)
                {
                    DbProductDataSource2 = await GetProductOrderByDistributionDateAsync(DbProductDataSource2);
                }
                else
                {
                    if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
                    {
                        DbProductDataSource2 = await GetProductsWithDistributionDateAsync(DbProductDataSource2);
                    }
                }

                DbProductDataSource2?.ForEach(async (x) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                   () => ProductGridDataSource.Add(x)).AsTask().ConfigureAwait(false));

                IsProductGridLoad = DbProductDataSource2.Any();

                BadgeCount = AppRef.CartItemCount;

                if (BadgeCount > 0)
                {
                    IsBadgeVisible = Visibility.Visible;
                    BadgeText = BadgeCount.ToString();
                }
                else
                {
                    IsBadgeVisible = Visibility.Collapsed;
                    BadgeText = "";
                }

                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

                    var selectedCustomer = await ((App)Application.Current).QueryService.GetSavedCustomerInformation(customerId);

                    if (selectedCustomer != null)
                    {
                        CustomerNameNumber = selectedCustomer.CustomerName + " " + (!string.IsNullOrWhiteSpace(selectedCustomer.CustomerNumber) ? selectedCustomer.CustomerNumber : "");
                        CustomerNumber = selectedCustomer.CustomerNumber;
                        CustomerAddresss = selectedCustomer.PhysicalAddress;
                        CustomerCityState = selectedCustomer.SubAddressText;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                        IsDirectCustomer = selectedCustomer.AccountType != 2;
                    }
                }

                if (AppRef.CurrentOrderId != 0 && (bool)AppRef.IsOrderTypeChanged)
                {
                    //bool isExecutable = true;

                    //if (AppRef.IsCreditRequestOrder ?? false)
                    //{
                    //    // This is to check it appear 1st time only
                    //    int memCurrentOrderId = MemoryCacheService.Instance.Get<int>("CurrentOrderId");
                    //    // This is to check it appear 1st time only
                    //    if (memCurrentOrderId != 0 || memCurrentOrderId == AppRef.CurrentOrderId)
                    //    {
                    //        isExecutable = false;
                    //    }
                    //}
                    //if (isExecutable)
                    //{
                    //    List<OrderDetailUIModel> orderDetailsData = await AppRef.QueryService.GetCartProductDetailsData(AppRef.CurrentDeviceOrderId);

                    //    foreach (var item in orderDetailsData)
                    //    {
                    //        item.OrderDetailUiToDataModel();

                    //        await CheckForOrderTypeAndSetOrderDetailsData(item.OrderDetailData);
                    //    }

                    //}
                    if (AppRef.IsCreditRequestOrder ?? false)
                    {
                        List<OrderDetailUIModel> orderDetailsData = await AppRef.QueryService.GetCartProductDetailsData(AppRef.CurrentDeviceOrderId);

                        foreach (var item in orderDetailsData)
                        {
                            item.OrderDetailUiToDataModel();

                            await CheckForOrderTypeAndSetOrderDetailsData(item.OrderDetailData);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "LoadInitialPageData", ex);
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async Task GetCategoryListData()
        {
            var categoryUIModelList = await ((App)Application.Current).QueryService
                .GetCategoriesForProductsList();
            if (categoryUIModelList != null && categoryUIModelList.Any())
            {
                CategoryCollection = new ObservableCollection<CategoryUIModel>(categoryUIModelList);
            }
            else
            {
                CategoryCollection = new ObservableCollection<CategoryUIModel>();
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
                if (categoryUIModel.CategoryId == -99 && categoryUIModel.IsSelected) IsSaleCategoryVisible = Visibility.Visible;
                else IsSaleCategoryVisible = Visibility.Collapsed;

                if (SelectedCategories.Any(x => x.CategoryId == -99))
                {
                    CategoryCollection.Where(x => x.CategoryId != categoryUIModel.CategoryId).ForEach(x => x.IsSelected = false);
                }

                await FilterProductsListOnFilterSelectUnSelect();
                await GetFilteredProductDetailValues();

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CategoryClicked", ex);
            }
        }
        private async Task BrandClicked(BrandUIModel brandUIModel)
        {
            try
            {
                await FilterProductsListOnFilterSelectUnSelect();
                await GetFilteredProductDetailValues();
            }

            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(BrandClicked), ex.StackTrace);
            }
        }
        private async Task StyleClicked(StyleUIModel styleUIModel)
        {
            try
            {
                await FilterProductsListOnFilterSelectUnSelect();
                await GetFilteredProductDetailValues();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "StyleClicked", ex);
            }
        }

        private async Task FilterProductsListOnFilterSelectUnSelect()
        {
            await GetFilteredCategoryBrandStyleData();

            DbProductDataSource2 = InitialProductDataSource.Where(y =>
            {
                bool match = true;

                if (SelectedCategories.Count() > 0)
                {
                    match &= SelectedCategories.Any(x => x.CategoryId == y.CatId);
                }
                if (SelectedBrands.Count() > 0)
                {
                    match &= SelectedBrands.Any(x => x.BrandId == y.BrandId);
                }
                if (SelectedStyles.Count() > 0)
                {
                    match &= SelectedStyles.Any(x => x.StyleId == y.StyleId);
                }

                if ((SelectedCategories.Count()
                + SelectedBrands.Count()
                + SelectedStyles.Count()) == 0)
                {
                    match &=
                   !intialExcludedCategories.Any(x => x == y.CatId);
                }

                return match;
            });

            await LoadSRCProductGridAsync();
        }

        private IEnumerable<SRCProductUIModel> ApplySortToSRCProductGrid(IEnumerable<SRCProductUIModel> enumerableSrcProducts)
        {
            if (!string.IsNullOrWhiteSpace(SortFavoriteText))
            {
                if (SortFavoriteText == "sort_down")
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderByDescending(x => x.IsFavorite);
                }
                else
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderBy(x => x.IsFavorite);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortItemNumberText))
            {
                if (SortItemNumberText == "sort_down")
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderByDescending(x => x.ItemNumber);
                }
                else
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderBy(x => x.ItemNumber);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortItemDescriptionText))
            {
                if (SortItemDescriptionText == "sort_down")
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderByDescending(x => x.ItemDescription);
                }
                else
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderBy(x => x.ItemDescription);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortLinkText))
            {
                if (SortLinkText == "sort_down")
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderByDescending(x => x.Link);
                }
                else
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderBy(x => x.Link);
                }
            }
            if (!string.IsNullOrWhiteSpace(SortDistributionDateText))
            {
                if (SortDistributionDateText == "sort_down")
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderByDescending(x => x.DistributionDate);
                }
                else
                {
                    enumerableSrcProducts = enumerableSrcProducts.OrderBy(x => x.DistributionDate);
                }
            }
            return enumerableSrcProducts;
        }

        private IEnumerable<SRCProductUIModel> ApplyFilterToSRCProductGrid(IEnumerable<SRCProductUIModel> enumerableSrcProducts)
        {
            return enumerableSrcProducts.Where(x =>
            {
                bool isMatched = true;
                if (!string.IsNullOrWhiteSpace(_filterItemNumberText))
                {
                    if (!string.IsNullOrWhiteSpace(x.ItemNumber))
                        isMatched &= x.ItemNumber.Contains(_filterItemNumberText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterItemDescriptionText))
                {
                    if (!string.IsNullOrWhiteSpace(x.ItemDescription))
                        isMatched &= x.ItemDescription.Contains(_filterItemDescriptionText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterLinkText))
                {
                    if (!string.IsNullOrWhiteSpace(x.Link))
                        isMatched &= x.Link.Contains(_filterLinkText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                if (!string.IsNullOrWhiteSpace(_filterDistributionDateText))
                {
                    if (!string.IsNullOrWhiteSpace(x.DistributionRecordedDate))
                        isMatched &= x.DistributionDate.Value.ToString("MM-dd-yyyy").Contains(_filterDistributionDateText, StringComparison.OrdinalIgnoreCase);
                    else isMatched &= false;
                }
                else isMatched &= true;

                return isMatched;
            });
        }

        private async Task LoadSRCProductGridAsync()
        {
            LoadingVisibilityHandler(true);

            IEnumerable<SRCProductUIModel> enumerableSrcProducts = Enumerable.Empty<SRCProductUIModel>();
            if (!string.IsNullOrWhiteSpace(headerSearchTextSelected))
            {
                enumerableSrcProducts = DbProductDataSource2.Where(x => x.SearchDisplayPath.ToLower().Contains(headerSearchTextSelected.ToLower()));
            }
            else { enumerableSrcProducts = DbProductDataSource2; }

            ProductGridDataSource.Clear();
            await Task.Run(() =>
            {
                enumerableSrcProducts = ApplyFilterToSRCProductGrid(enumerableSrcProducts);
                enumerableSrcProducts = ApplySortToSRCProductGrid(enumerableSrcProducts);
                enumerableSrcProducts.ForEach(async (item) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 () => ProductGridDataSource.Add(item)).AsTask().ConfigureAwait(false));
            });
            LoadingVisibilityHandler(false);
        }

        private async Task SortColumnClickCommandHandlerAsync(string args)
        {
            string colName = args.Split('|')[0];
            string textValue = args.Split('|')[1];
            ClearSortColOrder();
            switch (colName)
            {
                case "favorite":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortFavoriteText = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortFavoriteText = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortFavoriteText = "";
                    }
                    break;
                case "itemnumber":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortItemNumberText = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortItemNumberText = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortItemNumberText = "";
                    }
                    break;
                case "description":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortItemDescriptionText = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortItemDescriptionText = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortItemDescriptionText = "";
                    }
                    break;
                case "link":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortLinkText = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortLinkText = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortLinkText = "";
                    }
                    break;
                case "distributiondate":
                    if (string.IsNullOrWhiteSpace(textValue))
                    {
                        SortDistributionDateText = "sort_up";
                    }
                    else if (textValue == "sort_up")
                    {
                        SortDistributionDateText = "sort_down";
                    }
                    else if (textValue == "sort_down")
                    {
                        SortDistributionDateText = "";
                    }
                    break;

                default:
                    break;
            }
            await LoadSRCProductGridAsync();
        }

        private async Task FilterColumnClickCommandHandlerAsync(string args)
        {
            if (stopWatch == null)
            {
                stopWatch = Stopwatch.StartNew();
                isExecutionLocked = true;
            }
            else if (stopWatch.ElapsedMilliseconds >= currentMilliseconds)
            {
                isExecutionLocked = true;
                currentMilliseconds = (stopWatch.ElapsedMilliseconds + timeGap);
            }
            if (isExecutionLocked)
            {
                isExecutionLocked = false;


                string colName = args.Split('|')[0];
                string textValue = args.Split('|')[1];
                textValue = textValue?.Trim();

                await LoadSRCProductGridAsync();

            }
        }

        private void ClearSortColOrder()
        {
            SortFavoriteText = SortItemNumberText = SortItemDescriptionText = SortLinkText = SortDistributionDateText = "";
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
            }
            else if (dialog.Result == Result.No)
            {
                await ChangeCustomerEmptyCart();
            }
        }

        private void ChangeCustomerKeepCart()
        {
            AppRef.PreviousSelectedCustomerId = AppRef.SelectedCustomerId;
            AppRef.SelectedCustomerId = string.Empty;
            CustomerAddresss = string.Empty;
            CustomerCityState = string.Empty;
            CustomerNameNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;
        }

        private async Task ChangeCustomerEmptyCart()
        {
            AppRef.PreviousSelectedCustomerId = AppRef.SelectedCustomerId;
            AppRef.SelectedCustomerId = string.Empty;
            CustomerAddresss = string.Empty;
            CustomerCityState = string.Empty;
            CustomerNameNumber = string.Empty;
            CustomerTitlePanelVisibility = Visibility.Collapsed;

            await AppRef.QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);

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


        //private async Task GetCategoryListData()
        //{
        //    var categoryUIModelList = await ((App)Application.Current).QueryService
        //        .GetCategoryFilterDataForProductsList();
        //    if (categoryUIModelList != null && categoryUIModelList.Any())
        //    {
        //        CategoryList = new ObservableCollection<CategoryUIModel>(categoryUIModelList);
        //    }
        //    else
        //    {
        //        CategoryList = new ObservableCollection<CategoryUIModel>();
        //    }
        //}


        //private async Task GetBrandListData()
        //{
        //    BrandList = new ObservableCollection<BrandUIModel>();

        //    ObservableCollection<BrandUIModel> BrandListCopy = new ObservableCollection<BrandUIModel>();

        //    List<BrandUIModel> brandUIModelList = await ((App)Application.Current).QueryService.GetBrandFilterDataForProductsList();

        //    foreach (var brandmodel in brandUIModelList)
        //    {
        //        BrandUIModel brandUIModel = new BrandUIModel()
        //        {
        //            BrandId = brandmodel.BrandId,
        //            BrandName = brandmodel.BrandName,
        //            IsSelected = false,
        //            BrandImage = brandmodel.BrandImage,
        //            BrandImageFromLocal = Path.Combine(ApplicationConstants.APP_PATH, ApplicationConstants.BrandImageBaseFolder,
        //            HelperMethods.GetNameFromURL(brandmodel.BrandImage)),
        //            SelectedImage = brandmodel.SelectedImage
        //        };

        //        BrandListCopy.Add(brandUIModel);
        //    }

        //    BrandList = new ObservableCollection<BrandUIModel>(BrandListCopy);

        //    NoBrandVisibility = BrandList?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        //}

        private async Task GetBrandListData()
        {
            var brandUIModelList = await ((App)Application.Current).QueryService
                .GetBrandsAsync();
            if (brandUIModelList != null && brandUIModelList.Any())
            {
                BrandCollection = new ObservableCollection<BrandUIModel>(brandUIModelList);
                IsBrandGridLoad = true;
            }
            else
            {
                BrandCollection = new ObservableCollection<BrandUIModel>();
                IsBrandGridLoad = false;
            }
        }

        //private async Task GetStyleListData()
        //{
        //    StyleList = new ObservableCollection<StyleUIModel>();

        //    ObservableCollection<StyleUIModel> StyleListCopy = new ObservableCollection<StyleUIModel>();

        //    List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetStyleFilterDataForProductsList();

        //    foreach (var stylemodel in styleUIModelList)
        //    {
        //        StyleUIModel styleUIModel = new StyleUIModel()
        //        {
        //            StyleId = stylemodel.StyleId,
        //            StyleName = stylemodel.StyleName,
        //            IsSelected = false,
        //            StyleImage = "ms-appx:///Assets/SRCProduct/style_unselected.png"
        //        };

        //        StyleListCopy.Add(styleUIModel);
        //    }

        //    StyleList = new ObservableCollection<StyleUIModel>(StyleListCopy);

        //    NoStyleVisibility = StyleList?.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        //}
        private async Task GetStyleListData()
        {
            List<StyleUIModel> styleUIModelList = await ((App)Application.Current).QueryService.GetStylesDataAsync();
            if (styleUIModelList != null && styleUIModelList.Any())
            {
                StyleCollection = new ObservableCollection<StyleUIModel>(styleUIModelList);
                IsStyleGridLoad = true;
            }
            else
            {
                StyleCollection = new ObservableCollection<StyleUIModel>();
                IsStyleGridLoad = false;
            }
        }

        /// <summary>
        /// Cart image click event
        /// </summary>
        /// <param name="sRCProductUIModel"></param>
        private async Task CartImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            LoadingVisibility = Visibility.Visible;
            try
            {

                isCartCommandFired = isFromProductList;

                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    ShowNoCustomerSelected();
                }
                else
                {
                    bool isErrorOccurred = false;

                    if (AppRef.IsCreditRequestOrder == true)
                    {
                        if (sRCProductUIModel.SRCHoneyReturnable != 0)
                        {
                            await VerifyCartItems(sRCProductUIModel);
                        }
                        else
                        {
                            isErrorOccurred = true;
                            ShowProductCannotBeAddedToCart();
                        }
                    }
                    if (AppRef.IsDistributionOptionClicked == true && !isErrorOccurred)
                    {
                        if (sRCProductUIModel.SRCCanIOrder != 0)
                        {
                            await VerifyCartItems(sRCProductUIModel);
                        }
                        else
                        {
                            isErrorOccurred = true;
                            ShowProductCannotBeAddedToCart();
                        }

                    }
                    if (AppRef.IsCreditRequestOrder == false && AppRef.IsDistributionOptionClicked == false)
                    {
                        if (sRCProductUIModel.SRCHoneySellable != 0 && !isErrorOccurred)
                        {
                            await VerifyCartItems(sRCProductUIModel);
                        }
                        else
                        {
                            isErrorOccurred = true;
                            ShowProductCannotBeAddedToCart();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CartImageClicked", ex.StackTrace);
            }
            LoadingVisibility = Visibility.Collapsed;
        }

        private async Task VerifyCartItems(SRCProductUIModel sRCProductUIModel)
        {
            if (AppRef.CartItemCount > 0 && AppRef.CartDataFromScreen != 0)
            {
                bool result;
                if (!string.IsNullOrWhiteSpace(CustomerNumber)
                    && CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                {
                    result = await AlertHelper.Instance.ShowConfirmationAlert("", $"Empty Current cart and continue with the Sample Order?", "YES", "NO");
                }
                else
                {
                    result = await AlertHelper.Instance.ShowConfirmationAlert("", $"Empty Current cart and continue with the {(IsDirectCustomer ? "Distributor" : "Retail Sale")} Order?", "YES", "NO");
                }

                if (result)
                {
                    var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);

                    if (isSuccess)
                    {
                        AppRef.CartItemCount = 0;
                        (Application.Current as App).OrderPrintName = "";
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
                bool result = true;
                if (AppRef.CurrentOrderId == 0)
                {
                    result = await InsertEntryIntoOrderMaster();
                }

                if (result)
                {
                    if (await InsertOrderDetailData(sRCProductUIModel) != null)
                    {
                        sRCProductUIModel.IsAddedToCart = true;
                        BadgeCount++;
                        AppRef.CartDataFromScreen = 0;
                    }
                }
            }
            else
            {
                if (await DeleteCartItemFromDb(sRCProductUIModel.ProductID, AppRef.CurrentDeviceOrderId))
                {
                    sRCProductUIModel.IsAddedToCart = false;
                    BadgeCount--;
                }
            }

            if (sRCProductUIModel.IsAddedToCart)
            {
                sRCProductUIModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_selected.png";
                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_white.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_active.png";
            }
            else
            {
                sRCProductUIModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";
                ProductDetailModel.ProductDetailCartImage = "ms-appx:///Assets/ProductDetail/cart_black.png";
                ProductDetailModel.ProductDetailCartBackgroundImage = "ms-appx:///Assets/ProductDetail/button_inactive.png";
            }

             ((App)Application.Current).CartItemCount = BadgeCount;

            if (BadgeCount <= 0)
            {
                IsBadgeVisible = Visibility.Collapsed;
            }
            else
            {
                IsBadgeVisible = Visibility.Visible;
                BadgeText = BadgeCount.ToString();
            }

        }

        private async Task FavoriteImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            isFavoriteCommandFired = isFromProductList;
            await SelectDeselectFavorite(sRCProductUIModel);
        }

        private async Task FavoriteSalesDocsImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            isFavoriteSalesDocsCommandFired = isFromProductList;
            await SelectDeselectFavoriteSalesDocs(sRCProductUIModel);
        }

        private async Task SelectDeselectFavorite(SRCProductUIModel sRCProductUIModel)
        {
            if (!sRCProductUIModel.IsFavorite)
            {
                sRCProductUIModel.IsFavorite = true;
                //sRCProductUIModel.FavoriteImage = "ms-appx:///Assets/SRCProduct/favorite_selected.png";
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_selected.png";
                await InsertFavoriteData(sRCProductUIModel);
            }
            else
            {
                sRCProductUIModel.IsFavorite = false;
                //sRCProductUIModel.FavoriteImage = "ms-appx:///Assets/SRCProduct/favorite_normal.png";
                ProductDetailModel.FavoriteImage = "ms-appx:///Assets/ProductDetail/favorite_normal.png";

                await InsertFavoriteData(sRCProductUIModel);
                ///await DeleteFavoriteItemFromDb(sRCProductUIModel.ProductID);
            }
        }

        private async Task SelectDeselectFavoriteSalesDocs(SRCProductUIModel sRCProductUIModel)
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

        //private async Task InsertFavoriteData(SRCProductUIModel sRCProductUIModel)
        //{
        //    //List<BrandUIModel> brandUiModelList = await ((App)Application.Current).QueryService.GetBrandFilterDataForProductsList();
        //    //List<StyleUIModel> styleUiModelList = await ((App)Application.Current).QueryService.GetStyleFilterDataForProductsList();

        //    //var brandInfo = brandUiModelList.FirstOrDefault(x => x.BrandId == sRCProductUIModel.BrandId);
        //    //var styleInfo = styleUiModelList.FirstOrDefault(x => x.StyleId == sRCProductUIModel.StyleId);
        //    var brandInfo = BrandList.FirstOrDefault(x => x.BrandId == sRCProductUIModel.BrandId);
        //    var styleInfo = StyleList.FirstOrDefault(x => x.StyleId == sRCProductUIModel.StyleId);

        //    sRCProductUIModel.FavoriteUiToDataModel();

        //    var _list = new List<Favorite>() { sRCProductUIModel.FavoriteMasterData };

        //    _list[0].BrandName = brandInfo?.BrandName;
        //    _list[0].StyleName = styleInfo?.StyleName;
        //    _list[0].UserId = Convert.ToInt32(AppRef.LoginUserIdProperty);

        //    if (sRCProductUIModel.IsFavorite)
        //    {
        //        _list[0].isDeleted = false;
        //        _list[0].IsExported = 0;
        //        _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
        //    }
        //    else
        //    {
        //        _list[0].isDeleted = true;
        //        _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
        //    }

        //    await ((App)Application.Current).QueryService.InsertFavorite(_list);
        //}

        private async Task InsertFavoriteData(SRCProductUIModel sRCProductUIModel)
        {
            var brandInfo = BrandCollection.FirstOrDefault(x => x.BrandId == sRCProductUIModel.BrandId);
            var styleInfo = StyleCollection.FirstOrDefault(x => x.StyleId == sRCProductUIModel.StyleId);

            sRCProductUIModel.FavoriteUiToDataModel();

            var _list = new List<Favorite>() { sRCProductUIModel.FavoriteMasterData };

            _list[0].BrandName = brandInfo?.BrandName;
            _list[0].StyleName = styleInfo?.StyleName;
            _list[0].UserId = Convert.ToInt32(AppRef.LoginUserIdProperty);
            _list[0].IsExported = 0;

            if (sRCProductUIModel.IsFavorite)
            {
                _list[0].isDeleted = false;
                _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            }
            else
            {
                _list[0].isDeleted = true;
                _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            }

            await ((App)Application.Current).QueryService.InsertFavorite(_list).ConfigureAwait(false);
        }

        private async Task<bool> InsertEntryIntoOrderMaster()
        {
            var orderMasterRecord = new OrderMaster();

            orderMasterRecord.IsExported = 1;

            var newOrderAdded = await ((App)Application.Current).QueryService.InsertOrUpdateOrderMaster(orderMasterRecord);

            if (newOrderAdded != null)
            {
                ((App)Application.Current).CurrentOrderId = newOrderAdded.OrderID;

                ((App)Application.Current).CurrentDeviceOrderId = newOrderAdded.DeviceOrderID;
            }
            return newOrderAdded != null;
        }

        /// <summary>
        /// Insert Cart Item to database
        /// </summary>
        /// <param name="sRCProductUIModel"></param>
        /// <returns></returns>
        private async Task<OrderDetail> InsertOrderDetailData(SRCProductUIModel sRCProductUIModel)
        {
            //List<CategoryUIModel> categoryUiModelList = await ((App)Application.Current).QueryService.GetCategoryFilterDataForProductsList();

            //var categoryInfo = categoryUiModelList.FirstOrDefault(x => x.CategoryId == sRCProductUIModel.CatId);

            var categoryInfo = CategoryCollection.FirstOrDefault(x => x.CategoryId == sRCProductUIModel.CatId);

            sRCProductUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { sRCProductUIModel.OrderDetailMasterData };

            _list[0].Total = "0.00";

            _list[0].OrderId = ((App)Application.Current).CurrentOrderId;

            if (_list[0].Quantity == 0)
            {
                _list[0].Quantity = 1;
            }

            _list[0].DeviceOrderID = ((App)Application.Current).CurrentDeviceOrderId;
            _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);

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

                _list[0].Price = "0";
                _list[0].Total = "0.00";
                //_list[0].CreditRequest = categoryInfo?.CategoryId == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";
                _list[0].CreditRequest = "RTN-Retail Returns";
                ///SetCreditRequestType(categoryInfo, _list);
            }
            else if (IsDirectCustomer)
            {
                _list[0].Unit = "CA";
            }
            else
            {
                SetUomForCategoryId(categoryInfo, _list);
            }

            return await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        private async Task QuantityChangedAsync()
        {
            if (SelectedProductDetail?.IsAddedToCart == true && !string.IsNullOrEmpty(ProductDetailModel.QuantityDisplay))
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay);

                    if (SelectedProductDetail.Quantity > 0)
                    {
                        SelectedProductDetail.Quantity = SelectedProductDetail.Quantity * -1;

                        ProductDetailModel.QuantityDisplay = SelectedProductDetail.Quantity.ToString();
                    }
                }
                else
                {
                    SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay);
                }

                SelectedProductDetail.OrderDetailUiToDataModel();

                var _list = new List<OrderDetail>() { SelectedProductDetail.OrderDetailMasterData };

                await ((App)Application.Current).QueryService.UpdateOrderDetail(_list[0], AppRef.CurrentDeviceOrderId);
            }
            else
            {
                if ((bool)AppRef.IsCreditRequestOrder)
                {
                    SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay);

                    if (SelectedProductDetail.Quantity > 0)
                    {
                        SelectedProductDetail.Quantity = SelectedProductDetail.Quantity * -1;

                        ProductDetailModel.QuantityDisplay = SelectedProductDetail.Quantity.ToString();
                    }
                }
            }
        }

        private async Task PrintProductImageCommandHandlerAsync()
        {
            if (!string.IsNullOrEmpty(PreviewUrl))
            {
                await PrintHelper.ShowPrintUIAsync();
            }
        }

        private async Task CheckForOrderTypeAndSetOrderDetailsData(OrderDetail orderDetailItem)
        {
            if ((bool)AppRef.IsCreditRequestOrder)
            {
                //orderDetailItem.Quantity *= -1;
                //orderDetailItem.Price = "0";
                //orderDetailItem.Total = "0.00";
                //orderDetailItem.Unit = orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ? "BX" : "EA";
                //orderDetailItem.CreditRequest = orderDetailItem.CategoryId == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";
                // This is to check it appear 1st time only
                //int memCurrentOrderId = MemoryCacheService.Instance.Get<int>("CurrentOrderId");
                //// This is to check it appear 1st time only
                //if (memCurrentOrderId == 0 || memCurrentOrderId != AppRef.CurrentOrderId)
                //{
                //    orderDetailItem.Unit = orderDetailItem.CategoryId == 1 || orderDetailItem.CategoryId == 6 ? "BX" : "EA";
                //    orderDetailItem.CreditRequest = "RTN-Retail Returns";
                //}
                //orderDetailItem.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
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

                if (string.IsNullOrEmpty(orderDetailItem.Price))
                {
                    orderDetailItem.Price = "0";
                }

                if (string.IsNullOrEmpty(orderDetailItem.Total))
                {
                    orderDetailItem.Total = "0.00";
                }

                orderDetailItem.CreditRequest = "";

                orderDetailItem.UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                if (IsDirectCustomer)
                {
                    orderDetailItem.Unit = "CA";
                }
                else
                {
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

        /// <summary>
        /// Delete cart item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task<bool> DeleteCartItemFromDb(int productId, string deviceOrderId)
        {
            return await ((App)Application.Current).QueryService.DeleteOrderDetail(productId, deviceOrderId);
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
                NavigationService.NavigateShellFrame(typeof(CustomersListPage));
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

        private async Task GetFilteredCategoryBrandStyleData()
        {
            if (SelectedCategories != null && SelectedBrands != null && SelectedStyles != null)
            {
                if (SelectedCategories.Count() == 0 && SelectedBrands.Count() == 0 && SelectedStyles.Count() == 0)
                {
                    await GetCategoryListData();
                    await GetStyleListData();
                    await GetBrandListData();
                }
                else if (SelectedCategories.Count() > 0 && SelectedBrands.Count() == 0 && SelectedStyles.Count() == 0)
                {
                    await GetFilteredBrandListOnCategoryClick(SelectedCategories);

                    if (SelectedCategories.Any(x => x.CategoryId != -99))
                    {
                        await GetFilteredStyleListOnCategoryClick(SelectedCategories);
                    }
                    else
                    {
                        await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                               () =>
                               {
                                   StyleCollection.Clear();
                                   IsStyleGridLoad = false;
                               }).AsTask().ConfigureAwait(false);
                    }
                }
                else if (SelectedBrands.Count() > 0 && SelectedCategories.Count() == 0 && SelectedStyles.Count() == 0)
                {
                    await GetFilteredCategoryListOnBrandClick(SelectedBrands);
                    await GetFilteredStyleListOnBrandClick(SelectedBrands);
                }
                else if (SelectedStyles.Count() > 0 && SelectedCategories.Count() == 0 && SelectedBrands.Count() == 0)
                {
                    await GetFilteredCategoryListOnSytleClick(SelectedStyles);

                    await GetFilteredBrandListOnSytleClick(SelectedStyles);
                }
                else if (SelectedCategories.Count() > 0 && SelectedBrands.Count() > 0 && SelectedStyles.Count() == 0)
                {
                    await GetFilteredStyleListOnCategoryAndBrandClick(SelectedCategories, SelectedBrands);
                }
                else if (SelectedCategories.Count() > 0 && SelectedBrands.Count() == 0 && SelectedStyles.Count() > 0)
                {
                    await GetFilteredBrandListOnCategoryAndStyleClick(SelectedCategories, SelectedStyles);
                }
                else if (SelectedCategories.Count() == 0 && SelectedBrands.Count() > 0 && SelectedStyles.Count() > 0)
                {
                    await GetFilteredCategoryListOnBrandAndStyleClick(SelectedBrands, SelectedStyles);
                }
            }
        }

        private async Task GetFilteredProductDetailValues()
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                 async () =>
                 {
                     if (ProductGridDataSource?.Count > 0)
                     {
                         ProductDetailModel.ItemNumber = ProductGridDataSource[0]?.ItemNumber;
                         ProductDetailModel.ItemDescription = ProductGridDataSource[0]?.ItemDescription;
                         ProductDetailModel.ProductId = ProductGridDataSource[0].ProductID;
                         ProductDetailModel.Quantity = ProductGridDataSource[0].Quantity;
                         ProductDetailModel.SelectedProductDetailIndex = 0;
                         await GetSelectedProductValues(ProductGridDataSource[0]);
                     }
                 }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetFilteredProductDetailValues", ex);
            }
        }

        private async Task GetFilteredBrandListOnCategoryClick(IEnumerable<CategoryUIModel> selectedCategories)
        {
            try
            {
                //Based on Selected Categories Get All Related Brands
                IEnumerable<string> enumberableFilterBrandIds = InitialProductDataSource.Join(selectedCategories,
                    prd => prd.CatId, cat => cat.CategoryId,
                    (prd, cat) => Convert.ToString(prd.BrandId)).Distinct();

                var brandUIModelList = await ((App)Application.Current).QueryService
                .GetFilterBrandsAsync(string.Join(',', enumberableFilterBrandIds));

                if (brandUIModelList != null && brandUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                     () =>
                     {
                         BrandCollection = new ObservableCollection<BrandUIModel>(brandUIModelList);
                         IsBrandGridLoad = true;
                     }).AsTask().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetFilteredBrandListOnCategoryClick", ex.StackTrace);
            }
        }

        private async Task GetFilteredStyleListOnCategoryClick(IEnumerable<CategoryUIModel> selectedCategories)
        {
            try
            {
                //Based on Selected Categories Get All Related Styles
                IEnumerable<string> enumberableFilterStyleIds = InitialProductDataSource.Join(selectedCategories,
                    prd => prd.CatId, cat => cat.CategoryId,
                    (prd, cat) => Convert.ToString(prd.StyleId)).Distinct();

                var styleUIModelList = await ((App)Application.Current).QueryService
                .GetFilterStylesAsync(string.Join(',', enumberableFilterStyleIds));

                if (styleUIModelList != null && styleUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                      () =>
                      {
                          StyleCollection = new ObservableCollection<StyleUIModel>(styleUIModelList);
                          IsStyleGridLoad = true;
                      }).AsTask().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredStyleListOnCategoryClick), ex);
            }
        }

        private void CategorySelectUnselectUI(CategoryUIModel categoryUIModel)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(CategorySelectUnselectUI), ex.StackTrace);
            }
        }

        /// <summary>
        /// Selection of Style Filter Item
        /// </summary>
        /// <param name="styleUIModel"></param>
        private async Task GetFilteredCategoryListOnSytleClick(IEnumerable<StyleUIModel> selectedStyles)
        {
            try
            {
                //Based on Styles Get All Related Categories
                IEnumerable<string> enumberableFilterCatIds = InitialProductDataSource.Join(selectedStyles,
                    prd => prd.StyleId, b => b.StyleId,
                    (prd, b) => Convert.ToString(prd.CatId)).Distinct();

                var catUIModelList = await ((App)Application.Current).QueryService
                .GetFilterCategoriesAsync(string.Join(',', enumberableFilterCatIds));

                if (catUIModelList != null && catUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                          () => CategoryCollection = new ObservableCollection<CategoryUIModel>(catUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredCategoryListOnSytleClick), ex);
            }
        }

        private async Task GetFilteredBrandListOnSytleClick(IEnumerable<StyleUIModel> selectedStyles)
        {
            try
            {
                //Based on Styles Get All Related Brands
                IEnumerable<string> enumberableFilterBrandIds = InitialProductDataSource.Join(selectedStyles,
                    prd => prd.StyleId, b => b.StyleId,
                    (prd, b) => Convert.ToString(prd.BrandId)).Distinct();

                var brandUIModelList = await ((App)Application.Current).QueryService
                .GetFilterBrandsAsync(string.Join(',', enumberableFilterBrandIds));

                if (brandUIModelList != null && brandUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                          () => BrandCollection = new ObservableCollection<BrandUIModel>(brandUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredBrandListOnSytleClick), ex);
            }
        }

        /// <summary>
        /// Selection of Brand Filter Item
        /// </summary>
        /// <param name="brandUIModel"></param>
        private async Task GetFilteredCategoryListOnBrandClick(IEnumerable<BrandUIModel> selectedBrands)
        {
            try
            {
                //Based on Brands Get All Related Categories
                IEnumerable<string> enumberableFilterCatIds = InitialProductDataSource.Join(selectedBrands,
                    prd => prd.BrandId, b => b.BrandId,
                    (prd, b) => Convert.ToString(prd.CatId)).Distinct();

                var catUIModelList = await ((App)Application.Current).QueryService
                .GetFilterCategoriesAsync(string.Join(',', enumberableFilterCatIds));

                if (catUIModelList != null && catUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                         () => CategoryCollection = new ObservableCollection<CategoryUIModel>(catUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredCategoryListOnBrandClick), ex);
            }
        }

        private async Task GetFilteredStyleListOnBrandClick(IEnumerable<BrandUIModel> selectedBrands)
        {
            try
            {
                //Based on Brands Get All Related Styles
                IEnumerable<string> enumberableFilterStyleIds = InitialProductDataSource.Join(selectedBrands,
                    prd => prd.BrandId, b => b.BrandId,
                    (prd, b) => prd).Where(p => p.CatId != -99)
                    .Select(z => Convert.ToString(z.StyleId)).Distinct();

                List<StyleUIModel> styleUIModelList = null;

                if (enumberableFilterStyleIds.Count() > 0)
                    styleUIModelList = await ((App)Application.Current).QueryService
                .GetFilterStylesAsync(string.Join(',', enumberableFilterStyleIds));

                if (styleUIModelList != null && styleUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => StyleCollection = new ObservableCollection<StyleUIModel>(styleUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredStyleListOnBrandClick), ex);
            }
        }

        private async Task GetFilteredStyleListOnCategoryAndBrandClick(IEnumerable<CategoryUIModel> selectedCategories, IEnumerable<BrandUIModel> selectedBrands)
        {
            try
            {
                //Based on Selected Categories && Selected Brands Get All Related Styles
                IEnumerable<string> enumberableFilterStyleIds = InitialProductDataSource
                   .Where(x =>
                   SelectedCategories.Any(y => y.CategoryId == x.CatId)
                   && SelectedBrands.Any(y => y.BrandId == x.BrandId)
                   && x.CatId != -99
                   ).Select(z => Convert.ToString(z.StyleId)).Distinct();

                var styleUIModelList = await ((App)Application.Current).QueryService
               .GetFilterStylesAsync(string.Join(',', enumberableFilterStyleIds));

                if (styleUIModelList != null && styleUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           () => StyleCollection = new ObservableCollection<StyleUIModel>(styleUIModelList)).AsTask().ConfigureAwait(false);
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredStyleListOnCategoryAndBrandClick), ex);
            }
        }

        private async Task GetFilteredBrandListOnCategoryAndStyleClick(IEnumerable<CategoryUIModel> selectedCategories, IEnumerable<StyleUIModel> selectedStyles)
        {
            try
            {
                //Based on Selected Categories & Styles Get All Related Brands
                IEnumerable<string> enumberableFilterBrandIds = InitialProductDataSource
                  .Where(x =>
                  SelectedCategories.Any(y => y.CategoryId == x.CatId)
                  && selectedStyles.Any(y => y.StyleId == x.StyleId)
                  ).Select(z => Convert.ToString(z.BrandId)).Distinct();


                var brandUIModelList = await ((App)Application.Current).QueryService
                .GetFilterBrandsAsync(string.Join(',', enumberableFilterBrandIds));

                if (brandUIModelList != null && brandUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => BrandCollection = new ObservableCollection<BrandUIModel>(brandUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredBrandListOnCategoryAndStyleClick), ex);
            }

        }

        private async Task GetFilteredCategoryListOnBrandAndStyleClick(IEnumerable<BrandUIModel> selectedBrands, IEnumerable<StyleUIModel> selectedStyles)
        {
            try
            {
                //Based on Brands Get All Related Categories               
                IEnumerable<string> enumberableFilterCatIds = InitialProductDataSource
                .Where(x =>
                selectedBrands.Any(y => y.BrandId == x.BrandId)
                && selectedStyles.Any(y => y.StyleId == x.StyleId)
                ).Select(z => Convert.ToString(z.CatId)).Distinct();

                var catUIModelList = await ((App)Application.Current).QueryService
                .GetFilterCategoriesAsync(string.Join(',', enumberableFilterCatIds));

                if (catUIModelList != null && catUIModelList.Any())
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                            () => CategoryCollection = new ObservableCollection<CategoryUIModel>(catUIModelList)).AsTask().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(GetFilteredCategoryListOnBrandAndStyleClick), ex);
            }
        }

        /// <summary>
        /// All, Open Stock and promotional filtering
        /// </summary>
        /// <param name="selectedIndex"></param>
        private async Task HandleHeaderComboboxChanged(int selectedIndex)
        {
            try
            {
                //0-All 1-Open stock 2-Promotional
                int index = selectedIndex;

                ProductGridDataSource.Clear();

                await Task.Run(() =>
                {
                    if (index == 0)
                    {
                        DbProductDataSource2?.ForEach(async (x) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                              () => ProductGridDataSource.Add(x)).AsTask().ConfigureAwait(false));
                    }
                    else if (index == 1)
                    {
                        DbProductDataSource2?.Where(x => x.ProductType == 0).ForEach(async (x) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                                () => ProductGridDataSource.Add(x)).AsTask().ConfigureAwait(false));
                    }
                    else if (index == 2)
                    {
                        DbProductDataSource2?.Where(x => x.ProductType == 1).ForEach(async (x) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                             () => ProductGridDataSource.Add(x)).AsTask().ConfigureAwait(false));
                    }
                }).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(HandleHeaderComboboxChanged), ex.StackTrace);
            }
        }

        private void OnItemClick(ItemClickEventArgs args)
        {
            _ = args?.ClickedItem as ListBoxItem;
        }

        private async void SuggestionChoosen(SRCProductUIModel selectedItem)
        {
            try
            {
                if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                {
                    return;
                }

                ProductGridDataSource.Clear();

                var _filterItem = DbProductDataSource2.FirstOrDefault(x => x.ItemNumber.Equals(selectedItem.ItemNumber));
                if (_filterItem != null)
                {
                    await ListIconClicked(selectedItem);
                }
                ProductGridDataSource.Add(_filterItem);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(SuggestionChoosen), ex.StackTrace);
            }
        }

        private async Task HandleTextChangeHeaderCommand(string text)
        {
            try
            {
                HeaderSearchItemSource.Clear();

                if (string.IsNullOrWhiteSpace(text))
                {
                    var ifDataGridHasAlreadyData = DbProductDataSource2?.Count() == ProductGridDataSource?.Count;

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
                    var tempList = DbProductDataSource2?.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower())).ToList();

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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(HandleTextChangeHeaderCommand), ex.StackTrace);
            }
        }

        private void LoadHeaderSearchWithInitialData()
        {
            DbProductDataSource2.ForEach(x => { HeaderSearchItemSource.Add(x); });
        }

        private async Task LoadDataGridAndHeaderSearchWithInitialData()
        {
            try
            {
                LoadingVisibilityHandler(isLoading: true);

                ProductGridDataSource.Clear();

                await Task.Run(() =>
                DbProductDataSource2?.ForEach(async (x) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                          () => ProductGridDataSource.Add(x)).AsTask().ConfigureAwait(false))
                );
                HeaderSearchItemSource.Clear();

                LoadingVisibilityHandler(isLoading: false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(LoadDataGridAndHeaderSearchWithInitialData), ex.StackTrace);
            }
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }
        private async Task NavigateToCartPageAsync()
        {
            try
            {
                if (AppRef.CartItemCount > 0 && AppRef.CartDataFromScreen != 0)
                {
                    bool result;
                    if (!string.IsNullOrWhiteSpace(CustomerNumber)
                        && CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                    {
                        result = await AlertHelper.Instance.ShowConfirmationAlert("", $"Empty Current cart and continue with the Sample Order?", "YES", "NO");
                    }
                    else
                    {
                        result = await AlertHelper.Instance.ShowConfirmationAlert("", $"Empty Current cart and continue with the {(IsDirectCustomer ? "Distributor" : "Retail Sale")} Order ?", "YES", "NO");
                    }
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);

                        if (isSuccess)
                        {
                            AppRef.CartItemCount = 0;
                            (Application.Current as App).OrderPrintName = "";
                            BadgeCount = AppRef.CartItemCount;

                            if (BadgeCount <= 0)
                            {
                                IsBadgeVisible = Visibility.Collapsed;
                                await AlertHelper.Instance.ShowConfirmationAlert("", "Please add SRC products in cart.", "OK");
                            }
                            else
                            {
                                IsBadgeVisible = Visibility.Visible;
                                BadgeText = BadgeCount.ToString();
                                NavigationService.NavigateShellFrame(typeof(CartPage));
                            }
                        }
                    }
                }
                else if (AppRef.CartItemCount == 0)
                {
                    // NavigationService.NavigateShellFrame(typeof(CartPage));
                    //ContentDialog pleaseAddPopDialog = new ContentDialog
                    //{
                    //    Content = resourceLoader.GetString("PleaseAddPop"),
                    //    CloseButtonText = resourceLoader.GetString("OK")
                    //};

                    //await pleaseAddPopDialog.ShowAsync();
                    await AlertHelper.Instance.ShowConfirmationAlert("", "Please add SRC products in cart.", "OK");
                }
                else NavigationService.NavigateShellFrame(typeof(CartPage));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(NavigateToCartPageAsync), ex);
            }
        }

        private async Task DistributionImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            try
            {
                if (!IsDirectCustomer)
                {
                    isDistributionCommandFired = isFromProductList;

                    LoadingVisibility = Visibility.Visible;

                    if (sRCProductUIModel.IsDistributed == 0)
                    {
                        if (await RecordManualDistributionData(sRCProductUIModel.ProductID, Convert.ToInt32(AppRef.SelectedCustomerId)))
                        {
                            sRCProductUIModel.IsDistributed = 1;
                            sRCProductUIModel.DistributionImage = "ms-appx:///Assets/SRCProduct/distribution_selected.png";
                            sRCProductUIModel.DistributionRecordedDate = string.Format("{0:MM-dd-yyyy}", sRCProductUIModel.DistributionDate == null ? DateTime.Now : sRCProductUIModel.DistributionDate);
                        }
                    }
                    else
                    {
                        if (await RemoveManualProductDistributionData(sRCProductUIModel.ProductID, Convert.ToInt32(AppRef.SelectedCustomerId)))
                        {
                            sRCProductUIModel.IsDistributed = 0;
                            sRCProductUIModel.DistributionImage = "ms-appx:///Assets/SRCProduct/distribution_normal.png";
                            sRCProductUIModel.DistributionRecordedDate = string.Empty;
                        }
                    }

                    LoadingVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                LoadingVisibility = Visibility.Collapsed;

                ErrorLogger.WriteToErrorLog(GetType().Name, "DistributionImageClicked", ex.StackTrace);
            }
        }

        public async Task<bool> RecordManualDistributionData(int ProductId, int SelectedCustomerId)
        {
            return await AppRef.QueryService.AddDistributionDate(ProductId, SelectedCustomerId);
        }

        public async Task<bool> RemoveManualProductDistributionData(int ProductId, int SelectedCustomerId)
        {
            return await AppRef.QueryService.RemoveDistributionDate(ProductId, SelectedCustomerId);
        }

        private async Task<IEnumerable<SRCProductUIModel>> GetProductOrderByDistributionDateAsync(IEnumerable<SRCProductUIModel> srcProducts)
        {
            try
            {
                srcProducts = await GetProductsWithDistributionDateAsync(srcProducts);
                srcProducts = srcProducts.OrderByDescending(x => x.DistributionDate);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetProductOrderByDistributionDateAsync", ex);
            }
            return srcProducts;
        }

        private async Task<IEnumerable<SRCProductUIModel>> GetProductsWithDistributionDateAsync(IEnumerable<SRCProductUIModel> srcProducts)
        {
            try
            {
                var productDistributions = await AppRef.QueryService.GetProductDistributionsDataForSelectedCustomer(Convert.ToInt32(AppRef.SelectedCustomerId), IsDirectCustomer);

                if (productDistributions != null && productDistributions.Count > 0)
                {
                    foreach (var item in srcProducts)
                    {
                        var prd = productDistributions.FirstOrDefault(x => x.ProductId == item.ProductID);
                        if (prd != null)
                        {
                            item.IsDistributed = 1;
                            item.DistributionRecordedDate = prd.DistributionDate;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetProductsWithDistributionDateAsync", ex);
            }
            return srcProducts;
        }


        #region Product Detail screen Methods

        /// <summary>
        /// Add to Cart click on product Detail screen
        /// </summary>
        /// <param name="srcProductUiModel"></param>
        private async Task ProductDetailAddToCartAsync()
        {
            try
            {
                isFromProductList = false;
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (SelectedProductDetail != null && ProductDetailModel != null)
                    {
                        if (string.IsNullOrWhiteSpace(ProductDetailModel.QuantityDisplay) || ProductDetailModel.QuantityDisplay.Equals("0"))
                        {
                            if ((bool)AppRef.IsCreditRequestOrder)
                            {
                                SelectedProductDetail.Quantity = -1;
                                ProductDetailModel.QuantityDisplay = "-1";
                            }
                            else
                            {
                                SelectedProductDetail.Quantity = 1;
                                ProductDetailModel.QuantityDisplay = "1";
                            }
                        }
                        else
                        {
                            if ((bool)AppRef.IsCreditRequestOrder)
                            {
                                SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay) * -1;
                            }
                            else
                            {
                                SelectedProductDetail.Quantity = Convert.ToInt32(ProductDetailModel.QuantityDisplay);
                            }
                            await CartImageClicked(SelectedProductDetail);
                        }
                    }
                }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ProductDetailAddToCartAsync", ex.StackTrace);
            }
        }

        /// <summary>
        /// List icon click on product Detail screen
        /// </summary>
        /// <param name="srcProductUiModel"></param>
        private async Task ListIconClicked(SRCProductUIModel selectedProduct)
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        IsProductGridLoad = true;
                        IsNoProductLoad = false;
                        IsProductDetailLoad = false;
                        isFromProductList = true;
                        IsSaleCategoryVisible = (SelectedCategories.Any(x => x.CategoryId == -99) ? Visibility.Visible : Visibility.Collapsed);
                        LoadingVisibility = Visibility.Collapsed;
                    }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ListIconClicked", ex);
            }
        }

        private async void ProductSelected(SRCProductUIModel selectedProduct)
        {
            try
            {
                if (selectedProduct != null)
                {
                    await GetSelectedProductValues(selectedProduct);
                }
                ShowProductDetailScreen();

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ProductSelected", ex.StackTrace);
            }
        }

        private async Task GetSelectedProductValues(SRCProductUIModel selectedProduct)
        {
            try
            {
                selectedProduct?.SrcProductToProductDetail();

                //var product = new List<ProductDetailUiModel>() { selectedProduct?.ProductDetailMasterData };

                //if (product?.Count > 0)
                //{
                //    ProductDetailModel = product[0];
                //}

                if (selectedProduct?.ProductDetailMasterData != null) ProductDetailModel = selectedProduct?.ProductDetailMasterData;

                await GetAdditionalDocuments(selectedProduct);

                SelectedProductDetail = ProductGridDataSource?.FirstOrDefault(X => X.ProductID == selectedProduct.ProductID);

                SelectedProductDetail.Quantity = await ((App)Application.Current).QueryService.GetCartProductQuantityAsync(AppRef.CurrentDeviceOrderId, selectedProduct.ProductID);

                if (SelectedProductDetail != null)
                {
                    //if (string.IsNullOrEmpty(ProductDetailModel.QuantityDisplay) || ProductDetailModel.QuantityDisplay.Equals("0"))
                    //not in cart
                    if (SelectedProductDetail.Quantity == 0)
                    {
                        if ((bool)AppRef.IsCreditRequestOrder)
                            SelectedProductDetail.Quantity = ProductDetailModel.Quantity = -1;
                        else
                            SelectedProductDetail.Quantity = ProductDetailModel.Quantity = 1;
                    }
                    else // availabe in cart
                    {
                        if ((bool)AppRef.IsCreditRequestOrder)
                        {
                            if (SelectedProductDetail.Quantity > 0)
                                SelectedProductDetail.Quantity = (Convert.ToInt32(SelectedProductDetail.Quantity) * -1);
                        }
                        ProductDetailModel.Quantity = SelectedProductDetail.Quantity;
                    }
                    ProductDetailModel.QuantityDisplay = SelectedProductDetail.Quantity.ToString();
                    ProductDetailModel.SelectedProductDetailIndex = ProductGridDataSource.IndexOf(SelectedProductDetail);
                }

                SetLeftRightArrowImages();
                SetAddToCartDetailImage(selectedProduct);
                SetFavoriteDetailImage(selectedProduct);

                SetProductMainImage();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetSelectedProductValues", ex.StackTrace);
            }
        }

        private void SetProductMainImage()
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SetProductMainImage", ex.StackTrace);
            }
        }

        private async Task GetAdditionalDocuments(SRCProductUIModel selectedProduct)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "GetAdditionalDocuments", ex.StackTrace);
            }
        }

        private void SetLeftRightArrowImages()
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SetLeftRightArrowImages", ex.StackTrace);
            }
        }

        private void SetAddToCartDetailImage(SRCProductUIModel selectedProduct)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SetAddToCartDetailImage", ex.StackTrace);
            }
        }
        private void SetFavoriteDetailImage(SRCProductUIModel selectedProduct)
        {
            try
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
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SetAddToCartDetailImage", ex.StackTrace);
            }
        }

        private void ShowProductDetailScreen()
        {
            IsProductGridLoad = false;
            IsNoProductLoad = false;
            IsProductDetailLoad = true;
            //ProductDetailVisibility = Visibility.Visible;
            //ProductListVisibility = Visibility.Collapsed;
            //NoProductVisibility = Visibility.Collapsed;
            LoadingVisibility = Visibility.Collapsed;
        }

        private async Task RightArrowClicked()
        {
            try
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
                            await GetSelectedProductValues(nextProduct);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "RightArrowClicked", ex.StackTrace);
            }
        }

        private async Task LeftArrowClicked()
        {
            try
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
                                await GetSelectedProductValues(previousProduct);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "LeftArrowClicked", ex.StackTrace);
            }
        }

        private async Task ShowFileDoesNotExist()
        {
            try
            {
                ContentDialog productCannotBeAddedDialog = new ContentDialog
                {
                    Content = resourceLoader.GetString("FileDoesNotExist"),
                    CloseButtonText = resourceLoader.GetString("OK")
                };

                await productCannotBeAddedDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ShowFileDoesNotExist", ex.StackTrace);
            }
        }
        private async void FavoriteDetailImageClicked(SRCProductUIModel sRCProductUIModel)
        {
            try
            {
                isFromProductList = false;
                if (SelectedProductDetail != null)
                {
                    await FavoriteImageClicked(SelectedProductDetail);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "FavoriteDetailImageClicked", ex.StackTrace);
            }
        }

        private async Task ProductImageClickedAsync()
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                    {
                        var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.ProductImage);

                        if (!string.IsNullOrWhiteSpace(filePath))
                        {
                            ProductDetailModel.ProductImagePath = filePath;
                            await PreviewDocumentCommandHandlerAsync(ProductDetailModel.ProductImagePath);
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
                }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "ProductImageClickedAsync", ex.StackTrace);
            }
        }

        private async Task RetailImageClickedAsync()
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                    {
                        var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.RetailImage);

                        if (!string.IsNullOrWhiteSpace(filePath))
                        {
                            ProductDetailModel.ProductImagePath = filePath;
                            await PreviewDocumentCommandHandlerAsync(ProductDetailModel.ProductImagePath);
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
                }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "RetailImageClickedAsync", ex.StackTrace);
            }
        }

        private async Task FactsheetClickedAsync()
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (AppRef.LocalFileService.IsSrcZipFolderExist(Core.Interface.SrcZipFileType.Product))
                    {
                        var filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel?.Factsheet);

                        if (!string.IsNullOrWhiteSpace(filePath))
                        {
                            ProductDetailModel.ProductImagePath = filePath;
                            await PreviewDocumentCommandHandlerAsync(ProductDetailModel.ProductImagePath);
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
                }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "FactsheetClickedAsync", ex.StackTrace);
            }
        }

        private async Task PreviewDocumentCommandHandlerAsync(string url)
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                          () =>
                          {
                              if (!string.IsNullOrEmpty(url))
                              {
                                  PreviewUrl = null;
                                  PreviewUrl = url;
                                  PreviewDocumentVisibility = true;
                              }
                              else
                              {
                                  if (PrintHelper != null)
                                  {
                                      PrintHelper.UnregisterForPrinting();
                                  }

                                  PreviewDocumentVisibility = false;
                              }
                          }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(PreviewDocumentCommandHandlerAsync), ex);
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
                //if (!isSalesDocClicked)
                //    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, ProductDetailModel.Factsheet);
                //else
                //    filePath = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.SalesDocs, ProductDetailModel.SalesDocs);

                if (IsSaleCategoryVisible == Visibility.Collapsed)
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
        private async Task NumPadButtonClickedAsync(string value)
        {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                UpdateQuantityFieldValue(ProductDetailModel, value);
            });
        }
        private void UpdateQuantityFieldValue(ProductDetailUiModel item, string value)
        {
            try
            {
                quantityBeforeEdit = item.QuantityDisplay;
                if (value.Equals("C"))
                {
                    if (item?.QuantityDisplay?.Length > 0)
                    {
                        item.QuantityDisplay = item?.QuantityDisplay?.Remove(item.QuantityDisplay.Length - 1);
                        quantityString = item.QuantityDisplay;
                    }
                }
                else
                {
                    item.QuantityDisplay = item?.QuantityDisplay ?? "";
                    if (value == "-" && quantityString.IndexOf('-') != -1)
                    {
                        return;
                    }

                    if (value == "-" && !item.QuantityDisplay.StartsWith('-'))
                    {
                        quantityString = "-" + quantityString;
                    }
                    else if (item.QuantityDisplay.Length != 0)
                    {
                        int number;
                        bool success = true;
                        if (int.TryParse(quantityString, out number))
                        {
                            success = Math.Abs(number) <= 999;
                        }
                        if (success)
                        {
                            quantityString += value;
                        }
                    }
                    else
                    {
                        quantityString = value;
                    }
                    item.QuantityDisplay = quantityString;
                }

                if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                {
                    if (item?.QuantityDisplay != "-")
                    {
                        item.Quantity = Convert.ToInt32(item.QuantityDisplay);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "UpdateQuantityFieldValue", ex.StackTrace);
            }
        }


        #endregion

        #endregion
    }
}
