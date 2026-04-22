using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.Foundation;

namespace DRLMobile.Uwp.Helpers.MapHelpers
{
    public sealed class Cluster
    {
        public IList<PointOfInterest> Places { get; private set; } = new List<PointOfInterest>();

        public Geopoint Location { get; set; }

        public Point MapCordinates { get; set; }
    }
}
