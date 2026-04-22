using System;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace DRLMobile.Uwp.Helpers.MapHelpers
{
    public struct PlaceLocation
    {
        public PlaceLocation(double latitude, double longitude)
        {
            Geoposition = new BasicGeoposition() { Latitude = latitude, Longitude = longitude };
            MapCoordinates = GetMapCoordinates(Geoposition);
        }
        public BasicGeoposition Geoposition { get; }
        public Point MapCoordinates { get; }

        static private Point GetMapCoordinates(BasicGeoposition geoposition)
        {
            double latitude = Math.Max(Math.Min(geoposition.Latitude, 85.05112878), -85.05112878);

            double sinLatitude = Math.Sin(latitude * Math.PI / 180.0);
            return new Point
            {
                X = (geoposition.Longitude + 180.0) / 360.0,
                Y = 0.5 - Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude)) / (4 * Math.PI)
            };
        }
    }
}
