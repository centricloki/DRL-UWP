using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;
using DRLMobile.ExceptionHandler;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace DRLMobile.Uwp.View
{
    public sealed partial class ViewRouteListPage : Page
    {
        // C# 7.3 Compatible Constants
        private const string GLYPH_ARROW_DOWN = "\xE936";
        private const string GLYPH_ARROW_UP = "\xE935";
        private static readonly Thickness PADDING_ARROW_DOWN = new Thickness(15, 10, 15, 0);
        private static readonly Thickness PADDING_ARROW_UP = new Thickness(15, 0, 15, 10);

        private const int TARGET_MARKER_SIZE = 48;
        private const int MAX_ICON_CACHE_SIZE = 50;

        // C# 7.3 Compatible Field Initialization
        private static readonly ConcurrentDictionary<string, Task<RandomAccessStreamReference>>
            _routeIconCache = new ConcurrentDictionary<string, Task<RandomAccessStreamReference>>(StringComparer.OrdinalIgnoreCase);

        private readonly ViewRouteListPageViewModel ViewModel = new ViewRouteListPageViewModel();

        private CancellationTokenSource _cts = new CancellationTokenSource();

        public ViewRouteListPage()
        {
            DataContext = ViewModel;
            InitializeComponent();
            DownArrow.Glyph = GLYPH_ARROW_DOWN;
            Unloaded += ViewRouteListPage_Unloaded;
        }

        private void ViewRouteListPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
            }
            _cts = new CancellationTokenSource();
            Unloaded -= ViewRouteListPage_Unloaded;
        }

        private async Task ViewRouteListPageLoadedAsync(RouteListUIModel _navigationEventParam)
        {
            var shellPage = (Window.Current.Content as Frame) != null ? (Window.Current.Content as Frame).Content as ShellPage : null;
            if (shellPage != null)
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            try
            {
                if (ViewModel != null)
                    ViewModel.LoadingVisibilityHandler(true);
                await InitializeMap();

                if (_navigationEventParam != null)
                {
                    if (ViewModel != null && ViewModel.RouteDetailsItemSource != null)
                        ViewModel.RouteDetailsItemSource.Clear();

                    if (ViewModel != null && ViewModel.OnNavigatedToCommand != null)
                        ViewModel.OnNavigatedToCommand.Execute(_navigationEventParam);

                    if (ViewModel != null)
                    {
                        ViewModel.StartLocation = string.Empty;
                        ViewModel.EndLocation = string.Empty;
                    }

                    CustomerListPanel.Visibility = Visibility.Collapsed;
                    UpdateArrowState(false);

                    if (ViewModel != null)
                    {
                        if (ViewModel.PointOfInterestSource != null)
                            ViewModel.PointOfInterestSource.Clear();
                        ViewModel.CustomMapPinVisibility = Visibility.Collapsed;
                        ViewModel.CustomMapPinIsVisible = false;
                    }

                    if (myMap != null && myMap.Routes != null)
                        myMap.Routes.Clear();

                    await RefreshMapIcons(_cts.Token);

                    if (ViewModel != null)
                        ViewModel.IsAllChecked = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPage), nameof(ViewRouteListPageLoadedAsync), ex);
            }
            finally
            {
                if (ViewModel != null)
                    ViewModel.LoadingVisibilityHandler(false);
                if (shellPage != null)
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
            }
        }

        private async Task InitializeMap()
        {
            if (myMap != null)
                await myMap.InitializeAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
               await this.ViewRouteListPageLoadedAsync((RouteListUIModel)e.Parameter);
            }
        }

        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.CalculateButtonCommand != null)
                await ViewModel.CalculateButtonCommand.ExecuteAsync(myMap);

            await RefreshMapIcons(_cts.Token);
        }

        // ✅ CS1674 FIX: Removed 'using' on BitmapDecoder/BitmapEncoder.
        // WinRT types sometimes fail compiler IDisposable checks. 
        // Explicit try/finally with .Dispose() is the safe, compatible workaround.
        private static async Task<RandomAccessStreamReference> GetCachedIconAsync(string uri, CancellationToken token = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(uri))
                return null;

            Task<RandomAccessStreamReference> cachedTask;
            if (_routeIconCache.TryGetValue(uri, out cachedTask))
                return await cachedTask.ConfigureAwait(false);

            var loadTask = Task.Run(async () =>
            {
                try
                {
                    if (token.IsCancellationRequested)
                        token.ThrowIfCancellationRequested();

                    if (uri.StartsWith("ms-appx:", StringComparison.OrdinalIgnoreCase) ||
                        uri.StartsWith("ms-appdata:", StringComparison.OrdinalIgnoreCase))
                    {
                        var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(uri));
                        var stream = await file.OpenAsync(FileAccessMode.Read);

                        try
                        {
                            var decoder = await BitmapDecoder.CreateAsync(stream);
                            var transform = new BitmapTransform
                            {
                                ScaledWidth = (uint)TARGET_MARKER_SIZE,
                                ScaledHeight = (uint)TARGET_MARKER_SIZE,
                                InterpolationMode = BitmapInterpolationMode.Linear
                            };

                            var pixelData = await decoder.GetPixelDataAsync(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Straight,
                                transform,
                                ExifOrientationMode.RespectExifOrientation,
                                ColorManagementMode.DoNotColorManage);

                            // outStream must NOT be disposed here. Map control holds reference to it.
                            var outStream = new InMemoryRandomAccessStream();
                            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outStream);
                            encoder.SetPixelData(
                                BitmapPixelFormat.Bgra8,
                                BitmapAlphaMode.Straight,
                                (uint)TARGET_MARKER_SIZE,
                                (uint)TARGET_MARKER_SIZE,
                                decoder.DpiX,
                                decoder.DpiY,
                                pixelData.DetachPixelData());

                            await encoder.FlushAsync();
                            outStream.Seek(0);
                            return RandomAccessStreamReference.CreateFromStream(outStream);
                        }
                        finally
                        {
                            stream.Dispose();
                        }
                    }
                    else
                    {
                        return RandomAccessStreamReference.CreateFromUri(new Uri(uri));
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    return RandomAccessStreamReference.CreateFromUri(new Uri(uri));
                }
            }, token);

            if (_routeIconCache.Count >= MAX_ICON_CACHE_SIZE)
            {
                var itemsToRemove = _routeIconCache.Keys.Take(MAX_ICON_CACHE_SIZE / 4).ToList();
                foreach (var key in itemsToRemove)
                {
                    _routeIconCache.TryRemove(key, out _);
                }
            }

            _routeIconCache[uri] = loadTask;
            return await loadTask.ConfigureAwait(false);
        }

        private async Task RefreshMapIcons(CancellationToken token = default(CancellationToken))
        {
            try
            {
                if (token.IsCancellationRequested)
                    token.ThrowIfCancellationRequested();

                if (myMap != null)
                    await myMap.ClearAllAsync();

                if (ViewModel != null && ViewModel.PointOfInterestSource != null && ViewModel.PointOfInterestSource.Count > 0)
                {
                    var startItem = ViewModel.PointOfInterestSource.FirstOrDefault();
                    var endItem = ViewModel.PointOfInterestSource.LastOrDefault();

                    string startImgUri = startItem != null ? startItem.ImageSourceUri : null;
                    string endImgUri = endItem != null ? endItem.ImageSourceUri : null;
                    const string intermediateUri = "ms-appx:///Assets/Maps/MapPin-Red.png";

                    if (!string.IsNullOrEmpty(startImgUri) && !string.IsNullOrEmpty(endImgUri))
                    {
                        var startIconTask = GetCachedIconAsync(startImgUri, token);
                        var endIconTask = GetCachedIconAsync(endImgUri, token);
                        var intermediateIconTask = GetCachedIconAsync(intermediateUri, token);

                        await Task.WhenAll(startIconTask, endIconTask, intermediateIconTask);

                        var line = new OnTerra.MapsControl.UWP.MapPolyline
                        {
                            StrokeColor = Colors.Blue,
                            StrokeThickness = 4,
                            StrokeDashed = false,
                            StartMarkerLabel = "Start",
                            EndMarkerLabel = "End",
                            StartMarkerImage = await startIconTask,
                            EndMarkerImage = await endIconTask,
                            IntermediateMarkerImage = await intermediateIconTask,
                            Path = new OnTerra.MapsControl.UWP.Geopath(
                                ViewModel.PointOfInterestSource.Select(x =>
                                 new OnTerra.MapsControl.UWP.BasicGeoposition
                                 {
                                     Latitude = x.OnTerraLocation.Position.Latitude,
                                     Longitude = x.OnTerraLocation.Position.Longitude,
                                     Tag = x?.CustomerData?.CustomerID.ToString()
                                 }))
                        };

                        if (myMap != null)
                        {
                            await myMap.PolylineAsync(line);
                            OnTerra.MapsControl.UWP.GeoboundingBox geoboundingBox = OnTerra.MapsControl.UWP.GeoboundingBox.TryCompute(
                                ViewModel.PointOfInterestSource.Select(x =>
                             new OnTerra.MapsControl.UWP.BasicGeoposition
                             {
                                 Latitude = x.OnTerraLocation.Position.Latitude,
                                 Longitude = x.OnTerraLocation.Position.Longitude,
                             }));
                            await myMap.TrySetViewBoundsAsync(geoboundingBox, null, OnTerra.MapsControl.UWP.MapAnimationKind.Default);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when page is unloaded
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPage), nameof(RefreshMapIcons), ex);
            }
        }

        private void UpdateArrowState(bool isExpanded)
        {
            DownArrow.Glyph = isExpanded ? GLYPH_ARROW_UP : GLYPH_ARROW_DOWN;
            DownArrowButton.Padding = isExpanded ? PADDING_ARROW_UP : PADDING_ARROW_DOWN;
            CustomerListPanel.Visibility = isExpanded ? Visibility.Visible : Visibility.Collapsed;
        }

        private void DownArrowButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var isExpanded = CustomerListPanel.Visibility == Visibility.Visible;
            UpdateArrowState(!isExpanded);
        }

        private void myMap_MapElementClick(object sender, OnTerra.MapsControl.UWP.MapElementClickEventArgs args)
        {
            try
            {
                var mapClickedIcon = args.MapElements
                    .OfType<OnTerra.MapsControl.UWP.MapIcon>()
                    .FirstOrDefault();

                if (mapClickedIcon != null)
                {
                    // var currentPoint = mapClickedIcon.Tag as PointOfInterest;
                    if (int.TryParse(mapClickedIcon.Title, out int _customerId))
                    {
                        var currentPoint = ViewModel.PointOfInterestSource.Where(x => x.CustomerData != null).FirstOrDefault(x => x?.CustomerData.CustomerID == _customerId);
                        if (currentPoint != null && currentPoint.CustomerData != null)
                        {
                            customMapPinPopup.DeviceCustomerId = currentPoint.CustomerData.DeviceCustomerID;
                            customMapPinPopup.CustomerId = currentPoint.CustomerData.CustomerID;

                            if (ViewModel != null)
                            {
                                ViewModel.CustomMapPinVisibility = Visibility.Visible;
                                ViewModel.CustomMapPinIsVisible = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPage), nameof(myMap_MapElementClick), ex);
            }
        }

        private void PinPopUpFlyout_Opened(object sender, object e)
        {
            if (sender is Flyout flyout && flyout.Content is MapPinCustomPopUp popup)
            {
                if (ViewModel != null)
                {
                    popup.DeviceCustomerId = ViewModel.SelectedCustomerForPopup != null ? ViewModel.SelectedCustomerForPopup.DeviceCustomerID : string.Empty;
                    if (ViewModel.SelectedCustomerForPopup != null)
                    {
                        popup.CustomerId = ViewModel.SelectedCustomerForPopup.CustomerID;
                    }
                }
            }
        }

        private void EndLocationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (EndLocationSwitch.IsOn)
            {
                EndTextBox.Text = string.Empty;
                EndTextBox.IsReadOnly = true;
                if (ViewModel != null) ViewModel.IsEndCurrentLocation = true;
            }
            else
            {
                EndTextBox.IsReadOnly = false;
                if (ViewModel != null) ViewModel.IsEndCurrentLocation = false;
            }
        }

        private void StartLocationSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if (StartLocationSwitch.IsOn)
            {
                StartTextBox.Text = string.Empty;
                StartTextBox.IsReadOnly = true;
                if (ViewModel != null) ViewModel.IsStartCurrentLocation = true;
            }
            else
            {
                StartTextBox.IsReadOnly = false;
                if (ViewModel != null) ViewModel.IsStartCurrentLocation = false;
            }
        }

        private void CheckBoxGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is Grid grid && ViewModel != null && ViewModel.OnCheckBoxClicked != null)
            {
                ViewModel.OnCheckBoxClicked.Execute(grid.DataContext);
            }
        }

        private void SelectAllCheckBox_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel != null && ViewModel.SelectAllCommand != null)
            {
                ViewModel.SelectAllCommand.Execute(null);
            }
        }
    }
}