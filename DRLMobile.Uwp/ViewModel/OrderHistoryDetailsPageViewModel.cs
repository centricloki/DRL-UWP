using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Core.Services;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;


namespace DRLMobile.Uwp.ViewModel
{
    public class OrderHistoryDetailsPageViewModel : BaseModel
    {

        private readonly Windows.UI.Core.CoreDispatcher coreDispatcher;
        private readonly App AppReference = (App)Application.Current;
        public string priceBeforeEdit = string.Empty;

        #region command
        public IAsyncRelayCommand PriceChangedCommand { get; private set; }
        public ICommand NumPadButtonClickCommand { get; private set; }
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand EditSaveCommand { get; private set; }
        public ICommand ClosePopupCommand { get; private set; }
        public ICommand AddButtonCommand { get; private set; }
        public ICommand AutoSuggestTextChanged { get; private set; }
        public ICommand AutoSuggestSuggestionChoosen { get; private set; }
        public IAsyncRelayCommand QuantityChangedCommand { get; private set; }
        public ICommand SaveDeleteCommand { get; private set; }
        public ICommand ItemClickCommand { get; private set; }
        public ICommand PrintEmailCommand { get; set; }
        #endregion

        #region properties
        public string IsOpenedFrom { get; set; }

        public string PriceString { get; set; } = string.Empty;

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private StandardOptionsPrintHelper _printHelper;
        public StandardOptionsPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }

        private bool _loadTobaccoGrid;
        public bool LoadTobaccoGrid
        {
            get { return _loadTobaccoGrid; }
            set { SetProperty(ref _loadTobaccoGrid, value); }
        }

        private bool _loadRackPosPopGrid;
        public bool LoadRackPosPopGrid
        {
            get { return _loadRackPosPopGrid; }
            set { SetProperty(ref _loadRackPosPopGrid, value); }
        }

        private bool _isAdd;
        public bool IsAdd
        {
            get { return _isAdd; }
            set { SetProperty(ref _isAdd, value); }
        }

        private bool _loadNonTobaccoGrid;
        public bool LoadNonTobaccoGrid
        {
            get { return _loadNonTobaccoGrid; }
            set { SetProperty(ref _loadNonTobaccoGrid, value); }
        }

        private bool _loadRtnGrid;
        public bool LoadRtnGrid
        {
            get { return _loadRtnGrid; }
            set { SetProperty(ref _loadRtnGrid, value); }
        }

        private bool _loadDifGrid;
        public bool LoadDifGrid
        {
            get { return _loadDifGrid; }
            set { SetProperty(ref _loadDifGrid, value); }
        }

        private bool _isEditmode;
        public bool IsEditmode
        {
            get { return _isEditmode; }
            set { SetProperty(ref _isEditmode, value); }
        }

        private bool _isAddEditPopupVisible;
        public bool IsAddEditPopupVisible
        {
            get { return _isAddEditPopupVisible; }
            set { SetProperty(ref _isAddEditPopupVisible, value); }
        }

        private bool _isEditIconVisible;
        public bool IsEditIconVisible
        {
            get { return _isEditIconVisible; }
            set { SetProperty(ref _isEditIconVisible, value); }
        }

        private string _popupTitle;
        public string PopupTitle
        {
            get { return _popupTitle; }
            set { SetProperty(ref _popupTitle, value); }
        }

        private string _totalTobacco;
        public string TotalTobacco
        {
            get { return _totalTobacco; }
            set { SetProperty(ref _totalTobacco, value); }
        }

        private string _totalNonTobacco;
        public string TotalNonTobacco
        {
            get { return _totalNonTobacco; }
            set { SetProperty(ref _totalNonTobacco, value); }
        }

        private string _totalRtnTobacco;
        public string TotalRtnTobacco
        {
            get { return _totalRtnTobacco; }
            set { SetProperty(ref _totalRtnTobacco, value); }
        }

        private string _totalDifTobacco;
        public string TotalDifTobacco
        {
            get { return _totalDifTobacco; }
            set { SetProperty(ref _totalDifTobacco, value); }
        }

        private string _totalGrand;
        public string GrandTotalValue
        {
            get { return _totalGrand; }
            set { SetProperty(ref _totalGrand, value); }
        }

        private AddEditProductOrderHistoryUIModel _addEditUIModel;
        public AddEditProductOrderHistoryUIModel AddEditUIModel
        {
            get { return _addEditUIModel; }
            set { SetProperty(ref _addEditUIModel, value); }
        }

        private OrderHistoryDetailsPageUIModel _uiModel;
        public OrderHistoryDetailsPageUIModel UiModel
        {
            get { return _uiModel; }
            set { SetProperty(ref _uiModel, value); }
        }

        private string _quantityString = string.Empty;
        public string quantityString
        {
            get { return _quantityString; }
            set { SetProperty(ref _quantityString, value); }
        }

        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
        }

        private ObservableCollection<string> _creditRequestComoBoxSource;
        public ObservableCollection<string> CreditRequestComoBoxSource
        {
            get { return _creditRequestComoBoxSource; }
            set { SetProperty(ref _creditRequestComoBoxSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _tobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> TobaccoGridItemSource
        {
            get { return _tobaccoGridItemSource; }
            set { SetProperty(ref _tobaccoGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _nonTobaccoGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> NonTobaccoGridItemSource
        {
            get { return _nonTobaccoGridItemSource; }
            set { SetProperty(ref _nonTobaccoGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _rtnGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> RtnGridItemSource
        {
            get { return _rtnGridItemSource; }
            set { SetProperty(ref _rtnGridItemSource, value); }
        }

        private ObservableCollection<OrderHistoryDetailsGridUIModel> _difGridItemSource;
        public ObservableCollection<OrderHistoryDetailsGridUIModel> DifGridItemSource
        {
            get { return _difGridItemSource; }
            set { SetProperty(ref _difGridItemSource, value); }
        }

        private bool _isNonTobaccoGridVisible;
        public bool IsNonTobaccoGridVisible
        {
            get { return _isNonTobaccoGridVisible; }
            set { SetProperty(ref _isNonTobaccoGridVisible, value); }
        }

        private bool _isTobaccoGridVisible;
        public bool IsTobaccoGridVisible
        {
            get { return _isTobaccoGridVisible; }
            set { SetProperty(ref _isTobaccoGridVisible, value); }
        }

        private bool _isRtnGridVisible;
        public bool IsRtnGridVisible
        {
            get { return _isRtnGridVisible; }
            set { SetProperty(ref _isRtnGridVisible, value); }
        }

        private bool _isDifGridVisible;
        public bool IsDifGridVisible
        {
            get { return _isDifGridVisible; }
            set { SetProperty(ref _isDifGridVisible, value); }
        }

        private bool _isGrandTotalVisible;
        public bool IsGrandTotalVisible
        {
            get { return _isGrandTotalVisible; }
            set { SetProperty(ref _isGrandTotalVisible, value); }
        }

        private ObservableCollection<OrderDetail> _rackPosPopOrderDetailsItemSource;
        public ObservableCollection<OrderDetail> RackPosPopOrderDetailsItemSource
        {
            get { return _rackPosPopOrderDetailsItemSource; }
            set { SetProperty(ref _rackPosPopOrderDetailsItemSource, value); }
        }

        private BitmapImage _signPath;
        public BitmapImage SignPath
        {
            get { return _signPath; }
            set { SetProperty(ref _signPath, value); }
        }

        private bool _isSignPanelVisible;
        public bool IsSignPanelVisible
        {
            get { return _isSignPanelVisible; }
            set { SetProperty(ref _isSignPanelVisible, value); }
        }

        #endregion

        #region Constructor

        public OrderHistoryDetailsPageViewModel()
        {
            coreDispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;

            LoadRackPosPopGrid = false;
            LoadTobaccoGrid = false;
            LoadNonTobaccoGrid = false;

            NumPadButtonClickCommand = new RelayCommand<string>(NumPadButtonClicCommandHandler);
            AutoSuggestTextChanged = new RelayCommand<string>(AutoSuggestTextChangedHandler);
            AutoSuggestSuggestionChoosen = new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(AutoSuggestSuggestionChoosenHandler);
            ClosePopupCommand = new RelayCommand(ClosePopupCommandHandler);
            PriceChangedCommand = new AsyncRelayCommand<string>(PriceChangedCommandHandler);
            EditSaveCommand = new AsyncRelayCommand(EditSaveCommandHandler);
            QuantityChangedCommand = new AsyncRelayCommand<string>(QuantityChangedCommandHandler);
            OnNavigatedToCommand = new AsyncRelayCommand<OrderHistoryDetailsPageUIModel>(OnNavigatedToCommandHandler);
            NonTobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            TobaccoGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            RtnGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            DifGridItemSource = new ObservableCollection<OrderHistoryDetailsGridUIModel>();
            RackPosPopOrderDetailsItemSource = new ObservableCollection<OrderDetail>();
            CreditRequestComoBoxSource = new ObservableCollection<string>();
            AddButtonCommand = new RelayCommand(AddButtonCommandHandler);
            SaveDeleteCommand = new AsyncRelayCommand<string>(SaveDeleteCommandHandler);
            ItemClickCommand = new RelayCommand<object>(ItemClickCommandHandler);
            PrintEmailCommand = new AsyncRelayCommand<string>(PrintEmailCommandHandler);

            IsNonTobaccoGridVisible = false;
            IsTobaccoGridVisible = false;
            IsRtnGridVisible = false;
            IsDifGridVisible = false;
            IsGrandTotalVisible = false;
            IsEditIconVisible = false;
            IsAddEditPopupVisible = false;
            TotalDifTobacco = string.Empty;
            TotalNonTobacco = string.Empty;
            TotalRtnTobacco = string.Empty;
            TotalTobacco = string.Empty;
            GrandTotalValue = string.Empty;
        }

        #endregion

        #region Private Methods

        private async Task PrintEmailCommandHandler(string type)
        {
            switch (type)
            {
                case "Email":
                    await BuildEmail();
                    break;
                case "Print":
                    await BuildPrintableReceipt();
                    break;
            }
        }

        private async Task BuildPrintableReceipt()
        {
            try
            {
                //ViewModel.PrintHelper.RegisterForPrinting();
                IsLoading = true;
                decimal total = 0;

                var user = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                var distributor = await AppReference.QueryService.GetDistributorFromId(UiModel.OrderMasterData.CustomerDistributorID);

                List<EmailAndPrintOrder.OrderDetails> listOfDetails = new List<EmailAndPrintOrder.OrderDetails>();

                if ("5".Equals(UiModel.OrderHistoryModel.SalesType) || "10".Equals(UiModel.OrderHistoryModel.SalesType))
                {
                    //RackPosPopOrderDetailsItemSource
                    foreach (var item in RackPosPopOrderDetailsItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item?.ProductDescription,
                            ProductName = item?.ProductName,
                            Quantity = item.Quantity,
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            IsTobaccoProduct = item?.isTobbaco.ToString(),
                        };

                        listOfDetails.Add(emailOrderDetails);
                    }
                }
                else if ("8".Equals(UiModel.OrderMasterData.SalesType))
                {
                    total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalRtnTobacco) ? TotalRtnTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalDifTobacco) ? TotalDifTobacco.Split('$').LastOrDefault() : "0");

                    foreach (var rtn in RtnGridItemSource)
                    {
                        if (!string.IsNullOrEmpty(rtn.OrderDetailObject.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(rtn.OrderDetailObject.Price);

                            rtn.OrderDetailObject.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = rtn.DisplayProductDesc,
                            UOM = rtn.DisplayProductUnit,
                            Quantity = rtn.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(rtn.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = rtn?.CreditRequestType,
                            ProductName = rtn?.DisplayProductName,
                            GrandTotal = total,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName))?$" - {distributor.AssignUserName}":"")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "1"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }

                    foreach (var dif in DifGridItemSource)
                    {
                        if (!string.IsNullOrEmpty(dif.OrderDetailObject.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(dif.OrderDetailObject.Price);

                            dif.OrderDetailObject.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = dif.DisplayProductDesc,
                            UOM = dif.DisplayProductUnit,
                            Quantity = dif.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(dif.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = dif?.CreditRequestType,
                            ProductName = dif?.DisplayProductName,
                            GrandTotal = total,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "0"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
                else
                {
                    total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalNonTobacco) ? TotalNonTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalTobacco) ? TotalTobacco.Split('$').LastOrDefault() : "0");

                    foreach (var item in NonTobaccoGridItemSource)
                    {
                        if (!string.IsNullOrEmpty(item.OrderDetailObject.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(item.OrderDetailObject.Price);

                            item.OrderDetailObject.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.DisplayProductDesc,
                            UOM = item.DisplayProductUnit,
                            Quantity = item.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(item.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = item?.CreditRequestType,
                            ProductName = item?.DisplayProductName,
                            GrandTotal = total,
                            RetailDistributorNumber= UiModel.OrderMasterData?.RetailDistributorNumber,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "0"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }

                    foreach (var item in TobaccoGridItemSource)
                    {
                        if (!string.IsNullOrEmpty(item.OrderDetailObject.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(item.OrderDetailObject.Price);

                            item.OrderDetailObject.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.DisplayProductDesc,
                            UOM = item.DisplayProductUnit,
                            Quantity = item.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(item.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = item?.CreditRequestType,
                            ProductName = item?.DisplayProductName,
                            GrandTotal = total,
                            RetailDistributorNumber = UiModel.OrderMasterData?.RetailDistributorNumber,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "1"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }

                EmailAndPrintOrder.CustomerInfo customerInfo = new EmailAndPrintOrder.CustomerInfo()
                {
                    SalesRepresentative = UiModel?.OrderMasterData?.OrderMasterSellerName,
                    Address = string.IsNullOrWhiteSpace(UiModel.PhysicalAddress) ? UiModel.SelectedCustomer?.PhysicalAddress : UiModel.PhysicalAddress,
                    City = string.IsNullOrWhiteSpace(UiModel.PhysicalCity) ? UiModel.SelectedCustomer?.PhysicalAddressCityID : UiModel.PhysicalCity,
                    ContactEmail = UiModel?.SelectedCustomer?.ContactEmail,
                    CustomerName = UiModel?.SelectedCustomer?.CustomerName,
                    CustomerNumber = UiModel?.SelectedCustomer?.CustomerNumber,
                    OrderDate = UiModel.OrderMasterData?.OrderDate,
                    PurchaseNumber = UiModel.OrderMasterData.InvoiceNumber,
                    PONumber = UiModel.OrderMasterData?.PurchaseOrderNumber,
                    PermitNumber = UiModel.OrderMasterData?.OrderMasterSellerRepTobacco,
                    RegionID = UiModel.OrderMasterData.RegionId.ToString(),
                    Zip = string.IsNullOrWhiteSpace(UiModel.PhysicalZip) ? UiModel.SelectedCustomer?.PhysicalAddressZipCode : UiModel.PhysicalZip,
                    ContactPhone = UiModel.SelectedCustomer?.ContactPhone,
                    State = string.IsNullOrWhiteSpace(UiModel.SelectedPhysicalState) ? HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, UiModel.SelectedCustomer.PhysicalAddressStateID) : UiModel.SelectedPhysicalState,
                    ShipDate = UiModel.OrderMasterData?.PrebookShipDate,
                    CustomStatement = UiModel.OrderMasterData?.CustomStatement,
                    CustomerComment = UiModel.OrderMasterData?.CustomerComment,
                    UserName = AppReference.LoginUserNameProperty,
                    SalesRepEmail = user?.EmailID,
                    SalesRepPhone = user?.ContactNo,
                    SalesRepUsername = user?.UserName,
                    PrintName = UiModel?.OrderMasterData?.PrintName

                };

                var fileName = !string.IsNullOrEmpty(UiModel.OrderMasterData.InvoiceNumber) ? UiModel.OrderMasterData.InvoiceNumber : "Invoice";
                var folder = ApplicationData.Current.LocalFolder;
                var path = Path.Combine(folder.Path, string.Format("{0}_Receipt.pdf", fileName));

                //pass this to print
                //EmailAndPrintOrder.BuildPrintOrder BuildPrintOrder = new EmailAndPrintOrder.BuildPrintOrder(path, listOfDetails, customerInfo, UiModel.OrderMasterData.SalesType);

                //BuildPrintOrder.GeneratePrintForm();

                // Initialize print content, by passing the pdf file as IStorage File  
                // Commented by Amol for printing changes
                // PrintHelper.PreparePrintContent(new ReceiptPrintPage(path));
                PrintHelper.PreparePrintContent(new PrintOrderXAMLPage(listOfDetails, customerInfo, UiModel.OrderMasterData.SalesType, SignPath));
                //await Task.Delay(10000);

                await PrintHelper.ShowPrintUIAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(BuildPrintableReceipt), ex.StackTrace);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task BuildEmail()
        {
            try
            {
                IsLoading = true;
                decimal total = 0;
                var user = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                var distributor = await AppReference.QueryService.GetDistributorFromId(UiModel.OrderMasterData.CustomerDistributorID);

                List<EmailAndPrintOrder.OrderDetails> listOfDetails = new List<EmailAndPrintOrder.OrderDetails>();

                if ("5".Equals(UiModel.OrderHistoryModel.SalesType) || "10".Equals(UiModel.OrderHistoryModel.SalesType))
                {
                    //RackPosPopOrderDetailsItemSource
                    foreach (var item in RackPosPopOrderDetailsItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item?.ProductDescription,
                            ProductName = item?.ProductName,
                            Quantity = item.Quantity,
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
                else if ("8".Equals(UiModel.OrderMasterData.SalesType))
                {
                    total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalRtnTobacco) ? TotalRtnTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalDifTobacco) ? TotalDifTobacco.Split('$').LastOrDefault() : "0");

                    foreach (var rtn in RtnGridItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = rtn.DisplayProductDesc,
                            UOM = rtn.DisplayProductUnit,
                            Quantity = rtn.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(rtn.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = rtn?.CreditRequestType,
                            ProductName = rtn?.DisplayProductName,
                            GrandTotal = total,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }

                    foreach (var dif in DifGridItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = dif.DisplayProductDesc,
                            UOM = dif.DisplayProductUnit,
                            Quantity = dif.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(dif.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = dif?.CreditRequestType,
                            ProductName = dif?.DisplayProductName,
                            GrandTotal = total,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0)

                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
                else
                {
                    total = Convert.ToDecimal(!string.IsNullOrEmpty(TotalNonTobacco) ? TotalNonTobacco.Split('$').LastOrDefault() : "0") + Convert.ToDecimal(!string.IsNullOrEmpty(TotalTobacco) ? TotalTobacco.Split('$').LastOrDefault() : "0");

                    foreach (var item in NonTobaccoGridItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.DisplayProductDesc,
                            UOM = item.DisplayProductUnit,
                            Quantity = item.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(item.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = item?.CreditRequestType,
                            ProductName = item?.DisplayProductName,
                            GrandTotal = total,
                            RetailDistributorNumber = UiModel.OrderMasterData?.RetailDistributorNumber,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "0"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }

                    foreach (var item in TobaccoGridItemSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.DisplayProductDesc,
                            UOM = item.DisplayProductUnit,
                            Quantity = item.OrderDetailObject.Quantity,
                            Price = Convert.ToDecimal(item.OrderDetailObject.Price),
                            RegionID = UiModel.OrderMasterData?.RegionId.ToString(),
                            SalesTaxCertificate = UiModel.OrderMasterData?.RetailerSalesTaxCertificate,
                            TobaccoLicense = UiModel.OrderMasterData?.StateTobaccoLicence,
                            RetailerLicense = UiModel.OrderMasterData?.RetailerLicense,
                            ReturnReason = item?.CreditRequestType,
                            ProductName = item?.DisplayProductName,
                            GrandTotal = total,
                            RetailDistributorNumber = UiModel.OrderMasterData?.RetailDistributorNumber,
                            DistributorCity = distributor != null ? distributor?.PhysicalAddressCityID : string.Empty,
                            DistributorID = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? $"{distributor.DistributorID}, {distributor.PhysicalAddressStateName}{((!string.IsNullOrWhiteSpace(distributor?.AssignUserName)) ? $" - {distributor.AssignUserName}" : "")}" : distributor.DistributorID) : string.Empty,
                            DistributorName = distributor != null ? (UiModel.OrderMasterData.SalesType == "2" ? string.Empty : distributor?.CustomerName) : string.Empty,
                            DistributorState = Core.Helpers.HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, distributor != null ? distributor.ShippingAddressStateID : 0),
                            IsTobaccoProduct = "1"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }

                EmailAndPrintOrder.CustomerInfo customerInfo = new EmailAndPrintOrder.CustomerInfo()
                {
                    SalesRepresentative = UiModel?.OrderMasterData?.OrderMasterSellerName,
                    Address = string.IsNullOrWhiteSpace(UiModel.PhysicalAddress) ? UiModel.SelectedCustomer?.PhysicalAddress : UiModel.PhysicalAddress,
                    City = string.IsNullOrWhiteSpace(UiModel.PhysicalCity) ? UiModel.SelectedCustomer?.PhysicalAddressCityID : UiModel.PhysicalCity,
                    ContactEmail = UiModel?.SelectedCustomer?.ContactEmail,
                    CustomerName = UiModel?.SelectedCustomer?.CustomerName,
                    CustomerNumber = UiModel?.SelectedCustomer?.CustomerNumber,
                    OrderDate = UiModel.OrderMasterData?.OrderDate,
                    PurchaseNumber = UiModel.OrderMasterData.InvoiceNumber,
                    PONumber = UiModel.OrderMasterData?.PurchaseOrderNumber,
                    PermitNumber = UiModel.OrderMasterData?.OrderMasterSellerRepTobacco,
                    RegionID = UiModel.OrderMasterData.RegionId.ToString(),
                    Zip = string.IsNullOrWhiteSpace(UiModel.PhysicalZip) ? UiModel.SelectedCustomer?.PhysicalAddressZipCode : UiModel.PhysicalZip,
                    ContactPhone = UiModel.SelectedCustomer?.ContactPhone,
                    State = string.IsNullOrWhiteSpace(UiModel.SelectedPhysicalState) ? HelperMethods.GetValueFromIdNameDictionary(UiModel.StateDictionary, UiModel.SelectedCustomer.PhysicalAddressStateID) : UiModel.SelectedPhysicalState,
                    ShipDate = UiModel.OrderMasterData?.PrebookShipDate,
                    CustomStatement = UiModel.OrderMasterData?.CustomStatement,
                    CustomerComment = UiModel.OrderMasterData?.CustomerComment,
                    UserName = AppReference.LoginUserNameProperty,
                    SalesRepEmail = user?.EmailID,
                    SalesRepPhone = user?.ContactNo,
                    SalesRepUsername = user?.UserName,
                    PrintName = UiModel?.OrderMasterData?.PrintName
                };

                var fileName = !string.IsNullOrEmpty(UiModel.OrderMasterData.InvoiceNumber) ? UiModel.OrderMasterData.InvoiceNumber : "Invoice";
                var folder = ApplicationData.Current.LocalFolder;
                var path = Path.Combine(folder.Path, string.Format("{0}.pdf", fileName));
                var htmlpath = Path.Combine(folder.Path, string.Format("{0}.html", fileName));

                EmailAndPrintOrder.BuildOrderEmail emailBuiler = new EmailAndPrintOrder.BuildOrderEmail();

                var templatesList = LoadEmailTemplate.LoadTemplate(UiModel.OrderMasterData.SalesType, customerInfo.State);

                var res = emailBuiler.BuildEmailBody(Body: listOfDetails, customerInfo: customerInfo, UiModel.OrderMasterData.SalesType, "", templatesList);

                StorageFile storMypicfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets//Email//Republic.png"));
                byte[] imageMypicArray = System.IO.File.ReadAllBytes(storMypicfile.Path);
                string base64ImageMypic = Convert.ToBase64String(imageMypicArray);

                Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var resultSignature = "";
                if (string.IsNullOrEmpty(UiModel.OrderMasterData.CustomerSignatureFileName) == false)
                {
                    if (UiModel.OrderMasterData.CustomerSignatureFileName.StartsWith("http"))
                    {
                        resultSignature = await InvokeWebService.DownloadDocumentAndFileFromServer(Core.Helpers.HelperMethods.GetNameFromURL(UiModel.OrderMasterData.CustomerSignatureFileName), "6");
                    }
                    else
                    {
                        resultSignature = UiModel.OrderMasterData.CustomerSignatureFileName;
                    }
                    if (string.IsNullOrEmpty(resultSignature) == false)
                    {
                        StorageFile storSignaturefile = await StorageFile.GetFileFromPathAsync(resultSignature);
                        byte[] imageSignatureArray = System.IO.File.ReadAllBytes(storSignaturefile.Path);
                        string base64ImageSignature = Convert.ToBase64String(imageSignatureArray);

                        res = res.Replace("cid:Signature", "data:image/png;base64," + base64ImageSignature);
                    }
                }

                res = res.Replace("cid:MyPic", "data:image/png;base64," + base64ImageMypic);

                StorageFile file = await storageFolder.CreateFileAsync(fileName + ".html", CreationCollisionOption.ReplaceExisting);
                await Windows.Storage.FileIO.WriteTextAsync(file, res);

                var subject = string.Format("Order#: {0} - {1} - {2} - {3}", UiModel?.OrderMasterData.InvoiceNumber, UiModel?.CustomerNumber,
                    UiModel?.CustomerName, UiModel?.SalesType);

                var emailBody = new EmailModel()
                {
                    Subject = subject,
                    BodyHtml = res,
                    AttachmentListByPath = new List<string>() { file.Path }
                    //AttachmentListByPath = isEmailGenerted ? new List<string>() { path } : null
                };

                var isEmailSuccessfull = await EmailService.Instance.SendMailFromOutlook(emailBody);

                var delay = isEmailSuccessfull ? 8000 : 200;

                await Task.Delay(delay);

                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(BuildEmail), ex.StackTrace);
            }
        }

        private void ItemClickCommandHandler(object obj)
        {
            try
            {
                if (obj is OrderHistoryDetailsGridUIModel && IsEditmode)
                {
                    var o = obj as OrderHistoryDetailsGridUIModel;
                    IsAdd = false;
                    IsAddEditPopupVisible = true;
                    PopupTitle = "Edit Product Quantity/Unit";

                    AddEditUIModel = new AddEditProductOrderHistoryUIModel();
                    AddEditUIModel.EditedOrderDetail = o.OrderDetailObject;
                    AddEditUIModel.Quantity = o.DisplayProductQty;
                    AddEditUIModel.PriceToSave = o.DisplayProductUnitPrice.Split('$').LastOrDefault();
                    AddEditUIModel.Price = o.DisplayProductUnitPrice;
                    AddEditUIModel.SelectedUom = o.DisplayProductUnit;
                    AddEditUIModel.SubTotal = o.DisplayProductSubtotal;
                    AddEditUIModel.IsCreditRequest = "8".Equals(UiModel?.OrderMasterData?.SalesType);

                    if (o.Product != null)
                    {
                        AddEditUIModel.SelectedProduct = UiModel.DbProducts.FirstOrDefault(x => x.ProductID == o.Product.ProductID);
                        AddEditUIModel.ProductName = AddEditUIModel.SelectedProduct?.ProductName;
                        AddEditUIModel.ProductDesc = AddEditUIModel.SelectedProduct?.Description;
                    }
                    if (o.Brand != null)
                    {
                        AddEditUIModel.SelectedBrand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == o.Brand.BrandId);
                        AddEditUIModel.BrandName = AddEditUIModel.SelectedBrand?.BrandName;
                    }
                    if (o.Style != null)
                    {
                        AddEditUIModel.SelectedStyle = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == o.Style.StyleId);
                        AddEditUIModel.StyleName = AddEditUIModel.SelectedStyle?.StyleName;
                    }
                    if (o.Category != null)
                    {
                        AddEditUIModel.SelectedCategory = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == o.Category.CategoryID);
                    }
                    if (AddEditUIModel.IsCreditRequest)
                    {
                        GetCreditRequestDropDownValues(o?.Product?.isTobbaco);
                        AddEditUIModel.SelectedCreditRequest = o.CreditRequestType;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "ItemClickCommandHandler", ex.StackTrace);

            }
        }

        private async Task SaveDeleteCommandHandler(string type)
        {
            try
            {
                if ("SAVE".Equals(type))
                {
                    if (IsAdd)
                    {
                        if (AddEditUIModel.SelectedProduct != null && !string.IsNullOrWhiteSpace(AddEditUIModel.ProductName))
                        {
                            if (!string.IsNullOrWhiteSpace(AddEditUIModel.Quantity))
                            {
                                if (string.IsNullOrWhiteSpace(AddEditUIModel.Price))
                                {
                                    AddEditUIModel.Price = string.Format("${0:0.00}", 0);
                                }
                                if (AddEditUIModel.IsCreditRequest)
                                {
                                    await AddProductToListForCreditRequest();
                                }
                                else
                                {
                                    await AddProductToList();
                                }
                            }
                            else
                            {
                                ContentDialog emptyFieldDialog = new ContentDialog
                                {
                                    Title = "Alert",
                                    Content = "Please enter quantity to add a product",
                                    CloseButtonText = "OK"
                                };

                                await emptyFieldDialog.ShowAsync();
                            }
                        }
                        else
                        {
                            ContentDialog emptyFieldDialog = new ContentDialog
                            {
                                Title = "Alert",
                                Content = "Please search and select a product",
                                CloseButtonText = "OK"
                            };

                            await emptyFieldDialog.ShowAsync();

                            if (string.IsNullOrEmpty(AddEditUIModel.Price))
                            {
                                AddEditUIModel.Price = string.Format("{0:0.00}", 0);
                            }
                        }
                    }
                    else
                    {
                        if (AddEditUIModel.IsCreditRequest)
                        {
                            await UpdateOrderDetailsForCreditRequest();
                        }
                        else
                        {
                            await UpdateOrderDetails();
                        }
                    }
                }
                else
                {

                    if (AddEditUIModel.IsCreditRequest)
                    {
                        await DeleteItemCreditRequest();
                    }
                    else
                    {
                        await DeleteItem();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "SaveDeleteCommandHandler", ex.StackTrace);
            }
        }

        private async Task UpdateOrderDetails()
        {
            try
            {
                var date = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                AddEditUIModel.EditedOrderDetail.UpdatedDate = date;
                AddEditUIModel.EditedOrderDetail.Quantity = Convert.ToInt32(AddEditUIModel.Quantity);
                AddEditUIModel.EditedOrderDetail.Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                AddEditUIModel.EditedOrderDetail.Price = AddEditUIModel.Price.Split('$').LastOrDefault();
                AddEditUIModel.EditedOrderDetail.Unit = AddEditUIModel.SelectedUom;

                if (AddEditUIModel.SelectedProduct?.isTobbaco == 1)
                {
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductSubtotal = AddEditUIModel.SubTotal;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductQty = AddEditUIModel.Quantity;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnitPrice = AddEditUIModel.Price;
                    TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnit = AddEditUIModel.SelectedUom;
                }
                else
                {
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductSubtotal = AddEditUIModel.SubTotal;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductQty = AddEditUIModel.Quantity;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnitPrice = AddEditUIModel.Price;
                    NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId).DisplayProductUnit = AddEditUIModel.SelectedUom;
                }

                decimal total = 0;
                await AppReference.QueryService.InsertOrUpdateOrderDetail(AddEditUIModel.EditedOrderDetail);

                RecalculateMainGridTotal(date, ref total);
                await AppReference.QueryService.UpdateOrderGrandTotal(total.ToString("0.00"), UiModel.OrderMasterData.OrderID);
                IsAddEditPopupVisible = false;

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(UpdateOrderDetails), ex.StackTrace);
            }
        }
        private async Task UpdateOrderDetailsForCreditRequest()
        {
            try
            {
                //if (!AddEditUIModel.EditedOrderDetail.CreditRequest.Equals(AddEditUIModel.SelectedCreditRequest))
                if (AddEditUIModel.SelectedProduct != null && !string.IsNullOrWhiteSpace(AddEditUIModel.ProductName))
                {
                    var date = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);
                    if (AddEditUIModel.PriceToSave == null || AddEditUIModel.PriceToSave == "0")
                    {
                        var pricetemp = AddEditUIModel.Price.Split('$').LastOrDefault();
                        AddEditUIModel.PriceToSave = pricetemp.Split('.').FirstOrDefault();
                    }

                    var orderDetail = new OrderDetail()
                    {
                        OrderDetailId = AddEditUIModel.EditedOrderDetail.OrderDetailId,
                        isTobbaco = AddEditUIModel.SelectedProduct.isTobbaco,
                        OrderId = UiModel.OrderMasterData.OrderID,
                        DeviceOrderID = UiModel.OrderMasterData.DeviceOrderID.ToString(),
                        Price = AddEditUIModel.PriceToSave,
                        ProductDescription = AddEditUIModel.ProductDesc,
                        Quantity = string.IsNullOrEmpty(AddEditUIModel.Quantity) ? -1 : Convert.ToInt32(AddEditUIModel.Quantity),
                        ProductId = AddEditUIModel.SelectedProduct.ProductID,
                        BrandId = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandId : 0,
                        CategoryId = AddEditUIModel.SelectedCategory.CategoryID,
                        CreatedDate = date,
                        UpdatedDate = date,
                        BrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        CategoryName = AddEditUIModel.SelectedCategory.CategoryName,
                        ProductName = AddEditUIModel.SelectedProduct.ProductName,
                        StyleId = AddEditUIModel.SelectedStyle.StyleId,
                        Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault(),
                        StyleName = AddEditUIModel.StyleName,
                        Unit = AddEditUIModel.SelectedUom,
                        CreditRequest = AddEditUIModel.SelectedCreditRequest,
                    };
                    orderDetail = await AppReference.QueryService.InsertOrUpdateOrderDetail(orderDetail);


                    if (!string.IsNullOrEmpty(AddEditUIModel.Price))
                    {
                        var tempPrice = AddEditUIModel.Price.Split('$').LastOrDefault();

                        var decimalPoint = Convert.ToDecimal(tempPrice);

                        AddEditUIModel.Price = string.Format("${0}", decimalPoint.ToString("0.00"));
                    }

                    OrderHistoryDetailsGridUIModel newObject = new OrderHistoryDetailsGridUIModel()
                    {
                        OrderDetailObject = AddEditUIModel.EditedOrderDetail,
                        Brand = AddEditUIModel.SelectedBrand,
                        Category = AddEditUIModel.SelectedCategory,
                        CreditRequestType = AddEditUIModel.SelectedCreditRequest,
                        DisplayBrandName = AddEditUIModel?.SelectedBrand?.BrandName,
                        DisplayProductDesc = AddEditUIModel?.ProductDesc,
                        DisplayProductName = AddEditUIModel?.ProductName,
                        DisplayProductQty = AddEditUIModel?.Quantity,
                        DisplayProductSubtotal = AddEditUIModel?.SubTotal,
                        DisplayProductUnit = AddEditUIModel?.SelectedUom,
                        DisplayProductUnitPrice = AddEditUIModel.Price,
                        DisplayStyleName = AddEditUIModel?.StyleName,
                        Product = AddEditUIModel?.SelectedProduct,
                        Style = AddEditUIModel?.SelectedStyle
                    };

                    var productTotal = Convert.ToDouble(AddEditUIModel.SubTotal.Split('$').LastOrDefault());
                    if (AddEditUIModel.SelectedCreditRequest.Contains("RTN"))
                    {
                        var replaceObj = RtnGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId.Value);

                        if (replaceObj != null)
                        {
                            var itmindex = RtnGridItemSource.IndexOf(replaceObj);
                            RtnGridItemSource.Remove(replaceObj);
                            //RtnGridItemSource.Add(newObject);
                            RtnGridItemSource.Insert(itmindex, newObject);
                            var replacetotal = Convert.ToDouble(replaceObj.DisplayProductSubtotal.Split('$').LastOrDefault());
                            var totalnonTobacco = string.IsNullOrEmpty(TotalRtnTobacco) ? 0 : Convert.ToDouble(TotalRtnTobacco.Split('$').LastOrDefault());
                            var total = totalnonTobacco - replacetotal + productTotal;
                            TotalRtnTobacco = string.Format("${0}", total.ToString("0.00"));
                        }
                        else
                        {
                            var replaceObj2 = DifGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            if (replaceObj2 != null)
                            {
                                DifGridItemSource.Remove(replaceObj2);
                                RtnGridItemSource.Add(newObject);
                                var replacetotal = Convert.ToDouble(replaceObj2.DisplayProductSubtotal.Split('$').LastOrDefault());
                                var totalTobacco = string.IsNullOrEmpty(TotalDifTobacco) ? 0 : Convert.ToDouble(TotalDifTobacco.Split('$').LastOrDefault());
                                var total = totalTobacco - replacetotal;
                                TotalDifTobacco = string.Format("${0}", total.ToString("0.00"));

                                var totalnonTobacco = string.IsNullOrEmpty(TotalRtnTobacco) ? 0 : Convert.ToDouble(TotalRtnTobacco.Split('$').LastOrDefault());
                                var total2 = totalnonTobacco + productTotal;
                                TotalRtnTobacco = string.Format("${0}", total2.ToString("0.00"));
                            }
                        }
                    }
                    else
                    {
                        var replaceObj = RtnGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId.Value);
                        if (replaceObj != null)
                        {
                            RtnGridItemSource.Remove(replaceObj);
                            DifGridItemSource.Add(newObject);

                            var replacetotal = Convert.ToDouble(replaceObj.DisplayProductSubtotal.Split('$').LastOrDefault());
                            var totalnonTobacco = string.IsNullOrEmpty(TotalRtnTobacco) ? 0 : Convert.ToDouble(TotalRtnTobacco.Split('$').LastOrDefault());
                            var total = totalnonTobacco - replacetotal;
                            TotalRtnTobacco = string.Format("${0}", total.ToString("0.00"));

                            var totalTobacco = string.IsNullOrEmpty(TotalDifTobacco) ? 0 : Convert.ToDouble(TotalDifTobacco.Split('$').LastOrDefault());
                            var total2 = totalTobacco + productTotal;
                            TotalDifTobacco = string.Format("${0}", total2.ToString("0.00"));
                        }
                        else
                        {
                            var replaceObj2 = DifGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId.Value == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            if (replaceObj2 != null)
                            {
                                var itmindex = DifGridItemSource.IndexOf(replaceObj2);
                                DifGridItemSource.Remove(replaceObj2);
                                DifGridItemSource.Insert(itmindex, newObject);
                                //DifGridItemSource.Add(newObject);

                                var replacetotal = Convert.ToDouble(replaceObj2.DisplayProductSubtotal.Split('$').LastOrDefault());
                                var totalTobacco = string.IsNullOrEmpty(TotalDifTobacco) ? 0 : Convert.ToDouble(TotalDifTobacco.Split('$').LastOrDefault());
                                var total2 = totalTobacco - replacetotal + productTotal;
                                TotalDifTobacco = string.Format("${0}", total2.ToString("0.00"));
                            }
                        }
                    }

                    decimal grandTotal = 0;
                    RecalculateMainGridTotal(date, ref grandTotal);
                    await AppReference.QueryService.UpdateOrderGrandTotal(grandTotal.ToString("0.00"), UiModel.OrderMasterData.OrderID);

                }

                if (RtnGridItemSource.Any())
                {
                    IsRtnGridVisible = true;
                    LoadRtnGrid = true;
                }
                else
                {
                    IsRtnGridVisible = false;
                    LoadRtnGrid = false;
                }

                if (DifGridItemSource.Any())
                {
                    IsDifGridVisible = true;
                    LoadDifGrid = true;
                }
                else
                {
                    IsDifGridVisible = false;
                    LoadDifGrid = false;
                }

                IsAddEditPopupVisible = false;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "UpdateOrderDetailsForCreditRequest", ex.StackTrace);
            }
        }

        private async Task DeleteItem()
        {
            try
            {
                var totalItem = NonTobaccoGridItemSource.Count() + TobaccoGridItemSource.Count();

                if (totalItem == 1)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "You cannot delete all the item from the order", "Ok");
                    return;
                }

                var result = await AlertHelper.Instance.ShowConfirmationAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");

                if (result)
                {
                    var isDeleted = await AppReference.QueryService.DeleteOrderDetailFromHistoryPage(AddEditUIModel.EditedOrderDetail);

                    if (isDeleted)
                    {
                        if (AddEditUIModel.SelectedProduct?.isTobbaco == 1)
                        {
                            var objectToDel = TobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            TobaccoGridItemSource.Remove(objectToDel);
                        }
                        else
                        {
                            var objectToDel = NonTobaccoGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalNonTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalNonTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            NonTobaccoGridItemSource.Remove(objectToDel);
                        }
                    }

                    IsAddEditPopupVisible = false;

                    decimal total = 0;

                    var date = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                    if (TobaccoGridItemSource.Count == 0)
                        IsTobaccoGridVisible = false;
                    if (NonTobaccoGridItemSource.Count == 0)
                        IsNonTobaccoGridVisible = false;

                    RecalculateMainGridTotal(date, ref total);

                    await AppReference.QueryService.UpdateOrderGrandTotal(total.ToString("0.00"), UiModel.OrderMasterData.OrderID);

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(DeleteItem), ex.StackTrace);
            }
        }

        private async Task DeleteItemCreditRequest()
        {
            try
            {
                var totalItem = RtnGridItemSource.Count() + DifGridItemSource.Count();

                if (totalItem == 1)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "You cannot delete all the item from the order", "Ok");
                    return;
                }

                var result = await AlertHelper.Instance.ShowConfirmationAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");

                if (result)
                {
                    var isDeleted = await AppReference.QueryService.DeleteOrderDetailFromHistoryPage(AddEditUIModel.EditedOrderDetail);

                    if (isDeleted)
                    {
                        if (AddEditUIModel.SelectedCreditRequest.Contains("RTN"))
                        {
                            var objectToDel = RtnGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalRtnTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalRtnTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            RtnGridItemSource.Remove(objectToDel);
                        }
                        else
                        {
                            var objectToDel = DifGridItemSource.FirstOrDefault(x => x.OrderDetailObject.OrderDetailId == AddEditUIModel.EditedOrderDetail.OrderDetailId);
                            var oldTotalString = TotalDifTobacco.Split('$').LastOrDefault();
                            var deltedTotal = AddEditUIModel.SubTotal.Split('$').LastOrDefault();
                            var newTotal = Convert.ToDouble(oldTotalString) - Convert.ToDouble(deltedTotal);
                            TotalDifTobacco = string.Format("${0}", newTotal.ToString("0.00"));
                            DifGridItemSource.Remove(objectToDel);
                        }
                    }

                    IsAddEditPopupVisible = false;

                    decimal total = 0;

                    var date = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                    if (RtnGridItemSource.Count == 0)
                        IsRtnGridVisible = false;
                    if (DifGridItemSource.Count == 0)
                        IsDifGridVisible = false;

                    RecalculateMainGridTotal(date, ref total);

                    await AppReference.QueryService.UpdateOrderGrandTotal(total.ToString("0.00"), UiModel.OrderMasterData.OrderID);

                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(DeleteItemCreditRequest), ex.StackTrace);
            }
        }
        private async Task AddProductToList()
        {
            try
            {

                if (AddEditUIModel.SelectedProduct != null && !string.IsNullOrWhiteSpace(AddEditUIModel.ProductName))
                {
                    var date = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);

                    var orderDetail = new OrderDetail()
                    {
                        isTobbaco = AddEditUIModel.SelectedProduct.isTobbaco,
                        OrderId = UiModel.OrderMasterData.OrderID,
                        DeviceOrderID = UiModel.OrderMasterData.DeviceOrderID,
                        Price = AddEditUIModel.PriceToSave,
                        ProductDescription = AddEditUIModel.ProductDesc,
                        Quantity = Convert.ToInt32(AddEditUIModel.Quantity),
                        ProductId = AddEditUIModel.SelectedProduct.ProductID,
                        BrandId = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandId : 0,
                        CategoryId = AddEditUIModel.SelectedCategory.CategoryID,
                        CreatedDate = date,
                        UpdatedDate = date,
                        BrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        CategoryName = AddEditUIModel.SelectedCategory.CategoryName,
                        ProductName = AddEditUIModel.SelectedProduct.ProductName,
                        StyleId = AddEditUIModel.SelectedStyle.StyleId,
                        Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault(),
                        StyleName = AddEditUIModel.StyleName,
                        Unit = AddEditUIModel.SelectedUom,
                    };

                    orderDetail = await AppReference.QueryService.InsertOrUpdateOrderDetail(orderDetail);

                    if (!string.IsNullOrEmpty(AddEditUIModel.Price))
                    {
                        var tempPrice = AddEditUIModel.Price.Split('$').LastOrDefault();

                        var decimalPoint = Convert.ToDecimal(tempPrice);

                        AddEditUIModel.Price = string.Format("${0}", decimalPoint.ToString("0.00"));
                    }

                    var obj = new OrderHistoryDetailsGridUIModel()
                    {
                        Brand = AddEditUIModel.SelectedBrand,
                        Category = AddEditUIModel.SelectedCategory,
                        DisplayBrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        DisplayProductDesc = AddEditUIModel.ProductDesc,
                        DisplayProductName = AddEditUIModel.ProductName,
                        DisplayProductQty = AddEditUIModel.Quantity,
                        DisplayProductSubtotal = AddEditUIModel.SubTotal,
                        DisplayProductUnit = AddEditUIModel.SelectedUom,

                        DisplayProductUnitPrice = AddEditUIModel.Price,

                        DisplayStyleName = AddEditUIModel.StyleName,
                        Product = AddEditUIModel.SelectedProduct,
                        Style = AddEditUIModel.SelectedStyle,
                        OrderDetailObject = orderDetail,
                    };
                    var productTotal = Convert.ToDouble(AddEditUIModel.SubTotal.Split('$').LastOrDefault());
                    if (AddEditUIModel.SelectedProduct.isTobbaco == 1)
                    {
                        TobaccoGridItemSource.Add(obj);
                        var totalTobacco = string.IsNullOrEmpty(TotalTobacco) ? 0 : Convert.ToDouble(TotalTobacco.Split('$').LastOrDefault());
                        var total = totalTobacco + productTotal;
                        TotalTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsTobaccoGridVisible = true;
                        LoadTobaccoGrid = true;
                    }
                    else
                    {
                        NonTobaccoGridItemSource.Add(obj);
                        var totalnonTobacco = string.IsNullOrEmpty(TotalNonTobacco) ? 0 : Convert.ToDouble(TotalNonTobacco.Split('$').LastOrDefault());
                        var total = totalnonTobacco + productTotal;
                        TotalNonTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsNonTobaccoGridVisible = true;
                        LoadNonTobaccoGrid = true;
                    }

                    decimal grandTotal = 0;
                    RecalculateMainGridTotal(date, ref grandTotal);
                    await AppReference.QueryService.UpdateOrderGrandTotal(grandTotal.ToString("0.00"), UiModel.OrderMasterData.OrderID);

                    IsAddEditPopupVisible = false;
                }

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(AddProductToList), ex.StackTrace);
            }
        }

        private async Task AddProductToListForCreditRequest()
        {
            try
            {
                if (AddEditUIModel.SelectedProduct != null && !string.IsNullOrWhiteSpace(AddEditUIModel.ProductName))
                {
                    var date = DateTimeHelper.ConvertToDbInsertDateTimeMilliSecondFormat(DateTime.Now);

                    if (AddEditUIModel.PriceToSave == null || AddEditUIModel.PriceToSave == "0")
                    {
                        var pricetemp = AddEditUIModel.Price.Split('$').LastOrDefault();
                        AddEditUIModel.PriceToSave = pricetemp.Split('.').FirstOrDefault();
                    }

                    var orderDetail = new OrderDetail()
                    {
                        isTobbaco = AddEditUIModel.SelectedProduct.isTobbaco,
                        OrderId = UiModel.OrderMasterData.OrderID,
                        DeviceOrderID = UiModel.OrderMasterData.DeviceOrderID.ToString(),
                        Price = AddEditUIModel.PriceToSave,
                        ProductDescription = AddEditUIModel.ProductDesc,
                        Quantity = string.IsNullOrEmpty(AddEditUIModel.Quantity) ? -1 : Convert.ToInt32(AddEditUIModel.Quantity),
                        ProductId = AddEditUIModel.SelectedProduct.ProductID,
                        BrandId = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandId : 0,
                        CategoryId = AddEditUIModel.SelectedCategory.CategoryID,
                        CreatedDate = date,
                        UpdatedDate = date,
                        BrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        CategoryName = AddEditUIModel.SelectedCategory.CategoryName,
                        ProductName = AddEditUIModel.SelectedProduct.ProductName,
                        StyleId = AddEditUIModel.SelectedStyle.StyleId,
                        Total = AddEditUIModel.SubTotal.Split('$').LastOrDefault(),
                        StyleName = AddEditUIModel.StyleName,
                        Unit = AddEditUIModel.SelectedUom,
                        CreditRequest = AddEditUIModel.SelectedCreditRequest,
                    };
                    orderDetail = await AppReference.QueryService.InsertOrUpdateOrderDetail(orderDetail);

                    if (!string.IsNullOrEmpty(AddEditUIModel.Price))
                    {
                        var tempPrice = AddEditUIModel.Price.Split('$').LastOrDefault();

                        var decimalPoint = Convert.ToDecimal(tempPrice);

                        AddEditUIModel.Price = string.Format("${0}", decimalPoint.ToString("0.00"));
                    }

                    var obj = new OrderHistoryDetailsGridUIModel()
                    {
                        Brand = AddEditUIModel.SelectedBrand,
                        Category = AddEditUIModel.SelectedCategory,
                        DisplayBrandName = AddEditUIModel.SelectedBrand != null ? AddEditUIModel.SelectedBrand.BrandName : string.Empty,
                        DisplayProductDesc = AddEditUIModel.ProductDesc,
                        DisplayProductName = AddEditUIModel.ProductName,
                        DisplayProductQty = AddEditUIModel.Quantity,
                        DisplayProductSubtotal = AddEditUIModel.SubTotal,
                        DisplayProductUnit = AddEditUIModel.SelectedUom,
                        DisplayProductUnitPrice = AddEditUIModel.Price,
                        DisplayStyleName = AddEditUIModel.StyleName,
                        Product = AddEditUIModel.SelectedProduct,
                        Style = AddEditUIModel.SelectedStyle,
                        OrderDetailObject = orderDetail,
                        CreditRequestType = AddEditUIModel.SelectedCreditRequest
                    };
                    var productTotal = Convert.ToDouble(AddEditUIModel.SubTotal.Split('$').LastOrDefault());
                    if (AddEditUIModel.SelectedCreditRequest.Contains("RTN"))
                    {
                        RtnGridItemSource.Add(obj);
                        var totalTobacco = string.IsNullOrEmpty(TotalRtnTobacco) ? 0 : Convert.ToDouble(TotalRtnTobacco.Split('$').LastOrDefault());
                        var total = totalTobacco + productTotal;
                        TotalRtnTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsRtnGridVisible = true;
                        LoadRtnGrid = true;
                    }
                    else
                    {
                        DifGridItemSource.Add(obj);
                        var totalnonTobacco = string.IsNullOrEmpty(TotalDifTobacco) ? 0 : Convert.ToDouble(TotalDifTobacco.Split('$').LastOrDefault());
                        var total = totalnonTobacco + productTotal;
                        TotalDifTobacco = string.Format("${0}", total.ToString("0.00"));
                        IsDifGridVisible = true;
                        LoadDifGrid = true;
                    }

                    decimal grandTotal = 0;
                    RecalculateMainGridTotal(date, ref grandTotal);
                    await AppReference.QueryService.UpdateOrderGrandTotal(grandTotal.ToString("0.00"), UiModel.OrderMasterData.OrderID);

                    IsAddEditPopupVisible = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(AddProductToListForCreditRequest), ex.StackTrace);
            }
        }

        private async Task PriceChangedCommandHandler(string arg)
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           () =>
                           {
                               AddEditUIModel.PriceToSave = arg;
                               AddEditUIModel.Price = "$" + string.Format("{0:0.00}", Convert.ToDecimal(arg));
                               CalculateTotalForPopup();
                           }).AsTask().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "PriceChangedCommandHandler", ex.StackTrace);
            }
        }

        private async Task QuantityChangedCommandHandler(string qty)
        {
            try
            {
                await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal,
                           () => CalculateTotalForPopup()).AsTask().ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "QuantityChangedCommandHandler", ex.StackTrace);
            }
        }

        private void CalculateTotalForPopup()
        {
            try
            {
                double qty = Convert.ToDouble(AddEditUIModel.Quantity);
                double price = Convert.ToDouble(AddEditUIModel.Price.Split('$').LastOrDefault());
                double subTotal = qty * price;
                AddEditUIModel.SubTotal = string.Format("${0}", string.Format("{0:0.00}", subTotal));
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(CalculateTotalForPopup), ex.StackTrace);
            }
        }

        private void NumPadButtonClicCommandHandler(string arg)
        {
            try
            {
                if (IsOpenedFrom == "quantity")
                {
                    UpdateQuantityFieldValue(arg);
                }
                else
                {
                    UpdatePriceFieldValue(arg);
                }

                //CalculateTotalForPopup();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "NumPadButtonClicCommandHandler", ex.StackTrace);
            }
        }

        private void UpdatePriceFieldValue(string value)
        {
            try
            {
                if (value.Equals("C"))
                {
                    string currentPrice = AddEditUIModel.Price.Replace("$", "");
                    if (currentPrice.Length > 0)
                    {
                        AddEditUIModel.Price = "$" + currentPrice.Remove(currentPrice.Length - 1);
                        PriceString = AddEditUIModel.Price;
                    }
                }
                else
                {
                    PriceString = PriceString.Replace("$", "");
                    if (!PriceString.Contains("."))
                    {
                        if (value != ".")
                        {
                            if (PriceString.Length < 4)
                            {
                                PriceString += value;

                                AddEditUIModel.PriceToSave = PriceString;
                                AddEditUIModel.Price = "$" + PriceString;
                            }
                        }
                        else
                        {
                            PriceString += value;
                            AddEditUIModel.Price = "$" + PriceString;
                        }
                    }
                    else
                    {
                        if (value != ".")
                        {
                            string tail = PriceString.Substring(PriceString.LastIndexOf('.') + 1);

                            if (tail.Length == 0)
                            {
                                int dotIndex = PriceString.IndexOf('.');
                                int insertToIndex = ++dotIndex;
                                var newString = PriceString.Insert(insertToIndex, value);
                                PriceString = newString;

                                AddEditUIModel.PriceToSave = PriceString;
                                AddEditUIModel.Price = "$" + PriceString;
                            }
                            else if (tail.Length == 1)
                            {
                                PriceString += value;
                                AddEditUIModel.PriceToSave = PriceString;
                                AddEditUIModel.Price = "$" + PriceString;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "UpdatePriceFieldValue", ex.StackTrace);
            }
        }

        private void UpdateQuantityFieldValue(string value)
        {
            try
            {
                quantityBeforeEdit = AddEditUIModel.Quantity;
                if (value.Equals("C"))
                {
                    if (AddEditUIModel?.Quantity?.Length > 0)
                    {
                        AddEditUIModel.Quantity = AddEditUIModel?.Quantity?.Remove(AddEditUIModel.Quantity.Length - 1);
                        quantityString = AddEditUIModel.Quantity;
                    }
                }
                else
                {
                    AddEditUIModel.Quantity = AddEditUIModel?.Quantity ?? "";
                    if (value == "-" && quantityString.IndexOf('-') != -1)
                    {
                        return;
                    }

                    if (value == "-" && !AddEditUIModel.Quantity.StartsWith('-'))
                    {
                        quantityString = "-" + quantityString;
                    }
                    else if (AddEditUIModel.Quantity.Length != 0)
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


                    if (AddEditUIModel.IsCreditRequest)
                    {
                        if (!string.IsNullOrEmpty(quantityString) && !quantityString.Contains("-"))
                        {
                            quantityString = "-" + quantityString;
                        }
                    }

                    AddEditUIModel.Quantity = quantityString;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "UpdateQuantityFieldValue", ex.StackTrace);
            }
        }

        private void AutoSuggestSuggestionChoosenHandler(AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            try
            {
                var product = args.SelectedItem as ProductMaster;

                if (product.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
                    return;

                AddEditUIModel.ProductList.Clear();

                AddEditUIModel.SelectedBrand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == product.BrandId);
                AddEditUIModel.SelectedCategory = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == product.CatId);
                AddEditUIModel.SelectedStyle = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == product.StyleId);
                AddEditUIModel.SelectedProduct = product;
                AddEditUIModel.Quantity = "1";
                AddEditUIModel.Price = "$0.00";
                AddEditUIModel.ProductName = product.ProductName;
                AddEditUIModel.ProductDesc = product.Description;
                AddEditUIModel.BrandName = AddEditUIModel.SelectedBrand?.BrandName;
                AddEditUIModel.StyleName = AddEditUIModel.SelectedStyle?.StyleName;

                if (AddEditUIModel.IsCreditRequest)
                {
                    GetCreditRequestDropDownValues(AddEditUIModel?.SelectedProduct?.isTobbaco);

                    AddEditUIModel.Quantity = "-1";

                    AddEditUIModel.SelectedCreditRequest = AddEditUIModel?.SelectedCategory?.CategoryID == 6 ? "DIF-Destroyed" : "RTN-Retail Returns";
                    //AddEditUIModel.SelectedCreditRequest =  "RTN-Retail Returns";
                    if (AddEditUIModel?.SelectedCategory?.ERPCategoryId == 1010 || AddEditUIModel?.SelectedCategory?.ERPCategoryId == 1030)
                    {
                        AddEditUIModel.SelectedUom = "BX";
                    }
                    else
                    {
                        AddEditUIModel.SelectedUom = "EA";
                    }
                }
                else
                {
                    if (UiModel.SelectedCustomer?.AccountType != 2)
                    {
                        AddEditUIModel.SelectedUom = "CA";
                    }
                    else
                    {
                        AddEditUIModel.SelectedUom = SetUomForCategoryId(AddEditUIModel.SelectedCategory.ERPCategoryId);
                    }
                }
                AddEditUIModel.PriceToSave = AddEditUIModel.Price.Split('$').LastOrDefault();
                CalculateTotalForPopup();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "AutoSuggestSuggestionChoosenHandler", ex.StackTrace);
            }
        }

        private void GetCreditRequestDropDownValues(int? isTobbaco)
        {
            try
            {
                CreditRequestComoBoxSource.Clear();

                if (isTobbaco.HasValue && isTobbaco.Value != 1)
                {
                    CreditRequestComoBoxSource.Add("DIF-Destroyed");
                }
                CreditRequestComoBoxSource.Add("RTN-Retail Returns");
                CreditRequestComoBoxSource.Add("RTN-Out Of Code");
                CreditRequestComoBoxSource.Add("RTN-Defective");
                CreditRequestComoBoxSource.Add("RTN-Overstock");
                CreditRequestComoBoxSource.Add("RTN-Discontinued");
                CreditRequestComoBoxSource.Add("RTN-Reset");
                CreditRequestComoBoxSource.Add("RTN-Mispick");
                CreditRequestComoBoxSource.Add("RTN-Misship");
                CreditRequestComoBoxSource.Add("RTN-Order Error");
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "GetCreditRequestDropDownValues", ex.StackTrace);
            }
        }

        private void AutoSuggestTextChangedHandler(string text)
        {
            try
            {
                AddEditUIModel.ProductList.Clear();

                if (!string.IsNullOrWhiteSpace(text))
                {
                    //var tempList = UiModel.DbProducts.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower()) &&
                    //x.CatId != -88 && x.CatId != 10 && x.CatId != 11 && x.UOM == "CA" && x.CatId != -99 && x.IsDeleted == 0 &&
                    //x.SRCHoneySellable != 0 && x.SRCHoneyReturnable != 0 && x.SRCCanIOrder != 0 &&
                    //!UiModel.DbOrderDetailsData.Any(y => y.ProductId == x.ProductID)).ToList();
                    var tempList = UiModel.DbProducts.Where(x => x.SearchDisplayPath.ToLower().Contains(text.ToLower()) &&
                   x.CatId != -88 && x.CatId != 10 && x.CatId != 11 && x.UOM == "CA" && x.CatId != -99 && x.IsDeleted == 0 &&
                   !UiModel.DbOrderDetailsData.Any(y => y.ProductId == x.ProductID)).ToList();

                    if (AppReference.IsCreditRequestOrder == true)
                    {
                        tempList = tempList.Where(x => x.SRCHoneyReturnable != 0).ToList();
                    }
                    else if (AppReference.IsDistributionOptionClicked == true)
                    {
                        tempList = tempList.Where(x => x.SRCCanIOrder != 0).ToList();
                    }
                    else
                    {
                        tempList = tempList.Where(x => x.SRCHoneySellable != 0).ToList();
                    }

                    if (tempList == null || tempList.Count == 0)
                    {
                        AddEditUIModel.ProductList.Add(new ProductMaster() { ProductName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                    }
                    else
                    {
                        tempList.ForEach(x => AddEditUIModel.ProductList.Add(x));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "AutoSuggestTextChangedHandler", ex.StackTrace);
            }
        }

        private string SetUomForCategoryId(int id)
        {
            switch (id)
            {
                case 1010:
                case 1030:
                case 1031:
                case 1042:
                    return "BX";
                case 1020:
                case 1021:
                case 1040:
                case 1041:
                case 1043:
                case 1070:
                    return "EA";
                default:
                    return "CA";
            }
        }

        private void AddButtonCommandHandler()
        {
            try
            {
                AddEditUIModel = new AddEditProductOrderHistoryUIModel();
                AddEditUIModel.AutoSuggestionText = string.Empty;
                IsAddEditPopupVisible = true;
                IsAdd = true;
                PopupTitle = "Add Product In Cart";
                AddEditUIModel.IsCreditRequest = "8".Equals(UiModel?.OrderMasterData?.SalesType);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "AddButtonCommandHandler", ex.StackTrace);
            }

            ///AddEditUIModel.ProductList = new ObservableCollection<ProductMaster>(UiModel.DbProducts.Where(sRCProductUIModel => sRCProductUIModel.SRCHoneySellable != 0 && sRCProductUIModel.SRCHoneyReturnable != 0 && sRCProductUIModel.SRCCanIOrder != 0));
        }

        private void ClosePopupCommandHandler()
        {
            IsAddEditPopupVisible = false;
        }

        private async Task EditSaveCommandHandler()
        {
            try
            {
                if (IsEditmode)
                {
                    await AddOrderDetailsToDb();
                }
                IsEditmode = !IsEditmode;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), "EditSaveCommandHandler", ex.StackTrace);
            }
        }

        private async Task AddOrderDetailsToDb()
        {
            try
            {
                IsLoading = true;
                decimal total = 0;

                var date = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                RecalculateMainGridTotal(date, ref total);

                ///logic to get the updated data
                UiModel.OrderMasterData.GrandTotal = total.ToString("0.00");
                UiModel.OrderMasterData.UpdatedDate = date;
                UiModel.OrderMasterData.PurchaseOrderNumber = UiModel.PurchaseOrder;
                UiModel.OrderMasterData.OrderAddress = UiModel.PhysicalAddress;
                UiModel.OrderMasterData.OrderCityId = UiModel.PhysicalCity;
                UiModel.OrderMasterData.OrderStateId = HelperMethods.GetKeyFromIdNameDictionary(UiModel.StateDictionary, UiModel.SelectedPhysicalState);
                UiModel.OrderMasterData.OrderZipCode = UiModel.PhysicalZip;
                UiModel.OrderMasterData.StateTobaccoLicence = UiModel.StateTobaccoLicense;
                UiModel.OrderMasterData.RetailerLicense = UiModel.RetailerLicense;
                UiModel.OrderMasterData.RetailerSalesTaxCertificate = UiModel.RetailerSalesTaxCertificate;
                UiModel.OrderMasterData.EmailRecipients = UiModel.EmailTo;
                UiModel.OrderMasterData.CustomStatement = UiModel.CustomTaxStatement;
                UiModel.OrderMasterData.OrderMasterSellerRepTobacco = UiModel.OrderMasterSellerRepTobacco;
                UiModel.OrderMasterData.PrebookShipDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(UiModel.PreBookDate.DateTime);
                UiModel.OrderMasterData.CustomerComment = UiModel.Comments;

                await AppReference.QueryService.InsertOrUpdateOrderMaster(UiModel.OrderMasterData, false);

                var callActivityData = await AppReference.QueryService.GetCurrentCustomerCallActivityDataAsync(UiModel.OrderMasterData.DeviceCustomerID, UiModel.OrderMasterData.DeviceOrderID);

                if (callActivityData != null)
                {
                    callActivityData.GrandTotal = Convert.ToDecimal(UiModel.OrderMasterData.GrandTotal);
                    callActivityData.UpdateDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                    await AppReference.QueryService.UpdateCallActivityDataOnUpdateOrderFromHistory(callActivityData);
                }

                IsLoading = false;

                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Order Updated Successfully", "Yes");
                await AppReference.QueryService.UpdateOrderGrandTotal(total.ToString("0.00"), UiModel.OrderMasterData.OrderID);

            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(AddOrderDetailsToDb), ex.StackTrace);
            }
        }

        private void RecalculateMainGridTotal(string date, ref decimal total)
        {
            try
            {
                int qtyTotal = 0;
                decimal tobaccoTotal = 0;
                decimal nonTobaccoTotal = 0;
                decimal grandTotal = 0;
                if (TobaccoGridItemSource.Any() || NonTobaccoGridItemSource.Any())
                {
                    foreach (var item in TobaccoGridItemSource)
                    {
                        item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                        item.OrderDetailObject.Price = item.DisplayProductUnitPrice.Split('$').LastOrDefault();
                        item.OrderDetailObject.Unit = item.DisplayProductUnit;
                        item.OrderDetailObject.UpdatedDate = date;
                        tobaccoTotal = tobaccoTotal + (Convert.ToDecimal(item.OrderDetailObject.Price) * item.OrderDetailObject.Quantity);
                        qtyTotal += item.OrderDetailObject.Quantity;
                    }

                    foreach (var item in NonTobaccoGridItemSource)
                    {
                        item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                        item.OrderDetailObject.Price = item.DisplayProductUnitPrice.Split('$').LastOrDefault();
                        item.OrderDetailObject.Unit = item.DisplayProductUnit;
                        item.OrderDetailObject.UpdatedDate = date;
                        nonTobaccoTotal = nonTobaccoTotal + (Convert.ToDecimal(item.OrderDetailObject.Price) * item.OrderDetailObject.Quantity);
                        qtyTotal += item.OrderDetailObject.Quantity;
                    }

                    total = tobaccoTotal + nonTobaccoTotal;
                    grandTotal = tobaccoTotal + nonTobaccoTotal;

                    TotalTobacco = string.Format("${0}", tobaccoTotal.ToString("0.00"));
                    TotalNonTobacco = string.Format("${0}", nonTobaccoTotal.ToString("0.00"));
                    GrandTotalValue = string.Format("${0}", grandTotal.ToString("0.00"));
                }
                else
                {
                    foreach (var item in RtnGridItemSource)
                    {
                        item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                        item.OrderDetailObject.Price = item.DisplayProductUnitPrice.Split('$').LastOrDefault();
                        item.OrderDetailObject.Unit = item.DisplayProductUnit;
                        item.OrderDetailObject.UpdatedDate = date;
                        tobaccoTotal = tobaccoTotal + (Convert.ToDecimal(item.OrderDetailObject.Price) * item.OrderDetailObject.Quantity);
                        qtyTotal += item.OrderDetailObject.Quantity;
                    }

                    foreach (var item in DifGridItemSource)
                    {
                        item.OrderDetailObject.Quantity = (int)Convert.ToDouble(item.DisplayProductQty);
                        item.OrderDetailObject.Price = item.DisplayProductUnitPrice.Split('$').LastOrDefault();
                        item.OrderDetailObject.Unit = item.DisplayProductUnit;
                        item.OrderDetailObject.UpdatedDate = date;
                        nonTobaccoTotal = nonTobaccoTotal + (Convert.ToDecimal(item.OrderDetailObject.Price) * item.OrderDetailObject.Quantity);
                        qtyTotal += item.OrderDetailObject.Quantity;
                    }
                    total = tobaccoTotal + nonTobaccoTotal;
                    grandTotal = tobaccoTotal + nonTobaccoTotal;

                    TotalRtnTobacco = string.Format("${0}", tobaccoTotal.ToString("0.00"));
                    TotalDifTobacco = string.Format("${0}", nonTobaccoTotal.ToString("0.00"));
                    GrandTotalValue = string.Format("${0}", grandTotal.ToString("0.00"));
                }

                IsGrandTotalVisible = true;

                OrderDetailUpdatedModel updatedObj = new OrderDetailUpdatedModel() { Qty = qtyTotal, Price = total.ToString(), OrderId = UiModel.OrderMasterData.OrderID };

                OrderHistoryPage.UpdatedOrder = updatedObj;

            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(RecalculateMainGridTotal), ex.StackTrace);
            }
        }

        private async Task OnNavigatedToCommandHandler(OrderHistoryDetailsPageUIModel selectedObject)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }

            try
            {
                IsLoading = true;
                LoadTobaccoGrid = false;
                LoadNonTobaccoGrid = false;
                IsNonTobaccoGridVisible = false;
                IsTobaccoGridVisible = false;
                IsRtnGridVisible = false;
                IsDifGridVisible = false;
                IsGrandTotalVisible = false;
                LoadRackPosPopGrid = false;
                IsEditmode = false;
                IsSignPanelVisible = false;

                if ("5".Equals(selectedObject.OrderHistoryModel.SalesType) || "10".Equals(selectedObject.OrderHistoryModel.SalesType))
                {
                    UiModel = await AppReference.QueryService.GetOrderHistoryDetailData(selectedObject);

                    var distributorMaster = await AppReference.QueryService.GetDistributorFromId(UiModel.OrderMasterData.CustomerDistributorID);

                    UiModel.PopulateUI(distributorMaster);

                    if (UiModel.DbOrderDetailsData != null)
                    {
                        foreach (var item in UiModel.DbOrderDetailsData)
                        {
                            var product = UiModel?.DbProducts?.FirstOrDefault(x => x.ProductID == item.ProductId);
                            item.ProductDescription = product?.Description;
                            item.ProductName = product?.ProductName;
                            RackPosPopOrderDetailsItemSource.Add(item);
                        }
                    }

                    if (RackPosPopOrderDetailsItemSource != null && RackPosPopOrderDetailsItemSource.Any())
                    {
                        LoadRackPosPopGrid = true;
                    }
                }
                else
                {
                    UiModel = await AppReference.QueryService.GetOrderHistoryDetailData(selectedObject);

                    var distributorMaster = await AppReference.QueryService.GetDistributorFromId(UiModel.OrderMasterData.CustomerDistributorID);

                    UiModel.PopulateUI(distributorMaster);

                    decimal totalTobacco = 0;
                    decimal totalNonTobacco = 0;
                    decimal rtnTotal = 0;
                    decimal difTotal = 0;
                    decimal grandTotal = 0;
                    decimal decimalPrice = 0;

                    foreach (var d in UiModel.DbOrderDetailsData)
                    {
                        if (!string.IsNullOrEmpty(d.Price))
                        {
                            decimalPrice = Convert.ToDecimal(d.Price);
                        }

                        var obj = new OrderHistoryDetailsGridUIModel();
                        obj.Brand = UiModel.DbBrandData.FirstOrDefault(x => x.BrandId == d.BrandId);
                        obj.Category = UiModel.DbCategoryMaster.FirstOrDefault(x => x.CategoryID == d.CategoryId);
                        obj.Style = UiModel.DbStyleMaster.FirstOrDefault(x => x.StyleId == d.StyleId);
                        obj.Product = UiModel.DbProducts.FirstOrDefault(x => x.ProductID == d.ProductId);
                        obj.DisplayProductQty = d.Quantity.ToString();
                        obj.DisplayProductUnitPrice = string.IsNullOrEmpty(d.Price) ? "$0.00" : string.Format("${0}", decimalPrice.ToString("0.00"));
                        var total = Convert.ToDecimal(d.Price) * d.Quantity;
                        obj.DisplayProductSubtotal = string.Format("${0}", total.ToString("0.00"));
                        obj.OrderDetailObject = d;
                        obj.CreditRequestType = d.CreditRequest;
                        obj.DisplayProductUnit = d?.Unit;
                        obj.PropulateUI();

                        if (!"8".Equals(UiModel.OrderMasterData.SalesType))
                        {
                            int isTobaccoProduct = 0;
                            if (d.ProductId == obj.Product.ProductID)
                            {
                                isTobaccoProduct = obj.Product.isTobbaco;
                            }

                            if (obj.Product.isTobbaco == 0)
                            {
                                NonTobaccoGridItemSource.Add(obj);
                                totalNonTobacco += total;
                            }
                            else
                            {
                                TobaccoGridItemSource.Add(obj);
                                totalTobacco += total;
                            }

                            grandTotal = totalNonTobacco + totalTobacco;

                            GrandTotalValue = string.Format("${0}", grandTotal.ToString("0.00"));
                        }
                        else
                        {
                            if (d.CreditRequest.Contains("RTN"))
                            {
                                RtnGridItemSource.Add(obj);
                                rtnTotal += total;
                            }
                            else if (d.CreditRequest.Contains("DIF"))
                            {
                                DifGridItemSource.Add(obj);
                                difTotal += total;
                            }

                            grandTotal = rtnTotal + difTotal;

                            GrandTotalValue = string.Format("${0}", grandTotal.ToString("0.00"));
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(UiModel.OrderMasterData.SalesType)
                           && !UiModel.OrderMasterData.SalesType.Equals("5")
                           && !UiModel.OrderMasterData.SalesType.Equals("10")
                           && !UiModel.OrderMasterData.SalesType.Equals("13")
                           && UiModel.OrderMasterData.IsExported == 0)
                    {
                        IsEditIconVisible = true;
                    }
                    else
                    {
                        IsEditIconVisible = false;
                    }

                    if (NonTobaccoGridItemSource.Count > 0)
                    {
                        LoadNonTobaccoGrid = true;
                        IsNonTobaccoGridVisible = true;
                        IsGrandTotalVisible = true;
                        TotalNonTobacco = string.Format("${0}", totalNonTobacco.ToString("0.00"));
                    }
                    if (TobaccoGridItemSource.Count > 0)
                    {
                        LoadTobaccoGrid = true;
                        IsTobaccoGridVisible = true;
                        IsGrandTotalVisible = true;
                        TotalTobacco = string.Format("${0}", totalTobacco.ToString("0.00"));
                    }
                    if (RtnGridItemSource.Count > 0)
                    {
                        LoadRtnGrid = true;
                        IsRtnGridVisible = true;
                        IsGrandTotalVisible = true;
                        TotalRtnTobacco = string.Format("${0}", rtnTotal.ToString("0.00"));
                    }
                    if (DifGridItemSource.Count > 0)
                    {
                        LoadDifGrid = true;
                        IsDifGridVisible = true;
                        IsGrandTotalVisible = true;
                        TotalDifTobacco = string.Format("${0}", difTotal.ToString("0.00"));
                    }
                }

                var path = AppReference.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Signature, UiModel.OrderMasterData.CustomerSignatureFileName);

                if (!string.IsNullOrEmpty(path))
                {
                    SignPath = new BitmapImage();
                    SignPath.UriSource = new Uri(path);
                    IsSignPanelVisible = true;
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                IsLoading = false;
                ErrorLogger.WriteToErrorLog(nameof(OrderHistoryDetailsPageViewModel), nameof(OnNavigatedToCommandHandler), ex.StackTrace);
            }

            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = true;
            }
        }
        #endregion
    }
}
