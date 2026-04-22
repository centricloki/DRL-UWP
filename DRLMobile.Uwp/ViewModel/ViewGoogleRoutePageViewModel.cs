using DevExpress.Xpo.DB;

using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.DataSyncRequestModels;
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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.ViewModel
{
    public class ViewGoogleRoutePageViewModel : BaseModel
    {
        #region Properties

        private CancellationTokenSource _cts = null;
        private uint _desireAccuracyInMetersValue = 0;
        private RouteListUIModel SelectedRoute;

        private readonly ResourceLoader resourceLoader;

        private readonly App AppReference = (App)Application.Current;

        private string _headerTitle;
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { SetProperty(ref _headerTitle, value); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private int _routeId;
        public int RouteId
        {
            get { return _routeId; }
            set { SetProperty(ref _routeId, value); }
        }

        private string _deviceRouteId;
        public string DeviceRouteId
        {
            get { return _deviceRouteId; }
            set { SetProperty(ref _deviceRouteId, value); }
        }

        private string _routeName;
        public string RouteName
        {
            get { return _routeName; }
            set { SetProperty(ref _routeName, value); }
        }

        private string _userEmailId;
        public string UserEmailId
        {
            get { return _userEmailId; }
            set { SetProperty(ref _userEmailId, value); }
        }

        private string _userName;
        public string LoggedInUserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private Visibility _customMapPinVisibility = Visibility.Collapsed;
        public Visibility CustomMapPinVisibility
        {
            get { return _customMapPinVisibility; }
            set { SetProperty(ref _customMapPinVisibility, value); }
        }

        private bool _customMapPinIsVisible = false;
        public bool CustomMapPinIsVisible
        {
            get { return _customMapPinIsVisible; }
            set { SetProperty(ref _customMapPinIsVisible, value); }
        }

        private ObservableCollection<ViewRouteDetailsUIModel> SelectedCustomerList;
        private List<RouteRespActivity> OptimizedListRoutes;

        private ObservableCollection<ViewRouteDetailsUIModel> _routeDetailsUiItemSource;
        public ObservableCollection<ViewRouteDetailsUIModel> RouteDetailsItemSource
        {
            get { return _routeDetailsUiItemSource; }
            set { SetProperty(ref _routeDetailsUiItemSource, value); }
        }

        private ObservableCollection<ViewRouteDetailsUIModel> _headerSearchItemSource;
        public ObservableCollection<ViewRouteDetailsUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        private Visibility _loadingVisiblity;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisiblity; }
            set { SetProperty(ref _loadingVisiblity, value); }
        }

        public List<ViewRouteDetailsUIModel> RouteDetailsDBSource { get; set; }

        private string _startLocation = string.Empty;
        public string StartLocation
        {
            get { return _startLocation; }
            set { SetProperty(ref _startLocation, value); }
        }

        private string _endLocation = string.Empty;
        public string EndLocation
        {
            get { return _endLocation; }
            set { SetProperty(ref _endLocation, value); }
        }

        private bool _isStartCurrentLocation;
        public bool IsStartCurrentLocation
        {
            get { return _isStartCurrentLocation; }
            set { SetProperty(ref _isStartCurrentLocation, value); }
        }

        private bool _isEndCurrentLocation;
        public bool IsEndCurrentLocation
        {
            get { return _isEndCurrentLocation; }
            set { SetProperty(ref _isEndCurrentLocation, value); }
        }
        private ObservableCollection<PointOfInterest> _pointOfIntrestSource;
        public ObservableCollection<PointOfInterest> PointOfIntrestSource
        {
            get { return _pointOfIntrestSource; }
            set { SetProperty(ref _pointOfIntrestSource, value); }
        }

        private Geopoint _center;
        public Geopoint Center
        {
            get { return _center; }
            set { SetProperty(ref _center, value); }
        }

        private Geopoint _endGeopoint;
        public Geopoint EndGeopoint
        {
            get { return _endGeopoint; }
            set { SetProperty(ref _endGeopoint, value); }
        }

        private Geopoint _startGeopoint;
        public Geopoint StartGeopoint
        {
            get { return _startGeopoint; }
            set { SetProperty(ref _startGeopoint, value); }
        }

        private bool _isEditIconVisible;
        public bool IsEditIconVisible
        {
            get { return _isEditIconVisible; }
            set { SetProperty(ref _isEditIconVisible, value); }
        }

        private bool _isDeleteIconVisible;
        public bool IsDeleteIconVisible
        {
            get { return _isDeleteIconVisible; }
            set { SetProperty(ref _isDeleteIconVisible, value); }
        }

        private bool _isAllChecked;
        public bool IsAllChecked
        {
            get { return _isAllChecked; }
            set { SetProperty(ref _isAllChecked, value); }
        }

        public ViewRouteDetailsUIModel SelectedCustomerForPopup;
        const int MaxWaypointsLimit = 17;

        public struct GeocodeResult
        {
            public double Latitude;
            public double Longitude;
            public string City;
            public string State;
            public string Country;
            public bool Success;
        }

        /// <summary>
        /// Service to perform geocoding via the WebView2 bridge.
        /// </summary>
        public Func<string, Task<GeocodeResult?>> GeocodeService { get; set; }
        #endregion

        #region Command
        public ICommand OnNavigatedToCommand { get; private set; }
        public ICommand OnCheckBoxClicked { get; private set; }
        public ICommand EditButtonCommand { get; private set; }
        public ICommand DeleteButtonCommand { get; private set; }
        public IAsyncRelayCommand NavigationButtonCommand { get; private set; }
        public IAsyncRelayCommand CalculateButtonCommand { get; private set; }
        public ICommand HeaderSearchTextChangeCommand { private set; get; }
        public ICommand HeaderSearchSuggestionChoosenCommand { private set; get; }
        public ICommand CloseCustomMapPushPinCommand { get; private set; }
        public ICommand SelectAllCommand { get; set; }

        #endregion

        #region Map Initialization and Configuration

        /// <summary>
        /// Initializes the map view with default settings.
        /// Sets up command bindings and initializes data collections.
        /// </summary>
        public ViewGoogleRoutePageViewModel()
        {
            resourceLoader = ResourceLoader.GetForCurrentView();

            RouteDetailsDBSource = new List<ViewRouteDetailsUIModel>();
            RouteDetailsItemSource = new ObservableCollection<ViewRouteDetailsUIModel>();
            HeaderSearchItemSource = new ObservableCollection<ViewRouteDetailsUIModel>();
            PointOfIntrestSource = new ObservableCollection<PointOfInterest>();

            if (RouteDetailsItemSource != null)
            {
                RouteDetailsItemSource.CollectionChanged -= RouteDetailsItemSource_CollectionChanged;
                RouteDetailsItemSource.CollectionChanged += RouteDetailsItemSource_CollectionChanged;
            }
            RegisterCommand();

            LoadingVisibility = Visibility.Collapsed;
        }

        private void RouteDetailsItemSource_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (RouteDetailsItemSource != null)
            {
                for (int i = 0; i < RouteDetailsItemSource.Count; i++)
                {
                    RouteDetailsItemSource[i].ListIndex = i + 1;
                }
            }
        }
        #endregion

        #region Command Registration and Handling

        /// <summary>
        /// Registers all UI commands with their respective handlers.
        /// Centralizes command setup for easier maintenance.
        /// </summary>
        private void RegisterCommand()
        {
            OnNavigatedToCommand = new AsyncRelayCommand<RouteListUIModel>(OnNavigatedToCommandHandler);
            EditButtonCommand = new RelayCommand(EditButtonCommandHandler);
            DeleteButtonCommand = new AsyncRelayCommand(DeleteButtonCommandHandler);
            NavigationButtonCommand = new AsyncRelayCommand(NavigationButtonCommandHandler);
            CalculateButtonCommand = new AsyncRelayCommand(CalculateButtonCommandHandler);
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HeaderSearchTextChangeCommandHandler);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<ViewRouteDetailsUIModel>(SuggestionChoosen);
            OnCheckBoxClicked = new RelayCommand<ViewRouteDetailsUIModel>(OnCheckBoxClickedHandler);
            CloseCustomMapPushPinCommand = new RelayCommand(CloseCustomMapPushPinCommandHandler);
            SelectAllCommand = new RelayCommand(SelectAllCommandHandler);
        }

        private void CloseCustomMapPushPinCommandHandler()
        {
            CustomMapPinVisibility = Visibility.Collapsed;
            CustomMapPinIsVisible = false;
        }
        private async void OnCheckBoxClickedHandler(ViewRouteDetailsUIModel customer)
        {
            if (customer != null)
            {

                var seletedCustomer = RouteDetailsItemSource.FirstOrDefault(x => x.CustomerID == customer.CustomerID);
                if (seletedCustomer != null)
                {
                    var boolcheck = !seletedCustomer.IsChecked;
                    var checkedcount = RouteDetailsItemSource.Count(x => x.IsChecked == true);
                    if (checkedcount >= MaxWaypointsLimit && boolcheck)
                    {
                        await AlertHelper.Instance.ShowConfirmationAlert("Alert", $"Can not select more than {MaxWaypointsLimit} customers.", "OK");
                        return;
                    }
                    seletedCustomer.IsChecked = !seletedCustomer.IsChecked;
                    var index = RouteDetailsItemSource.IndexOf(seletedCustomer);
                    if (index >= 0)
                    {
                        RouteDetailsItemSource[index].IsChecked = seletedCustomer.IsChecked;
                    }

                    // Update IsAllChecked based on current state
                    UpdateSelectAllCheckboxState();
                }
            }
        }

        private void UpdateSelectAllCheckboxState()
        {
            if (RouteDetailsItemSource == null || RouteDetailsItemSource.Count == 0)
            {
                IsAllChecked = false;
                return;
            }

            var allChecked = RouteDetailsItemSource.Count(x => x.IsChecked) == RouteDetailsItemSource.Count;
            IsAllChecked = allChecked;
        }

        private async void SelectAllCommandHandler()
        {
            IsAllChecked = !IsAllChecked;

            if (RouteDetailsItemSource == null || RouteDetailsItemSource.Count == 0)
                return;

            if (IsAllChecked)
            {
                //Reset all items
                foreach (var item in RouteDetailsItemSource)
                {
                    item.IsChecked = false;
                }
                // Select only first 17 items
                for (int i = 0; i < RouteDetailsItemSource.Count; i++)
                {
                    if ((i) == MaxWaypointsLimit)
                    {
                        await AlertHelper.Instance.ShowConfirmationAlert(
                     "Selection Limit Reached",
                     $"Maximum {MaxWaypointsLimit} customers can be selected at once.\nThe first {MaxWaypointsLimit} customers have been selected.\nTo select different customers, uncheck some and try again.",
                     "OK");
                        return;
                    }
                    RouteDetailsItemSource[i].IsChecked = true;
                }
                if (RouteDetailsItemSource.Count == RouteDetailsItemSource.Where(x => x.IsChecked).Count())
                {
                    IsAllChecked = true;
                }
            }
            else
            {
                // Uncheck all items
                foreach (var item in RouteDetailsItemSource)
                {
                    item.IsChecked = false;
                }
            }
        }

        private async System.Threading.Tasks.Task OnNavigatedToCommandHandler(RouteListUIModel selectedRoute)
        {
            LoadingVisibilityHandler(isLoading: true);

            StartGeopoint = null;
            EndGeopoint = null;

            RouteId = selectedRoute.RouteId;
            DeviceRouteId = selectedRoute.DeviceRouteId;
            RouteName = selectedRoute.RouteName;
            HeaderTitle = "ROUTE - " + RouteName;
            SelectedRoute = selectedRoute;
            IsEditIconVisible = selectedRoute.EditIconVisibility == Visibility.Visible;
            IsDeleteIconVisible = selectedRoute.UserId == Convert.ToInt32(AppReference.LoginUserIdProperty);
            RouteDetailsDBSource = await AppReference.QueryService.GetRouteDetailsToView(DeviceRouteId);

            if (RouteDetailsDBSource != null && RouteDetailsDBSource.Count > 0)
            {
                int indexItem = 1;
                foreach (var item in RouteDetailsDBSource)
                {
                    item.ListIndex = indexItem;
                    RouteDetailsItemSource.Add(item);
                    indexItem++;
                }
            }

            LoadingVisibilityHandler(isLoading: false);
        }

        private async Task HeaderSearchTextChangeCommandHandler(string SearchText)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                var ifDataGridHasAlreadyData = RouteDetailsDBSource?.Count == RouteDetailsItemSource?.Count;

                if (ifDataGridHasAlreadyData)
                {
                    LoadHeaderSearchWithInitialData();
                }
                else
                {
                    await LoadDataGridAndHeaderSearchWithInitialData();
                }
            }
            else
            {
                var tempList = RouteDetailsDBSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(SearchText.ToLower())).ToList();

                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new ViewRouteDetailsUIModel() { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    tempList.ForEach(item =>
                    {
                        HeaderSearchItemSource.Add(item);
                    });
                }
            }
        }

        private async System.Threading.Tasks.Task LoadDataGridAndHeaderSearchWithInitialData()
        {
            LoadingVisibilityHandler(isLoading: true);

            await System.Threading.Tasks.Task.Delay(100);

            RouteDetailsItemSource.Clear();

            RouteDetailsDBSource.ForEach(item =>
            {
                RouteDetailsItemSource.Add(item);
            });

            HeaderSearchItemSource.Clear();

            LoadingVisibilityHandler(isLoading: false);
        }

        private void LoadHeaderSearchWithInitialData()
        {
            RouteDetailsDBSource.ForEach(item =>
            {
                HeaderSearchItemSource.Add(item);
            });
        }

        private void SuggestionChoosen(ViewRouteDetailsUIModel selectedItem)
        {
            if (selectedItem.SearchDisplayPath.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")))
            {
                return;
            }

            RouteDetailsItemSource.Clear();

            var _filterItem = RouteDetailsDBSource.FirstOrDefault(x => x.CustomerName.Equals(selectedItem.CustomerName));

            RouteDetailsItemSource.Add(_filterItem);
        }

        private async Task CalculateButtonCommandHandler()
        {
            LoadingVisibilityHandler(isLoading: true);
            StartGeopoint = null;
            EndGeopoint = null;

            var isSelected = RouteDetailsItemSource.Any(x => x.IsChecked);
            var isValidStart = !string.IsNullOrWhiteSpace(StartLocation.Trim()) || IsStartCurrentLocation;
            var isValidEnd = !string.IsNullOrWhiteSpace(EndLocation.Trim()) || IsEndCurrentLocation;

            SelectedCustomerList = new ObservableCollection<ViewRouteDetailsUIModel>();
            if (isSelected && isValidStart && isValidEnd)
            {
                bool isValidLocation = true;

                if (IsEndCurrentLocation || IsStartCurrentLocation)
                {
                    isValidLocation = await RequestLocationAccess();
                }
                if (isValidLocation)
                {
                    if (StartGeopoint == null && !string.IsNullOrWhiteSpace(StartLocation.Trim()))
                    {
                        if (int.TryParse(StartLocation, out int result))
                        {
                            if (result > 99999 || result < 9999)
                            {
                                isValidStart = false;
                            }
                            else
                            {
                                StartGeopoint = await GetGeoLocationFromAddress(StartLocation, 'S');
                                if (StartGeopoint == null)
                                {
                                    isValidStart = false;
                                }
                            }
                        }
                        else
                        {
                            StartGeopoint = await GetGeoLocationFromAddress(StartLocation, 'S');
                            if (StartGeopoint == null)
                            {
                                isValidStart = false;
                            }
                        }

                    }
                    if (EndGeopoint == null && !string.IsNullOrWhiteSpace(EndLocation.Trim()))
                    {
                        if (int.TryParse(EndLocation, out int result))
                        {
                            if (result > 99999 || result < 9999)
                            {
                                isValidEnd = false;
                            }
                            else
                            {
                                EndGeopoint = await GetGeoLocationFromAddress(EndLocation, 'E');
                                if (EndGeopoint == null)
                                {
                                    isValidEnd = false;
                                }
                            }
                        }
                        else
                        {
                            EndGeopoint = await GetGeoLocationFromAddress(EndLocation, 'E');
                            if (EndGeopoint == null)
                            {
                                isValidEnd = false;
                            }
                        }
                    }
                    if (isValidStart && isValidEnd)
                    {
                        foreach (var item in RouteDetailsItemSource)
                        {
                            if (item.IsChecked)
                            {
                                SelectedCustomerList.Add(item);
                            }
                        }

                        PointOfIntrestSource.Clear();

                        await PlotPinsForSelectedCustomers(SelectedCustomerList);

                        LoadingVisibilityHandler(isLoading: false);
                    }
                    else
                    {
                        LoadingVisibilityHandler(isLoading: false);
                        if (!isValidStart)
                        {
                            await ShowNoValidStartLocationAlert();
                        }
                        else if (!isValidEnd)
                        {
                            await ShowNoValidEndLocationAlert();
                        }
                    }
                }
            }
            else
            {
                LoadingVisibilityHandler(isLoading: false);

                if (!isSelected)
                {
                    await ShowNoCustomerAlert();
                }
                else if (!isValidStart)
                {
                    await ShowNoStartLocationAlert();
                }
                else if (!isValidEnd)
                {
                    await ShowNoEndLocationAlert();
                }
            }

            LoadingVisibilityHandler(isLoading: false);
        }
        #endregion

        #region Geocoding Operations

        /// <summary>
        /// Performs forward geocoding to convert an address string into geographic coordinates.
        /// Uses the WebView2 bridge to leverage Google Maps Geocoding API.
        /// </summary>
        /// <param name="address">The address string to geocode</param>
        /// <param name="type">Character indicating start ('S') or end ('E') location</param>
        /// <returns>Geopoint with coordinates if successful, null otherwise</returns>
        private async Task<Geopoint> GetGeoLocationFromAddress(string address, char type)
        {
            if (GeocodeService == null) return null;

            var result = await GeocodeService(address);
            if (result.HasValue && result.Value.Success)
            {
                var geoResult = result.Value;

                // Business Validation: Only US addresses allowed
                if (!geoResult.Country.Contains("United States", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                else
                {
                    if (!int.TryParse(address, out int presult))
                    {
                        if (!ContainsCityOrState(address, geoResult.City))
                            return null;
                    }

                    System.Diagnostics.Debug.WriteLine($@"[Debug] Type {type} Addr// 
                City- {geoResult.City}, State- {geoResult.State}, Country- {geoResult.Country}");

                    return new Geopoint(new BasicGeoposition { Latitude = geoResult.Latitude, Longitude = geoResult.Longitude });
                }
            }
            return null;
        }

        /// <summary>
        /// Validates that a provided address contains relevant city information.
        /// Helps prevent invalid geocoding results by checking business rules.
        /// </summary>
        /// <param name="address">The input address string</param>
        /// <param name="city">The city extracted from geocoding result</param>
        /// <returns>True if address contains city information, false otherwise</returns>
        public bool ContainsCityOrState(string address, string city)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            var tokens = SplitAddressIntoTokens(address);

            bool hasCity = !string.IsNullOrWhiteSpace(city) &&
                           tokens.Any(token => string.Equals(token, city, StringComparison.OrdinalIgnoreCase));

            return hasCity;
        }

        /// <summary>
        /// Splits an address string into individual tokens for analysis.
        /// Removes common separators and trims whitespace.
        /// </summary>
        /// <param name="address">The address to tokenize</param>
        /// <returns>List of cleaned address tokens</returns>
        private List<string> SplitAddressIntoTokens(string address)
        {
            char[] separators = { ',', ' ' };
            return address.Split(separators, StringSplitOptions.RemoveEmptyEntries)
                          .Select(s => s.Trim())
                          .Where(s => !string.IsNullOrEmpty(s))
                          .ToList();
        }
        #endregion

        #region Location Access and Permission Handling

        /// <summary>
        /// Requests access to the device's location services.
        /// Handles user permissions and retrieves current position.
        /// </summary>
        /// <returns>True if location access granted and position retrieved, false otherwise</returns>
        private async Task<bool> RequestLocationAccess()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();

                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        _cts = new CancellationTokenSource();
                        CancellationToken token = _cts.Token;
                        Geolocator geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };
                        Geoposition pos = await geolocator.GetGeopositionAsync().AsTask(token);

                        if (IsStartCurrentLocation)
                        {
                            StartGeopoint = pos.Coordinate.Point;
                        }
                        if (IsEndCurrentLocation)
                        {
                            EndGeopoint = pos.Coordinate.Point;
                        }
                        return true;

                    case GeolocationAccessStatus.Denied:
                        await ShowNoLocationAccessAlert();
                        return false;

                    case GeolocationAccessStatus.Unspecified:
                        await ShowLocationUnSpecified();
                        return false;
                    default:
                        return false;
                }
            }
            catch (TaskCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "RequestLocationAccess", ex.StackTrace);

                return false;
            }
            finally
            {
                _cts = null;
            }
        }

        private async Task PlotPinsForSelectedCustomers(ObservableCollection<ViewRouteDetailsUIModel> SelectedCustomerList)
        {
            if (SelectedCustomerList != null && SelectedCustomerList.Any())
            {
                bool isValidRoute = await ShowRouteOnMap(SelectedCustomerList);

                if (isValidRoute)
                {
                    PlotStartEndPins();
                    if (OptimizedListRoutes != null && OptimizedListRoutes.Count > 0)
                    {
                        foreach (var opitem in OptimizedListRoutes)
                        {
                            var item = SelectedCustomerList.FirstOrDefault(x => x.CustomerID.ToString().Equals(opitem.location_id));
                            if (item != null && !string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                            {
                                try
                                {
                                    PointOfIntrestSource.Add(new PointOfInterest
                                    {
                                        RouteData = item,
                                        Location = new Geopoint(new BasicGeoposition()
                                        {
                                            Latitude = Convert.ToDouble(item?.Latitude),
                                            Longitude = Convert.ToDouble(item?.Longitude)
                                        }),
                                        NormalizedAnchorPoint = new Point(0.5, 1),
                                        IsPinTextVisible = true,
                                        PinText = item.CustomerNumber,
                                        CustomerData = new MapCustomerData { CustomerID = item.CustomerID, DeviceCustomerID = item.DeviceCustomerID }
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ErrorLogger.WriteToErrorLog(nameof(ViewGoogleRoutePageViewModel), "PlotPinsForSelectedCustomers", ex.StackTrace + " - " + ex.Message);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var item in SelectedCustomerList)
                        {
                            if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                            {
                                try
                                {
                                    PointOfIntrestSource.Add(new PointOfInterest
                                    {
                                        RouteData = item,
                                        Location = new Geopoint(new BasicGeoposition()
                                        {
                                            Latitude = Convert.ToDouble(item?.Latitude),
                                            Longitude = Convert.ToDouble(item?.Longitude)
                                        }),
                                        NormalizedAnchorPoint = new Point(0.5, 1),
                                        IsPinTextVisible = true,
                                        PinText = item.CustomerNumber,
                                        CustomerData = new MapCustomerData { CustomerID = item.CustomerID, DeviceCustomerID = item.DeviceCustomerID }
                                    });
                                }
                                catch (Exception ex)
                                {
                                    ErrorLogger.WriteToErrorLog(nameof(ViewGoogleRoutePageViewModel), "PlotPinsForSelectedCustomers", ex.StackTrace + " - " + ex.Message);
                                }
                            }
                        }
                    }
                    if (!PointOfIntrestSource.Any())
                    {
                        await ShowNoRouteAlert();
                    }
                }
            }
            else
            {
                await ShowNoRouteAlert();
            }
        }

        private void PlotStartEndPins()
        {
            PointOfIntrestSource.Add(new PointOfInterest
            {
                Location = StartGeopoint,
                NormalizedAnchorPoint = new Point(0.5, 1),
                PinText = "start",
            });

            PointOfIntrestSource.Add(new PointOfInterest
            {
                Location = EndGeopoint,
                NormalizedAnchorPoint = new Point(0.5, 1),
                PinText = "End",
            });
        }

        private async Task<bool> ShowRouteOnMap(ObservableCollection<ViewRouteDetailsUIModel> SelectedCustomerList)
        {
            var webServiceResponse = await PathActivities(SelectedCustomerList);
            if (webServiceResponse != null && webServiceResponse.Any(x => x.type == "InvalidRoute"))
            {
                var problematicServiceIds = webServiceResponse.Where(x => x.type == "InvalidRoute").Select(x => x.location_id);
                var problematicCustomers = SelectedCustomerList.Where(x => problematicServiceIds.Contains(x.CustomerID.ToString())).ToList();
                string errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again.";
                if (problematicCustomers.Count == SelectedCustomerList.Count)
                {
                    errorMessage += "\n• Start/End location is far away.";
                }
                else
                {
                    errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again.";
                    foreach (var cust in problematicCustomers)
                    {
                        errorMessage += $"\n* {cust.CustomerName} (Address: {cust.CustomerAddress})";
                    }
                }
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", errorMessage, "OK");
                return false;
            }
            else if (webServiceResponse != null && webServiceResponse.Any(x => x.type == "RouteError"))
            {
                var problematicServiceIds = webServiceResponse.First().location_id.Replace("RouteError:", "");
                var problematicCustomers = SelectedCustomerList.Where(x => problematicServiceIds.Split(',').Contains(x.CustomerID.ToString())).ToList();
                string errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again.";
                if (problematicCustomers.Count == SelectedCustomerList.Count)
                {
                    errorMessage += "\n• Start/End location is far away.";
                }
                else
                {
                    errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again.";
                    foreach (var cust in problematicCustomers)
                    {
                        errorMessage += $"\n* {cust.CustomerName} (Address: {cust.CustomerAddress})";
                    }
                }
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", errorMessage, "OK");
                return false;
            }

            OptimizedListRoutes = new List<RouteRespActivity>();
            var path = new List<EnhancedWaypoint>();
            if (webServiceResponse != null)
            {
                if (webServiceResponse.Where(x => x.type == "service").Count() < SelectedCustomerList.Count)
                {
                    var problematicCustomers = SelectedCustomerList.Select(x => x.CustomerID.ToString())
                        .Except(webServiceResponse.Where(x => x.type == "service").Select(x => x.location_id)).ToList();
                    string errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again.";
                    foreach (var cust in SelectedCustomerList.Where(c => problematicCustomers.Contains(c.CustomerID.ToString())))
                    {
                        errorMessage += $"\n* {cust.CustomerName} (Address: {cust.CustomerAddress})";
                    }
                    //await InfoLogger.GetInstance.WriteToLogAsync(nameof(ViewGoogleRoutePageViewModel), nameof(ShowRouteOnMap), $"Line No. 813\n{errorMessage}");
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", errorMessage, "OK");
                    return false;
                }
                else
                {
                    path.Add(new EnhancedWaypoint(StartGeopoint, WaypointKind.Stop));
                    foreach (var activity in webServiceResponse)
                    {
                        if (activity.type == "service")
                        {
                            OptimizedListRoutes.Add(activity); //to get in the navigated button click
                            BasicGeoposition point = new BasicGeoposition() { Latitude = activity.address.lat, Longitude = activity.address.lon };
                            path.Add(new EnhancedWaypoint(new Geopoint(point), WaypointKind.Via));
                        }
                    }
                    path.Add(new EnhancedWaypoint(EndGeopoint, WaypointKind.Stop));
                }
            }
            else
            {
                path.Add(new EnhancedWaypoint(StartGeopoint, WaypointKind.Stop));
                for (int index = 0; index < SelectedCustomerList.Count; index++)
                {
                    var item = SelectedCustomerList[index];
                    if (!string.IsNullOrEmpty(item.Latitude) && !string.IsNullOrEmpty(item.Longitude))
                    {
                        BasicGeoposition point = new BasicGeoposition() { Latitude = Convert.ToDouble(item.Latitude), Longitude = Convert.ToDouble(item.Longitude) };
                        path.Add(new EnhancedWaypoint(new Geopoint(point), WaypointKind.Via));
                    }
                }
                path.Add(new EnhancedWaypoint(EndGeopoint, WaypointKind.Stop));
            }
            // In the Google Maps implementation, the actual pathfinding happens in JavaScript.
            // We've already verified the customers are reachable via PathActivities (GraphHopper).
            // This method now just returns true if basic conditions are met.
            return webServiceResponse != null;
        }
        private async Task<List<RouteRespActivity>> PathActivities(ObservableCollection<ViewRouteDetailsUIModel> SelectedCustomerList)
        {
            var optpath = new List<RouteService>();
            var startloc = new RouteAddress() { location_id = "startloc", lat = Convert.ToDouble(StartGeopoint.Position.Latitude), lon = Convert.ToDouble(StartGeopoint.Position.Longitude) };

            for (int index = 0; index < SelectedCustomerList.Count; index++)
            {
                var item = SelectedCustomerList[index];
                if (!string.IsNullOrEmpty(item.Latitude) && !string.IsNullOrEmpty(item.Longitude))
                {
                    var routeAddress = new RouteAddress() { location_id = item.CustomerID.ToString(), lat = Convert.ToDouble(item.Latitude), lon = Convert.ToDouble(item.Longitude) };
                    optpath.Add(new RouteService() { id = item.CustomerID.ToString(), name = item.CustomerID.ToString(), address = routeAddress });
                }
            }
            var endloc = new RouteAddress() { location_id = "endloc", lat = Convert.ToDouble(EndGeopoint.Position.Latitude), lon = Convert.ToDouble(EndGeopoint.Position.Longitude) };
            var webServiceResponse = await InvokeWebService.GetOptmizedRoute(startloc, optpath, endloc).ConfigureAwait(false);
            return webServiceResponse;


        }
        private Task ShowNoEndLocationAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please enter End location.", "OK");
        }

        private Task ShowNoValidEndLocationAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "End location is not valid. Please enter valid End location.", "OK");
        }

        private Task ShowNoStartLocationAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please enter Start location.", "OK");
        }

        private Task ShowNoValidStartLocationAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Start location is not valid. Please enter valid Start location.", "OK");
        }
        private Task ShowNoCustomerAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Please select atleast 1 Customer.", "OK");
        }

        private Task ShowNoRouteAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "There seems to be an issue with one of the addresses. Please correct it and attempt routing again.", "OK");
        }

        private Task ShowNoLocationAccessAlert()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "Access to location is denied. Please go to the settings and enable the location permission", "OK");
        }

        private Task ShowLocationUnSpecified()
        {
            return AlertHelper.Instance.ShowConfirmationAlert("Alert", "The Device does not have location capabilities", "OK");
        }

        private async Task DeleteButtonCommandHandler()
        {
            ContentDialog deleteRouteDialog = new ContentDialog
            {
                Title = resourceLoader.GetString("DeleteRouteText"),
                Content = resourceLoader.GetString("DeleteRouteMessage"),
                PrimaryButtonText = resourceLoader.GetString("YesText"),
                SecondaryButtonText = resourceLoader.GetString("NoText")
            };

            var result = await deleteRouteDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var data = await AppReference.QueryService.DeleteScheduledRoute(DeviceRouteId);

                if (data)
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Delete Route", "The route has been deleted", "OK");
                    NavigationService.GoBackInShell();
                }
                else
                {
                    await AlertHelper.Instance.ShowConfirmationAlert("Error", "Something went wrong, could not delete this route. Please try again", "OK");
                }
            }
        }

        private void EditButtonCommandHandler()
        {
            NavigationService.NavigateShellFrame(typeof(AddEditRoutePage), SelectedRoute);
        }

        public void LoadingVisibilityHandler(bool isLoading)
        {
            LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Map Initialization and Configuration
        private double CalculateDistanceKm(BasicGeoposition from, BasicGeoposition to)
        {
            const double EarthRadiusKm = 6371.0;

            var lat1 = DegreesToRadians(from.Latitude);
            var lon1 = DegreesToRadians(from.Longitude);
            var lat2 = DegreesToRadians(to.Latitude);
            var lon2 = DegreesToRadians(to.Longitude);

            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1) * Math.Cos(lat2) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);

        private async Task FindDistantWaypointsAsync(
    List<EnhancedWaypoint> waypoints,
    double maxDistanceKm = 300.0)
        {
            if (waypoints == null || waypoints.Count < 2)
                return;

            int halfCount = waypoints.Count / 2;
            int clusterSize = Math.Min(halfCount, waypoints.Count);
            var clusterPositions = waypoints.Take(clusterSize)
                                           .Select(w => w.Point.Position)
                                           .ToList();

            // Compute centroid of the cluster
            var centroid = new BasicGeoposition
            {
                Latitude = clusterPositions.Average(p => p.Latitude),
                Longitude = clusterPositions.Average(p => p.Longitude)
            };

            var distantWaypointMessages = new List<string>();
            int countWayPoint = waypoints.Count;
            //List<string> debugLogs = new List<string>();
            for (int i = 0; i < waypoints.Count; i++)
            {
                var wp = waypoints[i];
                var distanceKm = CalculateDistanceKm(centroid, wp.Point.Position);
                //debugLogs.Add($@"Debug DistantWaypointInfo Index {i}, DistanceFromClusterKm {distanceKm}, Latitude {wp.Point.Position.Latitude}, Longitude {wp.Point.Position.Longitude}");
                //Debug.WriteLine($@"Debug DistantWaypointInfo Index {i}, DistanceFromClusterKm {distanceKm}, Latitude {wp.Point.Position.Latitude}, Longitude {wp.Point.Position.Longitude}");

                if (distanceKm > maxDistanceKm)
                {
                    string message = null;
                    if (i == 0)
                        message = "Start location is far away.";
                    else if (i == (countWayPoint - 1))
                        message = "End location is far away.";
                    else
                    {
                        var selectedWayPoint = SelectedCustomerList
                            .FirstOrDefault(p => Math.Abs(wp.Point.Position.Latitude - Convert.ToDouble(p.Latitude)) < 0.0001
                                              && Math.Abs(wp.Point.Position.Longitude - Convert.ToDouble(p.Longitude)) < 0.0001);

                        if (selectedWayPoint != null)
                        {
                            message = $"{selectedWayPoint.CustomerName} (Address: {selectedWayPoint.CustomerAddress}).";
                        }
                        //else
                        //{
                        //    message = $"Waypoint at ({wp.Point.Position.Latitude}, {wp.Point.Position.Longitude}) is far away.";
                        //}
                    }
                    distantWaypointMessages.Add($"• {message}");
                }
            }

            if (distantWaypointMessages.Any())
            {
                var fullMessage = $"The following addresses are preventing route creation.\nPlease review or remove them and try again.\n" +
                                 string.Join("\n", distantWaypointMessages);
                //await InfoLogger.GetInstance.WriteToLogAsync(nameof(ViewGoogleRoutePageViewModel), nameof(FindDistantWaypointsAsync), $"{fullMessage}\n{string.Join("\n", debugLogs)}");
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", fullMessage, "OK");
            }
            else
            {
                await ShowNoRouteAlert();
            }
        }

        // Helper method to check if waypoints are in the same geographic region
        private bool AreWaypointsInSameRegion(List<EnhancedWaypoint> waypoints)
        {
            if (waypoints.Count < 2) return true;

            // Get the first point as reference
            var firstPoint = waypoints[0].Point.Position;

            foreach (var waypoint in waypoints.Skip(1))
            {
                var currentPoint = waypoint.Point.Position;

                // Check if points are on different continents (rough approximation)
                // Large longitude differences (> 60 degrees) often indicate different continents
                double longitudeDifference = Math.Abs(firstPoint.Longitude - currentPoint.Longitude);
                double latitudeDifference = Math.Abs(firstPoint.Latitude - currentPoint.Latitude);

                // If longitude difference is > 60 degrees, likely different continents
                if (longitudeDifference > 60)
                {
                    return false;
                }

                // Additional check: if both lat and lon differences are very large
                if (longitudeDifference > 40 && latitudeDifference > 20)
                {
                    return false;
                }
            }

            return true;
        }

        private async Task NavigationButtonCommandHandler()
        {
            if (PointOfIntrestSource == null || SelectedCustomerList == null)
            {
                LoadingVisibilityHandler(isLoading: false);
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "To get the driving direction, please plot the route using Calculate button.", "OK");
                return;
            }

            LoadingVisibilityHandler(isLoading: true);

            try
            {
                // === Step 1: Build list of waypoint info (address + coords) ===
                var waypointInfos = new List<(string Address, string Latitude, string Longitude)>();

                if (OptimizedListRoutes != null && OptimizedListRoutes.Count > 0)
                {
                    foreach (var route in OptimizedListRoutes)
                    {
                        var customer = RouteDetailsItemSource.FirstOrDefault(x => x.CustomerID.ToString() == route.location_id);
                        if (customer != null)
                        {
                            string cleanAddress = customer.CustomerAddress ?? $"{customer.Latitude},{customer.Longitude}";
                            waypointInfos.Add((cleanAddress, customer.Latitude, customer.Longitude));
                        }
                    }
                }
                else
                {
                    foreach (var item in SelectedCustomerList)
                    {
                        string coordAddress = $"{item.Latitude},{item.Longitude}";
                        waypointInfos.Add((coordAddress, item.Latitude, item.Longitude));
                    }
                }

                // Start & End as (Address, Lat, Lon)
                var startInfo = (
                    Address: $"{StartGeopoint.Position.Latitude},{StartGeopoint.Position.Longitude}",
                    Latitude: StartGeopoint.Position.Latitude,
                    Longitude: StartGeopoint.Position.Longitude
                );

                var endInfo = (
                    Address: $"{EndGeopoint.Position.Latitude},{EndGeopoint.Position.Longitude}",
                    Latitude: EndGeopoint.Position.Latitude,
                    Longitude: EndGeopoint.Position.Longitude
                );

                // === Step 1: Generate Google URLs (using addresses if available, split if needed) ===
                List<string> googleMapsUrls = BuildGoogleMapsUrlsWithAddresses(startInfo, waypointInfos, endInfo);

                // === Step 2: Get user info & send email ===
                var user = await (Application.Current as App).QueryService.GetLoggedInUserInformation(Convert.ToInt32(((App)Application.Current).LoginUserIdProperty));
                LoggedInUserName = user.FirstName + " " + user.LastName;
                UserEmailId = user.EmailId;

                await SendNavigationDirectionEmail(googleMapsUrls);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "NavigationButtonCommandHandler", ex.ToString());
                LoadingVisibilityHandler(isLoading: false);
                await AlertHelper.Instance.ShowConfirmationAlert("Error", "Failed to generate route links. Please try again.", "OK");
            }
        }

        private string CleanAddressString(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return address;
            string cleaned = address.Trim();
            cleaned = Regex.Replace(cleaned, @"\r\n|\r|\n", " ").Replace("#", "%23");
            cleaned = Regex.Replace(cleaned, @"\s+", "+");
            return cleaned;
        }

        private string BuildGoogleMapsUrlSegment(List<string> allPoints)
        {
            string origin = allPoints.First();
            string destination = allPoints.Last();
            string url = null;
            if (allPoints.Count > 2)
            {
                // Exclude first and last → only middle waypoints
                var waypointList = allPoints.Skip(1).Take(allPoints.Count - 2).ToList();
                string waypoints = waypointList.Any()
                    ? string.Join("|", waypointList)
                    : ""; // Google accepts empty waypoints

                url = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&travelmode=driving";

                if (!string.IsNullOrEmpty(waypoints))
                {
                    url += $"&waypoints={waypoints}";
                }
            }
            else
            {
                url = $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&travelmode=driving";
            }
            return url;
        }


        // ===== Helper: Build Google Maps URLs using addresses (with fallback to coords) =====
        private List<string> BuildGoogleMapsUrlsWithAddresses(
            (string Address, double Latitude, double Longitude) start,
            List<(string Address, string Latitude, string Longitude)> waypoints,
            (string Address, double Latitude, double Longitude) end)
        {
            var urls = new List<string>();
            int MaxWaypointsPerSegment = 8; // Google allows max 8 intermediate stops
            if (waypoints.Count <= MaxWaypointsPerSegment)
            {
                var allPoints = new List<string>
                {
                    start.Address
                };
                allPoints.AddRange(waypoints.Select(w => CleanAddressString(w.Address)));
                allPoints.Add(end.Address);
                urls.Add(BuildGoogleMapsUrlSegment(allPoints));
            }
            else
            {
                // Split into segments — overlap at boundary points
                int startIndex = 0;
                var currentStart = start;
                MaxWaypointsPerSegment = 9;
                int countWayPoint = waypoints.Count + 1;
                while (startIndex < countWayPoint)
                {
                    int takeCount = Math.Min(MaxWaypointsPerSegment, countWayPoint - startIndex);
                    var segmentWaypoints = waypoints.Skip(startIndex).Take(takeCount).ToList();

                    // Determine segment end
                    (string Address, double Latitude, double Longitude) segmentEnd;
                    if (startIndex + takeCount >= countWayPoint)
                    {
                        segmentEnd = end; // last segment ends at final destination
                    }
                    else
                    {
                        // End at last waypoint of this segment (so next starts there)
                        var lastWp = segmentWaypoints.Last();
                        segmentEnd = (CleanAddressString(lastWp.Address), Convert.ToDouble(lastWp.Latitude), Convert.ToDouble(lastWp.Longitude));
                    }

                    // Build URL for this segment
                    var points = new List<string>
                    {
                        currentStart.Address
                    };
                    points.AddRange(segmentWaypoints.Take(takeCount < MaxWaypointsPerSegment ? takeCount : takeCount - 1)
                    .Select(w => CleanAddressString(w.Address)));
                    points.Add(segmentEnd.Address);
                    urls.Add(BuildGoogleMapsUrlSegment(points));

                    // Next segment starts where this one ended
                    currentStart = segmentEnd;
                    startIndex += takeCount;
                }
            }

            return urls;
        }

        // ===== Updated Email Method (same as before, but kept for completeness) =====
        private async Task SendNavigationDirectionEmail(List<string> googleMapsUrls)
        {
            try
            {
                if (string.IsNullOrEmpty(UserEmailId) || string.IsNullOrEmpty(LoggedInUserName))
                {
                    LoadingVisibilityHandler(isLoading: false);
                    await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Missing email id! Could not send email.", "OK");
                    return;
                }
                var body = new StringBuilder();
                // Google Maps
                if (googleMapsUrls.Count > 1)
                {
                    body.AppendLine("<p>Please use these URLs for driving direction:</p>");
                    body.AppendLine("<p>Kindly start with Part 1 of 2, and once you complete it, continue your journey using Part 2 of 2.</p>");
                    body.AppendLine("<p>Driving Directions:");
                    for (int i = 0; i < googleMapsUrls.Count; i++)
                    {
                        body.AppendLine($"<p>{i + 1}. Part {i + 1} of {googleMapsUrls.Count}: {googleMapsUrls[i]}</p>");
                    }
                }
                else
                {
                    body.AppendLine($"<p>Please use this url for driving direction: {googleMapsUrls[0]}</p>");
                }

                string emailBodyHtml = body.ToString();

                var emailModel = new EmailModel()
                {
                    Subject = "Driving Directions For Route - " + RouteName,
                    BodyHtml = emailBodyHtml,
                    To = new List<string> { UserEmailId }
                };

                bool isEmailSent = await EmailService.Instance.SendMailFromOutlook(emailModel);
                await Task.Delay(isEmailSent ? 8000 : 2000);
                LoadingVisibilityHandler(isLoading: false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, "SendNavigationDirectionEmail", ex.ToString());
                LoadingVisibilityHandler(isLoading: false);
                await AlertHelper.Instance.ShowConfirmationAlert("Alert", "Failed to send email. Please try again.", "OK");
            }
        }

        #endregion
    }
}