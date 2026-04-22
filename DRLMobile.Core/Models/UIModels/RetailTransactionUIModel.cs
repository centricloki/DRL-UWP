using DRLMobile.Core.Models.DataModels;

using System.Collections.Generic;
using System.Collections.ObjectModel;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace DRLMobile.Core.Models.UIModels
{
    public class RetailTransactionUIModel : BaseModel
    {
        private CustomerMaster _customerData;
        public CustomerMaster CustomerData
        {
            get { return _customerData; }
            set { SetProperty(ref _customerData, value); }
        }

        private OrderDetailUIModel _orderDetailModel;
        public OrderDetailUIModel OrderDetailModel
        {
            get { return _orderDetailModel; }
            set { SetProperty(ref _orderDetailModel, value); }
        }

        private List<OrderDetailUIModel> _orderDetailsData;
        public List<OrderDetailUIModel> OrderDetailsData
        {
            get { return _orderDetailsData; }
            set { SetProperty(ref _orderDetailsData, value); }
        }

        private List<UserTaxStatement> _userTaxStatements;
        public List<UserTaxStatement> UserTaxStatementList
        {
            get { return _userTaxStatements; }
            set { SetProperty(ref _userTaxStatements, value); }
        }

        private int _orderId;
        public int OrderId
        {
            get { return _orderId; }
            set { SetProperty(ref _orderId, value); }
        }

        private string _deviceOrderId;
        public string DeviceOrderId
        {
            get { return _deviceOrderId; }
            set { SetProperty(ref _deviceOrderId, value); }
        }

        private int _customerId;
        public int CustomerId
        {
            get { return _customerId; }
            set { SetProperty(ref _customerId, value); }
        }

        private int _customerDistributorId;
        public int CustomerDistributorId
        {
            get { return _customerDistributorId; }
            set { SetProperty(ref _customerDistributorId, value); }
        }

        private bool _isDirectCustomer;
        public bool IsDirectCustomer
        {
            get { return _isDirectCustomer; }
            set { SetProperty(ref _isDirectCustomer, value); }
        }

        private string _transactionNumberText;
        public string TransactionNumberText
        {
            get { return _transactionNumberText; }
            set { SetProperty(ref _transactionNumberText, value); }
        }

        private string _transactionNumber;
        public string TransactionNumber
        {
            get { return _transactionNumber; }
            set { SetProperty(ref _transactionNumber, value); }
        }

        private string _invoiceNumber;
        public string InvoiceNumber
        {
            get { return _invoiceNumber; }
            set { SetProperty(ref _invoiceNumber, value); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get { return _accountNumber; }
            set { SetProperty(ref _accountNumber, value); }
        }

        private string _purchaseOrderNumber;
        public string PurchaseOrderNumber
        {
            get { return _purchaseOrderNumber; }
            set { SetProperty(ref _purchaseOrderNumber, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
        }

        private string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set { SetProperty(ref _cityName, value); }
        }

        private Dictionary<int, string> _states;
        public Dictionary<int, string> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }

        private string _zip;
        public string Zip
        {
            get { return _zip; }
            set { SetProperty(ref _zip, value); }
        }

        private string _tobaccoLicense;
        public string StateTobaccoLicense
        {
            get { return _tobaccoLicense; }
            set { SetProperty(ref _tobaccoLicense, value); }
        }


        private string _retailerLicense;
        public string RetailerLicense
        {
            get { return _retailerLicense; }
            set { SetProperty(ref _retailerLicense, value); }
        }

        private string _retailerSalesTaxCertificate;
        public string RetailerSalesTaxCertificate
        {
            get { return _retailerSalesTaxCertificate; }
            set { SetProperty(ref _retailerSalesTaxCertificate, value); }
        }

        private string _emailTo;
        public string EmailTo
        {
            get { return _emailTo; }
            set { SetProperty(ref _emailTo, value); }
        }

        private string _customTaxStatement;
        public string CustomTaxStatement
        {
            get { return _customTaxStatement; }
            set { SetProperty(ref _customTaxStatement, value); }
        }

        private string _salesCallNotes;
        public string RetailsSalesCallNotes
        {
            get { return _salesCallNotes; }
            set { SetProperty(ref _salesCallNotes, value); }
        }

        private string _republicSalesRepName;
        public string RepublicSalesRepName
        {
            get { return _republicSalesRepName; }
            set { SetProperty(ref _republicSalesRepName, value); }
        }

        private string _sellerRepresentativeTobaccoPermit;
        public string SellerRepresentativeTobaccoPermit
        {
            get { return _sellerRepresentativeTobaccoPermit; }
            set { SetProperty(ref _sellerRepresentativeTobaccoPermit, value); }
        }

        private string _prebookCity;
        public string PrebookCity
        {
            get { return _prebookCity; }
            set { SetProperty(ref _prebookCity, value); }
        }

        private Dictionary<int, string> _prebookState;
        public Dictionary<int, string> PrebookState
        {
            get { return _prebookState; }
            set { SetProperty(ref _prebookState, value); }
        }

        private string _prebookZip;
        public string PrebookZip
        {
            get { return _prebookZip; }
            set { SetProperty(ref _prebookZip, value); }
        }

        private List<DistributorAssignUser> _prebookDistributors;
        public List<DistributorAssignUser> PrebookDistributorList
        {
            get { return _prebookDistributors; }
            set { SetProperty(ref _prebookDistributors, value); }
        }

        private ObservableCollection<DistributorAssignUser> _distributors;
        public ObservableCollection<DistributorAssignUser> DistributorsList
        {
            get { return _distributors; }
            set { SetProperty(ref _distributors, value); }
        }

        private Visibility _signaturePanelVisibility = Visibility.Collapsed;
        public Visibility SignaturePanelVisibility
        {
            get { return _signaturePanelVisibility; }
            set { SetProperty(ref _signaturePanelVisibility, value); }
        }

        private Visibility _signatureTextButtonVisibility = Visibility.Visible;
        public Visibility SignatureTextButtonVisibility
        {
            get { return _signatureTextButtonVisibility; }
            set { SetProperty(ref _signatureTextButtonVisibility, value); }
        }

        private Visibility _customTaxStatementIconVisibility = Visibility.Collapsed;
        public Visibility CustomTaxStatementIconVisibility
        {
            get { return _customTaxStatementIconVisibility; }
            set { SetProperty(ref _customTaxStatementIconVisibility, value); }
        }

        private Visibility _prebookDateVisiblity = Visibility.Collapsed;
        public Visibility PrebookDateVisiblity
        {
            get { return _prebookDateVisiblity; }
            set { SetProperty(ref _prebookDateVisiblity, value); }
        }

        private string _printName;
        public string PrintName
        {
            get { return _printName; }
            set { SetProperty(ref _printName, value); }
        }

        private string _customerSignatureFileName;
        public string CustomerSignatureFileName
        {
            get { return _customerSignatureFileName; }
            set { SetProperty(ref _customerSignatureFileName, value); }
        }

        private BitmapImage _customerSignPath;
        public BitmapImage CustomerSignPath
        {
            get { return _customerSignPath; }
            set { SetProperty(ref _customerSignPath, value); }
        }

        private bool _isSignStarted;
        public bool IsSignStarted
        {
            get { return _isSignStarted; }
            set { SetProperty(ref _isSignStarted, value); }
        }

        private bool _isEmailValid;
        public bool IsEmailValid
        {
            get { return _isEmailValid; }
            set { SetProperty(ref _isEmailValid, value); }
        }

        private string _userEmailId;
        public string UserEmailId
        {
            get { return _userEmailId; }
            set { SetProperty(ref _userEmailId, value); }
        }

        private string _userPhone;
        public string UserPhone
        {
            get { return _userPhone; }
            set { SetProperty(ref _userPhone, value); }
        }

        private string _loggedInUsername;
        public string LoggedInUsername
        {
            get { return _loggedInUsername; }
            set { SetProperty(ref _loggedInUsername, value); }
        }

        private Visibility _OrderSalesOptionVisibility = Visibility.Collapsed;
        public Visibility OrderSalesOptionVisibility
        {
            get { return _OrderSalesOptionVisibility; }
            set { SetProperty(ref _OrderSalesOptionVisibility, value); }
        }

        private Visibility _sampleOrderOptionVisibility = Visibility.Collapsed;
        public Visibility SampleOrderOptionVisibility
        {
            get { return _sampleOrderOptionVisibility; }
            set { SetProperty(ref _sampleOrderOptionVisibility, value); }
        }

        private bool _isSampleOrder;
        public bool IsSampleOrder
        {
            get { return _isSampleOrder; }
            set { SetProperty(ref _isSampleOrder, value); }
        }

        private Visibility _rackOrderOptionVisibility = Visibility.Collapsed;
        public Visibility RackOrderOptionVisibility
        {
            get { return _rackOrderOptionVisibility; }
            set { SetProperty(ref _rackOrderOptionVisibility, value); }
        }

        private bool _isRackOrder;
        public bool IsRackOrder
        {
            get { return _isRackOrder; }
            set { SetProperty(ref _isRackOrder, value); }
        }

        private bool _isPopOrder;
        public bool IsPopOrder
        {
            get { return _isPopOrder; }
            set { SetProperty(ref _isPopOrder, value); }
        }

        private bool _isDistributionOrder;
        public bool IsDistributionOrder
        {
            get { return _isDistributionOrder; }
            set { SetProperty(ref _isDistributionOrder, value); }
        }

        private string _selectedPhysicalState;
        public string SelectedPhysicalState
        {
            get { return _selectedPhysicalState; }
            set { SetProperty(ref _selectedPhysicalState, value); }
        }

        private string _selectedPrebookState;
        public string SelectedPrebookState
        {
            get { return _selectedPrebookState; }
            set { SetProperty(ref _selectedPrebookState, value); }
        }

        private string _selectedDistributorName;
        public string SelectedDistributorName
        {
            get { return _selectedDistributorName; }
            set { SetProperty(ref _selectedDistributorName, value); }
        }

        private string _selectedSalesType;
        public string SelectedSalesType
        {
            get { return _selectedSalesType; }
            set { SetProperty(ref _selectedSalesType, value); }
        }

        private string _activityType;
        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType, value); }
        }

        private string _preBookShipDate;
        public string PreBookShipDate
        {
            get { return _preBookShipDate; }
            set { SetProperty(ref _preBookShipDate, value); }
        }

        private string _pageTitle;
        public string PageTitle
        {
            get { return _pageTitle; }
            set { SetProperty(ref _pageTitle, value); }
        }

        private string _grandTotal;
        public string GrandTotal
        {
            get { return _grandTotal; }
            set { SetProperty(ref _grandTotal, value); }
        }

        private bool _isCreditRequest;
        public bool IsCreditRequest
        {
            get { return _isCreditRequest; }
            set { SetProperty(ref _isCreditRequest, value); }
        }
        private bool _isCarStockOrder;
        public bool IsCarStockOrder
        {
            get { return _isCarStockOrder; }
            set { SetProperty(ref _isCarStockOrder, value); }
        }

        public string OrderDate { get; set; }

        public int LoggedInUserId { get; set; }

        private Visibility _tobaccoGridVisiblity = Visibility.Collapsed;
        public Visibility TobaccoGridVisibility
        {
            get { return _tobaccoGridVisiblity; }
            set { SetProperty(ref _tobaccoGridVisiblity, value); }
        }

        private Visibility _nonTobaccoGridVisiblity = Visibility.Collapsed;
        public Visibility NonTobaccoGridVisibility
        {
            get { return _nonTobaccoGridVisiblity; }
            set { SetProperty(ref _nonTobaccoGridVisiblity, value); }
        }

        private Visibility _rtnGridVisiblity = Visibility.Collapsed;
        public Visibility RtnGridVisibility
        {
            get { return _rtnGridVisiblity; }
            set { SetProperty(ref _rtnGridVisiblity, value); }
        }

        private Visibility _difGridVisiblity = Visibility.Collapsed;
        public Visibility DifGridVisibility
        {
            get { return _difGridVisiblity; }
            set { SetProperty(ref _difGridVisiblity, value); }
        }

        private Visibility _grandTotalVisiblity;
        public Visibility GrandTotalVisibility
        {
            get { return _grandTotalVisiblity; }
            set { SetProperty(ref _grandTotalVisiblity, value); }
        }

        private Visibility _rackOrderGridVisibility;
        public Visibility RackOrderGridVisibility
        {
            get { return _rackOrderGridVisibility; }
            set { SetProperty(ref _rackOrderGridVisibility, value); }
        }

        private string _commentsLabel;
        public string CommentsLabel
        {
            get { return _commentsLabel; }
            set { SetProperty(ref _commentsLabel, value); }
        }

        private string _retailDistributorNumber;
        public string RetailDistributorNumber
        {
            get { return _retailDistributorNumber; }
            set { SetProperty(ref _retailDistributorNumber, value); }
        }

        private ObservableCollection<DropDownUIModel> _saleType;
        public ObservableCollection<DropDownUIModel> SaleTypeCollection
        {
            get { return _saleType; }
            set { SetProperty(ref _saleType, value); }
        }
        private DropDownUIModel _chosenSaleType;
        public DropDownUIModel ChosenSaleType
        {
            get { return _chosenSaleType; }
            set { SetProperty(ref _chosenSaleType, value); }
        }
        private Visibility _distributorVisibility = Visibility.Collapsed;
        public Visibility DistributorVisibility
        {
            get { return _distributorVisibility; }
            set { SetProperty(ref _distributorVisibility, value); }
        }

        private void SetStateInformation()
        {
            States.TryGetValue((CustomerData.AccountType.Equals(1)? CustomerData.ShippingAddressStateID : CustomerData.PhysicalAddressStateID), out string physicalStateValue);
            SelectedPhysicalState = physicalStateValue ?? string.Empty;
        }

        public void SetSampleOrder()
        {
            if (!IsCreditRequest && !IsRackOrder && !IsPopOrder && !IsDistributionOrder && !IsCarStockOrder)
            {
                IsSampleOrder = !string.IsNullOrEmpty(CustomerData.CustomerNumber) && CustomerData.CustomerNumber.ToLower().StartsWith("x");

                if (IsSampleOrder)
                {
                    SampleOrderOptionVisibility = Visibility.Visible;
                }
                else
                {
                    SampleOrderOptionVisibility = Visibility.Collapsed;
                }
            }
        }

        public void SetPageTitle()
        {
            if (IsDirectCustomer)
            {
                if (IsCreditRequest)
                {
                    PageTitle = "Credit Request";
                }
                else
                {
                    PageTitle = "Distributor Order";
                }
            }
            else
            {
                PageTitle = "Retail Transaction";
            }
        }

        private void SetAccountAndPurchaseIdTextAndValue()
        {
            TransactionNumberText = IsDirectCustomer ? "Order #" : "Transaction #";

            TransactionNumber = CustomerData.DeviceCustomerID;

            AccountNumber = CustomerData.CustomerNumber;
        }

        public void CreateDistributorsCollection()
        {
            if (PrebookDistributorList != null && PrebookDistributorList.Count > 0)
            {
                DistributorsList = new ObservableCollection<DistributorAssignUser>(PrebookDistributorList);
            }
        }

        public void PopulateStateNameForDistributorList()
        {
            if (PrebookDistributorList != null && PrebookDistributorList.Count > 0)
            {
                foreach (var item in PrebookDistributorList)
                {
                    item.StateName = Helpers.HelperMethods.GetValueFromIdNameDictionary(PrebookState, id: item.PhysicalAddressStateID);
                }
            }
        }

        public void PopulateRetailTransactionUiModel()
        {
            OrderDetailModel = new OrderDetailUIModel();

            SetAccountAndPurchaseIdTextAndValue();
            SetStateInformation();

            CustomTaxStatementIconVisibility = UserTaxStatementList != null && UserTaxStatementList.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

            CustomTaxStatement = "All cigarette and tobacco product taxes are included in the total amount of this invoice.";
        }
    }
}