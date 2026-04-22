using DevExpress.Mvvm.Native;
using Microsoft.Toolkit.Mvvm.Input;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Interface;
using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.Helpers;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using DRLMobile.Uwp.Services;
using DRLMobile.Uwp.View;

namespace DRLMobile.Uwp.ViewModel
{
    public class MapPageViewModel : BaseModel
    {
        #region Fields
        private readonly App AppReference = (App)(Application.Current);
        private readonly CoreDispatcher coreDispatcher;
        private IMapsStaticDataSourceHelper StaticDataSourceHelper;
        private IEnumerable<ZoneMasterUIModel> DbZoneMasterList;
        private IEnumerable<RegionMasterUIModel> DbRegionMasterList;
        private IEnumerable<TerritoryMasterUIModel> DbTerritoryList;
        private UserMaster loggedInUser;
        private MapFilter PlotByCurrentFilter;
        public IEnumerable<MapCustomerData> DbMapCustomerData;
        private DateTime TodaysDate;
        public IDictionary<int, string> DbStateDict;
        public IList<CityMasterUIModel> DbCityList;
        //public List<Classification> AccountClassificationsList;
        public MapCustomerData SelectedCustomerForPopup;

        public RandomAccessStreamReference[] numberIconReferences = new RandomAccessStreamReference[8];
        public List<PointOfInterest> places = new List<PointOfInterest>();
        public int previousZoomLevel = 0;

        #endregion

        #region Commands
        ///public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand OnMapLegendItemClickCommand { get; private set; }
        public ICommand OnPlotByFilterItemClickCommand { get; private set; }
        public ICommand ItemSearchHeaderTextChangeCommand { get; private set; }
        public ICommand StateSearchHeaderTextChangeCommand { get; private set; }
        public ICommand CitySearchHeaderTextChangeCommand { get; private set; }
        public ICommand ZoneSearchHeaderTextChangeCommand { get; private set; }
        public ICommand RegionSearchHeaderTextChangeCommand { get; private set; }
        public ICommand TerritorySearchHeaderTextChangeCommand { get; private set; }
        public ICommand StateSelectionCommand { get; private set; }
        public ICommand CitySelectionCommand { get; private set; }
        public ICommand ClearFilterCommand { get; private set; }
        public ICommand ItemSelectionCommand { get; private set; }
        public ICommand ZoneSelectionCommand { get; private set; }
        public ICommand RegionSelectionCommand { get; private set; }
        public ICommand TerritorySelectionCommand { get; private set; }
        public ICommand FilterApplyCommand { get; private set; }
        public ICommand CloseCustomMapPushPinCommand { get; private set; }
        public ICommand AdvanceGoogleMapPageCommand { get; private set; }
        #endregion

        #region Properties

        private string _displayStartDate;
        public string DisplayStartDate
        {
            get { return _displayStartDate; }
            set { SetProperty(ref _displayStartDate, value); }
        }

        private string _displayEndDate;
        public string DisplayEndDate
        {
            get { return _displayEndDate; }
            set { SetProperty(ref _displayEndDate, value); }
        }

        private bool _isDateFilterDescriptionRowVisible;

        public bool IsDateFilterDescriptionRowVisible
        {
            get { return _isDateFilterDescriptionRowVisible; }
            set { SetProperty(ref _isDateFilterDescriptionRowVisible, value); }
        }


        private DateTimeOffset? _selectedStartDate;
        public DateTimeOffset? SelectedStartDate
        {
            get { return _selectedStartDate; }
            set { SetProperty(ref _selectedStartDate, value); }
        }

        private DateTimeOffset? _selectedEndDate;
        public DateTimeOffset? SelectedEndDate
        {
            get { return _selectedEndDate; }
            set { SetProperty(ref _selectedEndDate, value); }
        }

        private DateTimeOffset? _filterSelectedStartDate;
        public DateTimeOffset? FilterSelectedStartDate
        {
            get { return _filterSelectedStartDate; }
            set { SetProperty(ref _filterSelectedStartDate, value); }
        }

        private DateTimeOffset? _filterSelectedEndDate;
        public DateTimeOffset? FilterSelectedEndDate
        {
            get { return _filterSelectedEndDate; }
            set { SetProperty(ref _filterSelectedEndDate, value); }
        }

        private ZoneMasterUIModel _selectedZone;
        public ZoneMasterUIModel SelectedZone
        {
            get { return _selectedZone; }
            set { SetProperty(ref _selectedZone, value); }
        }


        private RegionMasterUIModel _selectedRegion;
        public RegionMasterUIModel SelectedRegion
        {
            get { return _selectedRegion; }
            set { SetProperty(ref _selectedRegion, value); }
        }

        private TerritoryMasterUIModel _selectedTerritory;
        public TerritoryMasterUIModel SelectedTerritory
        {
            get { return _selectedTerritory; }
            set { SetProperty(ref _selectedTerritory, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private ObservableCollection<PlotByTypeFilterUIModel> _plotByFilterSource;
        public ObservableCollection<PlotByTypeFilterUIModel> PlotByFilterSource
        {
            get { return _plotByFilterSource; }
            set { SetProperty(ref _plotByFilterSource, value); }
        }

        private ObservableCollection<CityMasterUIModel> _cityList;
        public ObservableCollection<CityMasterUIModel> CityList
        {
            get { return _cityList; }
            set { SetProperty(ref _cityList, value); }
        }

        private ObservableCollection<ZoneMasterUIModel> _zoneList;
        public ObservableCollection<ZoneMasterUIModel> ZoneList
        {
            get { return _zoneList; }
            set { SetProperty(ref _zoneList, value); }
        }

        private ObservableCollection<RegionMasterUIModel> _regionList;
        public ObservableCollection<RegionMasterUIModel> RegionList
        {
            get { return _regionList; }
            set { SetProperty(ref _regionList, value); }
        }


        private ObservableCollection<TerritoryMasterUIModel> _territoryList;
        public ObservableCollection<TerritoryMasterUIModel> TerritoryList
        {
            get { return _territoryList; }
            set { SetProperty(ref _territoryList, value); }
        }


        private ObservableCollection<MapsLegendFilterUIModel> _legendsFilterSource;
        public ObservableCollection<MapsLegendFilterUIModel> LegendsFilterSource
        {
            get { return _legendsFilterSource; }
            set { SetProperty(ref _legendsFilterSource, value); }
        }

        private Dictionary<int, string> _stateDictionary;
        public Dictionary<int, string> StateDictionary
        {
            get { return _stateDictionary; }
            set { SetProperty(ref _stateDictionary, value); }
        }

        //private ObservableCollection<PointOfInterest> _pointOfIntrestSource;
        //public ObservableCollection<PointOfInterest> PointOfIntrestSource
        //{
        //    get { return _pointOfIntrestSource; }
        //    set { SetProperty(ref _pointOfIntrestSource, value); }
        //}

        public ICollection<PointOfInterest> PointOfIntrestSource { get; set; }

        private ObservableCollection<ProductMasterUIModel> _productList;
        public ObservableCollection<ProductMasterUIModel> ProductList
        {
            get { return _productList; }
            set { SetProperty(ref _productList, value); }
        }
        private Geopoint _center;
        public Geopoint Center
        {
            get { return _center; }
            set { SetProperty(ref _center, value); }
        }


        private string _legendsLabelText;
        public string LegendsLabelText
        {
            get { return _legendsLabelText; }
            set { SetProperty(ref _legendsLabelText, value); }
        }


        private CityMasterUIModel _selectedCity;
        public CityMasterUIModel SelectedCity
        {
            get { return _selectedCity; }
            set { SetProperty(ref _selectedCity, value); }
        }

        private CityMasterUIModel _filterSelectedCity;
        public CityMasterUIModel FilterSelectedCity
        {
            get { return _filterSelectedCity; }
            set { SetProperty(ref _filterSelectedCity, value); }
        }


        private KeyValuePair<int, string>? _selectedState;
        public KeyValuePair<int, string>? SelectedState
        {
            get { return _selectedState; }
            set { SetProperty(ref _selectedState, value); }
        }

        private KeyValuePair<int, string>? _filterSelectedState;
        public KeyValuePair<int, string>? FilterSelectedState
        {
            get { return _filterSelectedState; }
            set { SetProperty(ref _filterSelectedState, value); }
        }

        private bool _isZoneDropDownEnabled;
        public bool IsZoneDropDownEnabled
        {
            get { return _isZoneDropDownEnabled; }
            set { SetProperty(ref _isZoneDropDownEnabled, value); }
        }

        private bool _isNationalHead;
        public bool IsNationalHead
        {
            get { return _isNationalHead; }
            set { SetProperty(ref _isNationalHead, value); }
        }


        private bool _isRegionDropDownEnabled;
        public bool IsRegionDropDownEnabled
        {
            get { return _isRegionDropDownEnabled; }
            set { SetProperty(ref _isRegionDropDownEnabled, value); }
        }


        private bool _isTerritoryDropDownEnabled;
        public bool IsTerritoryDropDownEnabled
        {
            get { return _isTerritoryDropDownEnabled; }
            set { SetProperty(ref _isTerritoryDropDownEnabled, value); }
        }

        private bool _isStateDropDownEnabled;
        public bool IsStateDropDownEnabled
        {
            get { return _isStateDropDownEnabled; }
            set { SetProperty(ref _isStateDropDownEnabled, value); }
        }

        private bool _isCityDropDownEnabled;
        public bool IsCityDropDownEnabled
        {
            get { return _isCityDropDownEnabled; }
            set { SetProperty(ref _isCityDropDownEnabled, value); }
        }


        private bool _isStartEndDateEnabled;
        public bool IsStartEndDateEnabled
        {
            get { return _isStartEndDateEnabled; }
            set { SetProperty(ref _isStartEndDateEnabled, value); }
        }

        private bool _isItemDropDownEnabled;
        public bool IsItemDropDownEnabled
        {
            get { return _isItemDropDownEnabled; }
            set { SetProperty(ref _isItemDropDownEnabled, value); }
        }


        private ZoneMasterUIModel _filterSelectedZone;
        public ZoneMasterUIModel FilterSelectedZone
        {
            get { return _filterSelectedZone; }
            set { SetProperty(ref _filterSelectedZone, value); }
        }


        private RegionMasterUIModel _filterSelectedRegion;
        public RegionMasterUIModel FilterSelectedRegion
        {
            get { return _filterSelectedRegion; }
            set { SetProperty(ref _filterSelectedRegion, value); }
        }


        private TerritoryMasterUIModel _filterSelectedTerritory;
        public TerritoryMasterUIModel FilterSelectedTerritory
        {
            get { return _filterSelectedTerritory; }
            set { SetProperty(ref _filterSelectedTerritory, value); }
        }


        private ProductMasterUIModel _filterSelectedItem;
        public ProductMasterUIModel FilterSelectedItem
        {
            get { return _filterSelectedItem; }
            set { SetProperty(ref _filterSelectedItem, value); }
        }

        private ProductMasterUIModel _selectedItem;
        public ProductMasterUIModel SelectedItem
        {
            get { return _selectedItem; }
            set { SetProperty(ref _selectedItem, value); }
        }

        private Visibility _customMapPinVisibility = Visibility.Collapsed;
        public Visibility CustomMapPinVisibility
        {
            get { return _customMapPinVisibility; }
            set { SetProperty(ref _customMapPinVisibility, value); }
        }

        #endregion

        #region Constructor
        public MapPageViewModel()
        {
            RegisterCommands();
            StaticDataSourceHelper = new MapsStaticDataSourceHelper();
            PlotByFilterSource = new ObservableCollection<PlotByTypeFilterUIModel>();
            StateDictionary = new Dictionary<int, string>();
            CityList = new ObservableCollection<CityMasterUIModel>();
            //PointOfIntrestSource = new ObservableCollection<PointOfInterest>();
            PointOfIntrestSource = new List<PointOfInterest>();
            ProductList = new ObservableCollection<ProductMasterUIModel>();
            //AccountClassificationsList = new List<Classification>();
            coreDispatcher = Windows.ApplicationModel.Core.CoreApplication
                       .MainView.CoreWindow.Dispatcher;
        }
        #endregion

        #region Private methods
        private void RegisterCommands()
        {
            /// OnNavigatedToCommand = new AsyncRelayCommand(OnNavigatedToCommandHandler);
            OnMapLegendItemClickCommand = new AsyncRelayCommand<MapsLegendFilterUIModel>(OnMapLegendItemClickCommandHandler);
            OnPlotByFilterItemClickCommand = new AsyncRelayCommand<PlotByTypeFilterUIModel>(OnPlotByFilterItemClickCommandHandler);
            ItemSearchHeaderTextChangeCommand = new AsyncRelayCommand<string>(ItemSearchHeaderTextChangeCommandHandler);
            StateSearchHeaderTextChangeCommand = new RelayCommand<string>(StateSearchHeaderTextChangeCommandHandler);
            CitySearchHeaderTextChangeCommand = new RelayCommand<string>(CitySearchHeaderTextChangeCommandHandler);
            StateSelectionCommand = new RelayCommand<KeyValuePair<int, string>>(StateSelectionCommandHandler);
            CitySelectionCommand = new RelayCommand<CityMasterUIModel>(CitySelectionCommandHandler);
            ClearFilterCommand = new RelayCommand(ClearFilterCommandHandler);
            ItemSelectionCommand = new RelayCommand<ProductMasterUIModel>(ItemSelectionCommandHandler);
            ZoneSearchHeaderTextChangeCommand = new RelayCommand<string>(ZoneSearchHeaderTextChangeCommandHandler);
            RegionSearchHeaderTextChangeCommand = new RelayCommand<string>(RegionSearchHeaderTextChangeCommandHandler);
            TerritorySearchHeaderTextChangeCommand = new RelayCommand<string>(TerritorySearchHeaderTextChangeCommandHandler);
            ZoneSelectionCommand = new RelayCommand<ZoneMasterUIModel>(ZoneSelectionCommandHandler);
            RegionSelectionCommand = new RelayCommand<RegionMasterUIModel>(RegionSelectionCommandHandler);
            TerritorySelectionCommand = new RelayCommand<TerritoryMasterUIModel>(TerritorySelectionCommandHandler);
            FilterApplyCommand = new AsyncRelayCommand(FilterApplyCommandHandler);
            CloseCustomMapPushPinCommand = new RelayCommand(CloseCustomMapPushPinCommandHandler);
            AdvanceGoogleMapPageCommand = new RelayCommand(() => NavigationService.NavigateShellFrame(typeof(GoogleMapPage)));
        }

        private void CloseCustomMapPushPinCommandHandler()
        {
            CustomMapPinVisibility = Visibility.Collapsed;
        }

        public async Task FilterApplyCommandHandler()
        {
            if (CustomMapPinVisibility == Visibility.Visible)
            { CustomMapPinVisibility = Visibility.Collapsed; }

            //validations
            bool isValid = false;
            bool isFilterApplied = false;

            if (IsStartEndDateEnabled)
            {
                if (FilterSelectedStartDate != null || FilterSelectedEndDate != null)
                {
                    isValid = await IfItIsAValidFilter();

                    if (isValid)
                    {
                        isFilterApplied = true;
                        await ApplyFilterAndFetchData();
                    }
                }
                else
                {
                    isValid = true;
                }
            }

            if (IsItemDropDownEnabled && isValid && !isFilterApplied)
            {
                isValid = await IfItIsAValidFilterForitemNo();

                if (isValid)
                {
                    await ApplyFilterAndFetchData();
                }
            }

            if (!IsStartEndDateEnabled && !IsItemDropDownEnabled)
            {
                await ApplyFilterAndFetchData();
            }

            if (IsStartEndDateEnabled && !IsItemDropDownEnabled && isValid && !isFilterApplied)
            {
                await ApplyFilterAndFetchData();
            }
        }

        private async Task ApplyFilterAndFetchData()
        {
            if (FilterSelectedZone != null) SelectedZone = new ZoneMasterUIModel { ZoneID = FilterSelectedZone.ZoneID, ZoneName = FilterSelectedZone.ZoneName };
            else
            {
                if (SelectedZone != null) SelectedZone.ZoneName = string.Empty;
                SelectedZone = null;
            }
            if (FilterSelectedRegion != null) SelectedRegion = new RegionMasterUIModel { ZoneID = FilterSelectedRegion.ZoneID, Regioname = FilterSelectedRegion.Regioname, RegionID = FilterSelectedRegion.RegionID };
            else
            {
                if (SelectedRegion != null) SelectedRegion.Regioname = string.Empty;
                SelectedRegion = null;
            }

            if (FilterSelectedTerritory != null) SelectedTerritory = new TerritoryMasterUIModel { RegionID = FilterSelectedTerritory.RegionID, TerritoryID = FilterSelectedTerritory.TerritoryID, TerritoryName = FilterSelectedTerritory.TerritoryName };
            else
            {
                if (SelectedTerritory != null) SelectedTerritory.TerritoryName = string.Empty;
                SelectedTerritory = null;
            }
            if (FilterSelectedState.HasValue) SelectedState = new KeyValuePair<int, string>(FilterSelectedState.Value.Key, FilterSelectedState.Value.Value);
            else
            {
                SelectedState = new KeyValuePair<int, string>();
                SelectedState = null;
            }

            if (FilterSelectedCity != null) SelectedCity = new CityMasterUIModel { CityID = FilterSelectedCity.CityID, CityName = FilterSelectedCity.CityName, StateID = FilterSelectedCity.StateID };
            else
            {
                if (SelectedCity != null) SelectedCity.CityName = string.Empty;
                SelectedCity = null;
            }

            if (FilterSelectedStartDate.HasValue && FilterSelectedEndDate.HasValue)
            {
                SelectedStartDate = FilterSelectedStartDate;
                SelectedEndDate = FilterSelectedEndDate;
                IsDateFilterDescriptionRowVisible = true;
                DisplayStartDate = SelectedStartDate.Value.DateTime.ToString(Core.Helpers.DateTimeHelper.USDateFormat, new CultureInfo("en-US"));
                DisplayEndDate = SelectedEndDate.Value.DateTime.ToString(Core.Helpers.DateTimeHelper.USDateFormat, new CultureInfo("en-US"));
            }
            else
            {
                IsDateFilterDescriptionRowVisible = false;
                SelectedStartDate = null;
                SelectedEndDate = null;
            }
            if (FilterSelectedItem != null)
            {
                SelectedItem = new ProductMasterUIModel { Description = FilterSelectedItem.Description, ProductID = FilterSelectedItem.ProductID, ProductName = FilterSelectedItem.ProductName };
            }
            else
            {
                if (SelectedItem != null)
                {
                    SelectedItem.ProductName = string.Empty;
                    SelectedItem.Description = string.Empty;
                }
                SelectedItem = null;
            }

            await GetMapDataOnBasisOfPlotByFilter(true);
        }

        private async Task<bool> IfItIsAValidFilter()
        {
            if (FilterSelectedStartDate != null && FilterSelectedEndDate != null)
            {
                if (FilterSelectedStartDate < FilterSelectedEndDate)
                {
                    return true;
                }
                else
                {
                    await ShowStartGreaterThanEndDateAlert();
                    return false;
                }
            }
            else if (FilterSelectedStartDate == null && FilterSelectedEndDate == null)
            {
                return true;
            }
            else
            {
                if (FilterSelectedStartDate == null)
                {
                    await ShowNoStartDateAlert();
                }
                else if (FilterSelectedEndDate == null)
                {
                    await ShowNoEndDateAlert();
                }
                return false;
            }
        }

        private async Task<bool> IfItIsAValidFilterForitemNo()
        {
            if (FilterSelectedItem != null)
            {
                return true;
            }
            else
            {
                await ShowNoItemAlert();
                return false;
            }
        }

        private Task ShowNoEndDateAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please select end date.", "OK");
        }
        private Task ShowNoStartDateAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please select start date.", "OK");
        }
        private Task ShowNoItemAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please enter Item No.", "OK");
        }
        private Task ShowStartGreaterThanEndDateAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "End Date should be greater than Start Date.", "OK");
        }
        private void TerritorySelectionCommandHandler(TerritoryMasterUIModel territory)
        {
            if (territory != null)
            {
                if (territory.TerritoryID == 0 && "None".Equals(territory.TerritoryName))
                {
                    FilterSelectedTerritory = new TerritoryMasterUIModel { TerritoryName = string.Empty };
                    FilterSelectedTerritory = null;
                }
                else
                {
                    FilterSelectedTerritory = new TerritoryMasterUIModel { RegionID = territory.RegionID, TerritoryName = territory?.TerritoryName, TerritoryID = territory.TerritoryID };
                }
            }
        }

        private void RegionSelectionCommandHandler(RegionMasterUIModel region)
        {
            if (region != null)
            {
                if (region.RegionID == 0 && "None".Equals(region.Regioname))
                {
                    FilterSelectedRegion = new RegionMasterUIModel { Regioname = string.Empty };
                    FilterSelectedRegion = null;
                    FilterSelectedTerritory = new TerritoryMasterUIModel { TerritoryName = string.Empty };
                    FilterSelectedTerritory = null;
                    IsTerritoryDropDownEnabled = false;
                }
                else
                {
                    FilterSelectedRegion = new RegionMasterUIModel { ZoneID = region.ZoneID, RegionID = region.RegionID, Regioname = region?.Regioname };
                    if (FilterSelectedTerritory != null) FilterSelectedTerritory.TerritoryName = null;
                    FilterSelectedTerritory = null;
                    var _territoriesTempList = DbTerritoryList.Where(x => x.RegionID == FilterSelectedRegion?.RegionID).OrderBy(x => x.TerritoryName);
                    TerritoryList = new ObservableCollection<TerritoryMasterUIModel>(_territoriesTempList);
                    IsTerritoryDropDownEnabled = true;
                    SetNonRowToTerritoryList();
                }
            }
        }

        private void ZoneSelectionCommandHandler(ZoneMasterUIModel zone)
        {
            if (zone != null)
            {
                if (zone.ZoneID == 0 && "None".Equals(zone.ZoneName))
                {
                    FilterSelectedZone = new ZoneMasterUIModel { ZoneName = string.Empty };
                    FilterSelectedZone = null;
                    IsRegionDropDownEnabled = false;
                    IsTerritoryDropDownEnabled = false;
                    FilterSelectedRegion = new RegionMasterUIModel { Regioname = string.Empty };
                    FilterSelectedRegion = null;
                    FilterSelectedTerritory = new TerritoryMasterUIModel { TerritoryName = string.Empty };
                    FilterSelectedTerritory = null;
                }
                else
                {
                    FilterSelectedZone = new ZoneMasterUIModel { ZoneID = zone.ZoneID, ZoneName = zone?.ZoneName };

                    if (FilterSelectedRegion != null)
                    {
                        FilterSelectedRegion.Regioname = null;
                    }

                    FilterSelectedRegion = null;

                    if (FilterSelectedTerritory != null)
                    {
                        FilterSelectedTerritory.TerritoryName = null;
                    }

                    FilterSelectedTerritory = null;

                    var _regionsTempList = DbRegionMasterList.Where(x => x.ZoneID == zone?.ZoneID);

                    RegionList = new ObservableCollection<RegionMasterUIModel>(_regionsTempList);

                    SetNonRowToRegionList();

                    IsTerritoryDropDownEnabled = false;
                    IsRegionDropDownEnabled = true;
                }
            }
        }

        public void TerritorySearchHeaderTextChangeCommandHandler(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var _tempList = DbTerritoryList.Where(x => x.RegionID == FilterSelectedRegion?.RegionID && x.TerritoryName.ToLower().Contains(text.ToLower()));
                TerritoryList = new ObservableCollection<TerritoryMasterUIModel>(_tempList.OrderBy(x => x.TerritoryName));
            }
            else
            {
                TerritoryList = new ObservableCollection<TerritoryMasterUIModel>(DbTerritoryList.Where(x => x.RegionID == FilterSelectedRegion?.RegionID).OrderBy(x => x.TerritoryName));
            }
            SetNonRowToTerritoryList();
        }

        public void RegionSearchHeaderTextChangeCommandHandler(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var _tempData = DbRegionMasterList.Where(x => x.ZoneID == FilterSelectedZone.ZoneID && x.Regioname.ToLower().Contains(text.ToLower()));
                RegionList = new ObservableCollection<RegionMasterUIModel>(_tempData.OrderBy(x => x.Regioname));
            }
            else
            {
                RegionList = new ObservableCollection<RegionMasterUIModel>(DbRegionMasterList.Where(x => x.ZoneID == FilterSelectedZone.ZoneID).OrderBy(x => x.Regioname));
            }
            SetNonRowToRegionList();
        }

        public void ZoneSearchHeaderTextChangeCommandHandler(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var _tempList = DbZoneMasterList.Where(x => x.ZoneName.ToLower().Contains(text.ToLower()));
                ZoneList = new ObservableCollection<ZoneMasterUIModel>(_tempList);
            }
            else
            {
                ZoneList = new ObservableCollection<ZoneMasterUIModel>(DbZoneMasterList);
            }
            SetNonRowToZoneList();
        }

        private void ItemSelectionCommandHandler(ProductMasterUIModel item)
        {
            if (item != null)
            {
                FilterSelectedItem = item;
            }
        }
        private void ClearFilterCommandHandler()
        {
            if (FilterSelectedRegion != null) FilterSelectedRegion.Regioname = null;
            if (FilterSelectedTerritory != null) FilterSelectedTerritory.TerritoryName = null;
            FilterSelectedState = new KeyValuePair<int, string>() { };
            if (FilterSelectedCity != null) FilterSelectedCity.CityName = null;
            if (FilterSelectedItem != null) FilterSelectedItem.ProductName = null;
            FilterSelectedRegion = null;
            FilterSelectedTerritory = null;
            FilterSelectedState = null;
            FilterSelectedCity = null;
            FilterSelectedStartDate = null;
            FilterSelectedEndDate = null;

            if (FilterSelectedItem != null)
            {
                FilterSelectedItem.ProductName = null;
                FilterSelectedItem.Description = null;
            }

            FilterSelectedItem = null;

            IsStateDropDownEnabled = true;
            IsCityDropDownEnabled = false;
            IsRegionDropDownEnabled = true;
            IsTerritoryDropDownEnabled = false;

            if (loggedInUser?.RoleID == 5 || loggedInUser?.RoleID == 6 || loggedInUser?.RoleID == 7)
            {
                //FilterSelectedZone.ZoneName = null;
                FilterSelectedZone = null;
                IsRegionDropDownEnabled = false;
            }
        }

        private void CitySelectionCommandHandler(CityMasterUIModel city)
        {
            FilterSelectedCity = new CityMasterUIModel { CityID = city.CityID, CityName = city?.CityName, StateID = city.StateID };
        }

        private void StateSelectionCommandHandler(KeyValuePair<int, string> selectedState)
        {
            FilterSelectedState = new KeyValuePair<int, string>(selectedState.Key, selectedState.Value);
            var tempCityListOnBasisOfState = DbCityList.Where(x => x.StateID == selectedState.Key);
            CityList = new ObservableCollection<CityMasterUIModel>(tempCityListOnBasisOfState.OrderBy(x => x.CityName));
            IsCityDropDownEnabled = true;
        }

        private void CitySearchHeaderTextChangeCommandHandler(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var tempCityList = DbCityList.Where(x => x.StateID == FilterSelectedState?.Key && x.CityName.ToLower().Contains(text.ToLower()));
                CityList = new ObservableCollection<CityMasterUIModel>(tempCityList);
            }
            else
            {
                var tempCityListOnBasisOfState = DbCityList.Where(x => x.StateID == FilterSelectedState?.Key);
                CityList = new ObservableCollection<CityMasterUIModel>(tempCityListOnBasisOfState.OrderBy(x => x.CityName));
            }
        }

        private void StateSearchHeaderTextChangeCommandHandler(string text)
        {
            var _tempData = DbStateDict.Where(x => x.Value.ToLower().Contains(text.ToLower()));
            StateDictionary = _tempData != null ? _tempData.ToDictionary(x => x.Key, y => y.Value) : new Dictionary<int, string>();
        }

        private async Task ItemSearchHeaderTextChangeCommandHandler(string text)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var _data = await AppReference.QueryService.GetProductDetailsOnBasisOfProductName(text);

                    var tempList = _data.Where(x => x.ProductName.ToLower().Contains(text.ToLower()) || x.Description.ToLower().Contains(text.ToLower())).ToList();

                    ProductList = new ObservableCollection<ProductMasterUIModel>(tempList.Select(x => x.CopyToMasterUIModel()));
                }
                else
                {
                    ProductList.Clear();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(ItemSearchHeaderTextChangeCommandHandler), ex.StackTrace);
            }
        }

        public async Task OnPlotByFilterItemClickCommandHandler(PlotByTypeFilterUIModel filter)
        {
            if (CustomMapPinVisibility == Visibility.Visible)
            { CustomMapPinVisibility = Visibility.Collapsed; }

            foreach (var item in PlotByFilterSource)
            {
                item.IsSelected = false;
            }

            var index = PlotByFilterSource.IndexOf(filter);

            if (index >= 0)
            {
                PlotByFilterSource[index].IsSelected = true;
                PlotByCurrentFilter = PlotByFilterSource[index].Tag;
            }

            SetLegendsSource();

            await GetMapDataOnBasisOfPlotByFilter();
        }

        private async Task GetMapDataOnBasisOfPlotByFilter(bool isFromProductFilter = false)
        {
            if (!isFromProductFilter)
            {
                DisableDateAndItem();
            }

            switch (PlotByCurrentFilter)
            {
                case MapFilter.TradeType:
                    LegendsLabelText = "Trade Type";
                    DisableDateAndItem();
                    await GetCustomerDataForTradeType();
                    break;
                case MapFilter.Rank:
                    LegendsLabelText = "Account Rank";
                    DisableDateAndItem();
                    await GetCustomerDataForAccountRank();
                    break;
                case MapFilter.CallDate:
                    LegendsLabelText = "Call Date";
                    EnableDateDisableItem();
                    await GetCustomerDataForCallActivity();
                    break;
                case MapFilter.CashSales:
                    LegendsLabelText = "Cash Sales";
                    EnableDateDisableItem();
                    if (FilterSelectedStartDate == null && FilterSelectedEndDate == null)
                    {
                        FilterSelectedStartDate = DateTimeOffset.UtcNow.AddDays(-90);
                        FilterSelectedEndDate = DateTimeOffset.UtcNow;
                        SelectedStartDate = FilterSelectedStartDate;
                        SelectedEndDate = FilterSelectedEndDate;
                        IsDateFilterDescriptionRowVisible = true;
                        DisplayStartDate = SelectedStartDate.Value.DateTime.ToString(Core.Helpers.DateTimeHelper.USDateFormat, new CultureInfo("en-US"));
                        DisplayEndDate = SelectedEndDate.Value.DateTime.ToString(Core.Helpers.DateTimeHelper.USDateFormat, new CultureInfo("en-US"));
                    }
                    await GetCustomerDataForCashSales();
                    break;
                case MapFilter.Item:
                    LegendsLabelText = "Item No";
                    EnableDateAndItem();
                    if (isFromProductFilter)
                    {
                        LegendsLabelText = "Item No " + SelectedItem?.ProductName + ", " + SelectedItem?.Description;
                        await GetCustomerDataForItemFilter();
                    }
                    else
                    {
                        PointOfIntrestSource.Clear();
                    }
                    break;
            }
        }

        private void EnableDateAndItem()
        {
            IsStartEndDateEnabled = true;
            IsItemDropDownEnabled = true;
        }

        private void EnableDateDisableItem()
        {
            IsStartEndDateEnabled = true;
            IsItemDropDownEnabled = false;
            if (FilterSelectedItem != null)
            {
                FilterSelectedItem.ProductName = null;
                FilterSelectedItem.Description = null;
            }
            FilterSelectedItem = null;
        }

        private void DisableDateAndItem()
        {
            IsItemDropDownEnabled = false;
            IsStartEndDateEnabled = false;
            FilterSelectedStartDate = null;
            FilterSelectedEndDate = null;
            if (FilterSelectedItem != null)
            {
                FilterSelectedItem.ProductName = null;
                FilterSelectedItem.Description = null;
            }
            FilterSelectedItem = null;
            SelectedStartDate = SelectedEndDate = null;
            IsDateFilterDescriptionRowVisible = false;
            DisplayEndDate = DisplayStartDate = "";
        }

        private void SetLegendsSource()
        {
            LegendsFilterSource?.Clear();

            switch (PlotByCurrentFilter)
            {
                case MapFilter.TradeType:
                    LegendsFilterSource = StaticDataSourceHelper.MapLegendsFiltersDataSourceForTradeType();
                    break;
                case MapFilter.Rank:
                    LegendsFilterSource = StaticDataSourceHelper.MapLegendsFiltersDataSourceForRank();
                    break;
                case MapFilter.CallDate:
                    LegendsFilterSource = StaticDataSourceHelper.MapLegendsFiltersDataSourceForCallDate();
                    break;
                case MapFilter.CashSales:
                    LegendsFilterSource = StaticDataSourceHelper.MapLegendsFiltersDataSourceForCashSales();
                    break;
                case MapFilter.Item:
                    LegendsFilterSource = StaticDataSourceHelper.MapLegendsFiltersDataSourceForItemNo();
                    break;
            }
        }

        public async Task OnMapLegendItemClickCommandHandler(MapsLegendFilterUIModel selectedItem, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (CustomMapPinVisibility == Visibility.Visible)
            { CustomMapPinVisibility = Visibility.Collapsed; }

            var index = LegendsFilterSource.IndexOf(selectedItem);
            if (index >= 0)
            {
                LegendsFilterSource[index].IsSelected = !LegendsFilterSource[index].IsSelected;
            }
            await FilterTheCurrentDataOnTheBasisOfLegendsAsync(token);
        }

        public async Task FilterTheCurrentDataOnTheBasisOfLegendsAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            switch (PlotByCurrentFilter)
            {
                case MapFilter.TradeType:
                    await ApplyTradeTypeLegendsFilterAsync();
                    break;
                case MapFilter.Rank:
                    await ApplyRankLegendsFilterAsync();
                    break;
                case MapFilter.CallDate:
                    await ApplyCallDateLegendsFilterAsync();
                    break;
                case MapFilter.CashSales:
                    await ApplyCashSalesLegendsFilterAsync();
                    break;
                case MapFilter.Item:
                    await ApplyItemLegendsFilter();
                    break;
            }

            if (!PointOfIntrestSource.Any())
            {
                await ShowNoCustomerAlert();
            }
        }

        private async Task ApplyItemLegendsFilter()
        {
            try
            {
                List<MapCustomerData> tempList = new List<MapCustomerData>();
                var unselectedItemLegends = LegendsFilterSource.Where(x => !x.IsSelected);
                foreach (var item in unselectedItemLegends)
                {
                    if (item.Tag == 1)
                    {
                        var soldItems = PointOfIntrestSource.Where(x => x.CustomerData.Sold > 0);
                        await RemoveItemsFromPointOfInterestSourceAsync(soldItems.ToList());
                    }
                    else
                    {
                        var unSoldItem = PointOfIntrestSource.Where(x => x.CustomerData.Sold <= 0);
                        await RemoveItemsFromPointOfInterestSourceAsync(unSoldItem.ToList());
                    }
                }
                var selectedItemLegends = LegendsFilterSource.Where(x => x.IsSelected);

                foreach (var item in selectedItemLegends)
                {
                    if (item.Tag == 1)
                    {
                        var soldItems = PointOfIntrestSource.Where(x => x.CustomerData.Sold > 0);
                        if (soldItems == null || soldItems.Count() <= 0)
                        {
                            await Task.Run(() =>
                            tempList.AddRange(DbMapCustomerData.Where(x => x.Sold > 0)));
                        }
                    }
                    else
                    {
                        var unSoldItem = PointOfIntrestSource.Where(x => x.CustomerData.Sold <= 0);
                        if (unSoldItem == null || unSoldItem.Count() <= 0)
                        {
                            await Task.Run(() =>
                            tempList.AddRange(DbMapCustomerData.Where(x => x.Sold <= 0)));
                        }
                    }
                }
                await LoopAndAddToSourceForItemSourceAsync(tempList);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(ApplyItemLegendsFilter), ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task ApplyCashSalesLegendsFilterAsync()
        {
            try
            {
                List<MapCustomerData> tempList = new List<MapCustomerData>();
                var unSelectedCashSales = LegendsFilterSource.Where(x => !x.IsSelected);
                var removedCashSalesItems = PointOfIntrestSource.Where(x => unSelectedCashSales.Any(y => y.Tag == x?.CustomerData.Tag)).ToList();
                await RemoveItemsFromPointOfInterestSourceAsync(removedCashSalesItems);
                var selectedCashSalesLegends = LegendsFilterSource.Where(x => x.IsSelected);
                foreach (var item in selectedCashSalesLegends)
                {
                    var presentList = PointOfIntrestSource.Where(x => x?.CustomerData.Tag == item.Tag);
                    if (presentList == null || presentList.Count() <= 0)
                    {
                        await Task.Run(() =>
                        {
                            var updatedData = GetPinTagsOnBasisOfCashSales(DbMapCustomerData.ToList());
                            tempList.AddRange(updatedData?.Where(x => x.Tag == item.Tag));
                        });
                    }
                }
                await LoopAndAddToSourceForCashSalesAsync(tempList);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(ApplyCashSalesLegendsFilterAsync), ex);
            }
        }

        private async Task ApplyCallDateLegendsFilterAsync()
        {
            try
            {
                List<MapCustomerData> tempList = new List<MapCustomerData>();
                var unSelectedCallDate = LegendsFilterSource.Where(x => !x.IsSelected);
                var removedCallDateItems = PointOfIntrestSource.Where(x => unSelectedCallDate.Any(y => y.Tag == x?.CustomerData.Tag));
                await RemoveItemsFromPointOfInterestSourceAsync(removedCallDateItems.ToList());
                var selectedCallDateLegends = LegendsFilterSource.Where(x => x.IsSelected);
                foreach (var item in selectedCallDateLegends)
                {
                    var presentList = PointOfIntrestSource.Where(x => x?.CustomerData.Tag == item.Tag);
                    if (presentList == null || presentList.Count() <= 0)
                    {
                        await Task.Run(() =>
                        tempList.AddRange(DbMapCustomerData?.Where(x => x.Tag == item.Tag)));
                    }
                }
                await LoopAndAddToSourceForCallDateAsync(tempList);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(ApplyCallDateLegendsFilterAsync), ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task ApplyRankLegendsFilterAsync()
        {
            try
            {
                List<MapCustomerData> tempList = new List<MapCustomerData>();
                var unselectedRanks = LegendsFilterSource.Where(x => !x.IsSelected);
                foreach (var item in unselectedRanks)
                {
                    if (item.Title.Equals("Other"))
                    {
                        var othersRanks = PointOfIntrestSource.Where(x => !"A".Equals(x?.CustomerData?.Rank) && !"B".Equals(x?.CustomerData?.Rank) && !"C".Equals(x?.CustomerData?.Rank)).ToList();
                        await RemoveItemsFromPointOfInterestSourceAsync(othersRanks);
                    }
                    else
                    {
                        var ranksToRemove = PointOfIntrestSource?.Where(x => !string.IsNullOrEmpty(x?.CustomerData?.Rank) && item.Rank.Equals(x?.CustomerData?.Rank)).ToList();
                        await RemoveItemsFromPointOfInterestSourceAsync(ranksToRemove);
                    }
                }
                var selectedRanks = LegendsFilterSource.Where(x => x.IsSelected);
                foreach (var item in selectedRanks)
                {
                    if (item.Title.Equals("Other"))
                    {
                        var selectedOther = PointOfIntrestSource.Where(x => !"A".Equals(x?.CustomerData?.Rank) && !"B".Equals(x?.CustomerData?.Rank) && !"C".Equals(x?.CustomerData?.Rank)).ToList();
                        if (selectedOther == null || selectedOther.Count() <= 0)
                        {
                            await Task.Run(() =>
                            tempList.AddRange(DbMapCustomerData.Where(x => !"A".Equals(x?.Rank) && !"B".Equals(x?.Rank) && !"C".Equals(x?.Rank))));
                        }
                    }
                    else
                    {
                        var ranksSelected = PointOfIntrestSource?.Where(x => !string.IsNullOrEmpty(x?.CustomerData?.Rank) && item.Rank.Equals(x?.CustomerData?.Rank));
                        if (ranksSelected == null || ranksSelected.Count() <= 0)
                        {
                            await Task.Run(() =>
                            tempList.AddRange(DbMapCustomerData.Where(x => !string.IsNullOrEmpty(x?.Rank) && item.Rank.Equals(x?.Rank))));
                        }
                    }
                }
                await LoopAndAddToSourceForRankAsync(tempList);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "ApplyRankLegendsFilter", ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task ApplyTradeTypeLegendsFilterAsync()
        {
            try
            {
                List<MapCustomerData> tempList = new List<MapCustomerData>();
                //check for unselected objects
                var unSelectedTradeType = LegendsFilterSource.Where(x => !x.IsSelected);

                var removedTradeTypesItems = PointOfIntrestSource.Where(x => unSelectedTradeType.Any(y => y.AccountClassificationIds.Contains(Convert.ToInt32(x.CustomerData.AccountClassification)))).ToList();

                await RemoveItemsFromPointOfInterestSourceAsync(removedTradeTypesItems);

                var selected = LegendsFilterSource.Where(x => x.IsSelected);

                foreach (var item in selected)
                {
                    var presentTradeTypeList = PointOfIntrestSource.Where(x => item.AccountClassificationIds.Contains(Convert.ToInt32(x.CustomerData.AccountClassification)));

                    if (presentTradeTypeList == null || presentTradeTypeList.Count() <= 0)
                    {
                        await Task.Run(() =>
                        tempList.AddRange(DbMapCustomerData?.Where(x => x != null && item.AccountClassificationIds.Contains(Convert.ToInt32(x?.AccountClassification)))));
                    }
                }

                await LoopAndAddToSourceTradeTypeAsync(tempList);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "ApplyTradeTypeLegendsFilterAsync", ex.StackTrace + " - " + ex.Message);
            }
        }

        private async Task RemoveItemsFromPointOfInterestSourceAsync(List<PointOfInterest> removedItems)
        {
            try
            {
                foreach (var item in removedItems)
                {
                    await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => PointOfIntrestSource.Remove(item));
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "RemoveItemsFromPointOfInterestSourceAsync", ex.StackTrace);
            }
        }

        private void RemoveItemsFromPointOfInterestSource(List<PointOfInterest> removedItems)
        {
            try
            {
                foreach (var item in removedItems)
                {
                    PointOfIntrestSource.Remove(item);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "RemoveItemsFromPointOfInterestSource", ex.StackTrace);
            }
        }

        public async Task OnNavigatedToCommandHandler()
        {
            try
            {
                if (CustomMapPinVisibility == Visibility.Visible)
                { CustomMapPinVisibility = Visibility.Collapsed; }

                CultureInfo.CurrentUICulture = new CultureInfo("en-US");
                TodaysDate = DateTime.Today;
                if (loggedInUser == null)
                {
                    loggedInUser = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                    ClearFilterCommandHandler();

                    ClearHeaderFilterDiscription();

                    IsDateFilterDescriptionRowVisible = false;
                    DisplayStartDate = null;
                    DisplayEndDate = null;
                    SelectedStartDate = null;
                    SelectedEndDate = null;
                    SelectedCity = null;
                    SelectedState = null;
                    LegendsLabelText = null;

                    DbStateDict = await AppReference.QueryService.GetStateDictionaryWhichHasCustomersAssociated();
                    DbCityList = await AppReference.QueryService.GetCityMasterDataWhichHasCustomersAssociated();
                    DbZoneMasterList = await AppReference.QueryService.GetZonesOnBasisOfCustomers();
                    DbRegionMasterList = await AppReference.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(null);
                    DbTerritoryList = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(null);

                    SetDefaultZoneRegionTerritory();

                    if (loggedInUser != null && ((loggedInUser.RoleID > 4 & loggedInUser.RoleID < 8) || loggedInUser.RoleID > 12))
                    {
                        IsCityDropDownEnabled = false;
                        IsStartEndDateEnabled = false;
                        IsItemDropDownEnabled = false;
                    }
                    else
                    {
                        IsCityDropDownEnabled = false;
                        IsStartEndDateEnabled = false;
                        IsItemDropDownEnabled = false;

                        IsStateDropDownEnabled = true;
                    }
                }
                else // if user changed then setting the cache objects again
                {
                    var loggedinUser_new = await AppReference.QueryService.GetUserData(AppReference.LoginUserNameProperty, AppReference.LoginUserPinProperty);
                    if (loggedInUser.UserId != loggedinUser_new.UserId)
                    {
                        loggedInUser = loggedinUser_new;
                        ClearFilterCommandHandler();

                        ClearHeaderFilterDiscription();

                        IsDateFilterDescriptionRowVisible = false;
                        DisplayStartDate = null;
                        DisplayEndDate = null;
                        SelectedStartDate = null;
                        SelectedEndDate = null;
                        SelectedCity = null;
                        SelectedState = null;
                        LegendsLabelText = null;

                        DbStateDict = await AppReference.QueryService.GetStateDictionaryWhichHasCustomersAssociated();
                        DbCityList = await AppReference.QueryService.GetCityMasterDataWhichHasCustomersAssociated();
                        DbZoneMasterList = await AppReference.QueryService.GetZonesOnBasisOfCustomers();
                        DbRegionMasterList = await AppReference.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(null);
                        DbTerritoryList = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(null);

                        SetDefaultZoneRegionTerritory();

                        if (loggedInUser != null && ((loggedInUser.RoleID > 4 & loggedInUser.RoleID < 8) || loggedInUser.RoleID > 12))
                        {
                            IsCityDropDownEnabled = false;
                            IsStartEndDateEnabled = false;
                            IsItemDropDownEnabled = false;
                        }
                        else
                        {
                            IsCityDropDownEnabled = false;
                            IsStartEndDateEnabled = false;
                            IsItemDropDownEnabled = false;

                            IsStateDropDownEnabled = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(LegendsLabelText))
                {
                    LegendsLabelText = "Trade Type";
                    PlotByCurrentFilter = MapFilter.TradeType;
                    PlotByFilterSource = StaticDataSourceHelper.GetPlotByTypeFiltersDataSource();

                    var classifications = await AppReference.QueryService.GetClassificationDict();

                    //AccountClassificationsList = classifications?.Values.ToList();
                    MapsStaticDataSourceHelper.ClassificationsList = classifications?.Values.ToList();
                    SetLegendsSource();
                }
                //
                if (DbStateDict == null || DbStateDict.Count == 0)
                { DbStateDict = await AppReference.QueryService.GetStateDictionaryWhichHasCustomersAssociated(); }

                if (DbCityList == null || DbCityList.Count == 0)
                { DbCityList = await AppReference.QueryService.GetCityMasterDataWhichHasCustomersAssociated(); }

                //National Roles
                if (loggedInUser != null && ((loggedInUser.RoleID > 4 & loggedInUser.RoleID < 8) || loggedInUser.RoleID > 12))
                {
                    /// need to confirm as in IOS for national head nothing plots on the map
                    //IsZoneDropDownEnabled = true;
                    //IsNationalHead = true;
                    //IsRegionDropDownEnabled = true;
                    //IsTerritoryDropDownEnabled = true;

                    //IsCityDropDownEnabled = false; //---
                    //IsStartEndDateEnabled = false; //---
                    //IsItemDropDownEnabled = false; //---

                    if (DbZoneMasterList == null || DbZoneMasterList.Count() == 0)
                    { DbZoneMasterList = await AppReference.QueryService.GetZonesOnBasisOfCustomers(); }


                }
                else
                {
                    //IsCityDropDownEnabled = false; //---
                    //IsStartEndDateEnabled = false; //---
                    //IsItemDropDownEnabled = false; //---
                    //IsZoneDropDownEnabled = true;
                    //IsNationalHead = false;
                    //IsRegionDropDownEnabled = true;
                    //IsTerritoryDropDownEnabled = true;
                    IsStateDropDownEnabled = true;

                    //DbZoneMasterList = await AppReference.QueryService.GetZoneFromZoneId(loggedInUser.ZoneId);

                    //DbRegionMasterList = await AppReference.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(DbZoneMasterList?.Select(x => x.ZoneID));

                    //DbTerritoryList = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(DbRegionMasterList?.Select(x => x.RegionID));
                }

                if (DbZoneMasterList == null || DbZoneMasterList.Count() == 0)
                { DbZoneMasterList = await AppReference.QueryService.GetZonesOnBasisOfCustomers(); }
                IsNationalHead = IsZoneDropDownEnabled = (DbZoneMasterList != null && DbZoneMasterList.Count() > 1);

                if (DbRegionMasterList == null || DbRegionMasterList.Count() == 0)
                { DbRegionMasterList = await AppReference.QueryService.GetRegionsOnBasisOfZoneIdsAndPresentCustomers(null); }
                IsRegionDropDownEnabled = (DbRegionMasterList != null && DbRegionMasterList.Count() > 1);

                if (DbTerritoryList == null || DbTerritoryList.Count() == 0)
                { DbTerritoryList = await AppReference.QueryService.GetTerritoryOnBasisOfRegionIdsAndPresentCustomers(null); }
                IsTerritoryDropDownEnabled = (DbTerritoryList != null && DbTerritoryList.Count() > 1);


                if (PlotByCurrentFilter == MapFilter.TradeType)
                {
                    await GetCustomerDataForTradeType();
                }
                else if (PlotByCurrentFilter == MapFilter.Rank)
                {
                    await GetCustomerDataForAccountRank();
                }
                else if (PlotByCurrentFilter == MapFilter.CashSales)
                {
                    await GetCustomerDataForCashSales();
                }
                else if (PlotByCurrentFilter == MapFilter.Item)
                {
                    await GetCustomerDataForItemFilter();
                }
                else if (PlotByCurrentFilter == MapFilter.CallDate)
                {
                    await GetCustomerDataForCallActivity();
                }

                if (PointOfIntrestSource.Count > 0)
                    Center = CalculateCenter(PointOfIntrestSource);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(OnNavigatedToCommandHandler), ex.StackTrace);
            }
        }

        private void ClearHeaderFilterDiscription()
        {
            try
            {
                if (SelectedZone != null)
                {
                    SelectedZone.ZoneName = string.Empty;
                }

                SelectedZone = null;

                if (SelectedRegion != null)
                {
                    SelectedRegion.Regioname = string.Empty;
                }

                SelectedRegion = null;

                if (SelectedTerritory != null)
                {
                    SelectedTerritory.TerritoryName = string.Empty;
                }

                SelectedTerritory = null;

                if (SelectedState != null)
                {
                    SelectedState = new KeyValuePair<int, string>();
                }

                SelectedState = null;

                if (SelectedCity != null)
                {
                    SelectedCity.CityName = string.Empty;
                }

                SelectedCity = null;

                if (SelectedItem != null)
                {
                    SelectedItem.ProductName = string.Empty;
                }

                SelectedItem = null;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(ClearHeaderFilterDiscription), ex.StackTrace);
            }
        }

        private void SetDefaultZoneRegionTerritory()
        {
            try
            {
                ZoneList = new ObservableCollection<ZoneMasterUIModel>(DbZoneMasterList);
                ZoneMasterUIModel defaultZone = null;
                if (loggedInUser?.ZoneId != 0)
                {
                    defaultZone = DbZoneMasterList?.FirstOrDefault(x => x.ZoneID == loggedInUser.ZoneId);
                }
                if (defaultZone == null)
                {
                    defaultZone = DbZoneMasterList?.FirstOrDefault();
                }
                SelectedZone = new ZoneMasterUIModel { ZoneID = defaultZone.ZoneID, ZoneName = defaultZone?.ZoneName };
                RegionMasterUIModel region = null;
                if (loggedInUser?.RegionId != 0)
                {
                    region = DbRegionMasterList?.FirstOrDefault(x => x.RegionID == loggedInUser?.RegionId && x.ZoneID == SelectedZone.ZoneID);
                }
                if (region == null)
                {
                    region = DbRegionMasterList?.FirstOrDefault(x => x.ZoneID == SelectedZone.ZoneID);
                }

                SelectedRegion = new RegionMasterUIModel { ZoneID = region.ZoneID, Regioname = region?.Regioname, RegionID = region.RegionID };
                RegionList = new ObservableCollection<RegionMasterUIModel>(DbRegionMasterList?.Where(x => x != null && x?.ZoneID == SelectedZone?.ZoneID));
                TerritoryList = new ObservableCollection<TerritoryMasterUIModel>(DbTerritoryList.Where(x => x?.RegionID == SelectedRegion?.RegionID));

                TerritoryMasterUIModel territory = null;
                if (loggedInUser.defterritoryid != 0)
                {
                    territory = TerritoryList?.FirstOrDefault(x => x.TerritoryID == loggedInUser.defterritoryid && x.RegionID == SelectedRegion.RegionID);

                }

                if (territory == null)
                {
                    territory = TerritoryList?.FirstOrDefault(x => x.RegionID == SelectedRegion.RegionID);
                }

                SelectedTerritory = new TerritoryMasterUIModel { RegionID = territory.RegionID, TerritoryID = territory.TerritoryID, TerritoryName = territory?.TerritoryName };
                FilterSelectedZone = new ZoneMasterUIModel() { ZoneID = SelectedZone.ZoneID, ZoneName = SelectedZone.ZoneName };
                FilterSelectedRegion = new RegionMasterUIModel() { ZoneID = SelectedRegion.ZoneID, Regioname = SelectedRegion.Regioname, RegionID = SelectedRegion.RegionID };
                FilterSelectedTerritory = new TerritoryMasterUIModel() { RegionID = SelectedTerritory.RegionID, TerritoryID = SelectedTerritory.TerritoryID, TerritoryName = SelectedTerritory.TerritoryName };

                SetNonRowToZoneList();
                SetNonRowToRegionList();
                SetNonRowToTerritoryList();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(SetDefaultZoneRegionTerritory), ex.StackTrace);
            }
        }

        private void SetNonRowToTerritoryList()
        {
            var nonTerritory = new TerritoryMasterUIModel { RegionID = 0, TerritoryID = 0, TerritoryName = "None" };
            TerritoryList.Insert(0, nonTerritory);
        }

        private void SetNonRowToRegionList()
        {
            var nonRegion = new RegionMasterUIModel { Regioname = "None", RegionID = 0, ZoneID = 0 };
            RegionList.Insert(0, nonRegion);
        }

        private void SetNonRowToZoneList()
        {
            var noneZone = new ZoneMasterUIModel { ZoneName = "None", ZoneID = 0 };
            ZoneList.Insert(0, noneZone);
        }

        private async Task GetCustomerDataForCallActivity()
        {
            PointOfIntrestSource.Clear();

            DbMapCustomerData = null;

            DbMapCustomerData = await AppReference.QueryService.GetMapDataForTradeTypeAndRankAndCallDate(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName);

            GetPinTagsOnTheBasisOfCallDate();

            if (DbMapCustomerData != null)
            {
                await PlotPinsForCallDateFromSource(DbMapCustomerData);
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Please try after sometime.", "OK");
            }
        }

        private async Task GetCustomerDataForTradeType()
        {
            PointOfIntrestSource.Clear();

            DbMapCustomerData = null;

            DbMapCustomerData = await AppReference.QueryService.GetMapDataForTradeTypeAndRankAndCallDate(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName).ConfigureAwait(false);

            if (DbMapCustomerData != null)
            {
                await PlotPinsForTradeTypeFromSource(DbMapCustomerData);
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Please try after sometime.", "OK");
            }
        }

        private async Task GetCustomerDataForAccountRank()
        {
            PointOfIntrestSource.Clear();

            DbMapCustomerData = null;

            DbMapCustomerData = await AppReference.QueryService.GetMapDataForTradeTypeAndRankAndCallDate(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName).ConfigureAwait(false);

            if (DbMapCustomerData != null)
            {
                await PlotPinsForAccountRankFromSource(DbMapCustomerData);
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Please try after sometime.", "OK");
            }
        }

        private async Task GetCustomerDataForCashSales()
        {
            PointOfIntrestSource.Clear();

            DbMapCustomerData = null;

            if (AppReference.LoggedInUserRoleId == 3 || AppReference.LoggedInUserRoleId == 6)
            {
                DbMapCustomerData = await AppReference.QueryService.GetMapDataForCashSalesForNationalAndZM(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName,
                    SelectedStartDate?.UtcDateTime, SelectedEndDate?.UtcDateTime);
            }
            else
            {
                DbMapCustomerData = await AppReference.QueryService.GetMapDataForCashSales(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName,
                    SelectedStartDate?.UtcDateTime, SelectedEndDate?.UtcDateTime);
            }

            if (DbMapCustomerData != null)
            {
                var updatedData = GetPinTagsOnBasisOfCashSales(DbMapCustomerData.ToList());

                await PlotPinsForCashSalesFromSource(updatedData);
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Please try after sometime.", "OK");
            }
        }

        private async Task GetCustomerDataForItemFilter()
        {
            PointOfIntrestSource.Clear();

            DbMapCustomerData = null;

            //var DbMapCustomerData1 = await AppReference.QueryService.GetMapsDataForItemsFilter1(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName, SelectedItem != null ? SelectedItem.ProductID : 0);


            DbMapCustomerData = await AppReference.QueryService.GetMapsDataForItemsFilter(SelectedZone?.ZoneID, SelectedRegion?.RegionID, SelectedTerritory?.TerritoryID, SelectedState?.Key.ToString(), SelectedCity?.CityName, SelectedItem != null ? SelectedItem.ProductID : 0).ConfigureAwait(false);

            if (DbMapCustomerData != null)
            {
                await PlotPinsOnTheBasisOfItemFilterFromSource(DbMapCustomerData.ToList());
            }
            else
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Something went wrong. Please try after sometime.", "OK");
            }
        }

        private async Task PlotPinsOnTheBasisOfItemFilterFromSource(List<MapCustomerData> source)
        {
            if (source != null && source.Count > 0)
            {
                await LoopAndAddToSourceForItemSourceAsync(source);

                if (!PointOfIntrestSource.Any())
                {
                    await ShowNoCustomerAlert();
                }
            }
            else
            {
                await ShowNoCustomerAlert();
            }
        }

        private async Task LoopAndAddToSourceForItemSourceAsync(List<MapCustomerData> tempList)
        {
            try
            {
                foreach (var item in tempList)
                {
                    if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                    {
                        var isSold = item.Sold > 0;

                        var legend = LegendsFilterSource.FirstOrDefault(x => x.Tag == (isSold ? 1 : 2));

                        bool isValidDateCheck = true;

                        if (SelectedStartDate.HasValue)
                        {
                            isValidDateCheck = item.CallActivityDate >= SelectedStartDate && item.CallActivityDate <= SelectedEndDate;
                        }

                        if (legend?.IsSelected == true && isValidDateCheck)
                        {

                            await coreDispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                            PointOfIntrestSource.Add(new PointOfInterest
                            {
                                CustomerData = item,
                                Location = new Geopoint(
                                                new BasicGeoposition()
                                                {
                                                    Latitude = Convert.ToDouble(item?.Latitude),
                                                    Longitude = Convert.ToDouble(item?.Longitude)
                                                }),
                                NormalizedAnchorPoint = new Point(0.5, 1),
                                PinColor = isSold ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Orange),
                                ImageSourceUri = legend.MapIconImagePath
                            })).AsTask().ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "LoopAndAddToSourceForItemSourceAsync", ex);
            }
        }

        private async Task PlotPinsForTradeTypeFromSource(IEnumerable<MapCustomerData> source)
        {
            if (source != null && source.Any())
            {
                await LoopAndAddToSourceTradeTypeAsync(source);

                if (!PointOfIntrestSource.Any())
                {
                    await ShowNoCustomerAlert();
                }
            }
            else
            {
                await ShowNoCustomerAlert();
            }
        }

        private async Task LoopAndAddToSourceTradeTypeAsync(IEnumerable<MapCustomerData> mapCustomerList)
        {
            try
            {
                foreach (var item in mapCustomerList)
                {
                    if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                    {
                        var legend = LegendsFilterSource.FirstOrDefault(x => x.AccountClassificationIds.Contains(Convert.ToInt32(item.AccountClassification)));
                        if (legend?.IsSelected == true)
                        {
                            try
                            {
                                await Task.Run(() =>
                                PointOfIntrestSource.Add(new PointOfInterest
                                {
                                    CustomerData = item,
                                    Location = new Geopoint(new BasicGeoposition()
                                    {
                                        Latitude = Convert.ToDouble(item?.Latitude),
                                        Longitude = Convert.ToDouble(item?.Longitude)
                                    }),
                                    //NormalizedAnchorPoint = new Point(0.5, 1),
                                    PinColor = legend?.BackgroundColor,
                                    ImageSourceUri = legend.MapIconImagePath
                                }));
                            }
                            catch (Exception ex)
                            {
                                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(LoopAndAddToSourceTradeTypeAsync), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(LoopAndAddToSourceTradeTypeAsync), ex);
            }
        }

        private async Task PlotPinsForAccountRankFromSource(IEnumerable<MapCustomerData> source)
        {
            try
            {
                if (source != null && source.Any())
                {
                    await LoopAndAddToSourceForRankAsync(source);

                    if (!PointOfIntrestSource.Any())
                    {
                        await ShowNoCustomerAlert();
                    }

                }
                else
                {
                    await ShowNoCustomerAlert();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(PlotPinsForAccountRankFromSource), ex.StackTrace);
            }
        }

        private async Task LoopAndAddToSourceForRankAsync(IEnumerable<MapCustomerData> tempList)
        {
            foreach (var item in tempList)
            {
                if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                {
                    var legend = LegendsFilterSource.Any(x => x != null && !string.IsNullOrEmpty(x.Rank) && x.Rank.Equals(item?.Rank))
                        ? LegendsFilterSource?.FirstOrDefault(x => x.Rank.Equals(item.Rank)) : LegendsFilterSource.LastOrDefault();

                    if (legend?.IsSelected == true)
                    {
                        try
                        {
                            await Task.Run(() =>
                            PointOfIntrestSource.Add(new PointOfInterest
                            {
                                CustomerData = item,
                                Location = new Geopoint(
                                        new BasicGeoposition()
                                        {
                                            Latitude = Convert.ToDouble(item?.Latitude),
                                            Longitude = Convert.ToDouble(item?.Longitude)
                                        }),
                                NormalizedAnchorPoint = new Point(0.5, 1),
                                PinColor = legend.BackgroundColor,
                                ImageSourceUri = legend.MapIconImagePath
                            }));
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(LoopAndAddToSourceForRankAsync), ex);
                        }
                    }
                }
            }
        }

        private async Task PlotPinsForCallDateFromSource(IEnumerable<MapCustomerData> source)
        {
            if (source != null && source.Any())
            {
                await LoopAndAddToSourceForCallDateAsync(source);

                if (!PointOfIntrestSource.Any())
                {
                    await ShowNoCustomerAlert();
                }
            }
            else
            {
                await ShowNoCustomerAlert();
            }
        }

        private async Task LoopAndAddToSourceForCallDateAsync(IEnumerable<MapCustomerData> tempList)
        {
            foreach (var item in tempList)
            {
                if (!string.IsNullOrEmpty(item.LastCallActivityDate) && !string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                {
                    var legend = LegendsFilterSource.FirstOrDefault(x => x.Tag == item.Tag);

                    bool isValidDateCheck = true;

                    if (SelectedStartDate.HasValue)
                    {
                        isValidDateCheck = item.CallActivityDate >= SelectedStartDate && item.CallActivityDate <= SelectedEndDate;
                    }

                    if (legend?.IsSelected == true && isValidDateCheck)
                    {
                        try
                        {
                            await Task.Run(() =>
                            PointOfIntrestSource.Add(new PointOfInterest
                            {
                                CustomerData = item,
                                Location = new Geopoint(
                                        new BasicGeoposition()
                                        {
                                            Latitude = Convert.ToDouble(item?.Latitude),
                                            Longitude = Convert.ToDouble(item?.Longitude)
                                        }),
                                NormalizedAnchorPoint = new Point(0.5, 1),
                                PinColor = legend.BackgroundColor,
                                ImageSourceUri = legend.MapIconImagePath
                            }));
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(LoopAndAddToSourceForCallDateAsync), ex);
                        }
                    }
                }
            }
        }

        private async Task PlotPinsForCashSalesFromSource(List<MapCustomerData> source)
        {
            try
            {
                if (source != null && source.Count != 0)
                {
                    await LoopAndAddToSourceForCashSalesAsync(source);

                    if (!PointOfIntrestSource.Any())
                    {
                        await ShowNoCustomerAlert();
                    }
                }
                else
                {
                    await ShowNoCustomerAlert();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "PlotPinsForCashSalesFromSource", ex.StackTrace);
            }
        }

        private async Task LoopAndAddToSourceForCashSalesAsync(List<MapCustomerData> tempList)
        {
            foreach (var item in tempList)
            {
                // if (!string.IsNullOrEmpty(item.LastCallActivityDate) && !string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                {
                    var legend = LegendsFilterSource.FirstOrDefault(x => x.Tag == item.Tag);

                    //bool isValidDateCheck = true;

                    //if (SelectedStartDate.HasValue)
                    //{
                    //    isValidDateCheck = item.CallActivityDate >= SelectedStartDate && item.CallActivityDate <= SelectedEndDate;
                    //}
                    //if (legend?.IsSelected == true && isValidDateCheck)
                    if (legend?.IsSelected == true)
                    {
                        try
                        {
                            await Task.Run(() =>
                            PointOfIntrestSource.Add(new PointOfInterest
                            {
                                CustomerData = item,
                                Location = new Geopoint(new BasicGeoposition()
                                {
                                    Latitude = Convert.ToDouble(item?.Latitude),
                                    Longitude = Convert.ToDouble(item?.Longitude)
                                }),
                                NormalizedAnchorPoint = new Point(0.5, 1),
                                PinColor = legend.BackgroundColor,
                                ImageSourceUri = legend.MapIconImagePath
                            }));
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), "LoopAndAddToSourceForCashSalesAsync", ex.StackTrace);
                        }
                    }
                }
            }
        }

        private async Task ShowNoCustomerAlert()
        {
            await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "No Customer found for the selected filter", "OK");
            });
        }

        private void GetPinTagsOnTheBasisOfCallDate()
        {
            try
            {
                var lastMonthDate = TodaysDate.AddMonths(-1);
                var lastThreeMonthDate = TodaysDate.AddMonths(-3);
                var lastSixMonthDate = TodaysDate.AddMonths(-6);
                var lastYearDate = TodaysDate.AddYears(-1);
                List<MapCustomerData> tmp = DbMapCustomerData.ToList();
                foreach (var item in tmp)
                {
                    if (!string.IsNullOrWhiteSpace(item.LastCallActivityDate))
                    {
                        if (item.CallActivityDate > lastMonthDate)
                        {
                            item.Tag = 1;
                        }
                        else if (item.CallActivityDate == lastMonthDate || (item.CallActivityDate > lastThreeMonthDate))
                        {
                            item.Tag = 2;
                        }
                        else if (item.CallActivityDate == lastThreeMonthDate || item.CallActivityDate > lastSixMonthDate)
                        {
                            item.Tag = 3;
                        }
                        else if (item.CallActivityDate == lastSixMonthDate || item.CallActivityDate > lastYearDate)
                        {
                            item.Tag = 4;
                        }
                        else
                        {
                            item.Tag = 5;
                        }
                    }
                }
                DbMapCustomerData = tmp;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPageViewModel), nameof(GetPinTagsOnTheBasisOfCallDate), ex);
            }
        }

        private List<MapCustomerData> GetPinTagsOnBasisOfCashSales(List<MapCustomerData> source)
        {
            List<MapCustomerData> tempDataSource = new List<MapCustomerData>();

            if (source != null && source.Count != 0)
            {
                foreach (var item in source)
                {
                    item.SetAmountValue();

                    tempDataSource.Add(item);
                }
            }

            return tempDataSource;
        }

        private bool ArePointsNear(Point point1, Point point2, double threshold)
        {
            double dx = point1.X - point2.X;
            double dy = point1.Y - point2.Y;
            double distanceSquared = dx * dx + dy * dy;
            return distanceSquared < threshold * threshold;
        }

        private Geopoint CalculateCenter(ICollection<PointOfInterest> places)
        {
            double totalLatitude = 0.0;
            double totalLongitude = 0.0;

            foreach (PointOfInterest place in places)
            {
                totalLatitude += place.Location.Position.Latitude;
                totalLongitude += place.Location.Position.Longitude;
            }

            return new Geopoint(new BasicGeoposition { Latitude = Convert.ToDouble(totalLatitude / places.Count), Longitude = Convert.ToDouble(totalLongitude / places.Count) });
        }

        public Point GetMapCoordinates(BasicGeoposition geoposition)
        {
            double latitude = Math.Max(Math.Min(geoposition.Latitude, 85.05112878), -85.05112878);

            double sinLatitude = Math.Sin(latitude * Math.PI / 180.0);

            return new Point
            {
                X = (geoposition.Longitude + 180.0) / 360.0,
                Y = 0.5 - Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude)) / (4 * Math.PI)
            };
        }

        #endregion

        #region Public Methods

        public async void SetLoader(bool isOpen) => await coreDispatcher.RunAsync(CoreDispatcherPriority.Normal, () => IsLoading = isOpen);

        internal void SetTheStateFlyoutList()
        {
            StateDictionary = DbStateDict.ToDictionary(x => x.Key, y => y.Value);
        }
        #endregion
    }
}
