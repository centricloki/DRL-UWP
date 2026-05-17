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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.Devices.Sms;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.ViewModel
{
    /// <summary>
    /// ViewModel for the Route List page, handling route display, calculation, and navigation functionality.
    /// Manages customer selection, geocoding, route optimization, and map pin visualization.
    /// </summary>
    public class ViewRouteListPageViewModel : BaseModel, IDisposable
    {
        #region Constants

        /// <summary>Maximum number of waypoints (customers) allowed in a single route calculation.</summary>
        private const int MaxWaypointsLimit = 17;

        /// <summary>Minimum value for a valid 5-digit US zip code.</summary>
        private const int MinZipCode = 10000;

        /// <summary>Maximum value for a valid 5-digit US zip code.</summary>
        private const int MaxZipCode = 99999;

        /// <summary>Default region suffix for geocoding queries when country is not specified.</summary>
        private const string DefaultGeocodeRegion = ",USA";

        /// <summary>Default accuracy in meters for location requests.</summary>
        private const uint DefaultLocationAccuracyInMeters = 0;

        /// <summary>Earth radius in kilometers for distance calculations (Haversine formula).</summary>
        private const double EarthRadiusKm = 6371.0;

        /// <summary>Maximum distance in kilometers to consider a waypoint "distant" from cluster centroid.</summary>
        private const double MaxWaypointDistanceFromClusterKm = 300.0;

        /// <summary>Maximum waypoints per segment when splitting Google Maps URLs.</summary>
        private const int MaxGoogleMapsWaypointsPerSegment = 9;

        #endregion

        #region Private Fields

        private SmsDevice2 _smsDevice;
        private CancellationTokenSource _cts;
        private uint _desireAccuracyInMetersValue = DefaultLocationAccuracyInMeters;
        private RouteListUIModel _selectedRoute;
        private readonly ResourceLoader _resourceLoader;
        private readonly App _appReference = (App)Application.Current;
        private bool _isDisposed;

        // Collections initialized at declaration to reduce constructor clutter
        private ObservableCollection<ViewRouteDetailsUIModel> _routeDetailsUiItemSource = new ObservableCollection<ViewRouteDetailsUIModel>();
        private ObservableCollection<ViewRouteDetailsUIModel> _headerSearchItemSource = new ObservableCollection<ViewRouteDetailsUIModel>();
        private ObservableCollection<PointOfInterest> _pointOfInterestSource = new ObservableCollection<PointOfInterest>();

        // Internal collections for route processing
        private ObservableCollection<ViewRouteDetailsUIModel> _selectedCustomerList;
        private List<RouteRespActivity> _optimizedListRoutes;

        // Backing fields for public properties
        private string _headerTitle;
        private bool _isLoading;
        private int _routeId;
        private string _deviceRouteId;
        private string _routeName;
        private string _userEmailId;
        private string _userName;
        private Visibility _customMapPinVisibility = Visibility.Collapsed;
        private bool _customMapPinIsVisible;
        private Visibility _loadingVisibility;
        private string _startLocation = string.Empty;
        private string _endLocation = string.Empty;
        private bool _isStartCurrentLocation;
        private bool _isEndCurrentLocation;
        private OnTerra.MapsControl.UWP.Geopoint _center;
        private OnTerra.MapsControl.UWP.Geopoint _endGeopoint;
        private OnTerra.MapsControl.UWP.Geopoint _startGeopoint;
        private bool _isEditIconVisible;
        private bool _isDeleteIconVisible;
        private bool _isAllChecked;
        private ViewRouteDetailsUIModel _selectedCustomerForPopup;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the header title displayed on the page.
        /// Bound to XAML HeaderControl Title property.
        /// </summary>
        public string HeaderTitle
        {
            get { return _headerTitle; }
            set { SetProperty(ref _headerTitle, value); }
        }

        /// <summary>
        /// Gets or sets the loading state indicator.
        /// Used to show/hide progress indicators during async operations.
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        /// <summary>
        /// Gets or sets the route identifier from the database.
        /// </summary>
        public int RouteId
        {
            get { return _routeId; }
            set { SetProperty(ref _routeId, value); }
        }

        /// <summary>
        /// Gets or sets the device-specific route identifier.
        /// Used for API calls to fetch route details.
        /// </summary>
        public string DeviceRouteId
        {
            get { return _deviceRouteId; }
            set { SetProperty(ref _deviceRouteId, value); }
        }

        /// <summary>
        /// Gets or sets the route name displayed in the UI.
        /// </summary>
        public string RouteName
        {
            get { return _routeName; }
            set { SetProperty(ref _routeName, value); }
        }

        /// <summary>
        /// Gets or sets the user's email ID for sending navigation directions.
        /// </summary>
        public string UserEmailId
        {
            get { return _userEmailId; }
            set { SetProperty(ref _userEmailId, value); }
        }

        /// <summary>
        /// Gets or sets the logged-in user's full name.
        /// </summary>
        public string LoggedInUserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the custom map pin control.
        /// Bound to XAML Visibility property for conditional rendering.
        /// </summary>
        public Visibility CustomMapPinVisibility
        {
            get { return _customMapPinVisibility; }
            set { SetProperty(ref _customMapPinVisibility, value); }
        }

        /// <summary>
        /// Gets or sets whether the custom map pin is currently visible.
        /// Used for internal state management.
        /// </summary>
        public bool CustomMapPinIsVisible
        {
            get { return _customMapPinIsVisible; }
            set { SetProperty(ref _customMapPinIsVisible, value); }
        }

        /// <summary>
        /// Gets or sets the collection of route details displayed in the DataGrid.
        /// Bound to XAML ItemsSource for customer list display.
        /// </summary>
        public ObservableCollection<ViewRouteDetailsUIModel> RouteDetailsItemSource
        {
            get { return _routeDetailsUiItemSource; }
            set { SetProperty(ref _routeDetailsUiItemSource, value); }
        }

        /// <summary>
        /// Gets or sets the collection used for header search suggestions.
        /// Bound to AutoSuggestBox ItemsSource for search functionality.
        /// </summary>
        public ObservableCollection<ViewRouteDetailsUIModel> HeaderSearchItemSource
        {
            get { return _headerSearchItemSource; }
            set { SetProperty(ref _headerSearchItemSource, value); }
        }

        /// <summary>
        /// Gets or sets the visibility of the loading indicator overlay.
        /// Bound to XAML Visibility property for progress UI.
        /// </summary>
        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set { SetProperty(ref _loadingVisibility, value); }
        }

        /// <summary>
        /// Gets or sets the raw route details fetched from the database.
        /// Internal use only; not bound to UI.
        /// </summary>
        public List<ViewRouteDetailsUIModel> RouteDetailsDBSource { get; set; } = new List<ViewRouteDetailsUIModel>();

        /// <summary>
        /// Gets or sets the start location input value from the UI.
        /// Bound to TextBox Text property for user input.
        /// </summary>
        public string StartLocation
        {
            get { return _startLocation; }
            set { SetProperty(ref _startLocation, value); }
        }

        /// <summary>
        /// Gets or sets the end location input value from the UI.
        /// Bound to TextBox Text property for user input.
        /// </summary>
        public string EndLocation
        {
            get { return _endLocation; }
            set { SetProperty(ref _endLocation, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the device's current location as the start point.
        /// Bound to CheckBox IsChecked property.
        /// </summary>
        public bool IsStartCurrentLocation
        {
            get { return _isStartCurrentLocation; }
            set { SetProperty(ref _isStartCurrentLocation, value); }
        }

        /// <summary>
        /// Gets or sets whether to use the device's current location as the end point.
        /// Bound to CheckBox IsChecked property.
        /// </summary>
        public bool IsEndCurrentLocation
        {
            get { return _isEndCurrentLocation; }
            set { SetProperty(ref _isEndCurrentLocation, value); }
        }

        /// <summary>
        /// Gets or sets the collection of points of interest to display on the map.
        /// Bound to OnTerra MapControl POI source for pin visualization.
        /// </summary>
        public ObservableCollection<PointOfInterest> PointOfInterestSource
        {
            get { return _pointOfInterestSource; }
            set { SetProperty(ref _pointOfInterestSource, value); }
        }

        /// <summary>
        /// Gets or sets the center point of the map view.
        /// Bound to MapControl Center property for viewport positioning.
        /// </summary>
        public OnTerra.MapsControl.UWP.Geopoint Center
        {
            get { return _center; }
            set { SetProperty(ref _center, value); }
        }

        /// <summary>
        /// Gets or sets the geopoint for the route end location.
        /// Used for route calculation and map pin placement.
        /// </summary>
        public OnTerra.MapsControl.UWP.Geopoint EndGeopoint
        {
            get { return _endGeopoint; }
            set { SetProperty(ref _endGeopoint, value); }
        }

        /// <summary>
        /// Gets or sets the geopoint for the route start location.
        /// Used for route calculation and map pin placement.
        /// </summary>
        public OnTerra.MapsControl.UWP.Geopoint StartGeopoint
        {
            get { return _startGeopoint; }
            set { SetProperty(ref _startGeopoint, value); }
        }

        /// <summary>
        /// Gets or sets whether the edit icon should be visible in the UI.
        /// Controls visibility of edit action button.
        /// </summary>
        public bool IsEditIconVisible
        {
            get { return _isEditIconVisible; }
            set { SetProperty(ref _isEditIconVisible, value); }
        }

        /// <summary>
        /// Gets or sets whether the delete icon should be visible in the UI.
        /// Controls visibility of delete action button based on user permissions.
        /// </summary>
        public bool IsDeleteIconVisible
        {
            get { return _isDeleteIconVisible; }
            set { SetProperty(ref _isDeleteIconVisible, value); }
        }

        /// <summary>
        /// Gets or sets the state of the "Select All" checkbox.
        /// Bound to CheckBox IsChecked for bulk selection functionality.
        /// </summary>
        public bool IsAllChecked
        {
            get { return _isAllChecked; }
            set { SetProperty(ref _isAllChecked, value); }
        }

        /// <summary>
        /// Gets or sets the selected customer for popup display.
        /// Used by ViewRouteListPage.xaml.cs line 339 for MapPinCustomPopUp binding.
        /// </summary>
        public ViewRouteDetailsUIModel SelectedCustomerForPopup
        {
            get { return _selectedCustomerForPopup; }
            set { SetProperty(ref _selectedCustomerForPopup, value); }
        }

        /// <summary>
        /// Gets or sets the reference to the OnTerra map control.
        /// Set by code-behind for route plotting and geocoding operations.
        /// </summary>
        public OnTerra.MapsControl.UWP.MapControl OnTerraMap { get; set; }

        #endregion

        #region Commands

        /// <summary>
        /// Command executed when the page is navigated to.
        /// Initializes route data and UI state.
        /// </summary>
        public ICommand OnNavigatedToCommand { get; private set; }

        /// <summary>
        /// Command executed when a customer checkbox is clicked.
        /// Handles selection logic and waypoint limit enforcement.
        /// </summary>
        public ICommand OnCheckBoxClicked { get; private set; }

        /// <summary>
        /// Command executed when the edit button is clicked.
        /// Navigates to the Add/Edit route page.
        /// </summary>
        public ICommand EditButtonCommand { get; private set; }

        /// <summary>
        /// Command executed when the delete button is clicked.
        /// Shows confirmation dialog and deletes the route if confirmed.
        /// </summary>
        public ICommand DeleteButtonCommand { get; private set; }

        /// <summary>
        /// Command executed when the navigation button is clicked.
        /// Generates Google Maps URLs and sends navigation directions via email.
        /// </summary>
        public ICommand NavigationButtonCommand { get; private set; }

        /// <summary>
        /// Command executed when the calculate button is clicked.
        /// Validates inputs, geocodes locations, and plots optimized route on map.
        /// </summary>
        public IAsyncRelayCommand CalculateButtonCommand { get; private set; }

        /// <summary>
        /// Command executed when header search text changes.
        /// Filters customer list based on search input.
        /// </summary>
        public ICommand HeaderSearchTextChangeCommand { get; private set; }

        /// <summary>
        /// Command executed when a search suggestion is chosen.
        /// Filters the main DataGrid to show only the selected customer.
        /// </summary>
        public ICommand HeaderSearchSuggestionChoosenCommand { get; private set; }

        /// <summary>
        /// Command executed to close the custom map push pin popup.
        /// Hides the popup UI element.
        /// </summary>
        public ICommand CloseCustomMapPushPinCommand { get; private set; }

        /// <summary>
        /// Command executed to select or deselect all customers.
        /// Toggles selection state with waypoint limit enforcement.
        /// </summary>
        public ICommand SelectAllCommand { get; set; }

        #endregion

        #region Constructor & Cleanup

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewRouteListPageViewModel"/> class.
        /// Sets up resource loader, event subscriptions, and command registrations.
        /// </summary>
        public ViewRouteListPageViewModel()
        {
            _resourceLoader = ResourceLoader.GetForCurrentView();

            // Subscribe to collection changed event with safety check to prevent duplicate subscriptions
            if (RouteDetailsItemSource != null)
            {
                RouteDetailsItemSource.CollectionChanged -= RouteDetailsItemSource_CollectionChanged;
                RouteDetailsItemSource.CollectionChanged += RouteDetailsItemSource_CollectionChanged;
            }

            RegisterCommands();
            LoadingVisibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Unsubscribes from events and cancels pending async operations.
        /// Call this when navigating away from the page to prevent memory leaks.
        /// Should be called from ViewRouteListPage.xaml.cs OnNavigatedFrom override.
        /// </summary>
        public void Cleanup()
        {
            if (_isDisposed) return;

            // Unsubscribe from collection changed event
            if (RouteDetailsItemSource != null)
            {
                RouteDetailsItemSource.CollectionChanged -= RouteDetailsItemSource_CollectionChanged;
            }

            // Cancel and dispose cancellation token source
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            // Clear collections to release references
            RouteDetailsItemSource?.Clear();
            HeaderSearchItemSource?.Clear();
            PointOfInterestSource?.Clear();

            _isDisposed = true;
        }

        /// <summary>
        /// Implements the IDisposable pattern for deterministic cleanup of resources.
        /// </summary>
        public void Dispose() => Cleanup();

        /// <summary>
        /// Event handler for RouteDetailsItemSource collection changes.
        /// Updates the ListIndex property for each item to maintain correct display order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The NotifyCollectionChangedEventArgs containing event data.</param>
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

        #region Command Registrations

        /// <summary>
        /// Registers all ICommand properties with their corresponding handler methods.
        /// Uses AsyncRelayCommand for async Task handlers to ensure proper exception handling.
        /// </summary>
        private void RegisterCommands()
        {
            OnNavigatedToCommand = new AsyncRelayCommand<RouteListUIModel>(OnNavigatedToCommandHandlerAsync);
            EditButtonCommand = new RelayCommand(EditButtonCommandHandler);
            DeleteButtonCommand = new AsyncRelayCommand(DeleteButtonCommandHandlerAsync);
            NavigationButtonCommand = new AsyncRelayCommand(NavigationButtonCommandHandlerAsync);
            CalculateButtonCommand = new AsyncRelayCommand<OnTerra.MapsControl.UWP.MapControl>(CalculateButtonCommandHandlerAsync);
            HeaderSearchTextChangeCommand = new AsyncRelayCommand<string>(HeaderSearchTextChangeCommandHandlerAsync);
            HeaderSearchSuggestionChoosenCommand = new RelayCommand<ViewRouteDetailsUIModel>(SuggestionChoosen);
            OnCheckBoxClicked = new AsyncRelayCommand<ViewRouteDetailsUIModel>(OnCheckBoxClickedHandlerAsync);
            CloseCustomMapPushPinCommand = new RelayCommand(CloseCustomMapPushPinCommandHandler);
            SelectAllCommand = new AsyncRelayCommand(SelectAllCommandHandlerAsync);
        }

        #endregion

        #region Command Handlers

        /// <summary>
        /// Handles the CloseCustomMapPushPinCommand execution.
        /// Hides the custom map pin popup UI.
        /// </summary>
        private void CloseCustomMapPushPinCommandHandler()
        {
            CustomMapPinVisibility = Visibility.Collapsed;
            CustomMapPinIsVisible = false;
        }

        /// <summary>
        /// Handles checkbox click events for customer selection.
        /// Enforces maximum waypoint limit and updates UI state.
        /// </summary>
        /// <param name="customer">The customer item that was clicked.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnCheckBoxClickedHandlerAsync(ViewRouteDetailsUIModel customer)
        {
            if (customer == null) return;

            var selectedCustomer = RouteDetailsItemSource.FirstOrDefault(x => x.CustomerID == customer.CustomerID);
            if (selectedCustomer == null) return;

            var shouldCheck = !selectedCustomer.IsChecked;
            var checkedCount = RouteDetailsItemSource.Count(x => x.IsChecked);

            if (checkedCount >= MaxWaypointsLimit && shouldCheck)
            {
                await ShowAlertAsync("Alert", $"Can not select more than {MaxWaypointsLimit} customers.");
                return;
            }

            selectedCustomer.IsChecked = shouldCheck;
            var index = RouteDetailsItemSource.IndexOf(selectedCustomer);
            if (index >= 0)
            {
                RouteDetailsItemSource[index].IsChecked = selectedCustomer.IsChecked;
            }

            UpdateSelectAllCheckboxState();
        }

        /// <summary>
        /// Updates the IsAllChecked property based on current selection state.
        /// Called after individual checkbox changes to maintain UI consistency.
        /// </summary>
        private void UpdateSelectAllCheckboxState()
        {
            IsAllChecked = RouteDetailsItemSource?.Count > 0 && RouteDetailsItemSource.All(x => x.IsChecked);
        }

        /// <summary>
        /// Handles the SelectAllCommand execution.
        /// Toggles selection of all customers with waypoint limit enforcement.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task SelectAllCommandHandlerAsync()
        {
            if (RouteDetailsItemSource == null || RouteDetailsItemSource.Count == 0) return;

            IsAllChecked = !IsAllChecked;

            if (IsAllChecked)
            {
                // Reset all items first
                foreach (var item in RouteDetailsItemSource) item.IsChecked = false;

                // Select only first MaxWaypointsLimit items
                for (int i = 0; i < RouteDetailsItemSource.Count; i++)
                {
                    if (i >= MaxWaypointsLimit)
                    {
                        await ShowAlertAsync("Selection Limit Reached",
                            $"Maximum {MaxWaypointsLimit} customers can be selected at once.\nThe first {MaxWaypointsLimit} customers have been selected.\nTo select different customers, uncheck some and try again.");
                        return;
                    }
                    RouteDetailsItemSource[i].IsChecked = true;
                }
                UpdateSelectAllCheckboxState();
            }
            else
            {
                foreach (var item in RouteDetailsItemSource) item.IsChecked = false;
            }
        }

        /// <summary>
        /// Handles page navigation initialization.
        /// Loads route details from database and populates UI collections.
        /// </summary>
        /// <param name="selectedRoute">The route selected from the previous page.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task OnNavigatedToCommandHandlerAsync(RouteListUIModel selectedRoute)
        {
            LoadingVisibilityHandler(isLoading: true);
            StartGeopoint = null;
            EndGeopoint = null;

            RouteId = selectedRoute.RouteId;
            DeviceRouteId = selectedRoute.DeviceRouteId;
            RouteName = selectedRoute.RouteName;
            HeaderTitle = $"ROUTE - {RouteName}";
            _selectedRoute = selectedRoute;
            IsEditIconVisible = selectedRoute.EditIconVisibility == Visibility.Visible;
            IsDeleteIconVisible = selectedRoute.UserId == Convert.ToInt32(_appReference.LoginUserIdProperty);

            RouteDetailsDBSource = await _appReference.QueryService.GetRouteDetailsToView(DeviceRouteId);

            if (RouteDetailsDBSource != null && RouteDetailsDBSource.Count > 0)
            {
                int indexItem = 1;
                foreach (var item in RouteDetailsDBSource)
                {
                    item.ListIndex = indexItem++;
                    RouteDetailsItemSource.Add(item);
                }
            }
            LoadingVisibilityHandler(isLoading: false);
        }

        /// <summary>
        /// Handles header search text changes.
        /// Filters customer list based on search input or restores initial data.
        /// </summary>
        /// <param name="searchText">The current search text input.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task HeaderSearchTextChangeCommandHandlerAsync(string searchText)
        {
            HeaderSearchItemSource.Clear();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                var ifDataGridHasAlreadyData = RouteDetailsDBSource?.Count == RouteDetailsItemSource?.Count;
                if (ifDataGridHasAlreadyData) LoadHeaderSearchWithInitialData();
                else await LoadDataGridAndHeaderSearchWithInitialDataAsync();
            }
            else
            {
                var tempList = RouteDetailsDBSource?.Where(x => x.SearchDisplayPath.ToLower().Contains(searchText.ToLower())).ToList();

                if (tempList == null || tempList.Count == 0)
                {
                    HeaderSearchItemSource.Add(new ViewRouteDetailsUIModel { CustomerName = ResourceExtensions.GetLocalized("NoResultsErrorMessage") });
                }
                else
                {
                    foreach (var item in tempList) HeaderSearchItemSource.Add(item);
                }
            }
        }

        /// <summary>
        /// Loads initial data into both the main DataGrid and header search collection.
        /// Used when search is cleared or page first loads.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task LoadDataGridAndHeaderSearchWithInitialDataAsync()
        {
            LoadingVisibilityHandler(isLoading: true);
            await Task.Delay(100);

            RouteDetailsItemSource.Clear();
            foreach (var item in RouteDetailsDBSource) RouteDetailsItemSource.Add(item);

            HeaderSearchItemSource.Clear();
            LoadingVisibilityHandler(isLoading: false);
        }

        /// <summary>
        /// Loads initial data into the header search collection only.
        /// Used when DataGrid already has data but search needs refreshing.
        /// </summary>
        private void LoadHeaderSearchWithInitialData()
        {
            foreach (var item in RouteDetailsDBSource) HeaderSearchItemSource.Add(item);
        }

        /// <summary>
        /// Handles search suggestion selection.
        /// Filters the main DataGrid to show only the selected customer.
        /// </summary>
        /// <param name="selectedItem">The customer item selected from suggestions.</param>
        private void SuggestionChoosen(ViewRouteDetailsUIModel selectedItem)
        {
            if (selectedItem?.SearchDisplayPath?.Contains(ResourceExtensions.GetLocalized("NoResultsErrorMessage")) == true) return;

            RouteDetailsItemSource.Clear();
            var filterItem = RouteDetailsDBSource.FirstOrDefault(x => x.CustomerName.Equals(selectedItem.CustomerName));
            if (filterItem != null) RouteDetailsItemSource.Add(filterItem);
        }

        #endregion

        #region Route Calculation & Geocoding

        /// <summary>
        /// Handles the Calculate button command execution.
        /// Validates inputs, geocodes locations, and initiates route plotting.
        /// </summary>
        /// <param name="myMap">Reference to the OnTerra MapControl for route visualization.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task CalculateButtonCommandHandlerAsync(OnTerra.MapsControl.UWP.MapControl myMap)
        {
            LoadingVisibilityHandler(isLoading: true);
            OnTerraMap = myMap;
            StartGeopoint = null;
            EndGeopoint = null;
            OnTerraMap?.Routes.Clear();

            var isSelected = RouteDetailsItemSource.Any(x => x.IsChecked);
            var isValidStart = !string.IsNullOrWhiteSpace(StartLocation?.Trim()) || IsStartCurrentLocation;
            var isValidEnd = !string.IsNullOrWhiteSpace(EndLocation?.Trim()) || IsEndCurrentLocation;

            _selectedCustomerList = new ObservableCollection<ViewRouteDetailsUIModel>();

            if (isSelected && isValidStart && isValidEnd)
            {
                bool isValidLocation = true;
                if (IsEndCurrentLocation || IsStartCurrentLocation)
                {
                    isValidLocation = await RequestLocationAccessAsync();
                }

                if (isValidLocation)
                {
                    if (StartGeopoint == null && !string.IsNullOrWhiteSpace(StartLocation?.Trim()))
                    {
                        var geopoint = await ValidateAndGeocodeLocationAsync(StartLocation);
                        if (geopoint != null)
                        {
                            StartGeopoint = geopoint;
                            isValidStart = true;
                        }
                        else isValidStart = false;
                    }

                    if (EndGeopoint == null && !string.IsNullOrWhiteSpace(EndLocation?.Trim()))
                    {
                        var geopoint = await ValidateAndGeocodeLocationAsync(EndLocation);
                        if (geopoint != null)
                        {
                            EndGeopoint = geopoint;
                            isValidEnd = true;
                        }
                        else isValidEnd = false;

                    }

                    if (isValidStart && isValidEnd)
                    {
                        foreach (var item in RouteDetailsItemSource)
                            if (item.IsChecked) _selectedCustomerList.Add(item);

                        PointOfInterestSource.Clear();
                        await PlotPinsForSelectedCustomersAsync(_selectedCustomerList);
                    }
                    else
                    {
                        if (!isValidStart) await ShowNoValidStartLocationAlertAsync();
                        else if (!isValidEnd) await ShowNoValidEndLocationAlertAsync();
                    }
                }
            }
            else
            {
                if (!isSelected) await ShowNoCustomerAlertAsync();
                else if (!isValidStart) await ShowNoStartLocationAlertAsync();
                else if (!isValidEnd) await ShowNoEndLocationAlertAsync();
            }
            LoadingVisibilityHandler(isLoading: false);
        }

        /// <summary>
        /// Validates address input and attempts to geocode it to a geopoint.
        /// For numeric inputs, validates as 5-digit US zip code (10000-99999).
        /// </summary>
        /// <param name="address">The address or zip code string to validate and geocode.</param>
        /// <param name="type">Location type indicator ('S' for start, 'E' for end).</param>
        /// <returns>True if validation and geocoding succeeded; otherwise false.</returns>
        private async Task<OnTerra.MapsControl.UWP.Geopoint> ValidateAndGeocodeLocationAsync(string address)
        {
            // Check if address is a numeric zip code
            if (int.TryParse(address, out int zipResult))
            {
                // Validate 5-digit US zip code range
                if (zipResult < MinZipCode || zipResult > MaxZipCode) return null;
            }
            return await GetGeoLocationFromAddressAsync(address);
        }

        /// <summary>
        /// Geocodes an address string to a geopoint using the OnTerra map service.
        /// Includes configurable region fallback for international addresses.
        /// </summary>
        /// <param name="address">The address string to geocode.</param>
        /// <param name="type">Location type indicator ('S' for start, 'E' for end).</param>
        /// <param name="cancellationToken">Optional token to cancel the async operation.</param>
        /// <returns>OnTerra.MapsControl.UWP.Geopoint if successful; otherwise null.</returns>
        private async Task<OnTerra.MapsControl.UWP.Geopoint> GetGeoLocationFromAddressAsync(
            string address, CancellationToken cancellationToken = default)
        {
            var geocodeQuery = BuildGeocodeQuery(address);
            var result = await OnTerraMap.GeocodeAndPlotAsync(geocodeQuery);

            if (result?.SuccessCount > 0 && result.Locations?.Any() == true)
            {
                var bestMatch = result.Locations[0];
                // Use OnTerra.MapsControl.UWP.BasicGeoposition to match constructor expectations
                return new OnTerra.MapsControl.UWP.Geopoint(new OnTerra.MapsControl.UWP.BasicGeoposition
                {
                    Latitude = bestMatch.Latitude,
                    Longitude = bestMatch.Longitude
                });
            }
            return null;
        }

        /// <summary>
        /// Builds a geocoding query string with intelligent region fallback.
        /// Appends ",USA" only if country is not already specified in the address.
        /// </summary>
        /// <param name="address">The base address string.</param>
        /// <returns>Fully qualified geocoding query string.</returns>
        private string BuildGeocodeQuery(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return address;
            if (address.IndexOf("USA", StringComparison.OrdinalIgnoreCase) >= 0 ||
                address.IndexOf("United States", StringComparison.OrdinalIgnoreCase) >= 0 ||
                address.IndexOf("US", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return address;
            }
            return $"{address}{DefaultGeocodeRegion}";
        }

        /// <summary>
        /// Checks if an address string contains a specific city or state token.
        /// Used for address validation and matching logic.
        /// </summary>
        /// <param name="address">The full address string to search.</param>
        /// <param name="city">The city or state name to search for.</param>
        /// <returns>True if the token is found; otherwise false.</returns>
        private bool ContainsCityOrState(string address, string city)
        {
            if (string.IsNullOrWhiteSpace(address)) return false;
            var tokens = address.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
            return !string.IsNullOrWhiteSpace(city) && tokens.Any(token => string.Equals(token, city, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Requests device location access and sets start/end geopoints if enabled.
        /// Handles all GeolocationAccessStatus cases with appropriate user alerts.
        /// </summary>
        /// <returns>True if location access was granted and geopoints set; otherwise false.</returns>
        private async Task<bool> RequestLocationAccessAsync()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                switch (accessStatus)
                {
                    case GeolocationAccessStatus.Allowed:
                        _cts = new CancellationTokenSource();
                        var geolocator = new Geolocator { DesiredAccuracyInMeters = _desireAccuracyInMetersValue };
                        var pos = await geolocator.GetGeopositionAsync().AsTask(_cts.Token);

                        if (IsStartCurrentLocation)
                            StartGeopoint = new OnTerra.MapsControl.UWP.Geopoint(new OnTerra.MapsControl.UWP.BasicGeoposition
                            {
                                Latitude = pos.Coordinate.Point.Position.Latitude,
                                Longitude = pos.Coordinate.Point.Position.Longitude
                            });

                        if (IsEndCurrentLocation)
                            EndGeopoint = new OnTerra.MapsControl.UWP.Geopoint(new OnTerra.MapsControl.UWP.BasicGeoposition
                            {
                                Latitude = pos.Coordinate.Point.Position.Latitude,
                                Longitude = pos.Coordinate.Point.Position.Longitude
                            });
                        return true;

                    case GeolocationAccessStatus.Denied:
                        await ShowNoLocationAccessAlertAsync();
                        return false;
                    case GeolocationAccessStatus.Unspecified:
                        await ShowLocationUnspecifiedAlertAsync();
                        return false;
                    default:
                        return false;
                }
            }
            catch (TaskCanceledException) { return false; }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(RequestLocationAccessAsync), ex.StackTrace);
                return false;
            }
            finally { _cts?.Dispose(); _cts = null; }
        }

        /// <summary>
        /// Plots map pins for selected customers, start location, and end location.
        /// Handles both optimized route results and fallback direct plotting.
        /// </summary>
        /// <param name="selectedCustomerList">Collection of selected customer items.</param>
        /// <param name="myMap">Reference to the OnTerra MapControl.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task PlotPinsForSelectedCustomersAsync(ObservableCollection<ViewRouteDetailsUIModel> selectedCustomerList)
        {
            if (selectedCustomerList == null || !selectedCustomerList.Any())
            {
                await ShowNoRouteAlertAsync();
                return;
            }

            bool isValidRoute = await ShowRouteOnMapAsync(selectedCustomerList);
            if (!isValidRoute) return;

            // Add start pin
            PointOfInterestSource.Add(new PointOfInterest
            {
                OnTerraLocation = StartGeopoint,
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                PinColor = new SolidColorBrush(Colors.Green),
                PinText = "start",
                ImageSourceUri = "ms-appx:///Assets/Maps/MapPin-Green.png"
            });

            // Add customer pins
            if (_optimizedListRoutes != null && _optimizedListRoutes.Count > 0)
            {
                int pinNumber = 1;
                foreach (var opItem in _optimizedListRoutes)
                {
                    if (pinNumber > MaxWaypointsLimit) break;
                    var imgLoc = pinNumber <= MaxWaypointsLimit
                        ? $"ms-appx:///Assets/Maps/MapPin-Red_{pinNumber}.png"
                        : "ms-appx:///Assets/Maps/MapPin-Red.png";

                    var item = selectedCustomerList.FirstOrDefault(x => x.CustomerID.ToString().Equals(opItem.location_id));
                    if (item != null && !string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                    {
                        try
                        {
                            PointOfInterestSource.Add(new PointOfInterest
                            {
                                RouteData = item,
                                OnTerraLocation = new OnTerra.MapsControl.UWP.Geopoint(new OnTerra.MapsControl.UWP.BasicGeoposition
                                {
                                    Latitude = Convert.ToDouble(item.Latitude),
                                    Longitude = Convert.ToDouble(item.Longitude)
                                }),
                                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                                PinColor = new SolidColorBrush(Colors.Red),
                                IsPinTextVisible = true,
                                PinText = item.CustomerNumber,
                                ImageSourceUri = imgLoc,
                                CustomerData = new MapCustomerData { CustomerID = item.CustomerID, DeviceCustomerID = item.DeviceCustomerID }
                            });
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPageViewModel), nameof(PlotPinsForSelectedCustomersAsync), $"{ex.StackTrace} - {ex.Message}");
                        }
                    }
                    pinNumber++;
                }
            }
            else
            {
                foreach (var item in selectedCustomerList)
                {
                    if (!string.IsNullOrWhiteSpace(item.Latitude) && !string.IsNullOrWhiteSpace(item.Longitude))
                    {
                        try
                        {
                            PointOfInterestSource.Add(new PointOfInterest
                            {
                                RouteData = item,
                                OnTerraLocation = new OnTerra.MapsControl.UWP.Geopoint(new OnTerra.MapsControl.UWP.BasicGeoposition
                                {
                                    Latitude = Convert.ToDouble(item.Latitude),
                                    Longitude = Convert.ToDouble(item.Longitude)
                                }),
                                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                                PinColor = new SolidColorBrush(Colors.Red),
                                IsPinTextVisible = true,
                                PinText = item.CustomerNumber,
                                ImageSourceUri = "ms-appx:///Assets/Maps/MapPin-Red.png",
                                CustomerData = new MapCustomerData { CustomerID = item.CustomerID, DeviceCustomerID = item.DeviceCustomerID }
                            });
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPageViewModel), nameof(PlotPinsForSelectedCustomersAsync), $"{ex.StackTrace} - {ex.Message}");
                        }
                    }
                }
            }

            // Add end pin
            PointOfInterestSource.Add(new PointOfInterest
            {
                OnTerraLocation = EndGeopoint,
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                PinColor = new SolidColorBrush(Colors.Yellow),
                PinText = "End",
                ImageSourceUri = "ms-appx:///Assets/Maps/MapPin-Yellow.png"
            });

            if (!PointOfInterestSource.Any()) await ShowNoRouteAlertAsync();
        }

        /// <summary>
        /// Plots only start and end location pins without customer waypoints.
        /// Used for simple point-to-point route visualization.
        /// </summary>
        private void PlotStartEndPins()
        {
            PointOfInterestSource.Add(new PointOfInterest
            {
                OnTerraLocation = StartGeopoint,
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                PinColor = new SolidColorBrush(Colors.Green),
                PinText = "start",
                ImageSourceUri = "ms-appx:///Assets/Maps/MapPin-Green.png"
            });

            PointOfInterestSource.Add(new PointOfInterest
            {
                OnTerraLocation = EndGeopoint,
                NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
                PinColor = new SolidColorBrush(Colors.Yellow),
                PinText = "End",
                ImageSourceUri = "ms-appx:///Assets/Maps/MapPin-Yellow.png"
            });
        }

        /// <summary>
        /// Calls the optimization web service and plots the resulting route on the map.
        /// Handles error responses and missing service alerts.
        /// </summary>
        /// <param name="myMap">Reference to the OnTerra MapControl.</param>
        /// <param name="selectedCustomerList">Collection of selected customer items.</param>
        /// <returns>True if route was successfully plotted; otherwise false.</returns>
        private async Task<bool> ShowRouteOnMapAsync(ObservableCollection<ViewRouteDetailsUIModel> selectedCustomerList)
        {
            var webServiceResponse = await PathActivitiesAsync(selectedCustomerList);

            if (webServiceResponse != null)
            {
                if (webServiceResponse.Any(x => x.type == "InvalidRoute" || x.type == "RouteError"))
                {
                    await ShowInvalidRouteAlertAsync(webServiceResponse, selectedCustomerList);
                    return false;
                }

                if (webServiceResponse.Count(x => x.type == "service") < selectedCustomerList.Count)
                {
                    await ShowMissingServiceAlertAsync(webServiceResponse, selectedCustomerList);
                    return false;
                }

                _optimizedListRoutes = new List<RouteRespActivity>();
                foreach (var activity in webServiceResponse)
                {
                    if (activity.type == "service") _optimizedListRoutes.Add(activity);
                }
                return true;
            }

            await ShowNoRouteAlertAsync();
            return false;
        }

        /// <summary>
        /// Shows an alert for invalid route responses from the optimization service.
        /// Lists problematic customer addresses that are preventing route creation.
        /// </summary>
        /// <param name="webServiceResponse">The response from the optimization web service.</param>
        /// <param name="selectedCustomerList">Collection of selected customer items.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task ShowInvalidRouteAlertAsync(
            List<RouteRespActivity> webServiceResponse,
            ObservableCollection<ViewRouteDetailsUIModel> selectedCustomerList)
        {
            var problematicIds = webServiceResponse
                .Where(x => x.type == "InvalidRoute" || x.type == "RouteError")
                .SelectMany(x => x.location_id?.Split(',') ?? Array.Empty<string>())
                .ToList();

            var problematicCustomers = selectedCustomerList.Where(x => problematicIds.Contains(x.CustomerID.ToString())).ToList();
            var errorMessage = problematicCustomers.Count == selectedCustomerList.Count
                ? "The following addresses are preventing route creation.\nPlease review or remove them and try again.\n• Start/End location is far away."
                : "The following addresses are preventing route creation.\nPlease review or remove them and try again." +
                  string.Concat(problematicCustomers.Select(c => $"\n* {c.CustomerName} (Address: {c.CustomerAddress})"));

            await ShowAlertAsync("Alert", errorMessage);
        }

        /// <summary>
        /// Shows an alert for missing service responses from the optimization service.
        /// Lists customer addresses that were not included in the optimized route.
        /// </summary>
        /// <param name="webServiceResponse">The response from the optimization web service.</param>
        /// <param name="selectedCustomerList">Collection of selected customer items.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task ShowMissingServiceAlertAsync(
            List<RouteRespActivity> webServiceResponse,
            ObservableCollection<ViewRouteDetailsUIModel> selectedCustomerList)
        {
            var serviceIds = webServiceResponse.Where(x => x.type == "service").Select(x => x.location_id).ToList();
            var problematicIds = selectedCustomerList.Select(x => x.CustomerID.ToString()).Except(serviceIds).ToList();
            var errorMessage = "The following addresses are preventing route creation.\nPlease review or remove them and try again." +
                string.Concat(selectedCustomerList.Where(c => problematicIds.Contains(c.CustomerID.ToString())).Select(c => $"\n* {c.CustomerName} (Address: {c.CustomerAddress})"));

            await ShowAlertAsync("Alert", errorMessage);
        }

        /// <summary>
        /// Prepares route data and calls the optimization web service.
        /// Converts UI models to RouteService objects for API consumption.
        /// </summary>
        /// <param name="selectedCustomerList">Collection of selected customer items.</param>
        /// <returns>List of RouteRespActivity from the optimization service.</returns>
        private async Task<List<RouteRespActivity>> PathActivitiesAsync(
            ObservableCollection<ViewRouteDetailsUIModel> selectedCustomerList)
        {
            var optPath = new List<RouteService>();
            var startLoc = new RouteAddress { location_id = "startloc", lat = StartGeopoint.Position.Latitude, lon = StartGeopoint.Position.Longitude };

            foreach (var item in selectedCustomerList)
            {
                if (!string.IsNullOrEmpty(item.Latitude) && !string.IsNullOrEmpty(item.Longitude))
                {
                    optPath.Add(new RouteService
                    {
                        id = item.CustomerID.ToString(),
                        name = item.CustomerID.ToString(),
                        address = new RouteAddress
                        {
                            location_id = item.CustomerID.ToString(),
                            lat = Convert.ToDouble(item.Latitude),
                            lon = Convert.ToDouble(item.Longitude)
                        }
                    });
                }
            }

            var endLoc = new RouteAddress { location_id = "endloc", lat = EndGeopoint.Position.Latitude, lon = EndGeopoint.Position.Longitude };
            return await InvokeWebService.GetOptmizedRoute(startLoc, optPath, endLoc).ConfigureAwait(false);
        }

        #endregion

        #region Alert Helpers (Extracted for Reusability)

        /// <summary>
        /// Generic helper method to show confirmation alerts.
        /// Reduces code duplication across multiple alert scenarios.
        /// </summary>
        /// <param name="title">The alert dialog title.</param>
        /// <param name="message">The alert dialog message content.</param>
        /// <returns>A Task representing the asynchronous alert display operation.</returns>
        private Task ShowAlertAsync(string title, string message) =>
            AlertHelper.Instance.ShowConfirmationAlert(title, message, "OK");

        /// <summary>Shows alert when end location is not provided.</summary>
        private Task ShowNoEndLocationAlertAsync() => ShowAlertAsync("Alert", "Please enter End location.");

        /// <summary>Shows alert when end location is invalid or cannot be geocoded.</summary>
        private Task ShowNoValidEndLocationAlertAsync() => ShowAlertAsync("Alert", "End location is not valid. Please enter valid End location.");

        /// <summary>Shows alert when start location is not provided.</summary>
        private Task ShowNoStartLocationAlertAsync() => ShowAlertAsync("Alert", "Please enter Start location.");

        /// <summary>Shows alert when start location is invalid or cannot be geocoded.</summary>
        private Task ShowNoValidStartLocationAlertAsync() => ShowAlertAsync("Alert", "Start location is not valid. Please enter valid Start location.");

        /// <summary>Shows alert when no customers are selected for routing.</summary>
        private Task ShowNoCustomerAlertAsync() => ShowAlertAsync("Alert", "Please select atleast 1 Customer.");

        /// <summary>Shows alert when route calculation fails due to address issues.</summary>
        private Task ShowNoRouteAlertAsync() => ShowAlertAsync("Alert", "There seems to be an issue with one of the addresses. Please correct it and attempt routing again.");

        /// <summary>Shows alert when device location access is denied by user.</summary>
        private Task ShowNoLocationAccessAlertAsync() => ShowAlertAsync("Alert", "Access to location is denied. Please go to the settings and enable the location permission");

        /// <summary>Shows alert when device does not support location services.</summary>
        private Task ShowLocationUnspecifiedAlertAsync() => ShowAlertAsync("Alert", "The Device does not have location capabilities");

        #endregion

        #region Delete & Edit Operations

        /// <summary>
        /// Handles the Delete button command execution.
        /// Shows confirmation dialog and deletes the route if user confirms.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task DeleteButtonCommandHandlerAsync()
        {
            var deleteRouteDialog = new ContentDialog
            {
                Title = _resourceLoader.GetString("DeleteRouteText"),
                Content = _resourceLoader.GetString("DeleteRouteMessage"),
                PrimaryButtonText = _resourceLoader.GetString("YesText"),
                SecondaryButtonText = _resourceLoader.GetString("NoText")
            };

            var result = await deleteRouteDialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                var data = await _appReference.QueryService.DeleteScheduledRoute(DeviceRouteId);
                if (data)
                {
                    await ShowAlertAsync("Delete Route", "The route has been deleted");
                    // NavigationService is a static class in this project
                    NavigationService.GoBackInShell();
                }
                else
                {
                    await ShowAlertAsync("Error", "Something went wrong, could not delete this route. Please try again");
                }
            }
        }

        /// <summary>
        /// Handles the Edit button command execution.
        /// Navigates to the Add/Edit route page with the selected route data.
        /// </summary>
        private void EditButtonCommandHandler()
        {
            // NavigationService is a static class in this project
            NavigationService.NavigateShellFrame(typeof(AddEditRoutePage), _selectedRoute);
        }

        #endregion

        #region Utility & UI Helpers

        /// <summary>
        /// Toggles the loading visibility indicator.
        /// Centralized method for consistent loading UI management.
        /// </summary>
        /// <param name="isLoading">True to show loading indicator; false to hide.</param>
        public void LoadingVisibilityHandler(bool isLoading) => LoadingVisibility = isLoading ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Shows a dialog for entering a phone number to receive navigation directions via SMS.
        /// Validates input and sends message if valid.
        /// </summary>
        /// <returns>A Task representing the asynchronous dialog operation.</returns>
        public async Task InputTextDialogAsync()
        {
            var inputTextBox = new TextBox { AcceptsReturn = false, Height = 32, PlaceholderText = "Enter 10 digit phone number" };
            inputTextBox.BeforeTextChanging += InputTextBox_BeforeTextChanging;
            inputTextBox.TextChanging += InputTextBox_TextChanging;

            var dialog = new ContentDialog
            {
                Content = inputTextBox,
                Title = "Please enter phone number to get the navigation directions on your phone.",
                IsSecondaryButtonEnabled = true,
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel"
            };

            if (await dialog.ShowAsync() == ContentDialogResult.Primary)
            {
                if (!string.IsNullOrWhiteSpace(inputTextBox.Text?.Trim()))
                    await SendTextMessageAsync(inputTextBox.Text.Trim());
                else
                    await ShowAlertAsync("Alert", "Please enter 10 digit phone number to send message");
            }

            // Unsubscribe from events to prevent memory leaks
            inputTextBox.BeforeTextChanging -= InputTextBox_BeforeTextChanging;
            inputTextBox.TextChanging -= InputTextBox_TextChanging;
        }

        /// <summary>
        /// Filters TextBox input to allow only numeric characters.
        /// Used for phone number input validation.
        /// </summary>
        /// <param name="sender">The TextBox raising the event.</param>
        /// <param name="args">The TextChanging event arguments.</param>
        private void InputTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args) =>
            sender.Text = new string(sender.Text.Where(char.IsDigit).ToArray());

        /// <summary>
        /// Cancels TextBox input if non-digit characters are entered or length exceeds 10 digits.
        /// Used for phone number input validation.
        /// </summary>
        /// <param name="sender">The TextBox raising the event.</param>
        /// <param name="args">The BeforeTextChanging event arguments.</param>
        private void InputTextBox_BeforeTextChanging(TextBox sender, TextBoxBeforeTextChangingEventArgs args)
        {
            if (args.NewText.Any(c => !char.IsDigit(c)) || args.NewText.Length > 10)
                args.Cancel = true;
        }

        /// <summary>
        /// Sends an SMS message with a hardcoded Google Maps URL to the specified phone number.
        /// Uses the device's SMS capability if available.
        /// </summary>
        /// <param name="phoneNumber">The 10-digit phone number to send the message to.</param>
        /// <returns>A Task representing the asynchronous SMS sending operation.</returns>
        private async Task SendTextMessageAsync(string phoneNumber)
        {
            if (_smsDevice == null) _smsDevice = SmsDevice2.GetDefault();

            if (_smsDevice != null)
            {
                try
                {
                    var smsTextMessage = new SmsTextMessage2 { To = phoneNumber, Body = "https://www.google.com/maps/dir/bhopal/pune/sagar" };
                    var result = await _smsDevice.SendMessageAndGetResultAsync(smsTextMessage);
                    if (result.IsSuccessful) await ShowAlertAsync("Confirm", "Message sent successfully to phone number.");
                    else await ShowAlertAsync("Error", "Message sending failed, please try again.");
                }
                catch (Exception ex) { ErrorLogger.WriteToErrorLog(GetType().Name, nameof(SendTextMessageAsync), ex.StackTrace); }
            }
            else { await ShowAlertAsync("Error", "This device does not have the capabilities to send text message."); }
        }

        /// <summary>
        /// Calculates the great-circle distance between two geopositions using the Haversine formula.
        /// Returns distance in kilometers.
        /// </summary>
        /// <param name="from">The starting geoposition.</param>
        /// <param name="to">The ending geoposition.</param>
        /// <returns>Distance in kilometers between the two points.</returns>
        private double CalculateDistanceKm(Windows.Devices.Geolocation.BasicGeoposition from, Windows.Devices.Geolocation.BasicGeoposition to)
        {
            var lat1 = DegreesToRadians(from.Latitude);
            var lon1 = DegreesToRadians(from.Longitude);
            var lat2 = DegreesToRadians(to.Latitude);
            var lon2 = DegreesToRadians(to.Longitude);
            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;
            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusKm * c;
        }

        /// <summary>
        /// Converts degrees to radians for trigonometric calculations.
        /// </summary>
        /// <param name="degrees">Angle in degrees.</param>
        /// <returns>Angle in radians.</returns>
        private double DegreesToRadians(double degrees) => degrees * (Math.PI / 180.0);

        /// <summary>
        /// Identifies waypoints that are too far from the cluster centroid.
        /// Shows alert with problematic addresses to help user correct routing issues.
        /// </summary>
        /// <param name="waypoints">List of enhanced waypoints to analyze.</param>
        /// <param name="maxDistanceKm">Maximum acceptable distance from centroid in kilometers.</param>
        /// <returns>A Task representing the asynchronous alert operation.</returns>
        private async Task FindDistantWaypointsAsync(List<EnhancedWaypoint> waypoints, double maxDistanceKm = MaxWaypointDistanceFromClusterKm)
        {
            if (waypoints == null || waypoints.Count < 2) return;

            int halfCount = waypoints.Count / 2;
            int clusterSize = Math.Min(halfCount, waypoints.Count);
            var clusterPositions = waypoints.Take(clusterSize).Select(w => w.Point.Position).ToList();

            var centroid = new Windows.Devices.Geolocation.BasicGeoposition
            {
                Latitude = clusterPositions.Average(p => p.Latitude),
                Longitude = clusterPositions.Average(p => p.Longitude)
            };

            var distantWaypointMessages = new List<string>();
            for (int i = 0; i < waypoints.Count; i++)
            {
                var wp = waypoints[i];
                var distanceKm = CalculateDistanceKm(centroid, wp.Point.Position);
                if (distanceKm > maxDistanceKm)
                {
                    string message;
                    if (i == 0) message = "Start location is far away.";
                    else if (i == (waypoints.Count - 1)) message = "End location is far away.";
                    else
                    {
                        var selectedWayPoint = _selectedCustomerList?.FirstOrDefault(p =>
                            Math.Abs(wp.Point.Position.Latitude - Convert.ToDouble(p.Latitude)) < 0.0001 &&
                            Math.Abs(wp.Point.Position.Longitude - Convert.ToDouble(p.Longitude)) < 0.0001);
                        message = selectedWayPoint != null ? $"{selectedWayPoint.CustomerName} (Address: {selectedWayPoint.CustomerAddress})." : $"Waypoint at ({wp.Point.Position.Latitude}, {wp.Point.Position.Longitude}) is far away.";
                    }
                    distantWaypointMessages.Add($"• {message}");
                }
            }

            if (distantWaypointMessages.Any())
                await ShowAlertAsync("Alert", $"The following addresses are preventing route creation.\nPlease review or remove them and try again.\n{string.Join("\n", distantWaypointMessages)}");
            else
                await ShowNoRouteAlertAsync();
        }

        /// <summary>
        /// Checks if all waypoints are within a reasonable geographic region.
        /// Prevents routing across extremely distant locations that would fail optimization.
        /// </summary>
        /// <param name="waypoints">List of enhanced waypoints to validate.</param>
        /// <returns>True if all waypoints are in the same region; otherwise false.</returns>
        private bool AreWaypointsInSameRegion(List<EnhancedWaypoint> waypoints)
        {
            if (waypoints.Count < 2) return true;
            var firstPoint = waypoints[0].Point.Position;
            foreach (var waypoint in waypoints.Skip(1))
            {
                var currentPoint = waypoint.Point.Position;
                double longitudeDifference = Math.Abs(firstPoint.Longitude - currentPoint.Longitude);
                double latitudeDifference = Math.Abs(firstPoint.Latitude - currentPoint.Latitude);
                if (longitudeDifference > 60 || (longitudeDifference > 40 && latitudeDifference > 20)) return false;
            }
            return true;
        }

        #endregion

        #region Navigation & Email Operations

        /// <summary>
        /// Handles the Navigation button command execution.
        /// Generates Google Maps URLs for the optimized route and sends them via email.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task NavigationButtonCommandHandlerAsync()
        {
            if (PointOfInterestSource == null || _selectedCustomerList == null)
            {
                LoadingVisibilityHandler(isLoading: false);
                await ShowAlertAsync("Alert", "To get the driving direction, please plot the route using Calculate button.");
                return;
            }

            LoadingVisibilityHandler(isLoading: true);
            try
            {
                var waypointInfos = new List<(string Address, string Latitude, string Longitude)>();

                if (_optimizedListRoutes != null && _optimizedListRoutes.Count > 0)
                {
                    foreach (var route in _optimizedListRoutes)
                    {
                        var customer = RouteDetailsItemSource.FirstOrDefault(x => x.CustomerID.ToString() == route.location_id);
                        if (customer != null)
                            waypointInfos.Add((customer.CustomerAddress ?? $"{customer.Latitude},{customer.Longitude}", customer.Latitude, customer.Longitude));
                    }
                }
                else
                {
                    foreach (var item in _selectedCustomerList)
                        waypointInfos.Add(($"{item.Latitude},{item.Longitude}", item.Latitude, item.Longitude));
                }

                var startInfo = ($"{StartGeopoint.Position.Latitude},{StartGeopoint.Position.Longitude}", StartGeopoint.Position.Latitude, StartGeopoint.Position.Longitude);
                var endInfo = ($"{EndGeopoint.Position.Latitude},{EndGeopoint.Position.Longitude}", EndGeopoint.Position.Latitude, EndGeopoint.Position.Longitude);

                var googleMapsUrls = BuildGoogleMapsUrlsWithAddresses(startInfo, waypointInfos, endInfo);

                var user = await _appReference.QueryService.GetLoggedInUserInformation(Convert.ToInt32(_appReference.LoginUserIdProperty));
                LoggedInUserName = $"{user.FirstName} {user.LastName}";
                UserEmailId = user.EmailId;

                await SendNavigationDirectionEmailAsync(googleMapsUrls);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(NavigationButtonCommandHandlerAsync), ex.ToString());
                LoadingVisibilityHandler(isLoading: false);
                await ShowAlertAsync("Error", "Failed to generate route links. Please try again.");
            }
        }

        /// <summary>
        /// Cleans an address string for use in Google Maps URLs.
        /// Removes line breaks, replaces spaces with +, and encodes special characters.
        /// </summary>
        /// <param name="address">The raw address string.</param>
        /// <returns>Cleaned address string suitable for URL encoding.</returns>
        private string CleanAddressString(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return address;
            string cleaned = address.Trim();
            cleaned = Regex.Replace(cleaned, @"\r\n|\r|\n", " ").Replace("#", "%23");
            cleaned = Regex.Replace(cleaned, @"\s+", "+");
            return cleaned;
        }

        /// <summary>
        /// Builds a single Google Maps URL segment for a subset of waypoints.
        /// Handles origin, destination, and up to 9 intermediate waypoints.
        /// </summary>
        /// <param name="allPoints">List of address strings including origin, waypoints, and destination.</param>
        /// <returns>Formatted Google Maps URL string.</returns>
        private string BuildGoogleMapsUrlSegment(List<string> allPoints)
        {
            string origin = allPoints.First();
            string destination = allPoints.Last();
            if (allPoints.Count > 2)
            {
                var waypointList = allPoints.Skip(1).Take(allPoints.Count - 2).ToList();
                string waypoints = waypointList.Any() ? string.Join("|", waypointList) : "";
                return $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&travelmode=driving&waypoints={waypoints}";
            }
            return $"https://www.google.com/maps/dir/?api=1&origin={origin}&destination={destination}&travelmode=driving";
        }

        /// <summary>
        /// Builds one or more Google Maps URLs to handle routes with many waypoints.
        /// Splits routes into segments of max 9 waypoints each to comply with Google Maps API limits.
        /// </summary>
        /// <param name="start">Start location with address and coordinates.</param>
        /// <param name="waypoints">List of waypoint locations with addresses and coordinates.</param>
        /// <param name="end">End location with address and coordinates.</param>
        /// <returns>List of Google Maps URL strings covering the full route.</returns>
        private List<string> BuildGoogleMapsUrlsWithAddresses(
            (string Address, double Latitude, double Longitude) start,
            List<(string Address, string Latitude, string Longitude)> waypoints,
            (string Address, double Latitude, double Longitude) end)
        {
            var urls = new List<string>();
            if (waypoints.Count <= MaxGoogleMapsWaypointsPerSegment)
            {
                var allPoints = new List<string> { start.Address };
                allPoints.AddRange(waypoints.Select(w => CleanAddressString(w.Address)));
                allPoints.Add(end.Address);
                urls.Add(BuildGoogleMapsUrlSegment(allPoints));
            }
            else
            {
                int startIndex = 0;
                var currentStart = start;
                int countWayPoint = waypoints.Count + 1;

                while (startIndex < countWayPoint)
                {
                    int takeCount = Math.Min(MaxGoogleMapsWaypointsPerSegment, countWayPoint - startIndex);
                    var segmentWaypoints = waypoints.Skip(startIndex).Take(takeCount).ToList();

                    (string Address, double Latitude, double Longitude) segmentEnd;
                    if (startIndex + takeCount >= countWayPoint) segmentEnd = end;
                    else
                    {
                        var lastWp = segmentWaypoints.Last();
                        segmentEnd = (CleanAddressString(lastWp.Address), Convert.ToDouble(lastWp.Latitude), Convert.ToDouble(lastWp.Longitude));
                    }

                    var points = new List<string> { currentStart.Address };
                    int actualTake = takeCount < MaxGoogleMapsWaypointsPerSegment ? takeCount : takeCount - 1;
                    points.AddRange(segmentWaypoints.Take(actualTake).Select(w => CleanAddressString(w.Address)));
                    points.Add(segmentEnd.Address);

                    urls.Add(BuildGoogleMapsUrlSegment(points));
                    currentStart = segmentEnd;
                    startIndex += takeCount;
                }
            }
            return urls;
        }

        /// <summary>
        /// Sends navigation direction emails with Google Maps URLs to the user.
        /// Handles single and multi-segment route URLs with appropriate messaging.
        /// </summary>
        /// <param name="googleMapsUrls">List of Google Maps URL strings to include in the email.</param>
        /// <returns>A Task representing the asynchronous email sending operation.</returns>
        private async Task SendNavigationDirectionEmailAsync(List<string> googleMapsUrls)
        {
            try
            {
                if (string.IsNullOrEmpty(UserEmailId) || string.IsNullOrEmpty(LoggedInUserName))
                {
                    LoadingVisibilityHandler(isLoading: false);
                    await ShowAlertAsync("Alert", "Missing email id! Could not send email.");
                    return;
                }

                var body = new StringBuilder();
                if (googleMapsUrls.Count > 1)
                {
                    body.AppendLine("\n\nPlease use these URLs for driving direction:\n\n");
                    body.AppendLine($"\n\nKindly start with Part 1 of {googleMapsUrls.Count}, and once you complete it, continue your journey using subsequent parts.\n\n");
                    body.AppendLine("\n\nDriving Directions:");
                    for (int i = 0; i < googleMapsUrls.Count; i++)
                        body.AppendLine($"\n\n{i + 1}. Part {i + 1} of {googleMapsUrls.Count}: {googleMapsUrls[i]}\n\n");
                }
                else
                {
                    body.AppendLine($"\n\nPlease use this url for driving direction: {googleMapsUrls[0]}\n\n");
                }

                var emailModel = new EmailModel
                {
                    Subject = $"Driving Directions For Route - {RouteName}",
                    BodyHtml = body.ToString(),
                    To = new List<string> { UserEmailId }
                };

                bool isEmailSent = await EmailService.Instance.SendMailFromOutlook(emailModel);
                await Task.Delay(isEmailSent ? 8000 : 2000);
                LoadingVisibilityHandler(isLoading: false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(GetType().Name, nameof(SendNavigationDirectionEmailAsync), ex.ToString());
                LoadingVisibilityHandler(isLoading: false);
                await ShowAlertAsync("Alert", "Failed to send email. Please try again.");
            }
        }

        #endregion
    }
}