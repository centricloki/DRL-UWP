using DRLMobile.Core.Models.DataSyncRequestModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace DRLMobile.Core.Models
{
    public class RoutingOptimizationResponseModel
    {
        public string job_id { get; set; }
        public string status { get; set; }
        public RouteRespSolution solution { get; set; }
    }

    public class RouteRespSolution
    {
        public int costs { get; set; }
        public int distance { get; set; }
        public int time { get; set; }
        public List<RouteRespRoute> routes { get; set; }
        public RouteUnAssigned unassigned { get; set; }
    }

    public class RouteRespRoute
    {
        public int costs { get; set; }
        public int distance { get; set; }
        public int time { get; set; }
        public List<RouteRespActivity> activities { get; set; }
    }

    public class RouteRespActivity
    {
        public string type { get; set; }
        public string location_id { get; set; }
        public RouteAddress address { get; set; }
    }

    public class RouteUnAssigned
    {
        public List<RouteUnAssignedDetail> details { get; set; }
    }
    public class RouteUnAssignedDetail
    {
        public string id { get; set; }
        public int code { get; set; }
        public string reason { get; set; }
    }
}
