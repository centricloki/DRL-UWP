using DevExpress.Core.Extensions;
using DevExpress.Mvvm.Native;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.FedExAddressValidationModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Converters;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.Helpers;

using MsgKit.Enums;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.ViewModel
{
    public class CustomerDetailsPageViewModel : BaseModel
    {
        #region properties

        private string _customerid = "";

        private readonly App AppReference = (App)(Application.Current);

        private CustomerDetailScreenUIModel _uiModel;
        public CustomerDetailScreenUIModel UiModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }

        private bool _previewDocumentVisibility;
        public bool PreviewDocumentVisibility
        {
            get { return _previewDocumentVisibility; }
            set { SetProperty(ref _previewDocumentVisibility, value); }
        }


        private bool _addDocumentVisibility;
        public bool AddDocumentVisibility
        {
            get { return _addDocumentVisibility; }
            set { SetProperty(ref _addDocumentVisibility, value); }
        }


        private string _previewUrl;
        public string PreviewUrl
        {
            get { return _previewUrl; }
            set { SetProperty(ref _previewUrl, value); }
        }

        public event EventHandler<CustomerDetailFlyoutType> FlyoutEvent;
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }
        private bool _isCarStockOrderEnable;
        public bool IsCarStockEnable
        {
            get { return _isCarStockOrderEnable; }
            set { SetProperty(ref _isCarStockOrderEnable, value); }
        }
        private ObservableCollection<WeekdayUIModel> _weekdays;
        public ObservableCollection<WeekdayUIModel> Weekdays
        {
            get { return _weekdays; }
            set { SetProperty(ref _weekdays, value); }
        }


        private bool _previewChainPlaybookVisibility;
        public bool PreviewChainPlaybookVisibility
        {
            get { return _previewChainPlaybookVisibility; }
            set { SetProperty(ref _previewChainPlaybookVisibility, value); }
        }

        private string _previewUrlOfChainPlaybook;
        public string PreviewUrlOfChainPlaybook
        {
            get { return _previewUrlOfChainPlaybook; }
            set { SetProperty(ref _previewUrlOfChainPlaybook, value); }
        }
        private PhotosPrintHelper _printHelper;
        public PhotosPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }
        public CustomerDetailScreenUIModel screenUIModel { get; set; }
        #endregion

        #region command
        public IAsyncRelayCommand OnNavigatedToCommandAsync { get; private set; }
        public ICommand OnNavigatingFromCommand { get; private set; }
        public ICommand SelectDristributonTappedCommand { get; private set; }
        public ICommand EditSaveCommand { get; private set; }
        public ICommand DistributorCancelCommand { get; private set; }
        public ICommand DistributorDoneCommand { get; private set; }
        public ICommand CancelDistributorCommand { get; private set; }
        public ICommand PlaceAnOderButtonCommand { get; private set; }
        public ICommand CreditRequestButtonCommand { get; private set; }
        public ICommand AddDocumentCommand { get; private set; }
        public ICommand PreviewDocumentCommand { get; private set; }
        public ICommand OrderHistoryButtonCommand { get; private set; }
        public ICommand DistributorButtonCommand { get; private set; }
        public ICommand RackOrderButtonCommand { get; private set; }
        public ICommand ActivitiesCommand { get; private set; }
        public ICommand PopOrderButtonCommand { get; private set; }
        public ICommand CarStockOrderButtonCommand { get; private set; }
        public ICommand AssosicatedCustomerPopupCommand { get; private set; }
        public ICommand BtnClickChainPlaybookCommand { get; private set; }
        public ICommand BtnClosePreviewChainPlaybookCommand { get; private set; }
        public ICommand BtnPrintChainPlaybookCommand { get; private set; }
        #endregion

        #region constructor
        public CustomerDetailsPageViewModel()
        {
            UiModel = new CustomerDetailScreenUIModel();
            EditSaveCommand = new AsyncRelayCommand(EditSaveCommandHandler);
            OnNavigatingFromCommand = new RelayCommand(OnNavigatingFromCommandHandler);
            OnNavigatedToCommandAsync = new AsyncRelayCommand<int>(OnNavigatedToCommandHandlerAsync);
            SelectDristributonTappedCommand = new RelayCommand(SelectDristributonTappedCommandHandler);
            DistributorCancelCommand = new RelayCommand(DistributorCancelCommandHandler);
            DistributorDoneCommand = new RelayCommand(DistributorDoneCommandHandler);
            PlaceAnOderButtonCommand = new AsyncRelayCommand(PlaceAnOderButtonCommandHandler);
            CancelDistributorCommand = new RelayCommand(CancelDistributorCommandHandler);
            AddDocumentCommand = new AsyncRelayCommand(AddDocumentCommandHandler);
            PreviewDocumentCommand = new RelayCommand<string>(PreviewDocumentCommandHandler);
            OrderHistoryButtonCommand = new RelayCommand(OrderHistoryButtonCommandHandler);
            CreditRequestButtonCommand = new AsyncRelayCommand(CreditRequestButtonCommandHandler);

            DistributorButtonCommand = new AsyncRelayCommand(DistributorButtonCommandHandler);
            RackOrderButtonCommand = new AsyncRelayCommand(RackOrderButtonCommandHandler);
            PopOrderButtonCommand = new AsyncRelayCommand(PopOrderButtonCommandHandler);
            ActivitiesCommand = new RelayCommand(ActivitiesCommandHandler);
            CarStockOrderButtonCommand = new AsyncRelayCommand(CarStockOrderButtonCommandHandler);
            AddDocumentVisibility = false;
            PreviewDocumentVisibility = false;
            AssosicatedCustomerPopupCommand = new RelayCommand(() => UiModel.IsPopUpOpenOfAssociatedCustomer = !UiModel.IsPopUpOpenOfAssociatedCustomer);
            BtnClickChainPlaybookCommand = new AsyncRelayCommand(BtnClickChainPlaybookCommandHandler);
            BtnClosePreviewChainPlaybookCommand = new RelayCommand(BtnClosePreviewChainPlaybookCommandHandler);
            BtnPrintChainPlaybookCommand = new AsyncRelayCommand(BtnPrintChainPlaybookCommandHandlerAsync);
        }

        #endregion

        #region private methods
        private void ActivitiesCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(ActivitiesPage), UiModel.CustomerData);
        }

        private void PreviewDocumentCommandHandler(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                PreviewUrl = url;
                PreviewDocumentVisibility = true;
            }
            else
                PreviewDocumentVisibility = false;
        }

        private async Task AddDocumentCommandHandler()
        {
            if (UiModel.CustomerData != null)
            {
                UiModel.CustomerDocuments = await AppReference.QueryService.GetCustomerDocuments(UiModel.CustomerData.CustomerID.Value);
                string localPath;
                foreach (var item in UiModel.CustomerDocuments)
                {
                    localPath = AppReference.LocalFileService.GetLocalFilePathByFileTypeForCustomerDocument(item.DocUrl);
                    if (!string.IsNullOrWhiteSpace(localPath))
                    {
                        item.DocUrl = localPath;
                        item.DisplayDocUrl = GetTheImageThumblineAsPerDocType(localPath);
                    }
                }
            }
            AddDocumentVisibility = !AddDocumentVisibility;
        }

        private void CancelDistributorCommandHandler()
        {
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private async Task<bool> IsSelectedCustomerAccountTypeDiff()
        {
            bool result = false;
            string selectedCustomerId = null;
            if (!string.IsNullOrEmpty(AppReference.SelectedCustomerId))
                selectedCustomerId = AppReference.SelectedCustomerId;
            else selectedCustomerId = AppReference.PreviousSelectedCustomerId;

            if (UiModel?.CustomerData != null && !string.IsNullOrWhiteSpace(selectedCustomerId))
            {
                CustomerMaster cartCustomer = await ((App)Application.Current).QueryService.GetCartCustomerInfoAsync(Convert.ToInt32(selectedCustomerId));

                //previous is x-customer
                if (!string.IsNullOrWhiteSpace(cartCustomer.CustomerNumber)
                    && !string.IsNullOrWhiteSpace(UiModel.CustomerData.CustomerNumber)
                    && (cartCustomer.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)
                    && UiModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)))
                    result = false;
                else
                    result = UiModel?.CustomerData.AccountType != cartCustomer.AccountType;
            }
            return result;
        }

        private async Task<bool> IsSelectedCustomerTypeDiff()
        {
            bool result = false;
            string selectedCustomerId = null;
            if (!string.IsNullOrEmpty(AppReference.SelectedCustomerId))
                selectedCustomerId = AppReference.SelectedCustomerId;
            else selectedCustomerId = AppReference.PreviousSelectedCustomerId;

            if (UiModel?.CustomerData != null && !string.IsNullOrWhiteSpace(selectedCustomerId))
            {
                CustomerMaster cartCustomer = await ((App)Application.Current).QueryService.GetCartCustomerInfoAsync(Convert.ToInt32(selectedCustomerId));
                if (!string.IsNullOrWhiteSpace(cartCustomer.CustomerNumber)
                    && !string.IsNullOrWhiteSpace(UiModel.CustomerData.CustomerNumber))
                {
                    if (cartCustomer.CustomerNumber != UiModel.CustomerData.CustomerNumber)
                    {
                        //previous is in-direct retail x-customer to direct sample order x-number,
                        if (cartCustomer.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)
                            && UiModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)
                            )
                        {
                            if ((AppReference.IsDistributionOptionClicked ?? false)) result = true;
                            else result = false;
                        }
                        // previous direct x-customernumber to direct non x-customernumber
                        else if (cartCustomer.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)
                            && !UiModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase) && cartCustomer.AccountType == UiModel.CustomerData.AccountType)
                        {
                            result = true;
                        }
                        else if (!cartCustomer.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase)
                             && UiModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase) && cartCustomer.AccountType == UiModel.CustomerData.AccountType)
                        {
                            result = true;
                        }
                        else
                            result = UiModel?.CustomerData.AccountType != cartCustomer.AccountType;
                    }
                }
                else
                    result = UiModel?.CustomerData.AccountType != cartCustomer.AccountType;
            }
            return result;
        }

        private async Task CarStockOrderButtonCommandHandler()
        {
            if (AppReference.CurrentOrderId != 0 && (bool)AppReference.IsCreditRequestOrder)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != _customerid) (Application.Current as App).OrderPrintName = "";

                if (!(AppReference.IsCarStockOrder ?? false))
                {
                    string msg = "Empty Current cart and continue with the Car Stock Order?";
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", msg, "YES", "NO");

                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);

                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            AppReference.IsCarStockOrder = true;
                            AppReference.IsDistributionOptionClicked = false;
                            AppReference.SelectedCustomerId = _customerid;
                            AppReference.IsCreditRequestOrder = false;

                            NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                        }
                    }
                }
                else
                {
                    AppReference.IsCarStockOrder = true;
                    AppReference.IsDistributionOptionClicked = false;
                    AppReference.SelectedCustomerId = _customerid;
                    AppReference.IsCreditRequestOrder = false;
                    NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                }

            }
            else
            {
                AppReference.CartItemCount = 0;
                AppReference.IsCarStockOrder = true;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.SelectedCustomerId = _customerid;
                AppReference.IsCreditRequestOrder = false;

                NavigationService.NavigateShellFrame(typeof(SRCProductPage));
            }
        }

        private async Task PlaceAnOderButtonCommandHandler()
        {
            if (AppReference.CurrentOrderId != 0 && (bool)AppReference.IsCreditRequestOrder)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            if (AppReference.CartItemCount > 0)
            {

                if (AppReference.SelectedCustomerId != _customerid) (Application.Current as App).OrderPrintName = "";

                bool isDiff = AppReference.CartDataFromScreen != 0;
                isDiff |= isDiff ? isDiff : (AppReference.IsCreditRequestOrder ?? false);
                isDiff |= isDiff ? isDiff : (AppReference.IsCarStockOrder ?? false);
                isDiff |= isDiff ? isDiff : await IsSelectedCustomerTypeDiff();
                //check coming from Rack/Pop,credit  or same Place ORder click again
                if (isDiff)
                {
                    string msg = null;
                    if (!string.IsNullOrWhiteSpace(UiModel?.CustomerData.CustomerNumber)
                        && UiModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                        msg = "Empty Current cart and continue with the Sample Order?";
                    else
                    {
                        // for in-direct customer
                        if (!UiModel.IsDirectCustomer)
                            msg = "Empty Current cart and continue with the Retail Sale Order?";
                        else msg = "Empty Current cart and continue with Distributor Order?";
                    }
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", msg, "YES", "NO");

                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);

                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            AppReference.IsCarStockOrder = false;
                            AppReference.IsDistributionOptionClicked = false;
                            AppReference.SelectedCustomerId = _customerid;
                            AppReference.IsCreditRequestOrder = false;

                            NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                        }
                    }
                }
                else
                {
                    AppReference.SelectedCustomerId = _customerid;
                    NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                }

            }
            else
            {
                AppReference.CartItemCount = 0;
                AppReference.IsCarStockOrder = false;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.SelectedCustomerId = _customerid;
                AppReference.IsCreditRequestOrder = false;

                NavigationService.NavigateShellFrame(typeof(SRCProductPage));
            }
        }

        private async Task DistributorButtonCommandHandler()
        {
            if (AppReference.CurrentOrderId != 0 && (bool)AppReference.IsCreditRequestOrder)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != _customerid) (Application.Current as App).OrderPrintName = "";

                bool isDiff = AppReference.CartDataFromScreen != 0;
                isDiff |= isDiff ? isDiff : (AppReference.IsCreditRequestOrder ?? false);
                isDiff |= isDiff ? isDiff : (AppReference.IsCarStockOrder ?? false);
                isDiff |= isDiff ? isDiff : await IsSelectedCustomerAccountTypeDiff();
                //check coming from Rack/Pop,credit  or same distribution click again
                if (isDiff)
                {
                    string msg = null;
                    // for in-direct customer
                    if (UiModel?.CustomerData.AccountType != 1)
                        msg = "Empty Current cart and continue with the Retail Sale Order?";
                    else msg = "Empty Current cart and continue with Distributor Order?";
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", msg, "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;

                            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                            AppReference.IsDistributionOptionClicked = true;
                            AppReference.IsCarStockOrder = false;
                            AppReference.IsCreditRequestOrder = false;
                            (Application.Current as App).OrderPrintName = "";
                            NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                        }
                    }
                }
                else
                {
                    AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                    NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                }
            }
            else
            {
                AppReference.CartItemCount = 0;

                AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                AppReference.IsDistributionOptionClicked = true;
                AppReference.IsCreditRequestOrder = false;
                AppReference.IsCarStockOrder = false;
                NavigationService.NavigateShellFrame(typeof(SRCProductPage));
            }
        }

        private async Task CreditRequestButtonCommandHandler()
        {
            if (AppReference.CurrentOrderId != 0)
            {
                AppReference.IsOrderTypeChanged = true;
            }
            else
            {
                AppReference.IsOrderTypeChanged = false;
            }

            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != UiModel?.CustomerData?.CustomerID.ToString()) (Application.Current as App).OrderPrintName = "";

                //check coming from POP/Rack or same credit request click again
                if (AppReference.CartDataFromScreen != 0 || !(AppReference.IsCreditRequestOrder ?? false))
                {
                    var result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Credit Request?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            AppReference.IsCarStockOrder = false;
                            AppReference.IsDistributionOptionClicked = false;
                            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                            AppReference.IsCreditRequestOrder = true;
                            NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                        }
                    }
                }
                else
                {
                    AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                    NavigationService.NavigateShellFrame(typeof(SRCProductPage));
                }
            }
            else
            {
                AppReference.CartItemCount = 0;
                AppReference.IsCarStockOrder = false;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                AppReference.IsCreditRequestOrder = true;

                NavigationService.NavigateShellFrame(typeof(SRCProductPage));
            }
        }

        private async Task BtnClickChainPlaybookCommandHandler()
        {
            IsLoading = true;
            try
            {
                string filePath = AppReference.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.SalesDocs, UiModel.ChainPlayBookProduct.SalesDocs);
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    PreviewUrlOfChainPlaybook = filePath;
                    PreviewChainPlaybookVisibility = true;
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert(ResourceExtensions.GetLocalized("ALERT"), ResourceExtensions.GetLocalized("FileDoesNotExist"), ResourceExtensions.GetLocalized("OK"));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CustomerDetailsPageViewModel), nameof(BtnClickChainPlaybookCommandHandler), ex);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void BtnClosePreviewChainPlaybookCommandHandler()
        {
            if (PrintHelper != null)
            {
                PrintHelper.UnregisterForPrinting();
            }
            PreviewChainPlaybookVisibility = false;
        }

        private async Task BtnPrintChainPlaybookCommandHandlerAsync()
        {
            if (!string.IsNullOrEmpty(PreviewUrlOfChainPlaybook))
            {
                await PrintHelper.ShowPrintUIAsync();
            }
        }
        private async Task RackOrderButtonCommandHandler()
        {
            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != UiModel?.CustomerData?.CustomerID.ToString()) (Application.Current as App).OrderPrintName = "";

                //check coming from SRC or same rack click again
                if (AppReference.CartDataFromScreen != 1)
                {
                    bool result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Rack Order?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            AppReference.IsCarStockOrder = false;
                            AppReference.IsDistributionOptionClicked = false;
                            AppReference.IsCreditRequestOrder = false;
                            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                            (Application.Current as App).OrderPrintName = "";
                            NavigationService.NavigateShellFrame(typeof(RackOrderListPage));
                        }
                    }
                }
                else
                {
                    AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                    NavigationService.NavigateShellFrame(typeof(RackOrderListPage));
                }
            }
            else
            {
                AppReference.CartItemCount = 0;
                AppReference.IsCarStockOrder = false;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.IsCreditRequestOrder = false;
                AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();

                NavigationService.NavigateShellFrame(typeof(RackOrderListPage));
            }
        }
        private async Task PopOrderButtonCommandHandler()
        {
            if (AppReference.CartItemCount > 0)
            {
                if (AppReference.SelectedCustomerId != UiModel?.CustomerData?.CustomerID.ToString()) (Application.Current as App).OrderPrintName = "";

                //check coming from SRC or same pop click again
                if (AppReference.CartDataFromScreen != 2)
                {
                    bool result = await AlertHelper.Instance.ShowConfirmationAlert("", "Empty Current cart and continue with the Pop Order?", "YES", "NO");
                    if (result)
                    {
                        var isSuccess = await ((App)Application.Current).QueryService.DeleteAllCartItems(AppReference.CurrentDeviceOrderId);
                        if (isSuccess)
                        {
                            AppReference.CartItemCount = 0;
                            AppReference.IsCarStockOrder = false;
                            AppReference.IsDistributionOptionClicked = false;
                            AppReference.IsCreditRequestOrder = false;
                            AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                            (Application.Current as App).OrderPrintName = "";
                            NavigationService.NavigateShellFrame(typeof(PopOrderPage));
                        }
                    }
                }
                else
                {
                    AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();
                    NavigationService.NavigateShellFrame(typeof(PopOrderPage));
                }
            }
            else
            {
                AppReference.CartItemCount = 0;
                AppReference.IsCarStockOrder = false;
                AppReference.IsDistributionOptionClicked = false;
                AppReference.IsCreditRequestOrder = false;
                AppReference.SelectedCustomerId = UiModel?.CustomerData?.CustomerID.ToString();

                NavigationService.NavigateShellFrame(typeof(PopOrderPage));
            }
        }

        private async Task EditSaveCommandHandler()
        {
            ShellPage shellPage = null;
            try
            {
                CustomerDetailFlyoutType type = CustomerDetailFlyoutType.None;
                if (UiModel.IsInEditMode)
                {
                    shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
                    if (shellPage != null)
                    {
                        shellPage.ViewModel.IsSideMenuItemClickable = false;
                    }
                    IsLoading = true;
                    //indirect customer
                    if (UiModel.CustomerData.AccountType == 2)
                    {
                        bool isValid = false;
                        if (UiModel.SelectedAccountClassification?.AccountClassificationId == 38)
                        {
                            isValid = await ShowConfirmationAlert("Warning", "Customer marked as 'Out of Business' will disappear from the app.\r\nDo you want to continue?");
                            if (isValid)
                            {
                                if (UiModel.CustomerData.CustomerID.ToString().Equals(AppReference.SelectedCustomerId, StringComparison.OrdinalIgnoreCase))
                                {
                                    AppReference.SelectedCustomerId = string.Empty;
                                    await AppReference.QueryService.ClearCart(AppReference.CurrentDeviceOrderId);
                                    AppReference.CartItemCount = 0;
                                    AppReference.CurrentOrderId = 0;
                                    AppReference.CartDataFromScreen = 0;
                                    AppReference.CurrentDeviceOrderId = string.Empty;
                                    AppReference.IsCreditRequestOrder = false;
                                    AppReference.IsOrderTypeChanged = false;
                                    AppReference.IsDistributionOptionClicked = false;
                                    AppReference.OrderPrintName = "";
                                }
                            }
                            else
                            {
                                return;
                            }
                        }
                        //indirect customer
                        if (!isValid) isValid = ValidateMandatoryFields(out type);
                        if (isValid)
                        {
                            UiModel.IsInEditMode = !UiModel.IsInEditMode;
                            Weekdays.ForEach((i) => i.IsEnabled = false);

                            IList<Int16> selectedWeekDays = Weekdays.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                            if (selectedWeekDays.Any())
                            {
                                UiModel.CustomerData.OrderDeliveryWeekDays = string.Join(',', selectedWeekDays);
                            }
                            else UiModel.CustomerData.OrderDeliveryWeekDays = null;


                            UiModel.ShippingAddress = string.IsNullOrWhiteSpace(UiModel.ShippingAddress) ? "" : UiModel.ShippingAddress;
                            UiModel.ShippingCity = string.IsNullOrWhiteSpace(UiModel.ShippingCity) ? "" : UiModel.ShippingCity;
                            UiModel.SelectedShippingState = string.IsNullOrWhiteSpace(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState;
                            UiModel.ShippingZip = string.IsNullOrWhiteSpace(UiModel.ShippingZip) ? "" : UiModel.ShippingZip;
                            UiModel.MailingAddress = string.IsNullOrWhiteSpace(UiModel.MailingAddress) ? "" : UiModel.MailingAddress;
                            UiModel.MailingCity = string.IsNullOrWhiteSpace(UiModel.MailingCity) ? "" : UiModel.MailingCity;
                            UiModel.SelectedMailingState = string.IsNullOrWhiteSpace(UiModel.SelectedMailingState) ? "" : UiModel.SelectedMailingState;
                            UiModel.MailingZip = string.IsNullOrWhiteSpace(UiModel.MailingZip) ? "" : UiModel.MailingZip;


                            // Run three tasks in parallel
                            Task<FedExAddressContentDialog> fedExValidPhysicalAddress = null;
                            Task<FedExAddressContentDialog> fedExValidShippingAddress = null;
                            Task<FedExAddressContentDialog> fedExValidMailingAddress = null;

                            //validate with fedEx Address Service
                            if (!UiModel.PhysicalAddress.Equals(screenUIModel.PhysicalAddress, StringComparison.OrdinalIgnoreCase)
                                || !UiModel.PhysicalCity.Equals(screenUIModel.PhysicalCity, StringComparison.OrdinalIgnoreCase)
                                || !UiModel.SelectedPhysicalState.Equals(screenUIModel.SelectedPhysicalState, StringComparison.OrdinalIgnoreCase)
                                || !UiModel.PhysicalZip.Equals(screenUIModel.PhysicalZip, StringComparison.OrdinalIgnoreCase))
                            { fedExValidPhysicalAddress = FedExServiceResponseAsync("physical"); }

                            if (UiModel.IsShippingAddressEditable &&
                                    (!string.IsNullOrWhiteSpace(UiModel.ShippingAddress)
                                    || !string.IsNullOrWhiteSpace(UiModel.ShippingCity)
                                    || !string.IsNullOrWhiteSpace(UiModel.SelectedShippingState)
                                    || !string.IsNullOrWhiteSpace(UiModel.ShippingZip)))
                            {
                                if (!UiModel.ShippingAddress.Equals(screenUIModel.ShippingAddress, StringComparison.OrdinalIgnoreCase)
                                   || !UiModel.ShippingCity.Equals(screenUIModel.ShippingCity, StringComparison.OrdinalIgnoreCase)
                                   || (!string.IsNullOrWhiteSpace(UiModel.SelectedShippingState)
                                   && !UiModel.SelectedShippingState.Equals((string.IsNullOrWhiteSpace(screenUIModel.SelectedShippingState) ? "" : screenUIModel.SelectedShippingState), StringComparison.OrdinalIgnoreCase))
                                   || !UiModel.ShippingZip.Equals(screenUIModel.ShippingZip, StringComparison.OrdinalIgnoreCase))
                                { fedExValidShippingAddress = FedExServiceResponseAsync("shipping"); }
                            }


                            if (UiModel.IsMailingAddressEditable &&
                                    (!string.IsNullOrWhiteSpace(UiModel.MailingAddress)
                                    || !string.IsNullOrWhiteSpace(UiModel.MailingCity)
                                    || !string.IsNullOrWhiteSpace(UiModel.SelectedMailingState)
                                    || !string.IsNullOrWhiteSpace(UiModel.MailingZip)))
                            {
                                if ((!UiModel.MailingAddress.Equals(screenUIModel.MailingAddress, StringComparison.OrdinalIgnoreCase)
                                       || !UiModel.MailingCity.Equals(screenUIModel.MailingCity, StringComparison.OrdinalIgnoreCase)
                                       || (!string.IsNullOrWhiteSpace(UiModel.SelectedMailingState)
                                   && !UiModel.SelectedMailingState.Equals((string.IsNullOrWhiteSpace(screenUIModel.SelectedMailingState) ? "" : screenUIModel.SelectedMailingState), StringComparison.OrdinalIgnoreCase))
                                       || !UiModel.MailingZip.Equals(screenUIModel.MailingZip, StringComparison.OrdinalIgnoreCase)))
                                { fedExValidMailingAddress = FedExServiceResponseAsync("mailing"); }
                            }
                            // Filter out null tasks
                            var taskList = new List<Task<FedExAddressContentDialog>>(3) { fedExValidPhysicalAddress, fedExValidShippingAddress, fedExValidMailingAddress }
                            .Where(t => t != null).Cast<Task<FedExAddressContentDialog>>().ToList();

                            if (taskList.Any()) // Only run if there's at least one task
                            {
                                // Wait for all tasks to complete
                                FedExAddressContentDialog[] results = await Task.WhenAll(taskList);
                                foreach (var item in results)
                                {
                                    if (item != null)
                                    {
                                        if (item.IsResolved) await ShowFedExAddressPopupAsync(item);
                                        else
                                        {
                                            if (item.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
                                            {
                                                await ShowSucessMsg("Incorrect Physical Address", "It seems like you have entered an incorrect address. Please review.");
                                            }
                                            else if (item.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
                                            {
                                                await ShowSucessMsg("Incorrect Shipping Address", "It seems like you have entered an incorrect address. Please review.");
                                            }
                                            else if (item.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
                                            {
                                                await ShowSucessMsg("Incorrect Mailing Address", "It seems like you have entered an incorrect address. Please review.");
                                            }
                                        }
                                    }
                                }
                            }

                            await SaveIndirectCustomer();
                            //reset the inital value
                            screenUIModel = new CustomerDetailScreenUIModel
                            {
                                PhysicalAddress = UiModel.PhysicalAddress,
                                PhysicalCity = UiModel.PhysicalCity,
                                SelectedPhysicalState = UiModel.SelectedPhysicalState,
                                PhysicalZip = UiModel.PhysicalZip,
                                ShippingAddress = UiModel.ShippingAddress,
                                ShippingCity = UiModel.ShippingCity,
                                SelectedShippingState = string.IsNullOrWhiteSpace(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState,
                                ShippingZip = UiModel.ShippingZip,
                                MailingAddress = UiModel.MailingAddress,
                                MailingCity = UiModel.MailingCity,
                                SelectedMailingState = UiModel.SelectedMailingState,
                                MailingZip = UiModel.MailingZip,
                            };
                            CustomerPageUIModel editedObject = new CustomerPageUIModel()
                            {
                                AccountType = UiModel.CustomerData.AccountType,
                                Address = UiModel?.PhysicalAddress,
                                City = UiModel?.PhysicalCity,
                                CustomerId = UiModel.CustomerData.CustomerID.Value,
                                CustomerName = UiModel?.CustomerName,
                                CustomerNumber = UiModel?.CustomerData?.CustomerNumber,
                                DeviceCustomerId = UiModel?.CustomerData?.DeviceCustomerID,
                                IsEllipsisVisible = UiModel.CustomerData.VripOrTravel == 1,
                                LastCallDate = DateTimeHelper.ConvertStringDateToMM_DD_YYYY(UiModel?.CustomerData?.LastCallActivityDate),
                                Rank = UiModel?.SelectedRank,
                                State = UiModel?.SelectedPhysicalState,
                                StoreType = UiModel?.SelectedAccountClassification?.AccountClassificationName,
                                VripOrTravel = UiModel.CustomerData.VripOrTravel,
                            };
                            CustomersListPage.EditedCustomer = editedObject;

                            await ShowSucessMsg("Success", "Customer data is updated successfully");

                            if (UiModel.SelectedAccountClassification.AccountClassificationId == 38)
                            {
                                NavigationService.NavigateShellFrame(typeof(CustomersListPage));
                            }
                        }
                        else
                        {
                            FlyoutEvent?.Invoke(this, type);
                        }
                    }
                    else
                    {
                        UiModel.SetContactForUpdate();

                        bool isAllContactValids = ValidateContactsForDirectCustomer(out type);

                        if (isAllContactValids)
                        {
                            UiModel.IsInEditMode = !UiModel.IsInEditMode;
                            await SaveContact();
                            await ShowSucessMsg("Success", "Contact Saved Successfully");
                        }
                        else
                        {
                            FlyoutEvent?.Invoke(this, type);
                        }
                    }

                }
                else
                {
                    UiModel.IsInEditMode = !UiModel.IsInEditMode;
                    UiModel.SetMailingEditable();
                    UiModel.SetShippingEditable();
                    UiModel.HandleContactEditForDirectCustomer();
                    if (UiModel.CustomerData.AccountType == 2)
                    {
                        Weekdays.ForEach((i) => i.IsEnabled = true);
                    }
                }

                //Not display on Edit Mode
                if (UiModel.IsAssociatedCustomerAvailable)
                    UiModel.IsAssociatedCustomerShow = UiModel.IsAssociatedCustomerAvailable && !UiModel.IsInEditMode;
            }
            catch (Exception ex)
            {
                await ShowSucessMsg("Error", "Something went wrong, please try again after some time");

                ErrorLogger.WriteToErrorLog(nameof(CustomerDetailsPageViewModel), nameof(EditSaveCommandHandler), ex);
            }
            finally
            {
                IsLoading = false;
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private bool ValidateContactsForDirectCustomer(out CustomerDetailFlyoutType type)
        {
            type = CustomerDetailFlyoutType.None;
            if (UiModel.ContactListItemSource.Count > 0)
            {
                foreach (var contact in UiModel.ContactListItemSource)
                {
                    if (string.IsNullOrEmpty(contact.ContactEmail) && string.IsNullOrEmpty(contact.SelectedRank) && string.IsNullOrEmpty(contact.ContactName) && string.IsNullOrEmpty(contact.ContactPhone))
                    {
                        type = CustomerDetailFlyoutType.ContactAllMandatoryFeilds;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactName))
                    {
                        type = CustomerDetailFlyoutType.ContactName;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactEmail))
                    {
                        type = CustomerDetailFlyoutType.ContactEmail;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.ContactPhone))
                    {
                        type = CustomerDetailFlyoutType.ContactPhone;
                        return false;
                    }
                    else if (string.IsNullOrEmpty(contact.SelectedRank))
                    {
                        type = CustomerDetailFlyoutType.ContactRank;
                        return false;
                    }



                    var isPhoneValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(contact.ContactPhone, typeof(string), null, null));
                    if (!isPhoneValid.Success)
                    {
                        type = CustomerDetailFlyoutType.InvalidContactPhone;
                        return false;
                    }
                    //if (!string.IsNullOrWhiteSpace(contact.ContactFax))
                    //{
                    //    var isFaxValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(contact.ContactFax, typeof(string), null, null));
                    //    if (!isFaxValid.Success)
                    //    {
                    //        type = CustomerDetailFlyoutType.InvalidContactFax;
                    //        return false;
                    //    }
                    //}
                    if (!string.IsNullOrWhiteSpace(contact.ContactEmail))
                    {
                        if (!HelperMethods.IsValidEmail(contact.ContactEmail))
                        {
                            type = CustomerDetailFlyoutType.InValidContactEmail;
                            return false;
                        }
                    }

                }
                return true;
            }
            else
                return true;
        }

        private async Task<FedExAddressContentDialog> FedExServiceResponseAsync(string addressType)
        {
            FedExAddressContentDialog enteredAddress = null;
            FedExAddressContentDialog suggestedAddress = null;

            if (addressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.PhysicalAddress,
                    City = UiModel.PhysicalCity,
                    StateOrProvinceCode = UiModel.SelectedPhysicalState,
                    PostalCode = UiModel.PhysicalZip
                };
            }
            else if (addressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = string.IsNullOrEmpty(UiModel.ShippingAddress) ? "" : UiModel.ShippingAddress,
                    City = string.IsNullOrEmpty(UiModel.ShippingCity) ? "" : UiModel.ShippingCity,
                    StateOrProvinceCode = string.IsNullOrEmpty(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState,
                    PostalCode = string.IsNullOrEmpty(UiModel.ShippingZip) ? "0" : UiModel.ShippingZip,
                };
            }
            else if (addressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = string.IsNullOrEmpty(UiModel.MailingAddress) ? "" : UiModel.MailingAddress,
                    City = string.IsNullOrEmpty(UiModel.MailingCity) ? "" : UiModel.MailingCity,
                    StateOrProvinceCode = string.IsNullOrEmpty(UiModel.SelectedMailingState) ? "" : UiModel.SelectedMailingState,
                    PostalCode = string.IsNullOrEmpty(UiModel.MailingZip) ? "0" : UiModel.MailingZip,
                };
            }

            string requestBody = JsonConvert.SerializeObject(new FedExAddressResolveRequest
            {
                AddressesToValidate = new FedExAddressesToValidate[1]
                {
                       new FedExAddressesToValidate
                       {
                            Address = new FedExAddress
                                {
                                    StreetLines = new string[1] { enteredAddress.StreetLines },
                                    City=enteredAddress.City,
                                    PostalCode=Convert.ToInt64(enteredAddress.PostalCode),
                                    StateOrProvinceCode= enteredAddress.StateOrProvinceCode,
                                    CountryCode ="US",
                                }
                       }
                }
            });

            FedExValidatedAddressResponse fedExValidatedAddressResponse =
                await AppReference.QueryService.CallFedExAddressValidationAPIAsync(requestBody);

            if (fedExValidatedAddressResponse != null)
            {
                FedExResolvedAddress fedExAddressResolved = fedExValidatedAddressResponse.Output.ResolvedAddresses.FirstOrDefault();
                if (fedExAddressResolved != null && (fedExAddressResolved.Attributes.Resolved.HasValue && fedExAddressResolved.Attributes.Resolved.Value))
                {
                    suggestedAddress = new FedExAddressContentDialog
                    {
                        StreetLines = ((fedExAddressResolved?.StreetLinesToken.Count() > 1)
                        ? string.Join(',', fedExAddressResolved?.StreetLinesToken)
                        : fedExAddressResolved?.StreetLinesToken.FirstOrDefault()),
                        City = fedExAddressResolved.City,
                        StateOrProvinceCode = fedExAddressResolved.StateOrProvinceCode,
                        PostalCode = Convert.ToString(fedExAddressResolved.ParsedPostalCode.Base),
                        AddressType = addressType,
                        IsResolved = true
                    };
                }
                else
                {
                    suggestedAddress = enteredAddress;
                    suggestedAddress.AddressType = addressType;
                }
            }
            return suggestedAddress;
        }

        private async Task ShowFedExAddressPopupAsync(FedExAddressContentDialog suggestedAddress)
        {
            FedExAddressContentDialog enteredAddress = null;
            if (suggestedAddress.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.PhysicalAddress,
                    City = UiModel.PhysicalCity,
                    StateOrProvinceCode = UiModel.SelectedPhysicalState,
                    PostalCode = UiModel.PhysicalZip
                };
            }
            else if (suggestedAddress.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.ShippingAddress,
                    City = UiModel.ShippingCity,
                    StateOrProvinceCode = string.IsNullOrWhiteSpace(UiModel.SelectedShippingState) ? "" : UiModel.SelectedShippingState,
                    PostalCode = UiModel.ShippingZip
                };
            }
            else if (suggestedAddress.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
            {
                enteredAddress = new FedExAddressContentDialog
                {
                    StreetLines = UiModel.MailingAddress,
                    City = UiModel.MailingCity,
                    StateOrProvinceCode = string.IsNullOrWhiteSpace(UiModel.SelectedMailingState) ? "" : UiModel.SelectedMailingState,
                    PostalCode = UiModel.MailingZip
                };
            }

            if ((!enteredAddress.StreetLines.Equals(suggestedAddress.StreetLines, StringComparison.OrdinalIgnoreCase)
                               || !enteredAddress.City.Equals(suggestedAddress.City, StringComparison.OrdinalIgnoreCase)
                               || !enteredAddress.StateOrProvinceCode.Equals(suggestedAddress.StateOrProvinceCode, StringComparison.OrdinalIgnoreCase)
                               || !enteredAddress.PostalCode.Equals(suggestedAddress.PostalCode, StringComparison.OrdinalIgnoreCase)))
            {
                AddressContentDialog contentDialog = new AddressContentDialog(enteredAddress, suggestedAddress);
                switch (suggestedAddress.AddressType)
                {
                    case "physical":
                        contentDialog.Title = "Confirm Physical Address";
                        break;
                    case "shipping":
                        contentDialog.Title = "Confirm Shipping Address";
                        break;
                    case "mailing":
                        contentDialog.Title = "Confirm Mailing Address";
                        break;
                }

                var result = await contentDialog.ShowAsync();

                if (result == ContentDialogResult.Secondary)
                {
                    if (suggestedAddress.AddressType.Equals("physical", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.PhysicalAddress = suggestedAddress.StreetLines;
                        UiModel.PhysicalCity = suggestedAddress.City;
                        UiModel.SelectedPhysicalState = suggestedAddress.StateOrProvinceCode;
                        UiModel.PhysicalZip = suggestedAddress.PostalCode;
                    }
                    else if (suggestedAddress.AddressType.Equals("shipping", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.ShippingAddress = suggestedAddress.StreetLines;
                        UiModel.ShippingCity = suggestedAddress.City;
                        UiModel.SelectedShippingState = suggestedAddress.StateOrProvinceCode;
                        UiModel.ShippingZip = suggestedAddress.PostalCode;
                    }
                    else if (suggestedAddress.AddressType.Equals("mailing", StringComparison.OrdinalIgnoreCase))
                    {
                        UiModel.MailingAddress = suggestedAddress.StreetLines;
                        UiModel.MailingCity = suggestedAddress.City;
                        UiModel.SelectedMailingState = suggestedAddress.StateOrProvinceCode;
                        UiModel.MailingZip = suggestedAddress.PostalCode;
                    }
                }
            }
        }

        private bool ValidateMandatoryFields(out CustomerDetailFlyoutType type)
        {
            type = CustomerDetailFlyoutType.None;

            if (string.IsNullOrWhiteSpace(UiModel.CustomerName))
            {
                type = CustomerDetailFlyoutType.CustomerName;
                return false;
            }
            else if (UiModel.SelectedAccountClassification == null)
            {
                type = CustomerDetailFlyoutType.AccountClassification;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.SelectedRank))
            {
                type = CustomerDetailFlyoutType.Rank;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalAddress))
            {
                type = CustomerDetailFlyoutType.Address;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalCity))
            {
                type = CustomerDetailFlyoutType.City;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.SelectedPhysicalState))
            {
                type = CustomerDetailFlyoutType.State;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalZip))
            {
                type = CustomerDetailFlyoutType.Zip;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail))
            {
                type = CustomerDetailFlyoutType.ContactEmail;
                return false;
            }

            bool isValidEmail = false;

            if (!string.IsNullOrWhiteSpace(UiModel.PhysicalContactEmail))
            {
                isValidEmail = HelperMethods.IsValidEmail(UiModel.PhysicalContactEmail);
            }

            if (!isValidEmail)
            {
                type = CustomerDetailFlyoutType.InvalidEmail;
                return false;
            }


            var phoneCheck = UiModel.PhysicalContactPhone;
            if (!string.IsNullOrWhiteSpace(phoneCheck))
            {
                if (phoneCheck.Equals("(___)-___-____"))
                    UiModel.PhysicalContactPhone = "";
            }

            var isPhoneValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel.PhysicalContactPhone, typeof(string), null, null));

            if (!isPhoneValid.Success && !string.IsNullOrWhiteSpace(UiModel.PhysicalContactPhone))
            {
                type = CustomerDetailFlyoutType.InvalidPhone;
                return false;
            }

            //var faxCheck = UiModel.PhysicalContactFax;
            //if (!string.IsNullOrWhiteSpace(faxCheck))
            //{
            //    if (faxCheck.Equals("(___)-___-____"))
            //        UiModel.PhysicalContactFax = "";
            //}
            //var isFaxValid = Constants.Constants.PhoneNumbRegex.Match((string)new StringToPhoneNumberConverter().Convert(UiModel.PhysicalContactFax, typeof(string), null, null));
            //if (!isFaxValid.Success && !string.IsNullOrWhiteSpace(UiModel.PhysicalContactFax))
            //{
            //    type = CustomerDetailFlyoutType.InvalidFax;
            //    return false;
            //}

            return true;
        }

        private async Task ShowSucessMsg(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = ResourceExtensions.GetLocalized("OK"),
                SecondaryButtonText = string.Empty
            };

            await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
        }

        private async Task SaveContact()
        {
            List<ContactMaster> deletedRecords = UiModel.GetDeletedContactRecords();

            await AppReference.QueryService.InsertOrUpdateContactForCustomerProfilePage(deletedRecords: deletedRecords,
                updatedRecords: UiModel.ContactListItemSource, userName: AppReference.LoginUserNameProperty,
                pin: AppReference.LoginUserPinProperty, selectedCustomerId: UiModel.CustomerData.DeviceCustomerID);
        }

        private async Task SaveIndirectCustomer()
        {
            UiModel.SetUpdateCustomerObject();

            if (UiModel.CustomerData.CustomerID == null)
            {
                var randonCustomerId = HelperMethods.GenerateRandomNumberForGivenRange(10000, 99999);

                UiModel.CustomerData.CustomerID = randonCustomerId;
            }
            await AppReference.QueryService.UpdateOrInsertCustomerData(UiModel.CustomerData, AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);

            ICollection<DistributorMaster> deletedRecords = UiModel.GetDeletedDristributorRecords();

            if (UiModel.DistributorList != null)
            {
                await AppReference.QueryService.InsertOrUpdateDistributorForCustomerProfilePage(deletedRecords: deletedRecords,
                                                updadtedRecords: UiModel.DistributorList,
                                                userName: AppReference.LoginUserNameProperty,
                                                pin: AppReference.LoginUserPinProperty,
                                                selectedCustomerDeviceId: UiModel.CustomerData.DeviceCustomerID);
            }
        }

        private async Task OnNavigatedToCommandHandlerAsync(int customerId)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }
            _customerid = customerId.ToString();
            try
            {
                screenUIModel = await AppReference.QueryService.GetCustomerDetailsDataForViewAndEditAsync(customerId);
                if (!string.IsNullOrWhiteSpace(screenUIModel.CustomerData.CustomerNumber)
                    && screenUIModel.CustomerData.CustomerNumber.StartsWith("x", StringComparison.OrdinalIgnoreCase))
                    IsCarStockEnable = true;

                //Not display on Edit Mode
                if (screenUIModel.IsAssociatedCustomerAvailable)
                {
                    screenUIModel.IsAssociatedCustomerShow = screenUIModel.IsAssociatedCustomerAvailable && !screenUIModel.IsInEditMode;
                }
                if (screenUIModel.IsAssociatedCustomerShow)
                {
                    #region Get Head Quarter & Chain Location Logic
                    //Load Chain Location of Head Quarter when IsParentFlag is ready
                    if (screenUIModel.CustomerData.IsParent == 1)
                    {
                        screenUIModel.AssociatedCustomerLabelText = "Chain Locations";
                        List<CustomerPageUIModel> associatedCustomers = await AppReference.QueryService.GetChainLocationCustomers(customerId);
                        if (associatedCustomers != null && associatedCustomers.Count > 0)
                        {
                            var convertedList = associatedCustomers
                            .Select(x => new CustomerListControlUIModel(x))
                            .ToList();

                            screenUIModel.AssociatedCustomerList = new ObservableCollection<CustomerListControlUIModel>(convertedList);
                            screenUIModel.IsAnyAssociatedCustomerAvailable = true;
                        }
                    }
                    //Load Head Quarter
                    if (screenUIModel.CustomerData.IsParent == 0 && (screenUIModel.CustomerData?.Parent ?? -1) > 0)
                    {
                        screenUIModel.AssociatedCustomerLabelText = "Head Quarter";
                        List<CustomerPageUIModel> associatedCustomers = await AppReference.QueryService.GetHeadQuarterCustomers(screenUIModel.CustomerData.Parent.Value);
                        if (associatedCustomers != null && associatedCustomers.Count > 0)
                        {
                            var convertedList = associatedCustomers
                            .Select(x => new CustomerListControlUIModel(x))
                            .ToList();
                            screenUIModel.AssociatedCustomerList = new ObservableCollection<CustomerListControlUIModel>(convertedList);
                            screenUIModel.IsAnyAssociatedCustomerAvailable = true;
                        }
                    }
                    #endregion
                }

                if (!screenUIModel.IsDirectCustomer)
                {
                    Weekdays = new ObservableCollection<WeekdayUIModel>
                {
                    new WeekdayUIModel { Id=1,Name = "Monday" },
                    new WeekdayUIModel { Id=2,Name = "Tuesday" },
                    new WeekdayUIModel { Id=3,Name = "Wednesday"  },
                    new WeekdayUIModel { Id=4,Name = "Thursday"  },
                    new WeekdayUIModel { Id=5,Name = "Friday"   },
                    new WeekdayUIModel { Id=6,Name = "Saturday" },
                    new WeekdayUIModel { Id=7,Name = "Sunday" }
                };
                    if (!string.IsNullOrWhiteSpace(screenUIModel.CustomerData.OrderDeliveryWeekDays))
                    {
                        foreach (string dayId in screenUIModel.CustomerData.OrderDeliveryWeekDays.Split(','))
                        {
                            WeekdayUIModel anySelectedDay = Weekdays.FirstOrDefault(x => x.Id == Convert.ToInt16(dayId));
                            if (anySelectedDay != null) anySelectedDay.IsSelected = true;
                        }
                    }
                }
                UiModel = screenUIModel;

                // On initial load physical address
                screenUIModel = new CustomerDetailScreenUIModel
                {
                    PhysicalAddress = UiModel.PhysicalAddress,
                    PhysicalCity = UiModel.PhysicalCity,
                    SelectedPhysicalState = UiModel.SelectedPhysicalState,
                    PhysicalZip = UiModel.PhysicalZip,
                    ShippingAddress = UiModel.ShippingAddress ?? "",
                    ShippingCity = UiModel.ShippingCity ?? "",
                    SelectedShippingState = UiModel.SelectedShippingState ?? "",
                    ShippingZip = UiModel.ShippingZip ?? "",
                    MailingAddress = UiModel.MailingAddress ?? "",
                    MailingCity = UiModel.MailingCity ?? "",
                    SelectedMailingState = UiModel.SelectedMailingState ?? "",
                    MailingZip = UiModel.MailingZip ?? "",
                };
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CustomerDetailsPageViewModel), nameof(OnNavigatedToCommandHandlerAsync), ex);
            }
            finally
            {
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private void OnNavigatingFromCommandHandler()
        {
            throw new NotImplementedException();
        }

        private void SelectDristributonTappedCommandHandler()
        {
            if (UiModel.IsInEditMode)
            {
                FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.DistributorFlyout);
            }
        }

        private void DistributorCancelCommandHandler()
        {
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private void DistributorDoneCommandHandler()
        {
            UiModel.DistributorDoneCommandHandler();
            FlyoutEvent?.Invoke(this, CustomerDetailFlyoutType.Close);
        }

        private void OrderHistoryButtonCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(OrderHistoryPage), UiModel.CustomerData);
        }

        private string GetTheImageThumblineAsPerDocType(string path)
        {
            //".jpg", ".jpeg", ".png", ".bmp"
            var returnPath = "";
            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg") || path.ToLower().Contains(".png") || path.ToLower().Contains(".bmp"))
            {
                returnPath = path;
            }
            else if (path.ToLower().Contains(".pdf"))
            {
                returnPath = (string)Application.Current.Resources["DocumentIconImage"];
            }
            else
            {
                returnPath = (string)Application.Current.Resources["DocumentIcon"];
            }

            return returnPath;
        }
        private async Task<bool> ShowConfirmationAlert(string title, string msg)
        {
            ContentDialog userLogoutDialog = new ContentDialog
            {
                Title = title,
                Content = msg,
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No"
            };

            var result = await userLogoutDialog.ShowAsync();
            userLogoutDialog.Hide();
            if (result == ContentDialogResult.Primary)
            {
                return true;
            }
            else return false;

        }
        #endregion
    }
}