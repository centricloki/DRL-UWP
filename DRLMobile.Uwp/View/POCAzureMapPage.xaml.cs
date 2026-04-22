using System;
using System.Diagnostics;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace DRLMobile.Uwp.View
{
    public sealed partial class POCAzureMapPage : Page
    {
        public POCAzureMapPage()
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
            
            Debug.WriteLine("=== POCAzureMapPage: Starting navigation ===");
            string appPath = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;
            webView.CoreWebView2.SetVirtualHostNameToFolderMapping("drlmaps.local", appPath, Microsoft.Web.WebView2.Core.CoreWebView2HostResourceAccessKind.Allow);
            var htmlPath = new Uri("http://drlmaps.local/Assets/Maps/POCAzureMap.html");
            webView.Source = htmlPath;
        }

        private void WebView_NavigationStarting(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            Debug.WriteLine($"POCAzureMapPage: NavigationStarting - {args.Uri}");
        }

        private async void WebView_NavigationCompleted(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs args)
        {
            if (args.IsSuccess)
            {
                Debug.WriteLine("POCAzureMapPage: NavigationCompleted - SUCCESS");
            }
            else
            {
                Debug.WriteLine($"POCAzureMapPage: NavigationCompleted - FAILED: {args.WebErrorStatus}");
                var dialog = new MessageDialog($"Map failed to load: {args.WebErrorStatus}", "Map Error");
                await dialog.ShowAsync();
            }
        }

        private void WebView_DOMContentLoaded(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2DOMContentLoadedEventArgs args)
        {
            Debug.WriteLine("POCAzureMapPage: DOMContentLoaded");
        }

        private void WebView_WebMessageReceived(Microsoft.UI.Xaml.Controls.WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            string message = "";
            try { message = args.TryGetWebMessageAsString(); } catch { }
            Debug.WriteLine($"POCAzureMapPage: WebMessageReceived: {message}");
        }
    }
}
