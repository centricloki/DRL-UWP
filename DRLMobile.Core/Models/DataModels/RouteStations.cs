using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class RouteStations
    {
        [JsonProperty("userid")]
        public int UserId { get; set; }

        [PrimaryKey, AutoIncrement]
        [JsonProperty("routestationid")]
        public int? RouteStationId { get; set; }

        [JsonProperty("routeid")]
        public int RouteId { get; set; }

        [JsonProperty("customerid")]
        public int CustomerId { get; set; }

        [JsonProperty("DeviceRouteId")]
        public string DeviceRouteId { get; set; }
    }
}