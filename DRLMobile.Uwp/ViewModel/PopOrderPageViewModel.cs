using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class PopOrderPageViewModel : ObservableObject
    {
        #region Properties
        private readonly App AppRef = (App)Application.Current;
        private readonly ResourceLoader resourceLoader;
        private readonly Object thisLock = new Object();

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
        private Visibility _customerTitlePanelVisibility = Visibility.Collapsed;
        public Visibility CustomerTitlePanelVisibility
        {
            get { return _customerTitlePanelVisibility; }
            set { SetProperty(ref _customerTitlePanelVisibility, value); }
        }
        private PopOrderCartUiModel _gridItemModel;
        public PopOrderCartUiModel GridItemModel
        {
            get { return _gridItemModel; }
            set { SetProperty(ref _gridItemModel, value); }
        }
        private ObservableCollection<PopOrderCartUiModel> _popOrderListGridDataSource;
        public ObservableCollection<PopOrderCartUiModel> PopOrderListGridDataSource
        {
            get { return _popOrderListGridDataSource; }
            set { SetProperty(ref _popOrderListGridDataSource, value); }
        }

        private List<PopOrderCartUiModel> _dbPopOrderListDataSource;
        public List<PopOrderCartUiModel> DbPopOrderListDataSource
        {
            get { return _dbPopOrderListDataSource; }
            set { SetProperty(ref _dbPopOrderListDataSource, value); }
        }

        private int _selectedProductId;
        public int SelectedProductID
        {
            get { return _selectedProductId; }
            set { SetProperty(ref _selectedProductId, value); }
        }

        private int _badgeCount = 0;
        public int BadgeCount
        {
            get { return _badgeCount; }
            set { SetProperty(ref _badgeCount, value); }
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

        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
        }

        private PopOrderPageUiModel _popOrderPageModel;
        public PopOrderPageUiModel PopOrderPageModel
        {
            get { return _popOrderPageModel; }
            set { SetProperty(ref _popOrderPageModel, value); }
        }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        private int _hierarchyTag = 0;
        public int HierarchyTag
        {
            get { return _hierarchyTag; }
            set { SetProperty(ref _hierarchyTag, value); }
        }

        private int[] _hierarchyTagArray;
        public int[] HierarchyTagArray
        {
            get { return _hierarchyTagArray; }
            set { SetProperty(ref _hierarchyTagArray, value); }
        }

        private bool _isSelectionChangedCalled = false;
        public bool IsSelectionChangedCalled
        {
            get { return _isSelectionChangedCalled; }
            set { SetProperty(ref _isSelectionChangedCalled, value); }
        }

        private bool _isPreviewDocumentVisibile;
        public bool IsPreviewDocumentVisibile
        {
            get { return _isPreviewDocumentVisibile; }
            set { SetProperty(ref _isPreviewDocumentVisibile, value); }
        }

        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        private PhotosPrintHelper _printHelper;
        public PhotosPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }

        private string _quantityString = string.Empty;
        public string quantityString
        {
            get { return _quantityString; }
            set { SetProperty(ref _quantityString, value); }
        }

        #endregion

        #region Commands
        public ICommand PageLoadedCommand { private set; get; }
        public ICommand NumPadGridButtonClickCommand { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand CartImageCommand { private set; get; }
        public ICommand CartButtonCommand { private set; get; }
        public ICommand ComboBoxSelectionChangedCommand { private set; get; }
        public ICommand UpdateDataOnBackPressedCommand { private set; get; }
        public ICommand ClosePreviewCommand { get; private set; }
        public ICommand PrintPOPImageCommand { get; private set; }

        #endregion

        #region Constructor
        public PopOrderPageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();
            DbPopOrderListDataSource = new List<PopOrderCartUiModel>();
            PopOrderListGridDataSource = new ObservableCollection<PopOrderCartUiModel>();
            IsPreviewDocumentVisibile = false;

            InitializeCommands();
        }
        #endregion

        #region Private Methods
        private async Task LoadInitialPageData()
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            try
            {
                SetInitialData();
                await FetchCartListData();
                SetBadgeValue();
                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
                {
                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);
                    var selectedCustomer = await AppRef.QueryService.GetSavedCustomerInformation(customerId);
                    if (selectedCustomer != null)
                    {
                        CustomerNameNumber = selectedCustomer.CustomerName + " "
                            + (!string.IsNullOrWhiteSpace(selectedCustomer.CustomerNumber) ? selectedCustomer.CustomerNumber : "");
                        CustomerAddresss = selectedCustomer.PhysicalAddress;
                        CustomerCityState = selectedCustomer.SubAddressText;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                    }
                }
                LoadingVisibility = Visibility.Collapsed;
                await AddImagePopOrderListGridDataSource();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "LoadInitialPageData", ex.StackTrace);
                LoadingVisibility = Visibility.Collapsed;
            }

            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = true;
            }
        }

        private async Task AddImagePopOrderListGridDataSource()
        {
            try
            {
                if (PopOrderListGridDataSource?.Count > 0)
                {
                    foreach (var item in PopOrderListGridDataSource)
                    {
                        var details = await GetAdditionalDocuments(item.ProductID);
                        var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, HelperMethods.GetNameFromURL(details?.ProductImage));
                        if (!string.IsNullOrWhiteSpace(productImage))
                        {
                            item.ProductImagePath = productImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(PopOrderPageViewModel), nameof(AddImagePopOrderListGridDataSource), ex);
            }
        }

        private void InitializeCommands()
        {
            NumPadGridButtonClickCommand = new RelayCommand<string>(NumPadGridButtonClicked);
            PageLoadedCommand = new AsyncRelayCommand(LoadInitialPageData);
            QuantityChangedCommand = new AsyncRelayCommand<PopOrderCartUiModel>(QuantityChangedAsync);
            CartImageCommand = new AsyncRelayCommand<PopOrderCartUiModel>(CartImageClicked);
            CartButtonCommand = new AsyncRelayCommand(NavigateToRetailTransactionPage);
            ComboBoxSelectionChangedCommand = new AsyncRelayCommand<int>(ComboBoxSelectionChanged);
            UpdateDataOnBackPressedCommand = new AsyncRelayCommand(RefreshDataOnBackPressed);
            ClosePreviewCommand = new RelayCommand(ClosePreviewCommandHandler);
            PrintPOPImageCommand = new AsyncRelayCommand(PrintPOPImageCommandHandler);
        }

        private void ClosePreviewCommandHandler()
        {
            if (PrintHelper != null)
            {
                PrintHelper.UnregisterForPrinting();
                PrintHelper = null;
            }

            IsPreviewDocumentVisibile = false;
        }

        private async Task PrintPOPImageCommandHandler()
        {
            if (!string.IsNullOrEmpty(PreviewUrl))
            {

                //await PrintHelper.PreparePrintContent()
                await PrintHelper.ShowPrintUIAsync();
            }
        }

        private async Task ComboBoxSelectionChanged(int brandId)
        {
            try
            {
                if (!IsSelectionChangedCalled)
                {
                    LoadingVisibility = Visibility.Visible;
                    BrandUIModel category, material, family, brand, group;
                    GetSelectedComboboxItem(out category, out material, out family, out brand, out group);
                    if (HierarchyTag > 0)
                    {
                        HierarchyTagArray[HierarchyTag - 1] = brandId;
                    }

                    await FetchFilteredGridData();
                    await FetchFilteredComboboxData();
                    IsSelectionChangedCalled = true;
                    SetSelectedDataForCombobox(category, material, family, brand, group);
                    IsSelectionChangedCalled = false;
                    LoadingVisibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                IsSelectionChangedCalled = true;
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private void GetSelectedComboboxItem(out BrandUIModel category, out BrandUIModel material, out BrandUIModel family, out BrandUIModel brand, out BrandUIModel group)
        {
            category = PopOrderPageModel?.SelectedCategory;
            material = PopOrderPageModel?.SelectedMaterial;
            family = PopOrderPageModel?.SelectedFamily;
            brand = PopOrderPageModel?.SelectedBrand;
            group = PopOrderPageModel?.SelectedGroup;
        }

        private void SetSelectedDataForCombobox(BrandUIModel category, BrandUIModel material, BrandUIModel family, BrandUIModel brand, BrandUIModel group)
        {
            PopOrderPageModel.SelectedCategory = PopOrderPageModel?.CategoryList?.FirstOrDefault(x => x.BrandId == category?.BrandId);
            PopOrderPageModel.SelectedMaterial = PopOrderPageModel?.MaterialList?.FirstOrDefault(x => x.BrandId == material?.BrandId);
            PopOrderPageModel.SelectedFamily = PopOrderPageModel?.FamilyList?.FirstOrDefault(x => x.BrandId == family?.BrandId);
            PopOrderPageModel.SelectedBrand = PopOrderPageModel?.BrandList?.FirstOrDefault(x => x.BrandId == brand?.BrandId);
            PopOrderPageModel.SelectedGroup = PopOrderPageModel?.GroupList?.FirstOrDefault(x => x.BrandId == group?.BrandId);
        }

        private async Task FetchFilteredComboboxData()
        {
            if (HierarchyTag != 1)
            {
                PopOrderPageModel.CategoryList = await AppRef.QueryService.GetFilteredComboboxData(HierarchyTagArray, 1);
            }
            if (HierarchyTag != 2)
            {
                PopOrderPageModel.MaterialList = await AppRef.QueryService.GetFilteredComboboxData(HierarchyTagArray, 2);
            }
            if (HierarchyTag != 3)
            {
                PopOrderPageModel.FamilyList = await AppRef.QueryService.GetFilteredComboboxData(HierarchyTagArray, 3);
            }
            if (HierarchyTag != 4)
            {
                PopOrderPageModel.BrandList = await AppRef.QueryService.GetFilteredComboboxData(HierarchyTagArray, 4);
            }
            if (HierarchyTag != 5)
            {
                PopOrderPageModel.GroupList = await AppRef.QueryService.GetFilteredComboboxData(HierarchyTagArray, 5);
            }
        }

        private async Task FetchFilteredGridData()
        {
            PopOrderPageModel.PopOrderCartUiModelList = await AppRef.QueryService.GetFilteredPopOrderCartData(AppRef.CurrentDeviceOrderId, HierarchyTagArray, HierarchyTag);
            DbPopOrderListDataSource.Clear();
            PopOrderListGridDataSource.Clear();
            DbPopOrderListDataSource = PopOrderPageModel?.PopOrderCartUiModelList;
            if (DbPopOrderListDataSource?.Count > 0)
            {
                foreach (var item in DbPopOrderListDataSource)
                {
                    var details = await GetAdditionalDocuments(item.ProductID);
                    var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, HelperMethods.GetNameFromURL(details?.ProductImage));
                    if (!string.IsNullOrWhiteSpace(productImage))
                    {
                        item.ProductImagePath = productImage;
                    }
                }
                DbPopOrderListDataSource.ForEach(x => PopOrderListGridDataSource.Add(x));
            }
        }
        private async Task RefreshDataOnBackPressed()
        {
            await SetCartData();
            SetBadgeValue();
        }

        private async Task SetCartData()
        {
            try
            {
                PopOrderListGridDataSource.Clear();
                var cartProductIdList = await ((App)Application.Current).QueryService.GetCartsProductsIdQuantity(AppRef.CurrentDeviceOrderId);
                if (DbPopOrderListDataSource != null && DbPopOrderListDataSource.Count > 0)
                {
                    DbPopOrderListDataSource?.ForEach(item =>
                    {
                        int tempCart = 1;
                        var cartTemp = cartProductIdList?.TryGetValue(item.ProductID, out tempCart);
                        item.IsAddedToCart = cartTemp.HasValue && cartTemp.Value;
                        if (cartTemp == true)
                        {
                            item.Quantity = tempCart;
                            item.QuantityDisplay = tempCart.ToString();
                        }
                        else
                        {
                            item.Quantity = 1;
                            item.QuantityDisplay = "1";
                        }
                    });
                }
                DbPopOrderListDataSource.ForEach(x => PopOrderListGridDataSource.Add(x));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SetCartData", ex.StackTrace);
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private void SetBadgeValue()
        {
            IsBadgeVisible = Visibility.Collapsed;
            BadgeCount = AppRef.CartItemCount;
            if (BadgeCount > 0)
            {
                IsBadgeVisible = Visibility.Visible;
                BadgeText = BadgeCount.ToString();
            }
        }

        private void SetInitialData()
        {
            DbPopOrderListDataSource.Clear();
            PopOrderListGridDataSource.Clear();
            PopOrderPageModel = new PopOrderPageUiModel();
            GridItemModel = new PopOrderCartUiModel();
            LoadingVisibility = Visibility.Visible;
            PopOrderPageModel = new PopOrderPageUiModel();
            HierarchyTagArray = new int[5];
            IsPreviewDocumentVisibile = false;
        }

        private async Task FetchCartListData()
        {
            try
            {
                PopOrderPageModel = await AppRef.QueryService.GetPopOrderCartData(AppRef.CurrentDeviceOrderId);

                DbPopOrderListDataSource = PopOrderPageModel?.PopOrderCartUiModelList;

                if (DbPopOrderListDataSource?.Count > 0)
                {
                    DbPopOrderListDataSource.ForEach(x => PopOrderListGridDataSource.Add(x));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "FetchCartListData", ex.StackTrace);
                LoadingVisibility = Visibility.Collapsed;
            }
        }

        private async Task<ProductDetailUiModel> GetAdditionalDocuments(int productID)
        {
            var productDetailUiModel = await AppRef.QueryService.GetProductAdditionalDocumentData(productID);
            return productDetailUiModel;
        }
        private void NumPadGridButtonClicked(string value)
        {
            UpdateGridQuantityFieldValue(PopOrderListGridDataSource, value);
        }

        private void UpdateGridQuantityFieldValue(ObservableCollection<PopOrderCartUiModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == GridItemModel?.ProductID)
                {
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
                        if (value != "0" && value != "-")
                        {
                            if (quantityString.Length < 4)
                            {
                                quantityString += value;
                                item.QuantityDisplay = quantityString;
                                item.Quantity = Convert.ToInt32(quantityString);
                            }
                        }
                        else if (value.Equals("0") && quantityString.Length != 0 && value != "-")
                        {
                            if (quantityString.Length < 4)
                            {
                                quantityString += value;
                                item.QuantityDisplay = quantityString;
                                item.Quantity = Convert.ToInt32(quantityString);
                            }
                        }
                        else
                        {
                            quantityString = string.Empty;
                            item.QuantityDisplay = string.Empty;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
                    break;
                }
            }
        }

        private async Task QuantityChangedAsync(PopOrderCartUiModel popOrderCartUiModel)
        {
            foreach (var item in PopOrderListGridDataSource)
            {
                if (item.ProductID != popOrderCartUiModel?.ProductID)
                {
                    if (string.IsNullOrWhiteSpace(item.QuantityDisplay))
                    {
                        item.QuantityDisplay = item.Quantity.ToString();
                    }
                }
                else
                {
                    item.Quantity = popOrderCartUiModel.Quantity;
                    if (popOrderCartUiModel?.IsAddedToCart == true && !string.IsNullOrEmpty(popOrderCartUiModel?.QuantityDisplay))
                    {
                        popOrderCartUiModel.OrderDetailUiToDataModel();
                        var _list = new List<OrderDetail>() { popOrderCartUiModel.OrderDetailMasterData };
                        await ((App)Application.Current).QueryService.UpdateOrderDetail(_list[0], AppRef.CurrentDeviceOrderId);
                    }
                }
            }
        }

        /// <summary>
        /// Cart image click event
        /// </summary>
        /// <param name="popOrderCartUiModel"></param>
        private async Task CartImageClicked(PopOrderCartUiModel popOrderCartUiModel)
        {
            try
            {
                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    await ShowNoCustomerSelected();
                }
                else
                {
                    await VerifyCartItems(popOrderCartUiModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CartImageClicked", ex.StackTrace);
            }
        }

        private async Task VerifyCartItems(PopOrderCartUiModel popOrderCartUiModel)
        {
            if (AppRef.CartItemCount > 0)
            {
                if (AppRef.CartDataFromScreen == 2)
                {
                    await SelectDeselectCart(popOrderCartUiModel);
                }
                else
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Pop Order?", "YES", "NO");

                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);

                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            await SelectDeselectCart(popOrderCartUiModel);
                            AppRef.IsCarStockOrder = false;
                            AppRef.IsDistributionOptionClicked = false;
                            AppRef.IsCreditRequestOrder = false;
                            (Application.Current as App).OrderPrintName = "";
                        }
                    }
                }
            }
            else
            {
                if (AppRef.CartDataFromScreen == 0)
                {
                    AppRef.IsCarStockOrder = false;
                    AppRef.IsDistributionOptionClicked = false;
                    AppRef.IsCreditRequestOrder = false;
                    (Application.Current as App).OrderPrintName = "";
                }
                await SelectDeselectCart(popOrderCartUiModel);
            }
        }

        private void DeleteCartItemsUiUpdate()
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
        }

        private async Task ShowNoCustomerSelected()
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

        /// <summary>
        /// Cart select deselect
        /// </summary>
        /// <param name="popOrderCartUiModel"></param>
        private async Task SelectDeselectCart(PopOrderCartUiModel popOrderCartUiModel)
        {
            if (!popOrderCartUiModel.IsAddedToCart)
            {
                AppRef.CartDataFromScreen = 2;

                if (AppRef.CurrentOrderId == 0)
                {
                    await InsertEntryIntoOrderMaster();
                }

                if (await InsertOrderDetailData(popOrderCartUiModel) != null)
                {
                    BadgeCount++;
                    popOrderCartUiModel.IsAddedToCart = true;
                    popOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_selected.png";
                }
            }
            else
            {
                await DeleteCartItemFromDb(popOrderCartUiModel.ProductID, AppRef.CurrentDeviceOrderId);
                BadgeCount--;
                popOrderCartUiModel.IsAddedToCart = false;
                popOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";

            }


            if (BadgeCount <= 0)
            {
                IsBadgeVisible = Visibility.Collapsed;
            }
            else
            {
                BadgeText = BadgeCount.ToString();
                IsBadgeVisible = Visibility.Visible;
            }
            AppRef.CartItemCount = BadgeCount;
        }

        private async Task InsertEntryIntoOrderMaster()
        {
            try
            {
                var orderMasterRecord = new OrderMaster();

                orderMasterRecord.IsExported = 1;

                var newOrderAdded = await AppRef.QueryService.InsertOrUpdateOrderMaster(orderMasterRecord);

                if (newOrderAdded != null)
                {
                    AppRef.CurrentOrderId = newOrderAdded.OrderID;
                    AppRef.CurrentDeviceOrderId = newOrderAdded.DeviceOrderID;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(PopOrderPageViewModel), nameof(InsertEntryIntoOrderMaster), ex.StackTrace);
            }
        }

        /// <summary>
        /// Insert Cart Item to database
        /// </summary>
        /// <param name="popOrderCartUiModel"></param>
        /// <returns></returns>
        private async Task<OrderDetail> InsertOrderDetailData(PopOrderCartUiModel popOrderCartUiModel)
        {
            popOrderCartUiModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { popOrderCartUiModel.OrderDetailMasterData };

            _list[0].Total = "0.00";
            _list[0].OrderId = AppRef.CurrentOrderId;
            _list[0].DeviceOrderID = AppRef.CurrentDeviceOrderId;
            _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);

            //await Task.Delay(50);

            return await AppRef.QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        /// <summary>
        /// Delete cart item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task<bool> DeleteCartItemFromDb(int productId, string deviceOrderId)
        {
            return await AppRef.QueryService.DeleteOrderDetail(productId, deviceOrderId);
        }

        private async Task NavigateToRetailTransactionPage()
        {
            if (AppRef.CartItemCount <= 0)
            {
                await ShowPleaseAddPopDialog();
            }
            else
            {
                if (AppRef.CartDataFromScreen == 2)
                {
                    NavigationService.NavigateShellFrame(typeof(RetailTransactionPage));
                }
                else
                {
                    bool result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Pop Order?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            await ShowPleaseAddPopDialog();
                            AppRef.IsCarStockOrder = false;
                            AppRef.IsDistributionOptionClicked = false;
                            AppRef.IsCreditRequestOrder = false;
                            (Application.Current as App).OrderPrintName = "";
                        }
                    }
                }
            }

        }
        private async Task ShowPleaseAddPopDialog()
        {

            ContentDialog pleaseAddPopDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("PleaseAddPop"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await pleaseAddPopDialog.ShowAsync();
        }
        #endregion
    }
}
