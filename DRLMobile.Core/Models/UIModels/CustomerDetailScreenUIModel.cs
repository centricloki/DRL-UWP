using DRLMobile.Core.Enums;
using DRLMobile.Core.Helpers;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;

using Microsoft.Toolkit.Mvvm.Input;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Core.Models.UIModels
{
    public class CustomerDetailScreenUIModel : BaseModel
    {
        #region Command
        public ICommand ShippingAddressCheckBoxCommand { get; private set; }
        public ICommand MailingAddressCheckBoxCommand { get; private set; }
        public ICommand FlyoutOpenCommand { get; private set; }
        public ICommand FlyoutTextChangedCommand { get; private set; }
        public ICommand DistributionSelectionCommand { get; private set; }
        public ICommand DeleteDistributorButtonCommand { get; private set; }
        public ICommand AddContactGridCommand { get; private set; }
        public ICommand RemoveContactCommand { get; private set; }
        #endregion

        #region Constructor

        public CustomerDetailScreenUIModel()
        {
            ContactListItemSource = new ObservableCollection<ContactMaster>();
            DistributorList = new ObservableCollection<DistributorAssignUser>();
            ShippingAddressCheckBoxCommand = new RelayCommand<bool>(ShippingAddressCheckBoxCommandHandler);
            MailingAddressCheckBoxCommand = new RelayCommand<bool>(MailingAddressCheckBoxCommandHandler);

            FlyoutTextChangedCommand = new RelayCommand<string>(FlyoutTextChangedCommandHandler);
            IsInEditMode = false;
            DistributionFlyoutListItemSource = new ObservableCollection<DistributorAssignUser>();
            DistributionSelectionCommand = new RelayCommand<DistributorAssignUser>(DistributionSelectionCommandHandler);
            DeleteDistributorButtonCommand = new RelayCommand<DistributorAssignUser>(DeleteDistributorButtonCommandHandler);
            AddContactGridCommand = new RelayCommand(AddContactGridCommandHandler);
            RemoveContactCommand = new RelayCommand<ContactMaster>(RemoveContactCommandHandler);
            ContactListItemSource.CollectionChanged -= ContactListCollectionChanged;
            ContactListItemSource.CollectionChanged += ContactListCollectionChanged;
            IsShippingAddressEditable = false;
            IsMailingAddressEditable = false;
        }

        #endregion

        #region Properties
        private ICollection<DistributorMaster> DeletedDistributors { get; set; }

        private string _mandatoryText;
        public string MandatoryText
        {
            get { return _mandatoryText; }
            set { SetProperty(ref _mandatoryText, value); HandleEditModeChange(); }
        }


        private bool _isInEditMode;
        public bool IsInEditMode
        {
            get { return _isInEditMode; }
            set { SetProperty(ref _isInEditMode, value); HandleEditModeChange(); }
        }

        private bool _isVisibleEditSaveButton;
        public bool IsVisibleEditSaveButton
        {
            get { return _isVisibleEditSaveButton; }
            set { SetProperty(ref _isVisibleEditSaveButton, value); }
        }

        private bool _isCheckedForSameAsPhysicalAddressShippingAddess;
        public bool IsCheckedForSameAsPhysicalAddressShippingAddess
        {
            get { return _isCheckedForSameAsPhysicalAddressShippingAddess; }
            set { SetProperty(ref _isCheckedForSameAsPhysicalAddressShippingAddess, value); }
        }

        private bool _isCheckedForSameAsPhysicalAddressMailingAddess;
        public bool IsCheckedForSameAsPhysicalAddressMailingAddess
        {
            get { return _isCheckedForSameAsPhysicalAddressMailingAddess; }
            set { SetProperty(ref _isCheckedForSameAsPhysicalAddressMailingAddess, value); }
        }

        private bool _isCustomerDetailsEditEnabled;
        public bool IsCustomerDetailsEditEnabled
        {
            get { return _isCustomerDetailsEditEnabled; }
            set { SetProperty(ref _isCustomerDetailsEditEnabled, value); }
        }

        private bool _isDirectCustomer;
        public bool IsDirectCustomer
        {
            get { return _isDirectCustomer; }
            set { SetProperty(ref _isDirectCustomer, value); }
        }

        private bool _isAssociatedCustomerAvailable;
        public bool IsAssociatedCustomerAvailable
        {
            get { return _isAssociatedCustomerAvailable; }
            set { SetProperty(ref _isAssociatedCustomerAvailable, value); }
        }

        private bool _isAssociatedCustomerShow;
        public bool IsAssociatedCustomerShow
        {
            get { return _isAssociatedCustomerShow; }
            set { SetProperty(ref _isAssociatedCustomerShow, value); }
        }

        private CustomerMaster _customerData;
        public CustomerMaster CustomerData
        {
            get { return _customerData; }
            set { SetProperty(ref _customerData, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        private string _accountType;
        public string AccountType
        {
            get { return _accountType; }
            set { SetProperty(ref _accountType, value); }
        }

        private Classification _selectedAccountClassification;
        public Classification SelectedAccountClassification
        {
            get { return _selectedAccountClassification; }
            set { SetProperty(ref _selectedAccountClassification, value); }
        }

        private string _selectedRank;
        public string SelectedRank
        {
            get { return _selectedRank; }
            set { SetProperty(ref _selectedRank, value); }
        }

        private string _generalComments;
        public string GeneralComments
        {
            get { return _generalComments; }
            set { SetProperty(ref _generalComments, value); }
        }

        #region drop down main

        private Dictionary<int, string> _states;
        public Dictionary<int, string> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }


        private Dictionary<int, string> _positions;
        public Dictionary<int, string> Positions
        {
            get { return _positions; }
            set { SetProperty(ref _positions, value); }
        }


        private Dictionary<int, string> _ranks;
        public Dictionary<int, string> Ranks
        {
            get { return _ranks; }
            set { SetProperty(ref _ranks, value); }
        }


        private Dictionary<int, Classification> _classifications;
        public Dictionary<int, Classification> Classifications
        {
            get { return _classifications; }
            set { SetProperty(ref _classifications, value); }
        }

        public ICollection<DistributorMaster> GetDeletedDristributorRecords()
        {
            //var tempList = new List<DistributorMaster>();
            //foreach (var item in DeletedDistributors)
            //{
            //    var isThere = DistributorList?.Any(x => x.CustomerID == item.CustomerID);
            //    if (isThere.HasValue && !isThere.Value)
            //    {
            //        tempList.Add(item);
            //    }
            //}
            //return tempList;
            return DeletedDistributors;
        }


        public List<ContactMaster> GetDeletedContactRecords()
        {
            List<ContactMaster> tempList = null;
            if (OnLoadContactList?.Count > 0)
            {
                tempList = new List<ContactMaster>();

                foreach (var item in OnLoadContactList)
                {
                    var isThere = ContactListItemSource?.Any(x => x.ContactID == item?.ContactID);

                    if (isThere.HasValue && !isThere.Value)
                    {
                        tempList.Add(item);
                    }
                }
            }
            return tempList;
        }
        #endregion

        #region Physical Address Details
        private string _physicalAddress;
        public string PhysicalAddress
        {
            get { return _physicalAddress; }
            set { SetProperty(ref _physicalAddress, value); HandleAddressChanges(AddressChanged.ADDRESS); }
        }

        private string _physicalCity;
        public string PhysicalCity
        {
            get { return _physicalCity; }
            set { SetProperty(ref _physicalCity, value); HandleAddressChanges(AddressChanged.CITY); }
        }

        private string _selectedPhysicalState;
        public string SelectedPhysicalState
        {
            get { return _selectedPhysicalState; }
            set { SetProperty(ref _selectedPhysicalState, value); HandleAddressChanges(AddressChanged.STATE); }
        }


        private string _physicalZip;
        public string PhysicalZip
        {
            get { return _physicalZip; }
            set { SetProperty(ref _physicalZip, value); HandleAddressChanges(AddressChanged.ZIP); }
        }


        private string _physicalContactPersonName;

        public string PhysicalContactPersonName
        {
            get { return _physicalContactPersonName; }
            set { SetProperty(ref _physicalContactPersonName, value); }
        }


        private string _physicalContactPersonRole;

        public string PhysicalContactPersonRole
        {
            get { return _physicalContactPersonRole; }
            set { SetProperty(ref _physicalContactPersonRole, value); }
        }


        private string _physicalContactEmail;

        public string PhysicalContactEmail
        {
            get { return _physicalContactEmail; }
            set { SetProperty(ref _physicalContactEmail, value); }
        }

        private string _physicalContactPhone;

        public string PhysicalContactPhone
        {
            get { return _physicalContactPhone; }
            set { SetProperty(ref _physicalContactPhone, value); }
        }

        private string _physicalContactFax;

        public string PhysicalContactFax
        {
            get { return _physicalContactFax; }
            set { SetProperty(ref _physicalContactFax, value); }
        }

        #endregion

        #region Shipping Address

        private string _shippingAddress;

        public string ShippingAddress
        {
            get { return _shippingAddress; }
            set { SetProperty(ref _shippingAddress, value); }
        }


        private string _shippingCity;

        public string ShippingCity
        {
            get { return _shippingCity; }
            set { SetProperty(ref _shippingCity, value); }
        }


        private string _selectedShippingState;

        public string SelectedShippingState
        {
            get { return _selectedShippingState; }
            set { SetProperty(ref _selectedShippingState, value); }
        }


        private string _shippingZip;

        public string ShippingZip
        {
            get { return _shippingZip; }
            set { SetProperty(ref _shippingZip, value); }
        }




        #endregion

        #region Mailing Address

        private string _mailingAddress;
        public string MailingAddress
        {
            get { return _mailingAddress; }
            set { SetProperty(ref _mailingAddress, value); }
        }


        private string _mailingCity;
        public string MailingCity
        {
            get { return _mailingCity; }
            set { SetProperty(ref _mailingCity, value); }
        }


        private string _selectedMailingState;
        public string SelectedMailingState
        {
            get { return _selectedMailingState; }
            set { SetProperty(ref _selectedMailingState, value); }
        }


        private string _mailingZip;
        public string MailingZip
        {
            get { return _mailingZip; }
            set { SetProperty(ref _mailingZip, value); }
        }




        #endregion

        #region Distributor list

        private ObservableCollection<DistributorAssignUser> __distributorList;
        public ObservableCollection<DistributorAssignUser> DistributorList
        {
            get { return __distributorList; }
            set { SetProperty(ref __distributorList, value); }
        }
        private bool _isChainPlaybookAvailable;
        public bool IsChainPlaybookAvailable
        {
            get { return _isChainPlaybookAvailable; }
            set { SetProperty(ref _isChainPlaybookAvailable, value); }
        }
        private ProductDetailUiModel _chainPlayBookProduct;
        public ProductDetailUiModel ChainPlayBookProduct
        {
            get { return _chainPlayBookProduct; }
            set { SetProperty(ref _chainPlayBookProduct, value); }
        }

        #endregion

        #region travel vrip visibility

        private Visibility _travelMainVisibility;

        public Visibility TravelMainVisibility
        {
            get { return _travelMainVisibility; }
            set { SetProperty(ref _travelMainVisibility, value); }
        }


        private Visibility _vripMainVisibility;

        public Visibility VripMainVisibility
        {
            get { return _vripMainVisibility; }
            set { SetProperty(ref _vripMainVisibility, value); }
        }

        #endregion

        #region names
        private string _keyAccountManagerName;
        public string KeyAccountManagerName
        {
            get { return _keyAccountManagerName; }
            set { SetProperty(ref _keyAccountManagerName, value); }
        }

        private string _payerName;

        public string PayerName
        {
            get { return _payerName; }
            set { SetProperty(ref _payerName, value); }
        }


        private string _regionName;

        public string RegionName
        {
            get { return _regionName; }
            set { SetProperty(ref _regionName, value); }
        }


        private string _zoneName;

        public string ZoneName
        {
            get { return _zoneName; }
            set { SetProperty(ref _zoneName, value); }
        }


        private string _territoryManagerName;
        public string TerritoryManagerName
        {
            get { return _territoryManagerName; }
            set { SetProperty(ref _territoryManagerName, value); }
        }


        private string _regionManagerName;
        public string RegionManagerName
        {
            get { return _regionManagerName; }
            set { SetProperty(ref _regionManagerName, value); }
        }


        private string _zoneManagerName;
        public string ZoneManagerName
        {
            get { return _zoneManagerName; }
            set { SetProperty(ref _zoneManagerName, value); }
        }

        private string _avpName;
        public string AVPName
        {
            get { return _avpName; }
            set { SetProperty(ref _avpName, value); }
        }

        private string _bdManagerName;
        public string BDManagerName
        {
            get { return _bdManagerName; }
            set { SetProperty(ref _bdManagerName, value); }
        }

        private string _territoryName;
        public string TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }


        #endregion

        #region number of location
        private string _numberOfLocations;
        public string NumberOfLocations
        {
            get { return _numberOfLocations; }
            set { SetProperty(ref _numberOfLocations, value); }
        }

        #endregion

        #region license
        private string _tobaccoLicense;
        public string TobaccoLicense
        {
            get { return _tobaccoLicense; }
            set { SetProperty(ref _tobaccoLicense, value); }
        }


        private string _retailLicense;
        public string RetailLicense
        {
            get { return _retailLicense; }
            set { SetProperty(ref _retailLicense, value); }
        }


        #endregion

        #region sales tax

        private string _salesTaxCertificate;
        public string SalesTaxCertificate
        {
            get { return _salesTaxCertificate; }
            set { SetProperty(ref _salesTaxCertificate, value); }
        }
        #endregion

        #region travel

        public VripTravelData TravelPoints { get; set; }

        private string _earnedPoints;

        public string EarnedPoints
        {
            get { return _earnedPoints; }
            set { SetProperty(ref _earnedPoints, value); }
        }



        private string _netPoints;

        public string NetPoints
        {
            get { return _netPoints; }
            set { SetProperty(ref _netPoints, value); }
        }


        private string _bonousPoints;

        public string BonousPoints
        {
            get { return _bonousPoints; }
            set { SetProperty(ref _bonousPoints, value); }
        }


        private string _needNextPoints;

        public string NeedNextPoints
        {
            get { return _needNextPoints; }
            set { SetProperty(ref _needNextPoints, value); }
        }

        #endregion

        #region vrip
        public VripTravelData VripPoints { get; set; }


        private string _travelYtdQty;
        public string TravelYtdQty
        {
            get { return _travelYtdQty; }
            set { SetProperty(ref _travelYtdQty, value); }
        }


        private string _travelTargetQty;
        public string TravelTargetQty
        {
            get { return _travelTargetQty; }
            set { SetProperty(ref _travelTargetQty, value); }
        }

        private string _travelNeedQty;
        public string TravelNeedQty
        {
            get { return _travelNeedQty; }
            set { SetProperty(ref _travelNeedQty, value); }
        }


        private string _travelPreviousYear;
        public string TravelPreviousYear
        {
            get { return _travelPreviousYear; }
            set { SetProperty(ref _travelPreviousYear, value); }
        }

        #endregion

        #region input validation flags

        private bool _isEmailValid;
        public bool IsEmailValid
        {
            get { return _isEmailValid; }
            set { SetProperty(ref _isEmailValid, value); }
        }


        #endregion

        #region contact

        private bool _isAddContactIcon;
        public bool IsAddContactIcon
        {
            get { return _isAddContactIcon; }
            set { SetProperty(ref _isAddContactIcon, value); }
        }


        public List<ContactMaster> OnLoadContactList { get; set; }


        private ObservableCollection<ContactMaster> _contactListItemSource;

        public ObservableCollection<ContactMaster> ContactListItemSource
        {
            get { return _contactListItemSource; }
            set { SetProperty(ref _contactListItemSource, value); }
        }


        private Visibility _contactMainVisibility;

        public Visibility ContactMainVisibility
        {
            get { return _contactMainVisibility; }
            set { SetProperty(ref _contactMainVisibility, value); }
        }

        #endregion

        #region dristributors
        public IEnumerable<DistributorAssignUser> OnLoadDistributorList { get; set; }
        public IEnumerable<DistributorAssignUser> MainDistributorList { get; set; }


        private ObservableCollection<DistributorAssignUser> _distributionFlyoutListItemSource;
        public ObservableCollection<DistributorAssignUser> DistributionFlyoutListItemSource
        {
            get { return _distributionFlyoutListItemSource; }
            set { SetProperty(ref _distributionFlyoutListItemSource, value); }
        }

        private bool _isDisrtibutorsVisible;
        public bool IsDisrtibutorsVisible
        {
            get { return _isDisrtibutorsVisible; }
            set { SetProperty(ref _isDisrtibutorsVisible, value); }
        }


        #endregion

        #region enable disable

        private bool _isShippingAddressEditable;

        public bool IsShippingAddressEditable
        {
            get { return _isShippingAddressEditable; }
            set { SetProperty(ref _isShippingAddressEditable, value); }
        }


        private bool _isMailingAddressEditable;

        public bool IsMailingAddressEditable
        {
            get { return _isMailingAddressEditable; }
            set { SetProperty(ref _isMailingAddressEditable, value); }
        }


        #endregion

        #region documents
        private ObservableCollection<CustomerDocumentUIModel> _customerDocuments;
        public ObservableCollection<CustomerDocumentUIModel> CustomerDocuments
        {
            get { return _customerDocuments; }
            set { SetProperty(ref _customerDocuments, value); }
        }
        #endregion


        private string _associatedCustomerLabelText;
        public string AssociatedCustomerLabelText
        {
            get { return _associatedCustomerLabelText; }
            set { SetProperty(ref _associatedCustomerLabelText, value); }
        }
        private bool _isAnyAssociatedCustomerAvailable;
        public bool IsAnyAssociatedCustomerAvailable
        {
            get { return _isAnyAssociatedCustomerAvailable; }
            set { SetProperty(ref _isAnyAssociatedCustomerAvailable, value); }
        }

        private bool _isPopUpOpenOfAssociatedCustomer;
        public bool IsPopUpOpenOfAssociatedCustomer
        {
            get { return _isPopUpOpenOfAssociatedCustomer; }
            set { SetProperty(ref _isPopUpOpenOfAssociatedCustomer, value); }
        }

        private ObservableCollection<CustomerListControlUIModel> _associatedCustomerList;
        public ObservableCollection<CustomerListControlUIModel> AssociatedCustomerList
        {
            get { return _associatedCustomerList; }
            set { SetProperty(ref _associatedCustomerList, value); }
        }
        #endregion

        #region Private Methods

        private void SetMailingAddress()
        {
            MailingAddress = CustomerData?.MailingAddress;
            MailingCity = CustomerData?.MailingAddressCityID;
            States.TryGetValue(CustomerData.MailingAddressStateID, out string mailingStateValue);
            SelectedMailingState = mailingStateValue ?? string.Empty;
            MailingZip = CustomerData.MailingAddressZipID;

            if (!string.IsNullOrEmpty(PhysicalAddress) && PhysicalAddress.Equals(MailingAddress) &&
              !string.IsNullOrEmpty(PhysicalCity) && PhysicalCity.Equals(MailingCity) &&
              !string.IsNullOrEmpty(SelectedPhysicalState) && SelectedPhysicalState.Equals(SelectedMailingState) &&
              !string.IsNullOrEmpty(PhysicalZip) && PhysicalZip.Equals(MailingZip))
            {
                IsCheckedForSameAsPhysicalAddressMailingAddess = true;
                IsMailingAddressEditable = false;
            }
            else
            {
                if (IsCustomerDetailsEditEnabled)
                    IsMailingAddressEditable = true;
                else
                    IsMailingAddressEditable = false;
            }
        }

        public void SetMailingEditable()
        {
            if (IsCheckedForSameAsPhysicalAddressMailingAddess)
            {
                IsMailingAddressEditable = false;
            }
            else
            {
                if (IsCustomerDetailsEditEnabled)
                    IsMailingAddressEditable = true;
                else
                    IsMailingAddressEditable = false;
            }
        }

        private void SetShippingAddress()
        {
            ShippingAddress = CustomerData?.ShippingAddress;
            ShippingCity = CustomerData?.ShippingAddressCityID;
            States.TryGetValue(CustomerData.ShippingAddressStateID, out string shippingStateValue);
            SelectedShippingState = shippingStateValue ?? string.Empty;
            ShippingZip = CustomerData.ShippingAddressZipCode;


            if (!string.IsNullOrEmpty(PhysicalAddress) && PhysicalAddress.Equals(ShippingAddress) &&
                !string.IsNullOrEmpty(PhysicalCity) && PhysicalCity.Equals(ShippingCity) &&
                !string.IsNullOrEmpty(SelectedPhysicalState) && SelectedPhysicalState.Equals(SelectedShippingState) &&
                !string.IsNullOrEmpty(PhysicalZip) && PhysicalZip.Equals(ShippingZip))
            {
                IsCheckedForSameAsPhysicalAddressShippingAddess = true;
                IsShippingAddressEditable = false;
            }
            else
            {
                if (IsCustomerDetailsEditEnabled)
                    IsShippingAddressEditable = true;
                else
                    IsShippingAddressEditable = false;
            }

        }

        public void SetShippingEditable()
        {
            if (IsCheckedForSameAsPhysicalAddressShippingAddess)
                IsShippingAddressEditable = false;
            else
            {
                if (IsCustomerDetailsEditEnabled)
                    IsShippingAddressEditable = true;
                else
                    IsShippingAddressEditable = false;
            }
        }

        private void SetPhysicalAddress()
        {
            PhysicalZip = CustomerData?.PhysicalAddressZipCode;
            PhysicalContactPersonName = CustomerData?.ContactName;
            PhysicalContactPersonRole = CustomerData?.ContactRole;
            PhysicalContactEmail = CustomerData?.ContactEmail;
            PhysicalContactPhone = CustomerData?.Phone;
            PhysicalContactFax = CustomerData?.Fax;
        }

        private void GetVripData()
        {
            if (VripPoints != null)
            {
                TravelYtdQty = CustomerData.Csytd;
                TravelTargetQty = CustomerData.Target;
                TravelNeedQty = CustomerData.CSNeeded;
                TravelPreviousYear = CustomerData.TravelYear;
            }
        }

        private void HandleContactVisibilityAndPopulateForDirectCustomer()
        {
            if (CustomerData?.AccountType == 1 && OnLoadContactList != null)
            {
                this.IsVisibleEditSaveButton = false;
                for (int index = 0; index < OnLoadContactList.Count; index++)
                {
                    var contact = OnLoadContactList[index];
                    contact.IsInEditMode = false;
                    contact.Ranks = Ranks;
                    contact.Positions = Positions;
                    contact.SelectedRank = Helpers.HelperMethods.GetValueFromIdNameDictionary(Ranks, contact.RankID);
                    contact.SelectedPosition = Helpers.HelperMethods.GetValueFromIdNameDictionary(Positions, contact.PositionID);
                    contact.PopulateUI();
                    ContactListItemSource.Add(contact);
                }
            }
            else
            {
                this.IsVisibleEditSaveButton = true;
            }
        }

        private void HandleMainVisibilityAsPerCustomerType()
        {
            if (CustomerData?.AccountType == 2)
            {
                //indirect customer
                ContactMainVisibility = Visibility.Collapsed;
                TravelMainVisibility = Visibility.Collapsed;
                VripMainVisibility = Visibility.Collapsed;
                IsDisrtibutorsVisible = true;
                MandatoryText = "*";
            }
            else
            {
                ContactMainVisibility = Visibility.Visible;
                TravelMainVisibility = Visibility.Visible;
                VripMainVisibility = Visibility.Visible;
                IsDisrtibutorsVisible = false;
                MandatoryText = "";
            }
        }

        private void GetPayerOrParentName(int accountType)
        {
            if (accountType == 2)
            {
                //indirect customer
                PayerName = string.IsNullOrWhiteSpace(CustomerData?.PayerName) ? CustomerData?.ParentName : CustomerData?.PayerName;
            }
            else
            {
                if (CustomerData.IsParent == 1)
                {
                    PayerName = CustomerData.ParentName;
                }
            }
        }

        private void MailingAddressCheckBoxCommandHandler(bool isChecked)
        {
            if (isChecked)
            {
                MailingAddress = PhysicalAddress;
                MailingCity = PhysicalCity;
                SelectedMailingState = SelectedPhysicalState;
                MailingZip = PhysicalZip;
                IsMailingAddressEditable = false;
            }
            else
            {
                MailingAddress = string.Empty;
                MailingCity = string.Empty;
                SelectedMailingState = null;
                MailingZip = string.Empty;
                IsMailingAddressEditable = true;
            }
        }

        private void ShippingAddressCheckBoxCommandHandler(bool isChecked)
        {
            if (isChecked)
            {
                ShippingAddress = PhysicalAddress;
                ShippingCity = PhysicalCity;
                SelectedShippingState = SelectedPhysicalState;
                ShippingZip = PhysicalZip;

                IsShippingAddressEditable = false;
            }
            else
            {
                ShippingAddress = string.Empty;
                ShippingCity = string.Empty;
                SelectedShippingState = null;
                ShippingZip = string.Empty;
                IsShippingAddressEditable = true;
            }
        }

        public void DistributorDoneCommandHandler()
        {
            var tempList = MainDistributorList.Where(x => x.IsSelected);
            DistributorList = new ObservableCollection<DistributorAssignUser>(tempList);
            DistributionFlyoutListItemSource.Clear();
            HandleEditModeChange();
        }

        private void FlyoutTextChangedCommandHandler(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 3)
                DistributionFlyoutListItemSource.Clear();
            else
            {
                var tempList = MainDistributorList.Where(x => x.CustomerName.ToLower().Contains(text.ToLower())).ToList();
                tempList.ForEach(x =>
                {
                    if (DistributorList.Any(y => y.CustomerID == x.CustomerID))
                    {
                        x.IsSelected = true;
                    }
                    else
                    {
                        x.IsSelected = false;
                    }

                });
                DistributionFlyoutListItemSource = new ObservableCollection<DistributorAssignUser>(tempList);
            }
        }

        private void DistributionSelectionCommandHandler(DistributorAssignUser obj)
        {
            var value = DistributionFlyoutListItemSource?.Any(x => x.CustomerID == obj.CustomerID);
            if (value.HasValue && value.Value && obj != null)
            {
                var isSelected = !obj.IsSelected;
                (DistributionFlyoutListItemSource?.FirstOrDefault(x => x.CustomerID == obj.CustomerID)).IsSelected = isSelected;
                DistributorAssignUser mainDistributor = MainDistributorList?.FirstOrDefault(x => x.CustomerID == obj.CustomerID);
                if (mainDistributor != null)
                {
                    mainDistributor.IsSelected = isSelected;
                    if (!mainDistributor.IsSelected)
                    {
                        DistributorAssignUser distributorAssignUser = DistributorList?.FirstOrDefault(x => x.CustomerID == mainDistributor.CustomerID);
                        if (distributorAssignUser != null)
                        {
                            DistributorList.Remove(distributorAssignUser);
                            if (DeletedDistributors == null) DeletedDistributors = new List<DistributorMaster> { distributorAssignUser };
                            else DeletedDistributors.Add(distributorAssignUser);
                        }

                    }
                }
            }
        }

        private void HandleEditModeChange()
        {
            if (IsInEditMode)
            {
                if (CustomerData.AccountType == 2)
                {
                    HandlerDistributorDeleteVisibility(Visibility.Visible);
                    IsCustomerDetailsEditEnabled = true;
                }
                else
                {
                    IsCustomerDetailsEditEnabled = false;
                }
            }
            else
            {
                HandlerDistributorDeleteVisibility(Visibility.Collapsed);
                foreach (var item in ContactListItemSource)
                {
                    item.IsInEditMode = false;
                }
                IsCustomerDetailsEditEnabled = false;
                IsAddContactIcon = false;
            }
        }

        private void HandlerDistributorDeleteVisibility(Visibility visibility)
        {
            foreach (var item in DistributorList)
            {
                item.IsDeleteIconVisibile = visibility;
            }
        }

        private void PopulateStateNameForDistributorList()
        {
            if (MainDistributorList != null)
            {
                foreach (var item in MainDistributorList)
                {
                    item.StateName = Helpers.HelperMethods.GetValueFromIdNameDictionary(States, id: item.PhysicalAddressStateID);
                }
            }
        }

        private void SetIsDirectCustomer()
        {
            IsDirectCustomer = CustomerData.AccountType != 2;
        }

        private void DeleteDistributorButtonCommandHandler(DistributorAssignUser distributor)
        {
            try
            {
                if (DeletedDistributors == null) DeletedDistributors = new List<DistributorMaster> { distributor };
                else DeletedDistributors.Add(distributor);

                DistributorList?.Remove(distributor);

                MainDistributorList.FirstOrDefault(x => x.CustomerID == distributor.CustomerID).IsSelected = false;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(CustomerDetailScreenUIModel), nameof(DeleteDistributorButtonCommandHandler), ex);
            }
        }

        private void CheckAlreadySelectedDistributors()
        {
            if (DistributorList != null && MainDistributorList != null)
            {
                foreach (var item in MainDistributorList)
                {
                    if (DistributorList.Any(y => y.CustomerID == item.CustomerID))
                    {
                        item.IsSelected = true;
                    }
                }
            }
        }

        private void SetLicenseAndTaxInfo()
        {
            TobaccoLicense = CustomerData?.StateTobaccoLicense;
            RetailLicense = CustomerData?.RetailerLicense;
            SalesTaxCertificate = CustomerData?.RetailerSalesTaxCertificate;
        }

        private void SetCustomerGeneralInformation()
        {
            CustomerName = CustomerData?.CustomerName;
            AccountType = CustomerData.AccountType == 2 ? "Indirect" : "Direct";
            SelectedRank = CustomerData?.Rank;
            PhysicalAddress = CustomerData?.PhysicalAddress;
            PhysicalCity = CustomerData?.PhysicalAddressCityID;
            NumberOfLocations = CustomerData?.StoreCount;
            GeneralComments = CustomerData?.GeneralComments;
            KeyAccountManagerName = CustomerData?.KeyAccountManagerName;

            States.TryGetValue(CustomerData.PhysicalAddressStateID, out string physicalStateValue);
            SelectedPhysicalState = physicalStateValue ?? string.Empty;
        }

        private void AddContactGridCommandHandler()
        {
            ContactListItemSource.Add(new ContactMaster() { IsInEditMode = IsInEditMode, Ranks = Ranks, Positions = Positions });
        }

        private void GetTravelData()
        {
            if (TravelPoints != null)
            {
                EarnedPoints = CustomerData.EarnedPoints;
                BonousPoints = CustomerData.BonusPoints;
                NetPoints = CustomerData.NetPoints;
                NeedNextPoints = CustomerData.NeededPoint;
            }
        }

        private void ContactListCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (IsInEditMode)
            {
                if (ContactListItemSource.Count == 3)
                    IsAddContactIcon = false;
                else
                    IsAddContactIcon = true;
            }
        }

        private void RemoveContactCommandHandler(ContactMaster contact)
        {
            if (OnLoadContactList?.Count > 0)
            {
                if (!OnLoadContactList.Any(x => x.ContactID == contact.ContactID)) OnLoadContactList.Add(contact);
            }
            else
            {
                OnLoadContactList = new List<ContactMaster>();
                OnLoadContactList.Add(contact);
            }
            ContactListItemSource?.Remove(contact);
        }

        private void HandleAddressChanges(AddressChanged type)
        {
            switch (type)
            {
                case AddressChanged.ADDRESS:
                    if (IsCheckedForSameAsPhysicalAddressMailingAddess)
                    {
                        MailingAddress = PhysicalAddress;
                    }
                    if (IsCheckedForSameAsPhysicalAddressShippingAddess)
                    {
                        ShippingAddress = PhysicalAddress;
                    }
                    break;

                case AddressChanged.CITY:
                    if (IsCheckedForSameAsPhysicalAddressMailingAddess)
                    {
                        MailingCity = PhysicalCity;
                    }
                    if (IsCheckedForSameAsPhysicalAddressShippingAddess)
                    {
                        ShippingCity = PhysicalCity;
                    }
                    break;

                case AddressChanged.STATE:
                    if (IsCheckedForSameAsPhysicalAddressMailingAddess)
                    {
                        SelectedMailingState = SelectedPhysicalState;
                    }
                    if (IsCheckedForSameAsPhysicalAddressShippingAddess)
                    {
                        SelectedShippingState = SelectedPhysicalState;
                    }
                    break;

                case AddressChanged.ZIP:
                    if (IsCheckedForSameAsPhysicalAddressMailingAddess)
                    {
                        MailingZip = PhysicalZip;
                    }
                    if (IsCheckedForSameAsPhysicalAddressShippingAddess)
                    {
                        ShippingZip = PhysicalZip;
                    }
                    break;
            }
        }

        #endregion

        #region Public Methods
        public void PopulateUiModel()
        {
            HandleMainVisibilityAsPerCustomerType();
            SetCustomerGeneralInformation();
            SetPhysicalAddress();
            SetShippingAddress();
            SetMailingAddress();
            GetPayerOrParentName(CustomerData.AccountType);
            SetLicenseAndTaxInfo();
            GetTravelData();
            GetVripData();
            HandleContactVisibilityAndPopulateForDirectCustomer();
            PopulateStateNameForDistributorList();
            HandleEditModeChange();
            CheckAlreadySelectedDistributors();
            SetIsDirectCustomer();


        }

        public void SetContactForUpdate()
        {
            foreach (var item in ContactListItemSource)
            {
                item.ContactName = item.DisplayContactName;
                item.ContactEmail = item.DisplayContactEmail;
                item.ContactPhone = item.DisplayContactPhone;
                item.ContactFax = item.DisplayContactFax;
            }
        }

        public void SetUpdateCustomerObject()
        {
            CustomerData.CustomerName = CustomerName;
            CustomerData.AccountClassification = SelectedAccountClassification?.AccountClassificationId.ToString();
            CustomerData.Rank = SelectedRank;
            CustomerData.PhysicalAddress = PhysicalAddress;
            CustomerData.PhysicalAddressCityID = PhysicalCity;
            CustomerData.PhysicalAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(States, SelectedPhysicalState);
            CustomerData.PhysicalAddressZipCode = PhysicalZip;
            CustomerData.ContactName = PhysicalContactPersonName;
            CustomerData.ContactRole = PhysicalContactPersonRole;
            CustomerData.ContactEmail = PhysicalContactEmail;
            CustomerData.ContactRole = PhysicalContactPersonRole;
            CustomerData.Phone = PhysicalContactPhone;
            CustomerData.Fax = PhysicalContactFax;
            CustomerData.ShippingAddress = ShippingAddress;
            CustomerData.ShippingAddressCityID = ShippingCity;
            CustomerData.ShippingAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(States, SelectedShippingState);
            CustomerData.ShippingAddressZipCode = ShippingZip;

            CustomerData.MailingAddress = MailingAddress;
            CustomerData.MailingAddressCityID = MailingCity;
            CustomerData.MailingAddressStateID = HelperMethods.GetKeyFromIdNameDictionary(States, SelectedMailingState);
            CustomerData.MailingAddressZipID = MailingZip;

            CustomerData.StoreCount = NumberOfLocations;

            CustomerData.StateTobaccoLicense = TobaccoLicense;
            CustomerData.RetailerLicense = RetailLicense;
            CustomerData.RetailerSalesTaxCertificate = SalesTaxCertificate;

            CustomerData.GeneralComments = GeneralComments;

            CustomerData.IsExported = 0;

            //Set isDeleted as 1, if account classification is 'Out Of Business (38)'
            CustomerData.isDeleted = SelectedAccountClassification?.AccountClassificationId == 38 ? 1 : 0;
        }

        public void HandleContactEditForDirectCustomer()
        {
            if (CustomerData.AccountType == 1)
            {
                IsAddContactIcon = ContactListItemSource.Count != 3;
                foreach (var item in ContactListItemSource)
                {
                    item.IsInEditMode = !item.IsInEditMode;
                }
            }
        }

        #endregion
    }
}