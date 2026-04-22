using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Uwp.View
{
    public sealed partial class GoogleMapPage : Page
    {
        public GoogleMapPage()
        {
            this.InitializeComponent();
            InitializeWebViewAsync();
        }

        private async void InitializeWebViewAsync()
        {
            await webView.EnsureCoreWebView2Async();
            
            webView.NavigationCompleted += WebView_NavigationCompleted;
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.WebMessageReceived += WebView_WebMessageReceived;
            webView.CoreWebView2.DOMContentLoaded += WebView_DOMContentLoaded;
            
            Debug.WriteLine("=== GoogleMapPage: Starting navigation ===");
            string appPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("drlmaps.local", appPath, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            var htmlPath = new Uri("http://drlmaps.local/Assets/Maps/GoogleMap.html");
            webView.Source = htmlPath;
        }

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Debug.WriteLine($"GoogleMapPage: NavigationStarting - {args.Uri}");
        }

        private async void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                Debug.WriteLine("GoogleMapPage: NavigationCompleted - SUCCESS");
            }
            else
            {
                Debug.WriteLine($"GoogleMapPage: NavigationCompleted - FAILED: {args.WebErrorStatus}");
                var dialog = new MessageDialog($"Map failed to load: {args.WebErrorStatus}", "Map Error");
                await dialog.ShowAsync();
            }
        }

        private void WebView_DOMContentLoaded(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs args)
        {
            Debug.WriteLine("GoogleMapPage: DOMContentLoaded");
        }

        private async void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            string json = "";
            try { json = args.TryGetWebMessageAsString(); } catch { }
            Debug.WriteLine($"GoogleMapPage: WebMessageReceived: {json}");

            if (string.IsNullOrEmpty(json)) return;

            try
            {
                // Simple JSON parsing (since we might not have a full JSON library handy, but UWP has Windows.Data.Json)
                var jsonObject = Windows.Data.Json.JsonObject.Parse(json);
                string type = jsonObject.GetNamedString("type", "");

                if (type == "mapReady")
                {
                    Debug.WriteLine("GoogleMapPage: Map is ready. Loading markers and starting routing demo...");
                    
                    // Define locations in C#
                    var locations = new Windows.Data.Json.JsonArray();
                    
                    var loc1 = new Windows.Data.Json.JsonObject();
                    loc1.SetNamedValue("lat", Windows.Data.Json.JsonValue.CreateNumberValue(34.0522));
                    loc1.SetNamedValue("lng", Windows.Data.Json.JsonValue.CreateNumberValue(-118.2437));
                    loc1.SetNamedValue("address", Windows.Data.Json.JsonValue.CreateStringValue("Los Angeles, CA"));
                    locations.Add(loc1);

                    var loc2 = new Windows.Data.Json.JsonObject();
                    loc2.SetNamedValue("lat", Windows.Data.Json.JsonValue.CreateNumberValue(40.7128));
                    loc2.SetNamedValue("lng", Windows.Data.Json.JsonValue.CreateNumberValue(-74.0060));
                    loc2.SetNamedValue("address", Windows.Data.Json.JsonValue.CreateStringValue("New York, NY"));
                    locations.Add(loc2);

                    var loc3 = new Windows.Data.Json.JsonObject();
                    loc3.SetNamedValue("lat", Windows.Data.Json.JsonValue.CreateNumberValue(41.8781));
                    loc3.SetNamedValue("lng", Windows.Data.Json.JsonValue.CreateNumberValue(-87.6298));
                    loc3.SetNamedValue("address", Windows.Data.Json.JsonValue.CreateStringValue("Chicago, IL"));
                    locations.Add(loc3);

                    string locationsJson = locations.Stringify();
                    
                    // Add markers from C#
                    await webView.CoreWebView2.ExecuteScriptAsync($"addMarkers('{locationsJson}')");

                    // Example: Route from Los Angeles to New York
                    await webView.CoreWebView2.ExecuteScriptAsync("createRoute('Los Angeles, CA', 'New York, NY')");
                }
                else if (type == "markerClick")
                {
                    string address = jsonObject.GetNamedString("address", "Unknown");
                    var dialog = new MessageDialog($"Marker Clicked in Google Map:\nAddress: {address}", "Google Map Interaction");
                    await dialog.ShowAsync();
                }
                else if (type == "routeCreated")
                {
                    Debug.WriteLine("GoogleMapPage: Route created successfully");
                }
                else if (type == "error")
                {
                    string message = jsonObject.GetNamedString("message", "Unknown error");
                    var dialog = new MessageDialog($"Map Error: {message}", "Error");
                    await dialog.ShowAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"GoogleMapPage: Error processing message: {ex.Message}");
            }
        }
    }
}
