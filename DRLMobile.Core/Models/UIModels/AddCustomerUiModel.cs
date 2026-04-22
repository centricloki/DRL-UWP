using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.DataModels;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace DRLMobile.Core.Models.UIModels
{
    public class AddCustomerUiModel : BaseModel
    {

        public IEnumerable<DistributorAssignUser> MainDistributorList { get; set; }
        public Dictionary<int, RegionMaster> RegionDictionary { get; set; }
        public Dictionary<int, ZoneMaster> ZoneDictionary { get; set; }

        public UserMaster LoggedInUser { get; set; }


        #region properties
        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
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

        private Dictionary<int, Classification> _classifications;
        public Dictionary<int, Classification> Classifications
        {
            get { return _classifications; }
            set { SetProperty(ref _classifications, value); }
        }

        private Dictionary<int, string> _ranks;
        public Dictionary<int, string> Ranks
        {
            get { return _ranks; }
            set { SetProperty(ref _ranks, value); }
        }

        private Dictionary<int, string> _states;
        public Dictionary<int, string> States
        {
            get { return _states; }
            set { SetProperty(ref _states, value); }
        }

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

        private string _generalComments;
        public string GeneralComments
        {
            get { return _generalComments; }
            set { SetProperty(ref _generalComments, value); }
        }

        private string _territoryName;
        public string TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }


        private string _numberOfLocations;
        public string NumberOfLocations
        {
            get { return _numberOfLocations; }
            set { SetProperty(ref _numberOfLocations, value); }
        }

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


        private string _salesTaxCertificate;
        public string SalesTaxCertificate
        {
            get { return _salesTaxCertificate; }
            set { SetProperty(ref _salesTaxCertificate, value); }
        }


        private ObservableCollection<DistributorAssignUser> __distributorList;
        public ObservableCollection<DistributorAssignUser> DistributorList
        {
            get { return __distributorList; }
            set { SetProperty(ref __distributorList, value); }
        }



        private ObservableCollection<DistributorAssignUser> _distributionFlyoutListItemSource;
        public ObservableCollection<DistributorAssignUser> DistributionFlyoutListItemSource
        {
            get { return _distributionFlyoutListItemSource; }
            set { SetProperty(ref _distributionFlyoutListItemSource, value); }
        }


        private RegionMaster _region;
        public RegionMaster Region
        {
            get { return _region; }
            set { SetProperty(ref _region, value); }
        }


        private ZoneMaster _zone;
        public ZoneMaster Zone
        {
            get { return _zone; }
            set { SetProperty(ref _zone, value); }
        }

        private ObservableCollection<TerritoryMaster> _territoryListItemSource;
        public ObservableCollection<TerritoryMaster> TerritoryListItemSource
        {
            get { return _territoryListItemSource; }
            set { SetProperty(ref _territoryListItemSource, value); }
        }

        private TerritoryMaster _selectedTerritory;
        public TerritoryMaster SelectedTerritory
        {
            get { return _selectedTerritory; }
            set { SetProperty(ref _selectedTerritory, value); ShowRegionAndZoneOntheBasisOfTerritory(); }
        }


        private bool _isTerritoryEditable;
        public bool IsTerritoryEditable
        {
            get { return _isTerritoryEditable; }
            set { SetProperty(ref _isTerritoryEditable, value); }
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

        #endregion


        #region constructor
        public AddCustomerUiModel()
        {
            DistributionFlyoutListItemSource = new ObservableCollection<DistributorAssignUser>();
            DistributorList = new ObservableCollection<DistributorAssignUser>();
            MainDistributorList = new List<DistributorAssignUser>();
        }


        public void PopulateUI()
        {
            //ShowRegionAndZoneOntheBasisOfTerritory();
            IsTerritoryEditable = LoggedInUser.RoleID != 1;
            PopulateStateNameForDistributorList();
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

        private void ShowRegionAndZoneOntheBasisOfTerritory()
        {
            if (SelectedTerritory != null)
            {
                RegionDictionary.TryGetValue(SelectedTerritory?.RegionID ?? 0, out RegionMaster region);
                Region = region ?? new RegionMaster();

                ZoneDictionary.TryGetValue(region?.ZoneID ?? 0, out ZoneMaster zone);
                Zone = zone?? new ZoneMaster();

            }
        }

        #endregion

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


    }
}
