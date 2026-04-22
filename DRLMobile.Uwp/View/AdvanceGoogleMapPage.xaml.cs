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
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI;

namespace DRLMobile.Uwp.View
{
    public sealed partial class AdvanceGoogleMapPage : Page
    {
        private MapPageViewModel ViewModel = new MapPageViewModel();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool itemStatus = true;
        private bool isMapReady = false;

        public AdvanceGoogleMapPage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            this.Loaded += AdvanceGoogleMapPage_Loaded;
            this.Unloaded += AdvanceGoogleMapPage_Unloaded;
            InitializeWebView();
        }

        private async void InitializeWebView()
        {
            try
            {
                await webView.EnsureCoreWebView2Async();

                if (DRLMobile.Core.Helpers.HelperMethods.IsProdEnviornment)
                {
                    webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                    webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
                }
                string appPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
                webView.CoreWebView2.SetVirtualHostNameToFolderMapping(
                    "drlmaps.local", appPath,
                    Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);

                webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
                webView.Source = new Uri("http://drlmaps.local/Assets/Maps/AdvanceGoogleMap.html");
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AdvanceGoogleMapPage), nameof(InitializeWebView), ex);
            }
        }

        private async void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            try
            {
                string message = args.TryGetWebMessageAsString();
                var data = JsonConvert.DeserializeObject<dynamic>(message);
                string type = data.type;

                if (type == "mapReady")
                {
                    ViewModel.SetLoader(true);
                    isMapReady = true;
                    await RefreshMapIcons();

                }
                else if (type == "markerClick")
                {
                    string customerId = data.customerId;
                    string deviceCustomerId = data.deviceCustomerId;

                    // Locate the customer in the ViewModel source to replicate behavior
                    var poi = ViewModel.PointOfIntrestSource.FirstOrDefault(x =>
                        x.CustomerData?.CustomerID.ToString() == customerId &&
                        x.CustomerData?.DeviceCustomerID.ToString() == deviceCustomerId);

                    if (poi != null)
                    {
                        customMapPinPopup.DeviceCustomerId = deviceCustomerId;
                        customMapPinPopup.CustomerId = Convert.ToInt32(customerId);
                        ViewModel.CustomMapPinVisibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AdvanceGoogleMapPage), nameof(CoreWebView2_WebMessageReceived), ex);
            }
            finally
            {
                ViewModel.SetLoader(false);
            }
        }

        private void AdvanceGoogleMapPage_Unloaded(object sender, RoutedEventArgs e)
        {
            OptionsAllCheckBox.Checked -= OptionsAllCheckBox_Checked;
            OptionsAllCheckBox.Unchecked -= OptionsAllCheckBox_Unchecked;
            if (ViewModel.PointOfIntrestSource.Count == 0)
            {
                this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Disabled;
            }
        }

        private async void AdvanceGoogleMapPage_Loaded(object sender, RoutedEventArgs e)
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
                if (isMapReady) await RefreshMapIcons();
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AdvanceGoogleMapPage), nameof(AdvanceGoogleMapPage_Loaded), ex);
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

            var context = (sender as StackPanel).DataContext as MapsLegendFilterUIModel;
            await ViewModel.OnMapLegendItemClickCommandHandler(context, cancellationTokenSource.Token);

            await RefreshMapIcons();

            itemStatus = true;
            ViewModel.SetLoader(false);
        }

        private async void OptionsAllCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            ViewModel.LegendsFilterSource.ForEach(x => { if (!x.IsSelected) x.IsSelected = true; });
            await ViewModel.FilterTheCurrentDataOnTheBasisOfLegendsAsync(CancellationToken.None);
            await RefreshMapIcons();
            ViewModel.SetLoader(false);
        }

        private async void OptionsAllCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            ViewModel.LegendsFilterSource.ForEach(x => { if (x.IsSelected) x.IsSelected = false; });
            await ViewModel.FilterTheCurrentDataOnTheBasisOfLegendsAsync(CancellationToken.None);
            await RefreshMapIcons();
            ViewModel.SetLoader(false);
        }

        private void SetCheckedState()
        {
            int itemCount = ViewModel.LegendsFilterSource.Count;
            int selectedCount = ViewModel.LegendsFilterSource.Where(x => x.IsSelected).Count();
            OptionsAllCheckBox.IsChecked = (itemCount == selectedCount);
        }

        private async void PlotAccountByFlyoutGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.SetLoader(true);
            var dataContext = (PlotByTypeFilterUIModel)(sender as Grid).DataContext;
            await ViewModel.OnPlotByFilterItemClickCommandHandler(dataContext);
            PlotAccountByFlyout.Hide();

            if (dataContext.Tag == MapFilter.Item)
            {
                ZoneFilterFlyout.ShowAt(ZoneFilterButton);
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
            if (ViewModel.IsRegionDropDownEnabled)
            {
                FlyoutRegionTextBox.Text = string.Empty;
                ViewModel?.RegionSearchHeaderTextChangeCommandHandler(string.Empty);
                RegionTypeFlyout.ShowAt(RegionSelectionGrid as FrameworkElement);
            }
        }

        private void ZoneSelectionGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.IsNationalHead)
            {
                FlyoutZoneTextBox.Text = "";
                ViewModel?.ZoneSearchHeaderTextChangeCommandHandler(string.Empty);
                ZoneTypeFlyout.ShowAt(ZoneSelectionGrid as FrameworkElement);
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

        private string ColorToHex(SolidColorBrush brush)
        {
            if (brush == null) return "#FF0000"; // Default red

            Color color = brush.Color;
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private async Task RefreshMapIcons()
        {
            if (!isMapReady) return;

            try
            {
                ViewModel.SetLoader(true);
                OptionsAllCheckBox.Checked -= OptionsAllCheckBox_Checked;
                OptionsAllCheckBox.Unchecked -= OptionsAllCheckBox_Unchecked;

                await webView.CoreWebView2.ExecuteScriptAsync("clearMap();");

                if (ViewModel.PointOfIntrestSource != null && ViewModel.PointOfIntrestSource.Count > 0)
                {
                    var markers = ViewModel.PointOfIntrestSource.Select(x => new
                    {
                        lat = x.Location.Position.Latitude,
                        lng = x.Location.Position.Longitude,
                        title = string.IsNullOrEmpty(x.CustomerData?.CustomerNumber) ? "" : x.CustomerData?.CustomerNumber,
                        color = ColorToHex(x.PinColor),
                        customerId = x.CustomerData?.CustomerID,
                        deviceCustomerId = x.CustomerData?.DeviceCustomerID
                    }).ToList();

                    string jsonMarkers = JsonConvert.SerializeObject(markers);
                    await webView.CoreWebView2.ExecuteScriptAsync($"addMarkers({jsonMarkers});");

                    await SetMapCenterAsync();
                }

                SetCheckedState();
                OptionsAllCheckBox.Checked += OptionsAllCheckBox_Checked;
                OptionsAllCheckBox.Unchecked += OptionsAllCheckBox_Unchecked;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(AdvanceGoogleMapPage), nameof(RefreshMapIcons), ex);
            }
            finally
            {
                ViewModel.SetLoader(false);
            }
        }

        public async Task SetMapCenterAsync()
        {
            if (!isMapReady || ViewModel.PointOfIntrestSource.Count == 0) return;

            var bounds = ViewModel.PointOfIntrestSource.Select(x => new
            {
                lat = x.Location.Position.Latitude,
                lng = x.Location.Position.Longitude
            }).ToList();

            string jsonBounds = JsonConvert.SerializeObject(bounds);
            await webView.CoreWebView2.ExecuteScriptAsync($"setMapView({jsonBounds});");
        }

        private void FlyoutZoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel?.ZoneSearchHeaderTextChangeCommand.Execute(FlyoutZoneTextBox.Text);
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
                ErrorLogger.WriteToErrorLog(nameof(AdvanceGoogleMapPage), nameof(ApplyFilterButton_Click), ex);
            }
        }
    }
}
