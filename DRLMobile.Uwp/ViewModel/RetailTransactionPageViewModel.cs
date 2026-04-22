using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

using iText.Layout.Properties;

using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Microsoft.UI.Xaml.Controls;

using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

using static DevExpress.Mvvm.Native.TaskLinq;

namespace DRLMobile.Uwp.ViewModel
{
    public class RetailTransactionPageViewModel : BaseModel
    {
        #region Properties

        private readonly ResourceLoader resourceLoader;
        private readonly App AppReference = (App)Application.Current;
        private readonly Random _random = new Random();
        private List<OrderDetailUIModel> DbOrderDetailDataSource;

        private StandardOptionsPrintHelper _printHelper;
        public StandardOptionsPrintHelper PrintHelper
        {
            get { return _printHelper; }
            set { SetProperty(ref _printHelper, value); }
        }

        private RetailTransactionUIModel _retailTransactionUIModel;
        public RetailTransactionUIModel RetailTransacUiModel
        {
            get { return _retailTransactionUIModel; }
            set { SetProperty(ref _retailTransactionUIModel, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _orderDetailGridDataSource;
        public ObservableCollection<OrderDetailUIModel> OrderDetailGridDataSource
        {
            get { return _orderDetailGridDataSource; }
            set { SetProperty(ref _orderDetailGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _tobaccoGridDataSource;
        public ObservableCollection<OrderDetailUIModel> TobaccoGridDataSource
        {
            get { return _tobaccoGridDataSource; }
            set { SetProperty(ref _tobaccoGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _nonTobaccoGridDataSource;
        public ObservableCollection<OrderDetailUIModel> NonTobaccoGridDataSource
        {
            get { return _nonTobaccoGridDataSource; }
            set { SetProperty(ref _nonTobaccoGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _rtnGridDataSource;
        public ObservableCollection<OrderDetailUIModel> RtnGridDataSource
        {
            get { return _rtnGridDataSource; }
            set { SetProperty(ref _rtnGridDataSource, value); }
        }

        private ObservableCollection<OrderDetailUIModel> _difGridDataSource;
        public ObservableCollection<OrderDetailUIModel> DifGridDataSource
        {
            get { return _difGridDataSource; }
            set { SetProperty(ref _difGridDataSource, value); }
        }

        private string _uperGridSubTotal;
        public string UperGridSubTotal
        {
            get { return _uperGridSubTotal; }
            set { SetProperty(ref _uperGridSubTotal, value); }
        }

        private string _lowerGridSubTotal;
        public string LowerGridSubTotal
        {
            get { return _lowerGridSubTotal; }
            set { SetProperty(ref _lowerGridSubTotal, value); }
        }

        private string _grandTotal;
        public string GrandTotal
        {
            get { return _grandTotal; }
            set { SetProperty(ref _grandTotal, value); }
        }

        public DateTime prebookDateTime { get; set; }

        private ObservableCollection<OrderDetailUIModel> _rackOrderGridDataSource;
        public ObservableCollection<OrderDetailUIModel> RackOrderGridDataSource
        {
            get { return _rackOrderGridDataSource; }
            set { SetProperty(ref _rackOrderGridDataSource, value); }
        }

        private string _quantityBeforeEdit = string.Empty;
        public string quantityBeforeEdit
        {
            get { return _quantityBeforeEdit; }
            set { SetProperty(ref _quantityBeforeEdit, value); }
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

        #region Command
        public ICommand ConfirmOrderCommand { get; private set; }
        public ICommand DistributionSelectionCommand { get; private set; }
        public ICommand DeleteImageCommand { private set; get; }
        public ICommand NumPadGridButtonClickCommand { get; private set; }
        public ICommand QuantityChangedCommand { get; private set; }
        public ICommand SaveSignatureCommand { get; private set; }
        public ICommand CancelSignPanelCommand { get; private set; }
        public ICommand AddCustomTaxStatementCommand { get; private set; }
        public ICommand SetCustomTaxStatementCommand { get; private set; }
        public ICommand SaleTypeCommand { get; private set; }
        #endregion

        #region Constructor

        public RetailTransactionPageViewModel()
        {
            RetailTransacUiModel = new RetailTransactionUIModel();
            DbOrderDetailDataSource = new List<OrderDetailUIModel>();
            OrderDetailGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            TobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            NonTobaccoGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            RtnGridDataSource = new ObservableCollection<OrderDetailUIModel>();
            DifGridDataSource = new ObservableCollection<OrderDetailUIModel>();

            resourceLoader = ResourceLoader.GetForCurrentView();

            ConfirmOrderCommand = new AsyncRelayCommand(ConfirmOrderButtonClickCommand);
            DistributionSelectionCommand = new RelayCommand<DistributorMaster>(DistributionSelectionCommandHandler);
            RackOrderGridDataSource = new ObservableCollection<OrderDetailUIModel>();// Remove Initalization
            DeleteImageCommand = new AsyncRelayCommand<OrderDetailUIModel>(DeleteImageClickedAsync);
            NumPadGridButtonClickCommand = new RelayCommand<string>(NumPadGridButtonClicked);
            QuantityChangedCommand = new AsyncRelayCommand<OrderDetailUIModel>(QuantityChangedAsync);
            AddCustomTaxStatementCommand = new RelayCommand<string>(AddCustomTaxStatementCommandHandler);
            SetCustomTaxStatementCommand = new RelayCommand<UserTaxStatement>(SetCustomTaxStatementCommandHandler);

            SaveSignatureCommand = new AsyncRelayCommand<string>(SaveSignatureCommandHandler);
            CancelSignPanelCommand = new RelayCommand(CancelSignatureActionHandler);
            SaleTypeCommand = new RelayCommand<DropDownUIModel>(SaleTypeCommandHandler);

            LoadingVisibility = Visibility.Collapsed;
        }

        #endregion

        #region Methods

        public async Task InitializeDataOnPageLoad()
        {
            try
            {
                LoadingVisibilityHandler(true);
                int customerId = Convert.ToInt32(AppReference.SelectedCustomerId);

                RetailTransacUiModel = await AppReference.QueryService.GetRetailTransactionData(customerId, AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);

                RetailTransacUiModel.IsCreditRequest = (bool)AppReference.IsCreditRequestOrder;

                RetailTransacUiModel.IsRackOrder = AppReference.CartDataFromScreen == 1 ? true : false;

                RetailTransacUiModel.IsPopOrder = AppReference.CartDataFromScreen == 2 ? true : false;

                RetailTransacUiModel.IsDistributionOrder = (bool)AppReference.IsDistributionOptionClicked;
                RetailTransacUiModel.IsCarStockOrder = AppReference.IsCarStockOrder ?? false;

                RetailTransacUiModel.SetSampleOrder();

                RetailTransacUiModel.OrderId = AppReference.CurrentOrderId;
                RetailTransacUiModel.DeviceOrderId = AppReference.CurrentDeviceOrderId;
                RetailTransacUiModel.CustomerId = (int)RetailTransacUiModel.CustomerData.CustomerID;

                if (AppReference.CartDataFromScreen == 0)
                {
                    RetailTransacUiModel.SetPageTitle();
                    RetailTransacUiModel.RackOrderGridVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.CommentsLabel = "Enter Retail Sales Call Notes :";
                    DbOrderDetailDataSource = await ((App)Application.Current).QueryService.GetCartProductDetailsData(AppReference.CurrentDeviceOrderId);
                    RetailTransacUiModel.OrderDetailsData = DbOrderDetailDataSource;
                    IEnumerable<DropDownUIModel> collectionOfSaleType = new List<DropDownUIModel> {
                                        new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.CashSale),
                                                Name = "Cash Sale"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.Prebook),
                                                Name = "Prebook"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.SuggestedOrder),
                                                Name = "Suggested Order"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.BillThrough),
                                                Name = "Bill Through"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.CashSalesInitiative),
                                                Name = "Cash Sales Initiative"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.ChainDistribution),
                                                Name = "Chain Distribution"
                                            },new DropDownUIModel
                                            {
                                                Id = (int)(SalesType.CreditCardSales),
                                                Name = "Credit Card Sales"
                                            }
                                        };
                    RetailTransacUiModel.SaleTypeCollection = new ObservableCollection<DropDownUIModel>(collectionOfSaleType);


                    if (DbOrderDetailDataSource?.Count > 0)
                    {
                        if (RetailTransacUiModel.IsCreditRequest)
                        {
                            RetailTransacUiModel.SelectedSalesType = "8";
                            RetailTransacUiModel.TobaccoGridVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.NonTobaccoGridVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.SampleOrderOptionVisibility = Visibility.Collapsed;

                            SetRtnDifGridView();
                        }
                        else
                        {
                            RetailTransacUiModel.RtnGridVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.DifGridVisibility = Visibility.Collapsed;
                            SetTobaccoNonTobaccoGrid();
                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;

                        }
                    }
                }
                else if (AppReference.CartDataFromScreen == 1)
                {
                    RetailTransacUiModel.PageTitle = "Rack Order";
                    RetailTransacUiModel.GrandTotalVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.SampleOrderOptionVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.IsRackOrder = true;
                    RetailTransacUiModel.CommentsLabel = "Enter Comment :";
                    DbOrderDetailDataSource = await ((App)Application.Current).QueryService.GetRackCartProductsData(AppReference.CurrentDeviceOrderId);
                    DbOrderDetailDataSource = DbOrderDetailDataSource.OrderBy(x => DateTimeHelper.ConvertToDBDateTime(x.CreatedDate)).ToList();
                    RetailTransacUiModel.OrderDetailsData = DbOrderDetailDataSource;
                    SetRackOrderGrid();
                    RetailTransacUiModel.RackOrderOptionVisibility = Visibility.Visible;
                    RetailTransacUiModel.SelectedSalesType = "5";
                }
                else if (AppReference.CartDataFromScreen == 2)
                {
                    RetailTransacUiModel.PageTitle = "POP Order";
                    RetailTransacUiModel.GrandTotalVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.SampleOrderOptionVisibility = Visibility.Collapsed;
                    RetailTransacUiModel.SelectedSalesType = "10";

                    RetailTransacUiModel.IsPopOrder = true;
                    RetailTransacUiModel.CommentsLabel = "Enter Comment :";
                    DbOrderDetailDataSource = await ((App)Application.Current).QueryService.GetRackCartProductsData(AppReference.CurrentDeviceOrderId);
                    DbOrderDetailDataSource = DbOrderDetailDataSource.OrderBy(x => DateTimeHelper.ConvertToDBDateTime(x.CreatedDate)).ToList();
                    RetailTransacUiModel.OrderDetailsData = DbOrderDetailDataSource;
                    SetRackOrderGrid();
                }

                if (!string.IsNullOrWhiteSpace(AppReference.OrderPrintName))
                {
                    RetailTransacUiModel.PrintName = AppReference.OrderPrintName;
                    //string fileName = RetailTransacUiModel.OrderId + "_" + RetailTransacUiModel.CustomerData?.DeviceCustomerID + "_OrderSignature.jpg";
                    string fileName = $"{RetailTransacUiModel.DeviceOrderId}_OrderSignature.jpg";
                    string filepath = AppReference.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Signature, fileName);
                    if (!string.IsNullOrEmpty(filepath))
                    {
                        // Open a stream for the selected file
                        Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                        StorageFile storageFile = await storageFolder.GetFileAsync(fileName);

                        // Ensure a file was selected
                        if (storageFile != null)
                        {
                            // Ensure the stream is disposed once the image is loaded
                            using (IRandomAccessStream fileStream = await storageFile.OpenAsync(Windows.Storage.FileAccessMode.Read))
                            {
                                if (fileStream.Size > 0)
                                {
                                    // Set the image source to the selected bitmap
                                    BitmapImage bitmapImage = new BitmapImage();
                                    bitmapImage.DecodePixelHeight = 80;
                                    bitmapImage.DecodePixelWidth = 230;

                                    await bitmapImage.SetSourceAsync(fileStream);
                                    RetailTransacUiModel.CustomerSignPath = bitmapImage;
                                    RetailTransacUiModel.CustomerSignatureFileName = filepath;
                                    RetailTransacUiModel.SignatureTextButtonVisibility = Visibility.Collapsed;
                                    RetailTransacUiModel.SignaturePanelVisibility = Visibility.Collapsed;
                                }
                            }
                        }
                        else
                        {
                            RetailTransacUiModel.PrintName = "";
                            RetailTransacUiModel.CustomerSignPath = null;
                            RetailTransacUiModel.CustomerSignatureFileName = "";
                            RetailTransacUiModel.SignatureTextButtonVisibility = Visibility.Visible;
                        }
                    }
                    else
                    {
                        RetailTransacUiModel.PrintName = "";
                        RetailTransacUiModel.CustomerSignPath = null;
                        RetailTransacUiModel.CustomerSignatureFileName = "";
                        RetailTransacUiModel.SignatureTextButtonVisibility = Visibility.Visible;
                    }
                }

                LoadingVisibilityHandler(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "InitializeDataOnPageLoad", ex.StackTrace);
            }
        }

        private void SetTobaccoNonTobaccoGrid()
        {
            decimal uperGridTotal = 0;
            decimal lowerGridTotal = 0;
            decimal grandTotal = 0;

            RetailTransacUiModel.GrandTotalVisibility = Visibility.Visible;

            List<OrderDetailUIModel> orderList = DbOrderDetailDataSource.ToList();
            orderList?.ForEach(x => OrderDetailGridDataSource.Add(x));

            var tobaccoList = orderList.Where(x => x.isTobbaco == 1).ToList();
            tobaccoList.ForEach(x => TobaccoGridDataSource.Add(x));

            foreach (var item in TobaccoGridDataSource)
            {
                decimal total = Convert.ToDecimal(item.Price) * item.Quantity;

                if (!string.IsNullOrEmpty(item.Price))
                {
                    var decimalPrice = Convert.ToDecimal(item.Price);

                    item.PriceDisplay = "$" + decimalPrice.ToString("0.00");

                }
                else
                {
                    item.PriceDisplay = "$0.00";
                }

                item.TotalDisplay = "$" + string.Format("{0:0.00}", total);

                uperGridTotal += total;
            }

            UperGridSubTotal = FormatStringToTwoDecimal(uperGridTotal);

            RetailTransacUiModel.TobaccoGridVisibility = (tobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            var nonTobaccoList = orderList.Where(x => x.isTobbaco == 0).ToList();
            nonTobaccoList.ForEach(x => NonTobaccoGridDataSource.Add(x));

            foreach (var item in NonTobaccoGridDataSource)
            {
                decimal total = Convert.ToDecimal(item.Price) * item.Quantity;

                if (!string.IsNullOrEmpty(item.Price))
                {
                    var decimalPrice = Convert.ToDecimal(item.Price);

                    item.PriceDisplay = "$" + decimalPrice.ToString("0.00");

                }
                else
                {
                    item.PriceDisplay = "$0.00";
                }

                item.TotalDisplay = "$" + string.Format("{0:0.00}", total);

                lowerGridTotal += total;
            }

            LowerGridSubTotal = FormatStringToTwoDecimal(lowerGridTotal);

            RetailTransacUiModel.NonTobaccoGridVisibility = (nonTobaccoList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            grandTotal = uperGridTotal + lowerGridTotal;

            RetailTransacUiModel.GrandTotal = string.Format("{0:0.00}", grandTotal);

            GrandTotal = FormatStringToTwoDecimal(grandTotal);
        }

        private void SetRtnDifGridView()
        {
            decimal uperGridTotal = 0;
            decimal lowerGridTotal = 0;
            decimal grandTotal = 0;

            RetailTransacUiModel.GrandTotalVisibility = Visibility.Visible;

            List<OrderDetailUIModel> orderList = DbOrderDetailDataSource.ToList();
            orderList?.ForEach(x => OrderDetailGridDataSource.Add(x));

            var rtnList = orderList.Where(x => x.CreditRequest.Contains("RTN")).ToList();
            rtnList.ForEach(x => RtnGridDataSource.Add(x));

            foreach (var item in RtnGridDataSource)
            {
                if (item.Quantity > 0)
                {
                    item.Quantity *= -1;
                }
                else if (item.Quantity == 0)
                {
                    item.Quantity = -1;
                }

                decimal total = Convert.ToDecimal(item.Price) * item.Quantity;

                if (!string.IsNullOrEmpty(item.Price))
                {
                    var decimalPrice = Convert.ToDecimal(item.Price);

                    item.PriceDisplay = "$" + decimalPrice.ToString("0.00");

                }
                else
                {
                    item.PriceDisplay = "$0.00";
                }

                item.TotalDisplay = "$" + string.Format("{0:0.00}", total);

                uperGridTotal += total;
            }

            UperGridSubTotal = FormatStringToTwoDecimal(uperGridTotal);

            RetailTransacUiModel.RtnGridVisibility = (rtnList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            var difList = orderList.Where(x => x.CreditRequest.Contains("DIF")).ToList();
            difList.ForEach(x => DifGridDataSource.Add(x));

            foreach (var item in DifGridDataSource)
            {
                if (item.Quantity > 0)
                {
                    item.Quantity *= -1;
                }
                else if (item.Quantity == 0)
                {
                    item.Quantity = -1;
                }

                decimal total = Convert.ToDecimal(item.Price) * item.Quantity;

                if (!string.IsNullOrEmpty(item.Price))
                {
                    var decimalPrice = Convert.ToDecimal(item.Price);

                    item.PriceDisplay = "$" + decimalPrice.ToString("0.00");

                }
                else
                {
                    item.PriceDisplay = "$0.00";
                }

                item.TotalDisplay = "$" + string.Format("{0:0.00}", total);

                lowerGridTotal += total;
            }

            LowerGridSubTotal = FormatStringToTwoDecimal(lowerGridTotal);

            RetailTransacUiModel.DifGridVisibility = (difList.Count > 0) ? Visibility.Visible : Visibility.Collapsed;

            grandTotal = uperGridTotal + lowerGridTotal;

            RetailTransacUiModel.GrandTotal = string.Format("{0:0.00}", grandTotal);
            GrandTotal = FormatStringToTwoDecimal(grandTotal);
        }

        private string RandomString(int size, bool lowerCase = false)
        {
            var builder = new StringBuilder(size);

            // char is a single Unicode character  
            char offset = lowerCase ? 'a' : 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }

        private int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        private string RandomInvoiceNumber()
        {
            var randomInvoiceNumber = new StringBuilder();

            // 4-Digits between 1000 and 9999  
            randomInvoiceNumber.Append(RandomNumber(1000, 9999));

            // 4-Letters upper case  
            randomInvoiceNumber.Append(RandomString(4));

            return randomInvoiceNumber.ToString();
        }

        private async Task ConfirmOrderButtonClickCommand()
        {
            if (string.IsNullOrWhiteSpace(RetailTransacUiModel.EmailTo))
            {
                await ProcessConfirmOrderFlow();
            }
            else if (IsEmailsValids())
            {
                await ProcessConfirmOrderFlow();
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please Enter valid email id", "OK");
            }
        }

        private bool IsEmailsValids()
        {
            try
            {
                Regex emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                var emails = RetailTransacUiModel.EmailTo.Split(',');
                bool isEmailValid = false;
                foreach (var email in emails)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var isValid = emailRegex.IsMatch(email);
                        if (!isValid)
                        {
                            isEmailValid = false;
                            break;
                        }
                        else
                        {
                            isEmailValid = true;
                        }
                    }
                }
                return isEmailValid;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(RetailTransactionPageViewModel), nameof(IsEmailsValids), ex.StackTrace);
                return false;
            }
        }
        private async Task ProcessConfirmOrderFlow()
        {
            try
            {
                if (AppReference.IsCreditRequestOrder.HasValue &&
                    AppReference.IsCreditRequestOrder.Value
                    )
                {
                    if (RetailTransacUiModel.OrderDetailsData != null && RetailTransacUiModel.OrderDetailsData.Any(x => x.Quantity > 0))
                    {
                        ContentDialog creditRequestCheckDialog = new ContentDialog
                        {
                            Content = "Please enter only -ve Quantity for this Order type",
                            CloseButtonText = ResourceLoader.GetForCurrentView().GetString("OK")
                        };
                        creditRequestCheckDialog.Closed += (ContentDialog sender, ContentDialogClosedEventArgs args)
                            => NavigationService.NavigateShellFrame(typeof(CartPage));
                        await creditRequestCheckDialog.ShowAsync();
                        return;
                    }
                }
                if (RetailTransacUiModel.SelectedSalesType.Equals("2") && (RetailTransacUiModel.DistributorsList == null || RetailTransacUiModel.DistributorsList?.Count == 0 || string.IsNullOrEmpty(RetailTransacUiModel.SelectedDistributorName)))
                {
                    ContentDialog noDistributorAddedDialog = new ContentDialog
                    {
                        Content = ResourceLoader.GetForCurrentView().GetString("SelectAddedDistributorMessage"),
                        CloseButtonText = ResourceLoader.GetForCurrentView().GetString("OK")
                    };

                    await noDistributorAddedDialog.ShowAsync();
                }
                else if (RetailTransacUiModel.SelectedSalesType.Equals("13") && (string.IsNullOrEmpty(RetailTransacUiModel.GrandTotal) || RetailTransacUiModel.GrandTotal.Equals("0.00")))
                {
                    ContentDialog zeroTransactionDialog = new ContentDialog
                    {
                        Content = "Can not proccess the $0 transaction using credit card. Please select different sales types to confirm the order.",
                        CloseButtonText = ResourceLoader.GetForCurrentView().GetString("OK")
                    };

                    await zeroTransactionDialog.ShowAsync();
                }
                else
                {
                    if (!string.IsNullOrEmpty(RetailTransacUiModel.PrintName) && !string.IsNullOrEmpty(RetailTransacUiModel.CustomerSignatureFileName))
                    {
                        if (RackOrderGridDataSource?.Count <= 0 && AppReference.CartDataFromScreen == 1)
                        {
                            await ShowPleaseAddRackPopUp();
                        }
                        else if (RackOrderGridDataSource?.Count <= 0 && AppReference.CartDataFromScreen == 2)
                        {
                            await ShowPleaseAddPopDialog();
                        }
                        else
                        {

                            RetailTransacUiModel.OrderDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);

                            if (string.IsNullOrEmpty(RetailTransacUiModel.PreBookShipDate))
                            {
                                RetailTransacUiModel.PreBookShipDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
                            }

                            if (!RetailTransacUiModel.SelectedSalesType.Equals("2"))
                            {
                                RetailTransacUiModel.CustomerDistributorId = 0;
                                RetailTransacUiModel.SelectedDistributorName = RetailTransacUiModel.RetailDistributorNumber = null;
                            }

                            if (AppReference.UpdatedCustomerIds == null)
                                AppReference.UpdatedCustomerIds = new List<string> { RetailTransacUiModel.CustomerData.DeviceCustomerID };
                            else AppReference.UpdatedCustomerIds.Append(RetailTransacUiModel.CustomerData.DeviceCustomerID);

                            if (RetailTransacUiModel.CustomerData.DeviceCustomerID != "0-0")
                            {
                                if (AppReference.UpdatedCustomerIds == null)
                                    AppReference.UpdatedCustomerIds = new List<string> { RetailTransacUiModel.CustomerData.DeviceCustomerID };
                                else if (!AppReference.UpdatedCustomerIds.Any(x => x == RetailTransacUiModel.CustomerData.DeviceCustomerID))
                                {
                                    AppReference.UpdatedCustomerIds.Append(RetailTransacUiModel.CustomerData.DeviceCustomerID);
                                }
                            }

                            CustomContentDialog customContentDialog = new CustomContentDialog();
                            customContentDialog.Title = resourceLoader.GetString("SelectInvoiceMethod");
                            customContentDialog.FirstButtonText = resourceLoader.GetString("PrintInvoice");
                            customContentDialog.SecondButtonText = resourceLoader.GetString("EmailInvoice");
                            customContentDialog.CancelButtonText = resourceLoader.GetString("OrderCancel");

                            await customContentDialog.ShowAsync();

                            // Print and Email Invoice button action
                            if (customContentDialog.Result == Result.Yes)
                            {
                                LoadingVisibilityHandler(true);

                                var tempInvoice = RandomInvoiceNumber();

                                RetailTransacUiModel.InvoiceNumber = string.IsNullOrEmpty(tempInvoice) ? "" : tempInvoice;

                                await AppReference.QueryService.InsertOrUpdateDataOnConfirmOrder(RetailTransacUiModel);

                                AppReference.CartItemCount = 0;
                                AppReference.CurrentOrderId = 0;
                                AppReference.CartDataFromScreen = 0;
                                AppReference.CurrentDeviceOrderId = string.Empty;
                                AppReference.IsCreditRequestOrder = false;
                                AppReference.IsOrderTypeChanged = false;
                                AppReference.IsDistributionOptionClicked = false;
                                AppReference.OrderPrintName = "";

                                await PrintOrderReceipt();
                            }
                            // Email Invoice button action
                            else if (customContentDialog.Result == Result.No)
                            {
                                LoadingVisibilityHandler(true);

                                var tempInvoice = RandomInvoiceNumber();

                                RetailTransacUiModel.InvoiceNumber = string.IsNullOrEmpty(tempInvoice) ? "" : tempInvoice;

                                await AppReference.QueryService.InsertOrUpdateDataOnConfirmOrder(RetailTransacUiModel);

                                AppReference.CartItemCount = 0;
                                AppReference.CurrentOrderId = 0;
                                AppReference.CartDataFromScreen = 0;
                                AppReference.CurrentDeviceOrderId = string.Empty;
                                AppReference.IsCreditRequestOrder = false;
                                AppReference.IsOrderTypeChanged = false;
                                AppReference.IsDistributionOptionClicked = false;
                                AppReference.OrderPrintName = "";

                                LoadingVisibilityHandler(false);

                                NavigationService.NavigateShellFrame(typeof(DashboardPage));
                            }

                        }
                    }
                    else
                    {
                        await ShowEmptyNameSignatureMessage();
                    }
                }
            }
            catch (Exception ex)
            {
                LoadingVisibilityHandler(false);

                ErrorLogger.WriteToErrorLog(GetType().Name, "ConfirmOrderButtonClickCommand", ex.StackTrace);
            }
        }
        private void DistributionSelectionCommandHandler(DistributorMaster obj)
        {
            try
            {
                var value = RetailTransacUiModel.DistributorsList?.Any(x => x.CustomerID == obj.CustomerID);

                if (value.HasValue && value.Value && obj != null)
                {
                    var isSelected = !obj.IsSelected;

                    (RetailTransacUiModel.DistributorsList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;

                    var selectedDistributor = RetailTransacUiModel.PrebookDistributorList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID);

                    if (selectedDistributor != null)
                    {
                        selectedDistributor.IsSelected = isSelected;

                        var selectedStateId = selectedDistributor.PhysicalAddressStateID;

                        RetailTransacUiModel.PrebookState.TryGetValue(selectedStateId, out string physicalStateValue);
                        RetailTransacUiModel.SelectedDistributorName = $"{selectedDistributor.DistributorID}, {selectedDistributor.StateName}";
                        if (!string.IsNullOrWhiteSpace(selectedDistributor?.AssignUserName))
                            RetailTransacUiModel.SelectedDistributorName += $" - {selectedDistributor.AssignUserName}";
                        RetailTransacUiModel.SelectedPrebookState = physicalStateValue ?? string.Empty;
                        RetailTransacUiModel.PrebookCity = selectedDistributor.PhysicalAddressCityID;
                        RetailTransacUiModel.PrebookZip = selectedDistributor.PhysicalAddressZipCode;
                        RetailTransacUiModel.CustomerDistributorId = selectedDistributor.CustomerID;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "DistributionSelectionCommandHandler", ex.StackTrace);
            }
        }

        private string FormatStringToTwoDecimal(decimal subTotalUpperGrid)
        {
            var formattedString = "$" + string.Format("{0:0.00}", subTotalUpperGrid);

            return formattedString;
        }

        private async Task SaveSignatureCommandHandler(string printName)
        {
            RetailTransacUiModel.SignaturePanelVisibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(RetailTransacUiModel.PrintName))
            {
                ContentDialog emptyFieldDialog = new ContentDialog
                {
                    Title = resourceLoader.GetString("ConfirmOrderErrorTitle"),
                    Content = resourceLoader.GetString("SignatureSaveError"),
                    CloseButtonText = resourceLoader.GetString("OK")
                };

                await emptyFieldDialog.ShowAsync();
            }
            else
            {
                RetailTransacUiModel.SignatureTextButtonVisibility = Visibility.Collapsed;
                RetailTransacUiModel.PrintName = printName;
                AppReference.OrderPrintName = printName;
                string path = AppReference.LocalFileService.GetLocalFilePathByFileType(Core.Interface.SrcZipFileType.Signature, RetailTransacUiModel.CustomerSignatureFileName);
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = path.Substring(path.IndexOf("LocalState") + 11);
                    // Open a stream for the selected file
                    Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                    StorageFile file = await storageFolder.GetFileAsync(fileName);

                    // Ensure a file was selected
                    if (file != null)
                    {
                        // Ensure the stream is disposed once the image is loaded
                        using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                        {
                            // Set the image source to the selected bitmap
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.DecodePixelHeight = 80;
                            bitmapImage.DecodePixelWidth = 230;

                            await bitmapImage.SetSourceAsync(fileStream);
                            RetailTransacUiModel.CustomerSignPath = bitmapImage;
                        }
                    }
                }
                else
                {
                    path = ""; RetailTransacUiModel.PrintName = path; RetailTransacUiModel.CustomerSignPath = null;
                    //RetailTransacUiModel.CustomerSignatureFileName = path;
                    RetailTransacUiModel.SignatureTextButtonVisibility = Visibility.Visible;
                }
                RetailTransacUiModel.CustomerSignatureFileName = path;


                if (RetailTransacUiModel.SelectedSalesType.Equals("13"))
                {
                    ContentDialog creditCardSalesDialog = new ContentDialog
                    {
                        Title = resourceLoader.GetString("ConfirmOrder"),
                        Content = resourceLoader.GetString("SquareAppMessage"),
                        CloseButtonText = resourceLoader.GetString("OK")
                    };

                    await creditCardSalesDialog.ShowAsync();
                }
            }
        }

        private void CancelSignatureActionHandler()
        {
            //RetailTransacUiModel.PrintName = string.Empty;

            RetailTransacUiModel.SignaturePanelVisibility = Visibility.Collapsed;
        }

        public void ShowSignaturePanel()
        {
            //RetailTransacUiModel.CustomerSignatureFileName = RetailTransacUiModel.OrderId + "_" + RetailTransacUiModel.CustomerData?.DeviceCustomerID + "_OrderSignature.jpg";
            if (string.IsNullOrWhiteSpace(RetailTransacUiModel.CustomerSignatureFileName))
                RetailTransacUiModel.CustomerSignatureFileName = $"{RetailTransacUiModel.DeviceOrderId}_OrderSignature.jpg";

            RetailTransacUiModel.SignaturePanelVisibility = Visibility.Visible;
        }

        public async Task ShowEmptyNameSignatureMessage()
        {
            ContentDialog emptyFieldDialog = new ContentDialog
            {
                Title = resourceLoader.GetString("ConfirmOrderErrorTitle"),
                Content = resourceLoader.GetString("ConfirmOrderErrorMessage"),
                PrimaryButtonText = resourceLoader.GetString("OK"),
                SecondaryButtonText = resourceLoader.GetString("CancelText")
            };

            ContentDialogResult result = await emptyFieldDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                emptyFieldDialog.Hide();

                ShowSignaturePanel();
            }
            else
            {
                emptyFieldDialog.Hide();
            }
        }

        public void ClearEmailToField()
        {
            if (!string.IsNullOrEmpty(RetailTransacUiModel.EmailTo) && RetailTransacUiModel.EmailTo.Equals(RetailTransacUiModel.UserEmailId))
            {
                RetailTransacUiModel.EmailTo = string.Empty;
            }
        }

        private void SetRackOrderGrid()
        {
            if (DbOrderDetailDataSource.Any())
            {
                RackOrderGridDataSource = new ObservableCollection<OrderDetailUIModel>(DbOrderDetailDataSource);
            }
            RetailTransacUiModel.RackOrderGridVisibility = (RackOrderGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Delete rack cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteImageClickedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            await ShowDeleteWarning(orderDetailUIModel);
        }

        private void AddCustomTaxStatementCommandHandler(string customTaxStatement)
        {
            RetailTransacUiModel.CustomTaxStatement = string.IsNullOrEmpty(customTaxStatement) ? string.Empty : customTaxStatement;
        }

        private void SetCustomTaxStatementCommandHandler(UserTaxStatement statement)
        {
            if (statement != null)
            {
                var selectedStatementDescription = statement.Description;

                if (!string.IsNullOrEmpty(RetailTransacUiModel.CustomTaxStatement))
                {
                    RetailTransacUiModel.CustomTaxStatement = string.Empty;
                }

                RetailTransacUiModel.CustomTaxStatement = selectedStatementDescription;
            }
            else
            {
                RetailTransacUiModel.CustomTaxStatement = string.Empty;
            }
        }

        /// <summary>
        /// Warning pop-up for deleting rack cart item
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task ShowDeleteWarning(OrderDetailUIModel orderDetailUIModel)
        {
            string warningTitle = string.Empty;
            if (AppReference.CartDataFromScreen == 1)
            {
                warningTitle = resourceLoader.GetString("RemoveRackOrComponent");
            }
            else if (AppReference.CartDataFromScreen == 2)
            {
                warningTitle = resourceLoader.GetString("RemovePopItem");
            }

            ContentDialog deleteCartItemDialog = new ContentDialog
            {
                Content = warningTitle,
                PrimaryButtonText = resourceLoader.GetString("YesCaps"),
                SecondaryButtonText = resourceLoader.GetString("NoCaps")
            };

            ContentDialogResult result = await deleteCartItemDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await DeleteCartItemAsync(orderDetailUIModel);
            }
            else
            {
                deleteCartItemDialog.Hide();
            }
        }

        /// <summary>
        /// Bl for deleting cart item.
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteCartItemAsync(OrderDetailUIModel orderDetailUIModel)
        {
            int CartValue = AppReference.CartItemCount;

            CartValue--;

            ((App)Application.Current).CartItemCount = CartValue;

            await DeleteItemFromDb(orderDetailUIModel);
            RackOrderGridDataSource.Remove(orderDetailUIModel);
            RetailTransacUiModel.RackOrderGridVisibility = (RackOrderGridDataSource.Count > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Delete item from database
        /// </summary>
        /// <param name="orderDetailUIModel"></param>
        private async Task DeleteItemFromDb(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();

            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };

            await ((App)Application.Current).QueryService.DeleteOrderDetail(_list[0].ProductId, AppReference.CurrentDeviceOrderId);
        }

        private void NumPadGridButtonClicked(string value)
        {
            UpdateQuantityFieldValue(RackOrderGridDataSource, value);
        }

        private void UpdateQuantityFieldValue(ObservableCollection<OrderDetailUIModel> gridDataSource, string value)
        {
            foreach (var item in gridDataSource)
            {
                if (item.ProductID == RetailTransacUiModel.OrderDetailModel?.ProductID)
                {
                    if (value.Equals("C"))
                    {
                        if (item?.QuantityDisplay?.Length > 0)
                        {
                            item.QuantityDisplay = item?.QuantityDisplay?.Remove(item.QuantityDisplay.Length - 1);
                            item.IsQuantityEdited = true;
                            quantityString = item.QuantityDisplay;
                        }
                    }
                    else
                    {
                        item.IsQuantityEdited = true;

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

                        //if (item?.QuantityDisplay?.Length == 0)
                        //{
                        //    if (value != "0" && value != "-")
                        //    {
                        //        item.QuantityDisplay = RetailTransacUiModel.OrderDetailModel?.QuantityDisplay + value;
                        //    }
                        //    else
                        //    {
                        //        item.QuantityDisplay = string.Empty;
                        //        item.IsQuantityEdited = true;
                        //    }
                        //}
                        //else if (item.QuantityDisplay.Length < 4 && (value != "-"))
                        //{
                        //    item.QuantityDisplay = RetailTransacUiModel.OrderDetailModel?.QuantityDisplay + value;
                        //}
                    }

                    if (!string.IsNullOrEmpty(item?.QuantityDisplay))
                    {
                        item.Quantity = Convert.ToInt32(item?.QuantityDisplay);
                    }
                }
            }
        }

        private async Task QuantityChangedAsync(OrderDetailUIModel orderDetailUIModel)
        {
            foreach (var item in RackOrderGridDataSource)
            {
                if (item.ProductID == orderDetailUIModel.ProductID)
                {
                    item.Quantity = orderDetailUIModel.Quantity;

                    await SaveValuesInDatabase(item);

                    break;
                }
            }
        }

        private async Task SaveValuesInDatabase(OrderDetailUIModel orderDetailUIModel)
        {
            orderDetailUIModel.OrderDetailUiToDataModel();
            var _list = new List<OrderDetail>() { orderDetailUIModel.OrderDetailData };
            _list[0].UpdatedDate = DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            await ((App)Application.Current).QueryService.InsertOrUpdateOrderDetail(_list[0]);
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

        private async Task ShowPleaseAddPopDialog()
        {

            ContentDialog pleaseAddPopDialog = new ContentDialog
            {
                Content = resourceLoader.GetString("PleaseAddPop"),
                CloseButtonText = resourceLoader.GetString("OK")
            };

            await pleaseAddPopDialog.ShowAsync();
        }

        private List<EmailAndPrintOrder.OrderDetails> ListOfOrderDetails()
        {
            List<EmailAndPrintOrder.OrderDetails> listOfDetails = new List<EmailAndPrintOrder.OrderDetails>();

            if ("5".Equals(RetailTransacUiModel.SelectedSalesType) || "10".Equals(RetailTransacUiModel.SelectedSalesType))
            {
                if (RackOrderGridDataSource != null && RackOrderGridDataSource.Count > 0)
                {
                    foreach (var item in RackOrderGridDataSource)
                    {
                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.ItemDescription,
                            ProductName = item.ItemNumber,
                            Quantity = item.Quantity,
                            RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                            SalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate,
                            TobaccoLicense = RetailTransacUiModel.StateTobaccoLicense,
                            RetailerLicense = RetailTransacUiModel.RetailerLicense,
                            IsTobaccoProduct = item.isTobbaco.ToString(),
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
            }
            else if ("8".Equals(RetailTransacUiModel.SelectedSalesType))
            {
                if (RtnGridDataSource != null && RtnGridDataSource.Count > 0)
                {
                    foreach (var rtn in RtnGridDataSource)
                    {
                        if (!string.IsNullOrEmpty(rtn.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(rtn.Price);

                            rtn.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = rtn.ItemDescription,
                            UOM = rtn.Unit,
                            Quantity = rtn.Quantity,
                            Price = Convert.ToDecimal(rtn.Price),
                            RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                            SalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate,
                            TobaccoLicense = RetailTransacUiModel.StateTobaccoLicense,
                            RetailerLicense = RetailTransacUiModel.RetailerLicense,
                            ReturnReason = rtn.CreditRequest,
                            ProductName = rtn.ItemNumber,
                            GrandTotal = Convert.ToDecimal(RetailTransacUiModel.GrandTotal),
                            RetailDistributorNumber = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.RetailDistributorNumber : string.Empty,
                            DistributorCity = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.PrebookCity : string.Empty,
                            DistributorID = RetailTransacUiModel.CustomerDistributorId > 0 ? Convert.ToString(RetailTransacUiModel.CustomerDistributorId) : string.Empty,
                            DistributorName = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedDistributorName : string.Empty,
                            DistributorState = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedPrebookState : string.Empty,
                            IsTobaccoProduct = "1"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }

                if (DifGridDataSource != null && DifGridDataSource.Count > 0)
                {
                    foreach (var dif in DifGridDataSource)
                    {
                        if (!string.IsNullOrEmpty(dif.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(dif.Price);

                            dif.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = dif.ItemDescription,
                            UOM = dif.Unit,
                            Quantity = dif.Quantity,
                            Price = Convert.ToDecimal(dif.Price),
                            RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                            SalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate,
                            TobaccoLicense = RetailTransacUiModel.StateTobaccoLicense,
                            RetailerLicense = RetailTransacUiModel.RetailerLicense,
                            ReturnReason = dif.CreditRequest,
                            ProductName = dif.ItemNumber,
                            GrandTotal = Convert.ToDecimal(RetailTransacUiModel.GrandTotal),
                            RetailDistributorNumber = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.RetailDistributorNumber : string.Empty,
                            DistributorCity = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.PrebookCity : string.Empty,
                            DistributorID = RetailTransacUiModel.CustomerDistributorId > 0 ? Convert.ToString(RetailTransacUiModel.CustomerDistributorId) : string.Empty,
                            DistributorName = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedDistributorName : string.Empty,
                            DistributorState = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedPrebookState : string.Empty,
                            IsTobaccoProduct = "0"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
            }
            else
            {
                if (TobaccoGridDataSource != null && TobaccoGridDataSource.Count > 0)
                {
                    foreach (var item in TobaccoGridDataSource)
                    {
                        if (!string.IsNullOrEmpty(item.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(item.Price);

                            item.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.ItemDescription,
                            UOM = item.Unit,
                            Quantity = item.Quantity,
                            Price = Convert.ToDecimal(item.Price),
                            RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                            SalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate,
                            TobaccoLicense = RetailTransacUiModel.StateTobaccoLicense,
                            RetailerLicense = RetailTransacUiModel.RetailerLicense,
                            ReturnReason = item.CreditRequest,
                            ProductName = item.ItemNumber,
                            GrandTotal = Convert.ToDecimal(RetailTransacUiModel.GrandTotal),
                            RetailDistributorNumber = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.RetailDistributorNumber : string.Empty,
                            DistributorCity = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.PrebookCity : string.Empty,
                            DistributorID = RetailTransacUiModel.CustomerDistributorId > 0 ? (RetailTransacUiModel.SelectedSalesType == "2" ? RetailTransacUiModel.SelectedDistributorName : Convert.ToString(RetailTransacUiModel.CustomerDistributorId)) : string.Empty,
                            DistributorName = RetailTransacUiModel.DistributorsList != null ? (RetailTransacUiModel.SelectedSalesType == "2" ? string.Empty : RetailTransacUiModel.SelectedDistributorName) : string.Empty,
                            DistributorState = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedPrebookState : string.Empty,
                            IsTobaccoProduct = "1"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }

                if (NonTobaccoGridDataSource != null && NonTobaccoGridDataSource.Count > 0)
                {
                    foreach (var item in NonTobaccoGridDataSource)
                    {
                        if (!string.IsNullOrEmpty(item.Price))
                        {
                            var decimalPoint = Convert.ToDecimal(item.Price);

                            item.Price = string.Format("{0:0.00}", decimalPoint.ToString());
                        }

                        EmailAndPrintOrder.OrderDetails emailOrderDetails = new EmailAndPrintOrder.OrderDetails()
                        {
                            Description = item.ItemDescription,
                            UOM = item.Unit,
                            Quantity = item.Quantity,
                            Price = Convert.ToDecimal(item.Price),
                            RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                            SalesTaxCertificate = RetailTransacUiModel.RetailerSalesTaxCertificate,
                            TobaccoLicense = RetailTransacUiModel.StateTobaccoLicense,
                            RetailerLicense = RetailTransacUiModel.RetailerLicense,
                            ReturnReason = item.CreditRequest,
                            ProductName = item.ItemNumber,
                            GrandTotal = Convert.ToDecimal(RetailTransacUiModel.GrandTotal),
                            RetailDistributorNumber = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.RetailDistributorNumber : string.Empty,
                            DistributorCity = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.PrebookCity : string.Empty,
                            DistributorID = RetailTransacUiModel.CustomerDistributorId > 0 ? (RetailTransacUiModel.SelectedSalesType == "2" ? RetailTransacUiModel.SelectedDistributorName : Convert.ToString(RetailTransacUiModel.CustomerDistributorId)) : string.Empty,
                            DistributorName = RetailTransacUiModel.DistributorsList != null ? (RetailTransacUiModel.SelectedSalesType == "2" ? string.Empty : RetailTransacUiModel.SelectedDistributorName) : string.Empty,
                            DistributorState = RetailTransacUiModel.DistributorsList != null ? RetailTransacUiModel.SelectedPrebookState : string.Empty,
                            IsTobaccoProduct = "0"
                        };
                        listOfDetails.Add(emailOrderDetails);
                    }
                }
            }

            return listOfDetails;
        }

        private EmailAndPrintOrder.CustomerInfo CustomerInfoForPrint()
        {
            EmailAndPrintOrder.CustomerInfo customerInfo = new EmailAndPrintOrder.CustomerInfo()
            {
                SalesRepresentative = RetailTransacUiModel.RepublicSalesRepName,
                Address = string.IsNullOrWhiteSpace(RetailTransacUiModel.Address) ? RetailTransacUiModel.CustomerData.PhysicalAddress : RetailTransacUiModel.Address,
                City = string.IsNullOrWhiteSpace(RetailTransacUiModel.CityName) ? RetailTransacUiModel.CustomerData.PhysicalAddressCityID : RetailTransacUiModel.CityName,
                ContactEmail = string.IsNullOrWhiteSpace(RetailTransacUiModel.EmailTo) ? RetailTransacUiModel.CustomerData.ContactEmail : RetailTransacUiModel.EmailTo,
                CustomerName = RetailTransacUiModel.CustomerData.CustomerName,
                CustomerNumber = RetailTransacUiModel.CustomerData.CustomerNumber,
                OrderDate = RetailTransacUiModel.OrderDate,
                PurchaseNumber = RetailTransacUiModel.InvoiceNumber,
                PONumber = RetailTransacUiModel.PurchaseOrderNumber,
                PermitNumber = RetailTransacUiModel.SellerRepresentativeTobaccoPermit,
                RegionID = RetailTransacUiModel.CustomerData.RegionId.ToString(),
                //Zip = RetailTransacUiModel.CustomerData.PhysicalAddressZipCode,
                Zip = RetailTransacUiModel.Zip,
                ContactPhone = RetailTransacUiModel.CustomerData.ContactPhone,
                //State = HelperMethods.GetValueFromIdNameDictionary(RetailTransacUiModel.States, RetailTransacUiModel.CustomerData.PhysicalAddressStateID),
                State = string.IsNullOrWhiteSpace(RetailTransacUiModel.SelectedPhysicalState) ? HelperMethods.GetValueFromIdNameDictionary(RetailTransacUiModel.States, RetailTransacUiModel.CustomerData.PhysicalAddressStateID) : RetailTransacUiModel.SelectedPhysicalState,
                ShipDate = RetailTransacUiModel.PreBookShipDate,
                CustomStatement = RetailTransacUiModel.CustomTaxStatement,
                CustomerComment = RetailTransacUiModel.RetailsSalesCallNotes,
                UserName = AppReference.LoginUserNameProperty,
                SalesRepEmail = RetailTransacUiModel.UserEmailId,
                SalesRepPhone = RetailTransacUiModel.UserPhone,
                SalesRepUsername = RetailTransacUiModel.LoggedInUsername,
                PrintName = RetailTransacUiModel.PrintName
            };

            return customerInfo;
        }

        private async Task PrintOrderReceipt()
        {
            try
            {
                var fileName = !string.IsNullOrEmpty(RetailTransacUiModel.InvoiceNumber) ? RetailTransacUiModel.InvoiceNumber : "Invoice";
                var folder = ApplicationData.Current.LocalFolder;
                var path = Path.Combine(folder.Path, string.Format("{0}_Receipt.pdf", fileName));

                var listOfDetails = ListOfOrderDetails();
                var customerInfo = CustomerInfoForPrint();
                PrintHelper.PreparePrintContent(new PrintOrderXAMLPage(listOfDetails, customerInfo, RetailTransacUiModel));
                await PrintHelper.ShowPrintUIAsync();
                DRLMobile.Uwp.Constants.Constants.NavigationFlagForRetailTransaction = true;
            }
            catch (Exception ex)
            {
                var errDescription = ex.Message + " " + ex.StackTrace + " " + ex.InnerException?.Message + " " + ex.InnerException;
                ErrorLogger.WriteToErrorLog(GetType().Name, "PrintOrderReceipt", errDescription);
                NavigationService.NavigateShellFrame(typeof(DRLMobile.Uwp.View.DashboardPage));
            }
            finally
            {
                LoadingVisibilityHandler(false);
            }
        }

        private void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SaleTypeCommandHandler(DropDownUIModel selectedItem)
        {
            if (selectedItem != null)
            {
                RetailTransacUiModel.ActivityType = selectedItem.Name;
                switch (((SalesType)selectedItem.Id))
                {
                    case SalesType.CashSale:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Cash Sale";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                    case SalesType.Prebook:
                        {
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Visible;
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Prebook";

                            if (RetailTransacUiModel.IsDirectCustomer)
                            {
                                RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            }
                            else
                            {
                                RetailTransacUiModel.DistributorVisibility = Visibility.Visible;

                                RetailTransacUiModel.CreateDistributorsCollection();
                                RetailTransacUiModel.PopulateStateNameForDistributorList();
                            }

                            ClearEmailToField();
                        }
                        break;
                    case SalesType.BillThrough:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Bill Through";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }

                        break;
                    case SalesType.SuggestedOrder:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Suggested Order";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                    case SalesType.CashSalesInitiative:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Cash Sales Initiative";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                    case SalesType.ChainDistribution:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Chain Distribution";

                            RetailTransacUiModel.EmailTo = RetailTransacUiModel.UserEmailId;

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                        }
                        break;
                    case SalesType.CreditCardSales:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Credit Card Sales";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                    case SalesType.DistributorInvoice:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Distributor Invoice";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                    default:
                        {
                            RetailTransacUiModel.SelectedSalesType = selectedItem.Id.ToString();
                            RetailTransacUiModel.ActivityType = "Car Stock Order";

                            RetailTransacUiModel.DistributorVisibility = Visibility.Collapsed;
                            RetailTransacUiModel.PrebookDateVisiblity = Visibility.Collapsed;

                            ClearEmailToField();
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
