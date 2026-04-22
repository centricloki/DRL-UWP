using DevExpress.Mvvm.Native;

using DRLMobile.Core.Enums;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.ExceptionHandler;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Windows.Devices.Geolocation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace DRLMobile.Uwp.View
{
    public sealed partial class AzureMapPage : Page
    {
        private MapPageViewModel ViewModel = new MapPageViewModel();
        private int intZoomLevel;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool itemStatus = true;

        private const string AzureMapHtmlPath = "ms-appx-web:///Assets/Maps/AzureMap.html";

        public AzureMapPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += MapPage_Loaded;
            Unloaded += MapPage_Unloaded;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            await webView.EnsureCoreWebView2Async();
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.WebMessageReceived += WebView_WebMessageReceived;
            webView.CoreWebView2.DOMContentLoaded += WebView_DOMContentLoaded;

            try
            {
                var htmlPath = new Uri(AzureMapHtmlPath);
                webView.Source = htmlPath;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AzureMapPage: Navigate failed - {ex.Message}");
            }
        }

        private void MapPage_Unloaded(object sender, RoutedEventArgs e)
        {
            ClearMapPins();
            OptionsAllCheckBox.Checked -= OptionsAllCheckBox_Checked;
            OptionsAllCheckBox.Unchecked -= OptionsAllCheckBox_Unchecked;
            if (ViewModel.PointOfIntrestSource.Count == 0)
            {
                this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            }
        }

        private async void MapPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }
            ViewModel.SetLoader(true);
            try
            {
                await ViewModel.OnNavigatedToCommandHandler();
                await RefreshMapIcons();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(MapPage_Loaded), ex);
            }
            finally
            {
                ViewModel.SetLoader(false);
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }

        private async void BottomLegendRelativePanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            if (!cancellationTokenSource.IsCancellationRequested) cancellationTokenSource.Cancel();

            itemStatus = false;
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }
            await ViewModel.OnMapLegendItemClickCommandHandler((sender as StackPanel).DataContext as MapsLegendFilterUIModel, cancellationTokenSource.Token);

            ViewModel.previousZoomLevel = intZoomLevel;

            await RefreshMapIcons();

            itemStatus = true;
            ViewModel.SetLoader(false);
        }

        private async void OptionsAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            if (!cancellationTokenSource.IsCancellationRequested) cancellationTokenSource.Cancel();
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }
            ViewModel.LegendsFilterSource.ForEach(x => { if (!x.IsSelected) x.IsSelected = true; });
            await ViewModel.FilterTheCurrentDataOnTheBasisOfLegendsAsync(cancellationTokenSource.Token);
            await RefreshMapIcons();
            ViewModel.SetLoader(false);
        }

        private async void OptionsAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            if (!cancellationTokenSource.IsCancellationRequested) cancellationTokenSource.Cancel();
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }
            ViewModel.LegendsFilterSource.ForEach(x => { if (x.IsSelected) x.IsSelected = false; });
            await ViewModel.FilterTheCurrentDataOnTheBasisOfLegendsAsync(cancellationTokenSource.Token);
            await RefreshMapIcons();
            ViewModel.SetLoader(false);
        }

        private void SetCheckedState()
        {
            int itemCount = ViewModel.LegendsFilterSource.Count;
            int selectedCount = ViewModel.LegendsFilterSource.Where(x => x.IsSelected).Count();

            if (itemCount == selectedCount) OptionsAllCheckBox.IsChecked = true;
            else OptionsAllCheckBox.IsChecked = false;
        }

        private async void PlotAccountByFlyoutGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.SetLoader(true);

            if (!cancellationTokenSource.IsCancellationRequested) cancellationTokenSource.Cancel();

            var dataContext = (PlotByTypeFilterUIModel)(sender as Grid).DataContext;

            await ViewModel.OnPlotByFilterItemClickCommandHandler(dataContext);

            PlotAccountByFlyout.Hide();

            if (dataContext.Tag == MapFilter.Item)
            {
                ZoneFilterFlyout.ShowAt(ZoneFilterButton);
            }

            ViewModel.previousZoomLevel = intZoomLevel;

            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.Dispose();
                cancellationTokenSource = new CancellationTokenSource();
            }
            await RefreshMapIcons();

            ViewModel.SetLoader(false);
        }

        private void PlotAccountByButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            PlotAccountByFlyout.ShowAt(PlotAccountByButton);
        }

        private void ZoneFilterButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ZoneFilterFlyout.ShowAt(ZoneFilterButton as FrameworkElement);
        }

        private void RegionSelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.RegionSelectionCommand?.Execute((sender as Grid).DataContext);
                RegionTypeFlyout.Hide();
            }
        }

        private void RegionSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (MapPageViewModel)(sender as Grid).DataContext;

            if (dataContext.IsRegionDropDownEnabled)
            {
                FlyoutRegionTextBox.Text = string.Empty;
                ViewModel?.RegionSearchHeaderTextChangeCommandHandler(string.Empty);
                RegionTypeFlyout.ShowAt(RegionSelectionGrid as FrameworkElement);
            }
        }

        private void TerritorySelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (MapPageViewModel)(sender as Grid).DataContext;
            if (dataContext.IsTerritoryDropDownEnabled)
            {
                FlyoutTerritoryTextBox.Text = string.Empty;
                ViewModel?.TerritorySearchHeaderTextChangeCommandHandler(string.Empty);
                TerritoryTypeFlyout.ShowAt(TerritorySelectionGrid as FrameworkElement);
            }
        }

        private void TerritorySelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.TerritorySelectionCommand?.Execute((sender as Grid).DataContext);
                TerritoryTypeFlyout.Hide();
            }
        }

        private void StateSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var dataContext = (MapPageViewModel)(sender as Grid).DataContext;

            if (dataContext.IsStateDropDownEnabled)
            {
                StateTypeFlyout.ShowAt(StateSelectionGrid as FrameworkElement);
                FlyoutStateTextBox.Text = string.Empty;
            }
        }

        private void StateSelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.StateSelectionCommand?.Execute((sender as Grid).DataContext);
                StateTypeFlyout.Hide();
            }
        }

        private void CitySelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var dataContext = (MapPageViewModel)(sender as Grid).DataContext;
                if (dataContext.IsCityDropDownEnabled)
                {
                    CityTypeFlyout.ShowAt(CitySelectionGrid as FrameworkElement);
                    FlyoutCityTextBox.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(CitySelectionGrid_Tapped), ex.Message);
            }
        }

        private void CitySelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.CitySelectionCommand?.Execute((sender as Grid).DataContext);
                CityTypeFlyout.Hide();
            }
        }

        private void ZoneSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var dataContext = (MapPageViewModel)(sender as Grid).DataContext;

                if (dataContext.IsNationalHead)
                {
                    FlyoutZoneTextBox.Text = "";
                    ViewModel?.ZoneSearchHeaderTextChangeCommandHandler(string.Empty);
                    ZoneTypeFlyout.ShowAt(ZoneSelectionGrid as FrameworkElement);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(ZoneSelectionGrid_Tapped), ex.Message);
            }
        }

        private void ZoneSelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.ZoneSelectionCommand?.Execute((sender as Grid).DataContext);
                ZoneTypeFlyout.Hide();
            }
        }

        private void ItemSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            try
            {
                var dataContext = (MapPageViewModel)(sender as Grid).DataContext;
                if (dataContext.IsItemDropDownEnabled)
                {
                    ItemTypeFlyout.ShowAt(ItemSelectionGrid as FrameworkElement);
                    FlyoutItemTextBox.Text = string.Empty;
                    ViewModel.ProductList.Clear();
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(ItemSelectionGrid_Tapped), ex.Message);
            }
        }

        private void ItemSelected(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.ItemSelectionCommand?.Execute((sender as Grid).DataContext);
                ItemTypeFlyout.Hide();
            }
        }

        private void mapItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ViewModel.SelectedCustomerForPopup = ((sender as Button).DataContext as PointOfInterest)?.CustomerData;
            }
            FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
        }

        private void StateTypeFlyout_Opened(object sender, object e)
        {
            ViewModel?.SetTheStateFlyoutList();
        }

        private void PinPopUpFlyout_Opened(object sender, object e)
        {
            if (sender is Flyout && (sender as Flyout).Content is MapPinCustomPopUp)
            {
                var popup = (sender as Flyout).Content as MapPinCustomPopUp;
                popup.DeviceCustomerId = ViewModel?.SelectedCustomerForPopup?.DeviceCustomerID;
                popup.CustomerId = ViewModel.SelectedCustomerForPopup.CustomerID;
            }
        }

        private void FlyoutZoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel?.ZoneSearchHeaderTextChangeCommand.Execute(FlyoutZoneTextBox.Text);
        }

        private async Task RefreshMapIcons()
        {
            try
            {
                ViewModel.SetLoader(true);
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                OptionsAllCheckBox.Checked -= OptionsAllCheckBox_Checked;
                OptionsAllCheckBox.Unchecked -= OptionsAllCheckBox_Unchecked;

                ClearMapPins();

                if (ViewModel.PointOfIntrestSource != null && ViewModel.PointOfIntrestSource.Count > 0)
                {
                    foreach (var item in ViewModel.PointOfIntrestSource)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();
                        await AddMapIconAsync(item);
                    }
                    SetMapCenterAsync();
                }
                SetCheckedState();
                OptionsAllCheckBox.Checked += OptionsAllCheckBox_Checked;
                OptionsAllCheckBox.Unchecked += OptionsAllCheckBox_Unchecked;
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is OperationCanceledException);
            }
            catch (Exception ex)
            {
                if (!(ex is OperationCanceledException))
                    ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(RefreshMapIcons), ex);
            }
            finally
            {
                ViewModel.SetLoader(false);
            }
        }

        private async void SetMapCenterAsync()
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                if (ViewModel.PointOfIntrestSource != null && ViewModel.PointOfIntrestSource.Count > 0)
                {
                    var pinsJson = SerializePinsToJson(ViewModel.PointOfIntrestSource);
                    await ExecuteJavaScriptAsync($"fitMapToPins({pinsJson})");
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(SetMapCenterAsync), ex);
            }
        }

        private async Task AddMapIconAsync(PointOfInterest item)
        {
            try
            {
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                var pinData = new
                {
                    lat = item.Location.Position.Latitude,
                    lng = item.Location.Position.Longitude,
                    title = string.IsNullOrEmpty(item.CustomerData?.CustomerNumber) ? "" : item.CustomerData?.CustomerNumber,
                    customerId = item.CustomerData?.CustomerID ?? 0,
                    deviceCustomerId = item.CustomerData?.DeviceCustomerID ?? "",
                    imageUri = item.ImageSourceUri ?? ""
                };

                var pinJson = System.Text.Json.JsonSerializer.Serialize(new[] { pinData });
                await ExecuteJavaScriptAsync($"addPinsFromCSharp({pinJson})");
            }
            catch (AggregateException ae)
            {
                ae.Handle(e => e is OperationCanceledException);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(AddMapIconAsync), ex);
            }
        }

        private string SerializePinsToJson(System.Collections.ObjectModel.ObservableCollection<PointOfInterest> pins)
        {
            var pinList = new List<Dictionary<string, object>>();
            foreach (var pin in pins)
            {
                var pinDict = new Dictionary<string, object>
                {
                    { "lat", pin.Location.Position.Latitude },
                    { "lng", pin.Location.Position.Longitude },
                    { "title", pin.PinText ?? "" },
                    { "customerId", pin.CustomerData?.CustomerID ?? 0 },
                    { "deviceCustomerId", pin.CustomerData?.DeviceCustomerID ?? "" },
                    { "imageUri", pin.ImageSourceUri ?? "" }
                };
                pinList.Add(pinDict);
            }
            return System.Text.Json.JsonSerializer.Serialize(pinList);
        }

        private void ClearMapPins()
        {
            _ = ExecuteJavaScriptAsync("clearAllPins()");
        }

        private async Task ExecuteJavaScriptAsync(string script)
        {
            try
            {
                if (webView.CoreWebView2 != null)
                {
                    await webView.ExecuteScriptAsync(script);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ExecuteJavaScriptAsync error: {ex.Message}");
            }
        }

        private void myMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
        }

        private async void MapPinClicked(string pinData)
        {
            try
            {
                var pinInfo = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(pinData);
                if (pinInfo != null && pinInfo.ContainsKey("customerId"))
                {
                    var customerId = Convert.ToInt32(pinInfo["customerId"]);
                    var deviceCustomerId = pinInfo.ContainsKey("deviceCustomerId") ? pinInfo["deviceCustomerId"]?.ToString() : null;

                    if (!string.IsNullOrEmpty(deviceCustomerId))
                    {
                        customMapPinPopup.DeviceCustomerId = deviceCustomerId;
                    }
                    customMapPinPopup.CustomerId = customerId;
                    ViewModel.CustomMapPinVisibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), "MapPinClicked", ex.Message);
            }
        }

        private async void ApplyFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SetLoader(true);
                ZoneFilterFlyout.Hide();
                await ViewModel.FilterApplyCommandHandler();
                await RefreshMapIcons();
                ViewModel.SetLoader(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(ApplyFilterButton_Click), ex);
            }
        }

        private async void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ViewModel.SetLoader(true);
                ZoneFilterFlyout.Hide();
                await ViewModel.ClearFilterCommand.ExecuteAsync(null);
                await RefreshMapIcons();
                ViewModel.SetLoader(false);
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AzureMapPage), nameof(ClearFilterButton_Click), ex);
            }
        }

        private bool NotNull(object value)
        {
            if (value != null)
            {
                if (value is System.Collections.ICollection)
                    return (value as System.Collections.ICollection)?.Count > 0;

                return true;
            }
            return false;
        }

        private double IsItemsActive()
        {
            return itemStatus ? 1d : 0.1d;
        }

        private bool InvertBool(bool value) => !value;

        #region WebView Event Handlers

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"AzureMapPage: NavigationStarting - {args.Uri}");
        }

        private async void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                System.Diagnostics.Debug.WriteLine("AzureMapPage: NavigationCompleted - SUCCESS");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"AzureMapPage: NavigationCompleted - FAILED: {args.WebErrorStatus}");
                var dialog = new Windows.UI.Popups.MessageDialog($"Map failed to load: {args.WebErrorStatus}", "Map Error");
                await dialog.ShowAsync();
            }
        }

        private void WebView_DOMContentLoaded(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("AzureMapPage: DOMContentLoaded");
        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            string message = "";
            try { message = args.TryGetWebMessageAsString(); } catch { }
            System.Diagnostics.Debug.WriteLine($"AzureMapPage: WebMessageReceived: {message}");
            if (!string.IsNullOrEmpty(message))
            {
                if (message.StartsWith("pinClicked:"))
                {
                    var pinData = message.Substring(11);
                    MapPinClicked(pinData);
                }
            }
        }

        #endregion
    }
}
