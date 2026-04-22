using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class RackOrderCartPageViewModel : BaseModel
    {
        #region Properties
        private readonly App AppRef = (App)Application.Current;
        private readonly ResourceLoader resourceLoader;

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
        private RackOrderCartUiModel _rackOrderCartUiModel;
        public RackOrderCartUiModel RackOrderCartUIModel
        {
            get { return _rackOrderCartUiModel; }
            set { SetProperty(ref _rackOrderCartUiModel, value); }
        }
        private RackOrderCartUiModel _gridItemModel;
        public RackOrderCartUiModel GridItemModel
        {
            get { return _gridItemModel; }
            set { SetProperty(ref _gridItemModel, value); }
        }
        private ObservableCollection<RackOrderCartUiModel> _rackOrderListGridDataSource;
        public ObservableCollection<RackOrderCartUiModel> RackOrderListGridDataSource
        {
            get { return _rackOrderListGridDataSource; }
            set { SetProperty(ref _rackOrderListGridDataSource, value); }
        }
        private List<RackOrderCartUiModel> _dbRackOrderListDataSource;
        public List<RackOrderCartUiModel> DbRackOrderListDataSource
        {
            get { return _dbRackOrderListDataSource; }
            set { SetProperty(ref _dbRackOrderListDataSource, value); }
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
        private string _headerQuantityBeforeEdit = string.Empty;
        public string HeaderQuantityBeforeEdit
        {
            get { return _headerQuantityBeforeEdit; }
            set { SetProperty(ref _headerQuantityBeforeEdit, value); }
        }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
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
        public ICommand NumPadButtonClickCommand { get; private set; }
        public ICommand NumPadGridButtonClickCommand { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand HeaderQuantityChangedCommand { get; private set; }
        public ICommand CartImageCommand { private set; get; }
        public ICommand CartButtonCommand { private set; get; }

        #endregion

        #region Constructor

        public RackOrderCartPageViewModel()
        {
            InitializeCommands();
            RackOrderListGridDataSource = new ObservableCollection<RackOrderCartUiModel>();
            DbRackOrderListDataSource = new List<RackOrderCartUiModel>();
            RackOrderCartUIModel = new RackOrderCartUiModel();
            GridItemModel = new RackOrderCartUiModel();
            resourceLoader = ResourceLoader.GetForCurrentView();
            LoadingVisibility = Visibility.Visible;
        }

        private void InitializeCommands()
        {
            NumPadButtonClickCommand = new RelayCommand<string>(NumPadButtonClicked);
            NumPadGridButtonClickCommand = new RelayCommand<string>(NumPadGridButtonClicked);
            PageLoadedCommand = new AsyncRelayCommand<RackOrderUiModel>(LoadInitialPageData);
            QuantityChangedCommand = new AsyncRelayCommand<RackOrderCartUiModel>(QuantityChangedAsync);
            CartImageCommand = new AsyncRelayCommand<RackOrderCartUiModel>(CartImageClicked);
            CartButtonCommand = new AsyncRelayCommand(NavigateToRetailTransactionPage);
            HeaderQuantityChangedCommand = new AsyncRelayCommand(HeaderQuantityChangedAsync);
        }


        #endregion

        #region Private Methods
        private async Task LoadInitialPageData(RackOrderUiModel rackOrderUiModel)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            try
            {
                if (rackOrderUiModel != null)
                {
                    SelectedProductID = rackOrderUiModel.ProductID;
                    await FetchCartListData(rackOrderUiModel.BrandId);
                }
                BadgeCount = AppRef.CartItemCount;

                if (BadgeCount > 0)
                {
                    IsBadgeVisible = Visibility.Visible;
                    BadgeText = BadgeCount.ToString();
                }
                if (!string.IsNullOrEmpty(AppRef.SelectedCustomerId.Trim()))
                {

                    int customerId = Convert.ToInt32(AppRef.SelectedCustomerId);

                    var selectedCustomer = await AppRef.QueryService.GetSavedCustomerInformation(customerId);

                    if (selectedCustomer != null)
                    {
                        CustomerNameNumber = selectedCustomer.CustomerName + " " + (!string.IsNullOrWhiteSpace(selectedCustomer.CustomerNumber) ? selectedCustomer.CustomerNumber : "");
                        CustomerAddresss = selectedCustomer.PhysicalAddress;
                        CustomerCityState = selectedCustomer.SubAddressText;
                        CustomerTitlePanelVisibility = Visibility.Visible;
                    }
                }
                LoadingVisibility = Visibility.Collapsed;


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
        private async Task FetchCartListData(int brandId)
        {
            DbRackOrderListDataSource = await AppRef.QueryService.GetRackOrderCartData(brandId, AppRef.CurrentDeviceOrderId);

            if (DbRackOrderListDataSource?.Count > 0)
            {
                foreach (var item in DbRackOrderListDataSource)
                {
                    var details = await GetAdditionalDocuments(item.ProductID);
                    var productImage = AppRef.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Product, HelperMethods.GetNameFromURL(details?.ProductImage));
                    if (!string.IsNullOrWhiteSpace(productImage))
                    {
                        item.ProductImagePath = productImage;
                    }
                    if (item.ProductID != SelectedProductID)
                    {
                        item.IsAddedInGrid = true;
                    }
                    else
                    {
                        SetHeaderItemData(item);
                    }
                }
                DbRackOrderListDataSource.Where(x => x.IsAddedInGrid).ToList().ForEach(x => RackOrderListGridDataSource.Add(x));
            }
        }

        private void SetHeaderItemData(RackOrderCartUiModel item)
        {
            RackOrderCartUIModel.ProductID = item.ProductID;
            RackOrderCartUIModel.ProductName = item.ProductName;
            RackOrderCartUIModel.Description = item.Description;
            RackOrderCartUIModel.ProductImagePath = item.ProductImagePath;
            RackOrderCartUIModel.Quantity = item.Quantity;
            RackOrderCartUIModel.QuantityDisplay = item.QuantityDisplay;
            RackOrderCartUIModel.IsAddedToCart = item.IsAddedToCart;
            item.IsAddedInGrid = false;
        }

        private async Task<ProductDetailUiModel> GetAdditionalDocuments(int productID)
        {
            var productDetailUiModel = await AppRef.QueryService.GetProductAdditionalDocumentData(productID);
            return productDetailUiModel;
        }

        private void NumPadButtonClicked(string value)
        {
            UpdateQuantityFieldValue(RackOrderCartUIModel, value);
        }

        private void NumPadGridButtonClicked(string value)
        {
            UpdateGridQuantityFieldValue(RackOrderListGridDataSource, value);
        }

        private void UpdateQuantityFieldValue(RackOrderCartUiModel item, string value)
        {
            if (value == "C")
            {
                // quantityString = item.QuantityDisplay;
                if (item?.QuantityDisplay.Length > 0)
                {
                    item.QuantityDisplay = item?.QuantityDisplay.Remove(item.QuantityDisplay.Length - 1);
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
            //else { item.Quantity = 1; }
        }

        private void UpdateGridQuantityFieldValue(ObservableCollection<RackOrderCartUiModel> gridDataSource, string value)
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
                }
            }
        }

        private async Task QuantityChangedAsync(RackOrderCartUiModel rackOrderCartUiModel)
        {
            if (rackOrderCartUiModel != null && RackOrderListGridDataSource != null && RackOrderListGridDataSource.Count > 0)
            {
                foreach (var item in RackOrderListGridDataSource)
                {
                    if (item.ProductID != rackOrderCartUiModel.ProductID)
                    {
                        if (string.IsNullOrWhiteSpace(item.QuantityDisplay))
                        {
                            item.QuantityDisplay = item.Quantity.ToString();
                        }
                    }
                    else
                    {
                        item.Quantity = rackOrderCartUiModel.Quantity;
                        if (rackOrderCartUiModel?.IsAddedToCart == true && !string.IsNullOrEmpty(rackOrderCartUiModel?.QuantityDisplay))
                        {
                            rackOrderCartUiModel.OrderDetailUiToDataModel();

                            var _list = new List<OrderDetail>() { rackOrderCartUiModel.OrderDetailMasterData };

                            await ((App)Application.Current).QueryService.UpdateOrderDetail(_list[0], AppRef.CurrentDeviceOrderId.ToString());
                        }
                    }
                }
            }
        }

        private async Task HeaderQuantityChangedAsync()
        {
            if (RackOrderCartUIModel?.IsAddedToCart == true && !string.IsNullOrEmpty(RackOrderCartUIModel.QuantityDisplay))
            {
                RackOrderCartUIModel.Quantity = Convert.ToInt32(RackOrderCartUIModel.QuantityDisplay);
                RackOrderCartUIModel.OrderDetailUiToDataModel();

                var _list = new List<OrderDetail>() { RackOrderCartUIModel.OrderDetailMasterData };

                await ((App)Application.Current).QueryService.UpdateOrderDetail(_list[0], AppRef.CurrentDeviceOrderId);
            }
        }

        /// <summary>
        /// Cart image click event
        /// </summary>
        /// <param name="rackOrderCartUiModel"></param>
        private async Task CartImageClicked(RackOrderCartUiModel rackOrderCartUiModel)
        {
            try
            {
                if (string.IsNullOrEmpty(AppRef.SelectedCustomerId))
                {
                    await ShowNoCustomerSelected();
                }
                else
                {
                    await VerifyCartItems(rackOrderCartUiModel);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "CartImageClicked", ex.StackTrace);
            }
        }

        private async Task VerifyCartItems(RackOrderCartUiModel rackOrderCartUiModel)
        {
            if (AppRef.CartItemCount > 0)
            {
                if (AppRef.CartDataFromScreen == 1)
                {
                    await SelectDeselectCart(rackOrderCartUiModel);
                }
                else
                {
                    bool result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Rack Order?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            await SelectDeselectCart(rackOrderCartUiModel);
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
                await SelectDeselectCart(rackOrderCartUiModel);
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
        /// <param name="rackOrderCartUiModel"></param>
        private async Task SelectDeselectCart(RackOrderCartUiModel rackOrderCartUiModel)
        {
            if (!rackOrderCartUiModel.IsAddedToCart)
            {
                rackOrderCartUiModel.IsAddedToCart = true;
                BadgeCount++;
                AppRef.CartDataFromScreen = 1;
                rackOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_selected.png";

                if (AppRef.CurrentOrderId == 0)
                {
                    await InsertEntryIntoOrderMaster();
                }

                await InsertOrderDetailData(rackOrderCartUiModel);
            }
            else
            {
                rackOrderCartUiModel.IsAddedToCart = false;

                BadgeCount--;

                rackOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";

                await DeleteCartItemFromDb(rackOrderCartUiModel.ProductID, AppRef.CurrentDeviceOrderId);
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

            AppRef.CartItemCount = BadgeCount;
        }

        private async Task InsertEntryIntoOrderMaster()
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

        /// <summary>
        /// Insert Cart Item to database
        /// </summary>
        /// <param name="rackOrderCartUiModel"></param>
        /// <returns></returns>
        private async Task InsertOrderDetailData(RackOrderCartUiModel rackOrderCartUiModel)
        {
            rackOrderCartUiModel.OrderDetailUiToDataModel();
            var _list = new List<OrderDetail>() { rackOrderCartUiModel.OrderDetailMasterData };
            _list[0].Total = "0";
            _list[0].OrderId = AppRef.CurrentOrderId;
            _list[0].DeviceOrderID = AppRef.CurrentDeviceOrderId;
            _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);
            await AppRef.QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        /// <summary>
        /// Delete cart item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteCartItemFromDb(int productId, string deviceOrderId)
        {
            await AppRef.QueryService.DeleteOrderDetail(productId, deviceOrderId);
        }
        private async Task NavigateToRetailTransactionPage()
        {
            if (AppRef.CartItemCount <= 0)
            {
                await ShowPleaseAddRackPopUp();
            }
            else
            {
                if (AppRef.CartDataFromScreen == 1)
                {
                    NavigationService.NavigateShellFrame(typeof(RetailTransactionPage));
                }
                else
                {
                    bool result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Rack Order?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            await ShowPleaseAddRackPopUp();
                            AppRef.IsCarStockOrder = false;
                            AppRef.IsDistributionOptionClicked = false;
                            AppRef.IsCreditRequestOrder = false;
                            (Application.Current as App).OrderPrintName = "";
                        }
                    }
                }
            }

        }
        private async Task ShowPleaseAddRackPopUp()
        {

            ContentDialog pleaseAddRackDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("PleaseAddRack"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await pleaseAddRackDialog.ShowAsync();
        }
        #endregion
    }
}
