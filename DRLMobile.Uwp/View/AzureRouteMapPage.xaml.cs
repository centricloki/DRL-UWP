using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using System.Linq;
using Windows.UI.Popups;

namespace DRLMobile.Uwp.View
{
    public sealed partial class AzureRouteMapPage : Page
    {
        ViewRouteListPageViewModel ViewModel = new ViewRouteListPageViewModel();
        private const string AzureMapHtmlPath = "ms-appx-web:///Assets/Maps/AzureRouteMap.html";

        public AzureRouteMapPage()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
            DownArrow.Glyph = "\xe936";
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
                System.Diagnostics.Debug.WriteLine($"AzureRouteMapPage: Navigate failed - {ex.Message}");
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode == NavigationMode.New)
            {
                var parameters = (RouteListUIModel)e.Parameter;
                ViewModel?.RouteDetailsItemSource.Clear();
                ViewModel?.OnNavigatedToCommand?.Execute(parameters);

                ViewModel.StartLocation = ViewModel.EndLocation = "";
                CustomerListPanel.Visibility = Visibility.Collapsed;
                DownArrow.Glyph = "\xe936";
                DownArrowButton.Padding = new Thickness(15, 10, 15, 0);

                ViewModel.PointOfIntrestSource.Clear();
                ViewModel.CustomMapPinVisibility = Visibility.Collapsed;
                ViewModel.CustomMapPinIsVisible = false;
                ViewModel.IsAllChecked = false;

                ClearMapPins();
            }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            ShowLoading(true);
            try
            {
                await (ViewModel.CalculateButtonCommand as Microsoft.Toolkit.Mvvm.Input.IAsyncRelayCommand).ExecuteAsync(null);
                await RefreshMapIcons();
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async Task RefreshMapIcons()
        {
            try
            {
                ClearMapPins();

                if (ViewModel.PointOfIntrestSource != null && ViewModel.PointOfIntrestSource.Count > 0)
                {
                    var pinsJson = SerializePinsToJson(ViewModel.PointOfIntrestSource);
                    await ExecuteJavaScriptAsync($"addPinsFromCSharp({pinsJson})");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshMapIcons error: {ex.Message}");
            }
        }

        private string SerializePinsToJson(ObservableCollection<PointOfInterest> pins)
        {
            var pinList = new List<Dictionary<string, object>>();
            foreach (var pin in pins)
            {
                var pinDict = new Dictionary<string, object>
                {
                    { "lat", pin.Location.Position.Latitude },
                    { "lng", pin.Location.Position.Longitude },
                    { "title", pin.PinText ?? "" },
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

        private void ShowLoading(bool show)
        {
            LoadingOverlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DownArrowButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CustomerListPanel.Visibility = (CustomerListPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            DownArrow.Glyph = (CustomerListPanel.Visibility == Visibility.Visible) ? "\xe935" : "\xe936";
            var downArrowPadding = new Thickness(15, 10, 15, 0);
            var upArrowPadding = new Thickness(15, 0, 15, 10);
            DownArrowButton.Padding = (CustomerListPanel.Visibility == Visibility.Visible) ? upArrowPadding : downArrowPadding;
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
                    ViewModel.CustomMapPinIsVisible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MapPinClicked error: {ex.Message}");
            }
        }

        private void EndLocationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (EndLocationSwitch.IsOn)
            {
                EndTextBox.Text = string.Empty;
                EndTextBox.IsReadOnly = true;
                ViewModel.IsEndCurrentLocation = true;
            }
            else
            {
                EndTextBox.IsReadOnly = false;
                ViewModel.IsEndCurrentLocation = false;
            }
        }

        private void StartLocationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (StartLocationSwitch.IsOn)
            {
                StartTextBox.Text = string.Empty;
                StartTextBox.IsReadOnly = true;
                ViewModel.IsStartCurrentLocation = true;
            }
            else
            {
                StartTextBox.IsReadOnly = false;
                ViewModel.IsStartCurrentLocation = false;
            }
        }

        private void CheckBoxGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid)
            {
                ViewModel?.OnCheckBoxClicked?.Execute((sender as Grid).DataContext);
            }
        }

        private void SelectAllCheckBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel?.SelectAllCommand?.Execute(null);
        }

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine($"AzureRouteMapPage: NavigationStarting - {args.Uri}");
        }

        private async void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                System.Diagnostics.Debug.WriteLine("AzureRouteMapPage: NavigationCompleted - SUCCESS");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"AzureRouteMapPage: NavigationCompleted - FAILED: {args.WebErrorStatus}");
                var dialog = new MessageDialog($"Map failed to load: {args.WebErrorStatus}", "Map Error");
                await dialog.ShowAsync();
            }
        }

        private void WebView_DOMContentLoaded(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("AzureRouteMapPage: DOMContentLoaded");
        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            string message = "";
            try { message = args.TryGetWebMessageAsString(); } catch { }
            System.Diagnostics.Debug.WriteLine($"AzureRouteMapPage: WebMessageReceived: {message}");
            if (!string.IsNullOrEmpty(message))
            {
                if (message.StartsWith("pinClicked:"))
                {
                    var pinData = message.Substring(11);
                    MapPinClicked(pinData);
                }
            }
        }
    }
}
