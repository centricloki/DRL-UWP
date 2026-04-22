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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MapPage : Page
    {
        private MapPageViewModel ViewModel = new MapPageViewModel();
        private int intZoomLevel;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private bool itemStatus = true;

        public MapPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
            Loaded += MapPage_Loaded;
            Unloaded += MapPage_Unloaded;
        }

        private void MapPage_Unloaded(object sender, RoutedEventArgs e)
        {
            myMap?.MapElements?.Clear();
            myMap?.Children?.Clear();
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
                ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(MapPage_Loaded), ex);
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

            int mapZoomLevel = (int)myMap.ZoomLevel;

            ViewModel.previousZoomLevel = mapZoomLevel;

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

            int mapZoomLevel = (int)myMap.ZoomLevel;

            ViewModel.previousZoomLevel = mapZoomLevel;

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
                ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(CitySelectionGrid_Tapped), ex.Message);
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
                ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(ZoneSelectionGrid_Tapped), ex.Message);
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
                ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(ItemSelectionGrid_Tapped), ex.Message);
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

                // Erase the old map icons.
                myMap?.MapElements?.Clear();
                myMap?.Children?.Clear();

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
                    ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(RefreshMapIcons), ex);
            }
            finally
            {
                ViewModel.SetLoader(false);
            }
        }
        public async void SetMapCenterAsync()
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (ViewModel.PointOfIntrestSource.Count > 1)
            {
                GeoboundingBox geoboundingBox = GeoboundingBox.TryCompute(ViewModel.PointOfIntrestSource.Select(x =>
                new BasicGeoposition
                {
                    Latitude = x.Location.Position.Latitude,
                    Longitude = x.Location.Position.Longitude,
                }));
                await myMap.TrySetViewBoundsAsync(geoboundingBox, null, MapAnimationKind.None);
            }
            else
            {
                PointOfInterest place = ViewModel.PointOfIntrestSource.FirstOrDefault();
                if (place != null)
                    await myMap.TrySetViewAsync(new Geopoint(new BasicGeoposition { Latitude = place.Location.Position.Latitude, Longitude = place.Location.Position.Longitude })
                        , 14, null, null, MapAnimationKind.None);
            }
        }
        private async Task AddMapIconAsync(PointOfInterest item)
        {
            await Windows.ApplicationModel.Core.CoreApplication
                       .MainView.CoreWindow.Dispatcher
                       .RunAsync(CoreDispatcherPriority.Normal,
                    async () =>
                    {
                        try
                        {
                            cancellationTokenSource.Token.ThrowIfCancellationRequested();

                            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(item.ImageSourceUri));
                            var imageReference = await ResizeImageAsync(file, 25, 40);
                            //var streamImage = RandomAccessStreamReference.CreateFromUri(new Uri(item.ImageSourceUri));
                            MapIcon mapIcon = new MapIcon();
                            //mapIcon.Image = streamImage;
                            mapIcon.Image = imageReference;
                            mapIcon.Location = item.Location;
                            mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 0.5);
                            mapIcon.Title = string.IsNullOrEmpty(item.CustomerData?.CustomerNumber) ? "" : item.CustomerData?.CustomerNumber;
                            mapIcon.Tag = item;
                            mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                            myMap.MapElements.Add(mapIcon);
                        }
                        catch (AggregateException ae)
                        {
                            ae.Handle(e => e is OperationCanceledException);
                        }
                        catch (Exception ex)
                        {
                            ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(AddMapIconAsync), ex);
                        }
                    });
        }
        private async Task<RandomAccessStreamReference> ResizeImageAsync(StorageFile imageFile, uint scaledWidth, uint scaledHeight)
        {
            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);

                //create a RandomAccessStream as output stream
                var memStream = new InMemoryRandomAccessStream();

                //creates a new BitmapEncoder and initializes it using data from an existing BitmapDecoder
                BitmapEncoder encoder = await BitmapEncoder.CreateForTranscodingAsync(memStream, decoder);

                //resize the image
                encoder.BitmapTransform.ScaledWidth = scaledWidth;
                encoder.BitmapTransform.ScaledHeight = scaledHeight;

                //commits and flushes all of the image data
                await encoder.FlushAsync();

                //return the output stream as RandomAccessStreamReference
                return RandomAccessStreamReference.CreateFromStream(memStream);
            }
        }

        private void myMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            try
            {
                MapIcon mapClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;

                if (mapClickedIcon != null && mapClickedIcon.Tag != null)
                {
                    if ((mapClickedIcon.Tag as PointOfInterest) != null)
                    {
                        var currentPoint = mapClickedIcon.Tag as PointOfInterest;

                        customMapPinPopup.DeviceCustomerId = currentPoint.CustomerData.DeviceCustomerID;
                        customMapPinPopup.CustomerId = currentPoint.CustomerData.CustomerID;

                        ViewModel.CustomMapPinVisibility = Visibility.Visible;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPage), "myMap_MapElementClick", ex.Message);
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
                ErrorLogger.WriteToErrorLog(nameof(MapPage), nameof(ApplyFilterButton_Click), ex);
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
    }
}