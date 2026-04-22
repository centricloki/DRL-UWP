using DRLMobile.Core.Models;
using DRLMobile.Core.Models.DataModels;
using DRLMobile.Core.Models.UIModels;
using DRLMobile.Uwp.Helpers.MapHelpers;
using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace DRLMobile.Uwp.Helpers
{
    public class PointOfInterest : BaseModel
    {
        public Geopoint Location { get; set; }

        public Point NormalizedAnchorPoint { get; set; }

        public SolidColorBrush PinColor { get; set; }

        private string _ImageSourceUri;
        public string ImageSourceUri
        {
            get { return _ImageSourceUri; }
            set { SetProperty(ref _ImageSourceUri, value); }
        }

        private MapCustomerData _customerData;
        public MapCustomerData CustomerData
        {
            get { return _customerData; }
            set { SetProperty(ref _customerData, value); }
        }

        private ViewRouteDetailsUIModel _routeData;
        public ViewRouteDetailsUIModel RouteData
        {
            get { return _routeData; }
            set { SetProperty(ref _routeData, value); }
        }

        private bool _isPinTextVisible;
        public bool IsPinTextVisible
        {
            get { return _isPinTextVisible; }
            set { SetProperty(ref _isPinTextVisible, value); }
        }


        private string _pinText;
        public string PinText
        {
            get { return _pinText; }
            set { SetProperty(ref _pinText, value); }
        }
    }
}
