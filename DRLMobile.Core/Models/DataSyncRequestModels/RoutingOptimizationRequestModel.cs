using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class RoutingOptimizationRequestModel
    {
        public List<RouteVehicle> vehicles { get; set; }
        public List<RoutVehicleType> vehicle_types { get; set; }
        public List<RouteService> services { get; set; }
    }
    public class RouteVehicle
    {
        public string vehicle_id { get; set; }
        public string type_id { get; set; }
        public RouteAddress start_address { get; set; }
        public RouteAddress end_address { get; set; }

    }
    public class RoutVehicleType
    {
        public string type_id { get; set; }
        public string profile { get; set; }
    }
    public class RouteService
    {
        public string id { get; set; }
        public string name { get; set; }
        public RouteAddress address { get; set; }

    }
    public class RouteAddress
    {
        public string location_id { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }

    }
}
