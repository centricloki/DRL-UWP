using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.CustomControls;
using DRLMobile.Uwp.Helpers;
using DRLMobile.Uwp.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using System.Linq;
using DRLMobile.ExceptionHandler;
using Windows.UI;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace DRLMobile.Uwp.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ViewRouteListPage : Page
    {
        ViewRouteListPageViewModel ViewModel = new ViewRouteListPageViewModel();
        private RouteListUIModel _navigationEventParam = null;
        public ObservableCollection<string> Numbers { get; set; }
        /// private CancellationTokenSource _cts = null;
        ///private uint _desireAccuracyInMetersValue = 0;

        public ViewRouteListPage()
        {
            DataContext = ViewModel;
            this.InitializeComponent();
            DownArrow.Glyph = "\xe936";
            Loaded += ViewRouteListPage_Loaded;
            Unloaded += ViewRouteListPage_Unloaded;
        }

        private void ViewRouteListPage_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private async void ViewRouteListPage_Loaded(object sender, RoutedEventArgs e)
        {
            ShellPage shellPage = ((Window.Current.Content as Frame).Content as ShellPage);
            if (shellPage != null)
            {
                shellPage.ViewModel.IsSideMenuItemClickable = false;
            }
            try
            {
                await InitializeMap();
                if (_navigationEventParam != null)
                {
                    ViewModel?.RouteDetailsItemSource.Clear();
                    ViewModel?.OnNavigatedToCommand?.Execute(_navigationEventParam);

                    ViewModel.StartLocation = ViewModel.EndLocation = "";
                    CustomerListPanel.Visibility = Visibility.Collapsed;
                    DownArrow.Glyph = "\xe936";
                    DownArrowButton.Padding = new Thickness(15, 10, 15, 0);

                    ViewModel.PointOfIntrestSource.Clear();
                    ViewModel.CustomMapPinVisibility = Visibility.Collapsed;
                    ViewModel.CustomMapPinIsVisible = false;
                    myMap.Routes.Clear();
                    RefreshMapIcons();
                    ViewModel.IsAllChecked = false;
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPage), nameof(ViewRouteListPage_Loaded), ex);
            }
            finally
            {
                if (shellPage != null)
                {
                    shellPage.ViewModel.IsSideMenuItemClickable = true;
                }
            }
        }
        private async Task InitializeMap()
        {
            await myMap.InitializeAsync();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.NavigationMode == NavigationMode.New)
            {
                _navigationEventParam = (RouteListUIModel)e.Parameter;
            }
        }
        private async void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.CalculateButtonCommand.ExecuteAsync(myMap);
            await RefreshMapIcons();
        }

        private async Task RefreshMapIcons()
        {
            try
            {
                await myMap.ClearAllAsync();

                if (ViewModel.PointOfIntrestSource != null && ViewModel.PointOfIntrestSource.Count > 0)
                {
                    //var pushPins = new List<OnTerra.MapsControl.UWP.MapIcon>(ViewModel.PointOfIntrestSource.Count);
                    //foreach (var item in ViewModel.PointOfIntrestSource)
                    //{
                    //    var streamImage = RandomAccessStreamReference.CreateFromUri(new Uri(item.ImageSourceUri));
                    //    OnTerra.MapsControl.UWP.MapIcon mapIcon = new OnTerra.MapsControl.UWP.MapIcon();
                    //    mapIcon.Image = streamImage;
                    //    mapIcon.Location = item.OnTerraLocation;
                    //    mapIcon.NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1);
                    //    mapIcon.Title = item.PinText;
                    //    mapIcon.Tag = item;
                    //    mapIcon.CollisionBehaviorDesired = OnTerra.MapsControl.UWP.MapElementCollisionBehavior.RemainVisible;
                    //    pushPins.Add(mapIcon);
                    //}
                    //await myMap.PushpinAsync(pushPins);

                    var startImgUri = ViewModel.PointOfIntrestSource.FirstOrDefault().ImageSourceUri;
                    var endImgUri = ViewModel.PointOfIntrestSource.LastOrDefault().ImageSourceUri;

                    OnTerra.MapsControl.UWP.MapPolyline line = new OnTerra.MapsControl.UWP.MapPolyline
                    {
                        StrokeColor = Colors.Blue,
                        StrokeThickness = 4,
                        StrokeDashed = false,
                        StartMarkerLabel = "Start",
                        EndMarkerLabel = "End",
                        StartMarkerImage = RandomAccessStreamReference.CreateFromUri(
        new Uri(startImgUri)),
                        EndMarkerImage = RandomAccessStreamReference.CreateFromUri(
        new Uri(endImgUri)),
                        IntermediateMarkerImage = RandomAccessStreamReference.CreateFromUri(
        new Uri("ms-appx:///Assets/Maps/MapPin-Red.png")),
                        Path = new OnTerra.MapsControl.UWP.Geopath(
                            ViewModel.PointOfIntrestSource.Select(x => x.OnTerraLocation.Position)
                            )
                    };
                    await myMap.PolylineAsync(line);
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ViewRouteListPage), nameof(RefreshMapIcons), ex);
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

        private void myMap_MapElementClick(object sender, OnTerra.MapsControl.UWP.MapElementClickEventArgs args)
        {
            try
            {
                OnTerra.MapsControl.UWP.MapIcon mapClickedIcon = args.MapElements.FirstOrDefault(x => x is OnTerra.MapsControl.UWP.MapIcon) as OnTerra.MapsControl.UWP.MapIcon;
                if (mapClickedIcon != null && mapClickedIcon.Tag != null)
                {
                    if ((mapClickedIcon.Tag as PointOfInterest) != null)
                    {
                        var currentPoint = mapClickedIcon.Tag as PointOfInterest;

                        customMapPinPopup.DeviceCustomerId = currentPoint.CustomerData.DeviceCustomerID;
                        customMapPinPopup.CustomerId = currentPoint.CustomerData.CustomerID;

                        ViewModel.CustomMapPinVisibility = Visibility.Visible;
                        ViewModel.CustomMapPinIsVisible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapPage), "myMap_MapElementClick", ex.Message);
            }
        }


        //private async void myMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
        //{
        //    try
        //    {
        //        MapIcon mapClickedIcon = args.MapElements.FirstOrDefault(x => x is MapIcon) as MapIcon;

        //        if (mapClickedIcon != null && mapClickedIcon.Tag != null)
        //        {
        //            if ((mapClickedIcon.Tag as PointOfInterest) != null)
        //            {
        //                var currentPoint = mapClickedIcon.Tag as PointOfInterest;
        //                if (currentPoint.CustomerData != null)
        //                {
        //                    customMapPinPopup.DeviceCustomerId = currentPoint.CustomerData.DeviceCustomerID;
        //                    customMapPinPopup.CustomerId = currentPoint.CustomerData.CustomerID;
        //                    ViewModel.CustomMapPinVisibility = Visibility.Visible;
        //                    ViewModel.CustomMapPinIsVisible = true;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        //ErrorLogger.WriteToErrorLog(nameof(MapPage), "myMap_MapElementClick", ex.Message);
        //    }
        //}

        private void mapItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                ViewModel.SelectedCustomerForPopup = ((sender as Button).DataContext as PointOfInterest)?.RouteData;

                if (ViewModel?.SelectedCustomerForPopup != null)
                {
                    //FlyoutBase.ShowAttachedFlyout(sender as FrameworkElement);
                    customMapPinPopup.DeviceCustomerId = ViewModel?.SelectedCustomerForPopup.DeviceCustomerID;
                    customMapPinPopup.CustomerId = (int)(ViewModel?.SelectedCustomerForPopup.CustomerID);
                    ViewModel.CustomMapPinVisibility = Visibility.Visible;
                }
            }
        }

        private void PinPopUpFlyout_Opened(object sender, object e)
        {
            if (sender is Flyout && (sender as Flyout).Content is MapPinCustomPopUp)
            {
                var popup = (sender as Flyout).Content as MapPinCustomPopUp;

                popup.DeviceCustomerId = ViewModel?.SelectedCustomerForPopup?.DeviceCustomerID;

                if (ViewModel != null)
                {
                    popup.CustomerId = ViewModel.SelectedCustomerForPopup.CustomerID;
                }
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
    }
}
