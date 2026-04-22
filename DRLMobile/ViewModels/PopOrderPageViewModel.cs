using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Helpers;
using DRLMobile.Services;
using DRLMobile.Views;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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

namespace DRLMobile.ViewModels
{
    public class PopOrderPageViewModel : ObservableObject
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
        #endregion

        #region Commands
        public ICommand PageLoadedCommand { private set; get; }
        public ICommand NumPadGridButtonClickCommand { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand CartImageCommand { private set; get; }
        public ICommand CartButtonCommand { private set; get; }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        #endregion

        #region Constructor

        public PopOrderPageViewModel()
        {
            InitializeCommands();
            GridItemModel = new PopOrderCartUiModel();
            resourceLoader = ResourceLoader.GetForCurrentView();
            LoadingVisibility = Visibility.Visible;
            DbPopOrderListDataSource = new List<PopOrderCartUiModel>();
            PopOrderListGridDataSource = new ObservableCollection<PopOrderCartUiModel>();
        }

        private void InitializeCommands()
        {
            NumPadGridButtonClickCommand = new RelayCommand<string>(NumPadGridButtonClicked);
            PageLoadedCommand = new AsyncRelayCommand(LoadInitialPageData);
            QuantityChangedCommand = new AsyncRelayCommand<PopOrderCartUiModel>(QuantityChangedAsync);
            CartImageCommand = new AsyncRelayCommand<PopOrderCartUiModel>(CartImageClicked);
            CartButtonCommand = new AsyncRelayCommand(NavigateToRetailTransactionPage);
        }

        #endregion

        #region Private Methods
        private async Task LoadInitialPageData()
        {
            try
            {
                await FetchCartListData();
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
                        CustomerNameNumber = selectedCustomer.CustomerName + " " + selectedCustomer.CustomerNumber;
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
        }
        private async Task FetchCartListData()
        {
            DbPopOrderListDataSource = new List<PopOrderCartUiModel>();
                //await AppRef.QueryService.GetPopOrderCartData(AppRef.CurrentOrderId);
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
                            //item.IsQuantityEdited = true;
                        }
                    }
                    else
                    {
                        if (item?.QuantityDisplay?.Length == 0)
                        {
                            if (value != "0" && value != "-")
                            {
                                item.QuantityDisplay = GridItemModel?.QuantityDisplay + value;
                            }
                            else
                            {
                                item.QuantityDisplay = string.Empty;
                                //item.IsQuantityEdited = true;
                            }
                        }
                        else if (item.QuantityDisplay.Length < 4 && (value != "-"))
                        {
                            item.QuantityDisplay = GridItemModel?.QuantityDisplay + value;
                        }
                    }
                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
                }
            }
        }

        private async Task QuantityChangedAsync(PopOrderCartUiModel arg)
        {
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
                    ShowNoCustomerSelected();
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
                if (AppRef.CartDataFromScreen == 1)
                {
                    SelectDeselectCart(popOrderCartUiModel);
                }
                else
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", ResourceExtensions.GetLocalized("EmptySrcOrPopCart"), "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentOrderId);
                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            SelectDeselectCart(popOrderCartUiModel);
                        }
                    }
                }
            }
            else
            {
                SelectDeselectCart(popOrderCartUiModel);
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

        /// <summary>
        /// Cart select deselect
        /// </summary>
        /// <param name="popOrderCartUiModel"></param>
        private async void SelectDeselectCart(PopOrderCartUiModel popOrderCartUiModel)
        {
            if (!popOrderCartUiModel.IsAddedToCart)
            {
                popOrderCartUiModel.IsAddedToCart = true;

                BadgeCount++;
                AppRef.CartDataFromScreen = 1;
                popOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_selected.png";

                if (AppRef.CurrentOrderId == 0)
                {
                    await InsertEntryIntoOrderMaster();
                }

                await InsertOrderDetailData(popOrderCartUiModel);
            }
            else
            {
                popOrderCartUiModel.IsAddedToCart = false;

                BadgeCount--;

                popOrderCartUiModel.CartImage = "ms-appx:///Assets/SRCProduct/cart_normal.png";

                await DeleteCartItemFromDb(popOrderCartUiModel.ProductID, AppRef.CurrentOrderId);
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
            }
        }

        /// <summary>
        /// Insert Cart Item to database
        /// </summary>
        /// <param name="popOrderCartUiModel"></param>
        /// <returns></returns>
        private async Task InsertOrderDetailData(PopOrderCartUiModel popOrderCartUiModel)
        {
            popOrderCartUiModel.OrderDetailUiToDataModel();
            var _list = new List<OrderDetail>() { popOrderCartUiModel.OrderDetailMasterData };
            _list[0].Total = "0";
            _list[0].OrderId = AppRef.CurrentOrderId;
            _list[0].DeviceOrderID = Convert.ToString(AppRef.CurrentOrderId);
            _list[0].CreatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            await AppRef.QueryService.InsertOrUpdateOrderDetail(_list[0]);
        }

        /// <summary>
        /// Delete cart item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteCartItemFromDb(int productId, int deviceOrderId)
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
                    NavigationService.Navigate<RetailTransactionPage>();
                }
                else
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", ResourceExtensions.GetLocalized("EmptySrcOrPopCart"), "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppRef.CurrentOrderId);
                        if (isSuccess)
                        {
                            DeleteCartItemsUiUpdate();
                            await ShowPleaseAddRackPopUp();
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
