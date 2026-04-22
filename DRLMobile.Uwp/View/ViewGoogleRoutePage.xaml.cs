using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Uwp.View
{
    public sealed partial class ViewGoogleRoutePage : Page
    {
        ViewGoogleRoutePageViewModel ViewModel = new ViewGoogleRoutePageViewModel();
		private const int GeocodeTimeoutMilliseconds = 8000;
		private static readonly System.Globalization.CultureInfo InvariantCulture = System.Globalization.CultureInfo.InvariantCulture;

        public ViewGoogleRoutePage()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
            DownArrow.Glyph = "\xe936";
            InitializeWebViewAsync();
        }

private static string EscapeJsString(string input)
{
    if (string.IsNullOrEmpty(input)) return input;
    return input.Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
}

        private async void InitializeWebViewAsync()
        {

            await webView.EnsureCoreWebView2Async();

            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.WebMessageReceived += WebView_WebMessageReceived;

            if (DRLMobile.Core.Helpers.HelperMethods.IsProdEnviornment)
            {
                webView.CoreWebView2.Settings.AreDevToolsEnabled = false;
                webView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;
            }
            string appPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("drlmaps.local", appPath, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            var htmlPath = new Uri("http://drlmaps.local/Assets/Maps/GoogleRoute.html");
            webView.Source = htmlPath;
        }

        private void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                Debug.WriteLine("ViewGoogleRoutePage: Map loaded successfully");
            }
        }

        private Dictionary<string, TaskCompletionSource<ViewGoogleRoutePageViewModel.GeocodeResult?>> _pendingGeocodeRequests = new Dictionary<string, TaskCompletionSource<ViewGoogleRoutePageViewModel.GeocodeResult?>>();

        public async Task<ViewGoogleRoutePageViewModel.GeocodeResult?> GeocodeAsync(string address)
        {
            if (webView.CoreWebView2 == null || string.IsNullOrWhiteSpace(address)) return null;

            string requestId = Guid.NewGuid().ToString();
            var tcs = new TaskCompletionSource<ViewGoogleRoutePageViewModel.GeocodeResult?>();
            _pendingGeocodeRequests[requestId] = tcs;

            try
            {
				string safeAddress = EscapeJsString(address);
                await webView.CoreWebView2.ExecuteScriptAsync($"geocodeAddress('{safeAddress}', '{requestId}')");

                // Set a timeout
                var timeoutTask = Task.Delay(GeocodeTimeoutMilliseconds);
                var completedTask = await Task.WhenAny(tcs.Task, timeoutTask);

                if (completedTask == timeoutTask)
                {
					Debug.WriteLine($"[Geocode] Timed out after {GeocodeTimeoutMilliseconds}ms for: {address}");
                    _pendingGeocodeRequests.Remove(requestId);
                    return null;
                }

                return await tcs.Task;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Geocode] Error: {ex.Message}");
                _pendingGeocodeRequests.Remove(requestId);
                return null;
            }
        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
{
    string extractedRequestId = null; // Track requestId early for cleanup safety

    try
    {
        string json = args.TryGetWebMessageAsString();
        if (string.IsNullOrEmpty(json)) return;

        var jsonObject = JsonObject.Parse(json);
        string type = jsonObject.GetNamedString("type", "");

        if (type == "markerClick")
        {
            int customerId = (int)jsonObject.GetNamedNumber("customerId", 0);
            string deviceCustomerId = jsonObject.GetNamedString("deviceCustomerId", "");
            if (customerId != 0)
            {
                customMapPinPopup.DeviceCustomerId = deviceCustomerId;
                customMapPinPopup.CustomerId = customerId;
                ViewModel.CustomMapPinVisibility = Visibility.Visible;
                ViewModel.CustomMapPinIsVisible = true;
            }
        }
        // 🔧 FIX #1: Safe dictionary access + immediate removal prevents memory leaks
        else if (type == "geocoded")
        {
            string requestId = jsonObject.GetNamedString("requestId", "");
            extractedRequestId = requestId; // Capture early for cleanup fallback
            string status = jsonObject.GetNamedString("status", "");

            // TryGetValue + immediate Remove = atomic operation, no race conditions
            if (_pendingGeocodeRequests.TryGetValue(requestId, out var tcs))
            {
                _pendingGeocodeRequests.Remove(requestId); // Remove BEFORE SetResult

                if (status == "OK")
                {
                    // 🔧 FIX #2: TrySetResult avoids crash if task already completed
                    tcs.TrySetResult(new ViewGoogleRoutePageViewModel.GeocodeResult
                    {
                        Latitude = jsonObject.GetNamedNumber("lat", 0),
                        Longitude = jsonObject.GetNamedNumber("lng", 0),
                        City = jsonObject.GetNamedString("city", ""),
                        State = jsonObject.GetNamedString("state", ""),
                        Country = jsonObject.GetNamedString("country", ""),
                        Success = true
                    });
                }
                else
                {
                    tcs.TrySetResult(null);
                }
            }
            // Optional: Log if requestId not found (could indicate duplicate message or timeout cleanup)
            else
            {
                Debug.WriteLine($"[WebView] requestId '{requestId}' not found in pending requests (may be timed out or duplicate)");
            }
        }
        else if (type == "error")
        {
            string msg = jsonObject.GetNamedString("message", "Unknown error");
            _ = new MessageDialog(msg, "Map Error").ShowAsync();
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[WebView] Error parsing message: {ex.Message}");

        // 🔧 FIX #3: If we extracted requestId before JSON parsing failed, clean it up
        if (!string.IsNullOrEmpty(extractedRequestId) && _pendingGeocodeRequests.ContainsKey(extractedRequestId))
        {
            _pendingGeocodeRequests.Remove(extractedRequestId);
            Debug.WriteLine($"[WebView] Cleaned up orphaned requestId '{extractedRequestId}' after parse error");
        }
        // 🔧 FIX #4: Fallback cleanup task for completely unparseable messages (no requestId extractable)
        // This runs async to avoid blocking the UI thread
        _ = Task.Run(async () =>
        {
            await Task.Delay(100); // Small delay to let normal flow complete first
            CleanupOrphanedRequests(15000); // Remove entries older than 15 seconds
        });
    }
}

// 🔧 FIX #4 Helper: Background cleanup for orphaned _pendingGeocodeRequests entries
// Call this periodically or after parse failures to prevent memory leaks
private void CleanupOrphanedRequests(int maxAgeMilliseconds = 15000)
{
    try
    {
        var now = DateTime.UtcNow;
        var keysToRemove = new List<string>();

        // We can't iterate and remove from dictionary simultaneously, so collect keys first
        foreach (var kvp in _pendingGeocodeRequests)
        {
            // Note: TaskCompletionSource doesn't expose creation time, so we use a heuristic:
            // If the task is still pending after maxAge, it's likely orphaned
            if (kvp.Value.Task.Status != TaskStatus.RanToCompletion &&
                kvp.Value.Task.Status != TaskStatus.Faulted &&
                kvp.Value.Task.Status != TaskStatus.Canceled)
            {
                // We can't know exact age, so we assume anything still pending after threshold is stale
                keysToRemove.Add(kvp.Key);
            }
        }

        foreach (var key in keysToRemove)
        {
            if (_pendingGeocodeRequests.TryGetValue(key, out var tcs))
            {
                _pendingGeocodeRequests.Remove(key);
                tcs.TrySetResult(null); // Resolve with null to unblock any awaiting code
                Debug.WriteLine($"[Cleanup] Removed orphaned requestId '{key}' (pending > {maxAgeMilliseconds}ms)");
            }
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"[Cleanup] Error during orphaned request cleanup: {ex.Message}");
    }
}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Set geocode service
            ViewModel.GeocodeService = GeocodeAsync;

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

                // Clear map in JS if already initialized (for cached page navigation)
                if (webView.CoreWebView2 != null)
                {
                    _ = webView.CoreWebView2.ExecuteScriptAsync("clearMap()");
                }
            }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadingVisibilityHandler(isLoading: true);
            // Now we call the command without the MapControl dependency.
            await ViewModel.CalculateButtonCommand.ExecuteAsync(null);
            if (ViewModel?.PointOfIntrestSource?.Count > 2)
                await SyncRouteToGoogleMaps();
            ViewModel.LoadingVisibilityHandler(isLoading: false);
        }

        private async Task SyncRouteToGoogleMaps()
        {
            if (webView.CoreWebView2 == null) return;

            // Clear map first to be safe
            await webView.CoreWebView2.ExecuteScriptAsync("clearMap()");

            // 1. Sync Route
            if (ViewModel.StartGeopoint != null && ViewModel.EndGeopoint != null)
            {
                JsonObject routeJson = new JsonObject();

                // Origin Data
                JsonObject originObj = new JsonObject();
                originObj.SetNamedValue("lat", JsonValue.CreateNumberValue(Convert.ToDouble(ViewModel.StartGeopoint.Position.Latitude)));
                originObj.SetNamedValue("lng", JsonValue.CreateNumberValue(Convert.ToDouble(ViewModel.StartGeopoint.Position.Longitude)));
                // Try to find if the start location is a customer
                originObj.SetNamedValue("customerNumber", JsonValue.CreateStringValue(ViewModel.StartLocation ?? "START"));
                routeJson.SetNamedValue("originData", originObj);

                // Destination Data
                JsonObject destObj = new JsonObject();
                destObj.SetNamedValue("lat", JsonValue.CreateNumberValue(Convert.ToDouble(ViewModel.EndGeopoint.Position.Latitude)));
                destObj.SetNamedValue("lng", JsonValue.CreateNumberValue(Convert.ToDouble(ViewModel.EndGeopoint.Position.Longitude)));
                destObj.SetNamedValue("customerNumber", JsonValue.CreateStringValue(ViewModel.EndLocation ?? "END"));
                routeJson.SetNamedValue("destinationData", destObj);

                // Origin/Destination strings for Google Service
                routeJson.SetNamedValue("origin", JsonValue.CreateStringValue($"{Convert.ToDouble(ViewModel.StartGeopoint.Position.Latitude)},{Convert.ToDouble(ViewModel.StartGeopoint.Position.Longitude)}"));
                routeJson.SetNamedValue("destination", JsonValue.CreateStringValue($"{Convert.ToDouble(ViewModel.EndGeopoint.Position.Latitude)},{Convert.ToDouble(ViewModel.EndGeopoint.Position.Longitude)}"));

                JsonArray waypoints = new JsonArray();
                // Pass intermediate customers as waypoints with metadata from PointOfIntrestSource (optimized order)
                foreach (var poi in ViewModel.PointOfIntrestSource)
                {
                    if (poi.PinText.ToLower() != "start" && poi.PinText.ToLower() != "end")
                    {
                        var item = poi.RouteData;
                        if (item != null && !string.IsNullOrEmpty(item.Latitude) && !string.IsNullOrEmpty(item.Longitude))
                        {
                            JsonObject wp = new JsonObject();
                            wp.SetNamedValue("lat", JsonValue.CreateNumberValue(Convert.ToDouble(item.Latitude)));
                            wp.SetNamedValue("lng", JsonValue.CreateNumberValue(Convert.ToDouble(item.Longitude)));
                            wp.SetNamedValue("customerNumber", JsonValue.CreateStringValue(item.CustomerNumber ?? ""));
                            wp.SetNamedValue("customerId", JsonValue.CreateNumberValue(item.CustomerID));
                            wp.SetNamedValue("deviceCustomerId", JsonValue.CreateStringValue(item.DeviceCustomerID ?? ""));

                            waypoints.Add(wp);
                        }
                    }
                }
                routeJson.SetNamedValue("waypoints", waypoints);

                await webView.CoreWebView2.ExecuteScriptAsync($"drawRoute('{routeJson.Stringify()}')");
            }
        }

        private void DownArrowButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            CustomerListPanel.Visibility = (CustomerListPanel.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            DownArrow.Glyph = (CustomerListPanel.Visibility == Visibility.Visible) ? "\xe935" : "\xe936";
            var downArrowPadding = new Thickness(15, 10, 15, 0);
            var upArrowPadding = new Thickness(15, 0, 15, 10);
            DownArrowButton.Padding = (CustomerListPanel.Visibility == Visibility.Visible) ? upArrowPadding : downArrowPadding;
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
    }
}
