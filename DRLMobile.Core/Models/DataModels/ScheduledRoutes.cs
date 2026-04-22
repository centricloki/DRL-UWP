using SQLite;
using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ScheduledRoutes
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty("routeid")]
        public int? RouteId { get; set; }

        [JsonProperty("routename")]
        public string RouteName { get; set; }

        [JsonProperty("streetname")]
        public string StreetName { get; set; }

        [JsonProperty("startdate")]
        public string StartDate { get; set; }

        [JsonProperty("enddate")]
        public string EndDate { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }

        [JsonProperty("userid")]
        public int UserId { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("zipcode")]
        public string Zipcode { get; set; }

        [JsonProperty("addressname")]
        public string AddressName { get; set; }

        [JsonProperty("routetype")]
        public string RouteType { get; set; }

        public int CityId { get; set; }

        [JsonProperty("routedescription")]
        public string RouteBrief { get; set; }

        [JsonProperty("housenumber")]
        public string HouseNo { get; set; }
        public string City { get; set; }
        public string StateProvinceRegion { get; set; }
        public string Country { get; set; }
        public int IsExported { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string ImportedFrom { get; set; }
        public string UpdatedBy { get; set; }
        public int CustomerId { get; set; }

        [JsonProperty("devicerouteid")]
        public string DeviceRouteId { get; set; }
        public int IsDeleted { get; set; }

        [JsonProperty("idAssignToTSM")]
        public int idAssignToTSM { get; set; }

        private int _cityidFromServer;
        [Ignore]
        [JsonProperty("cityid")]
        public int CityidFromServer
        {
            get { return _cityidFromServer; }
            set
            {
                _cityidFromServer = value;

                CityId = Convert.ToInt32(value);
            }
        }

        private bool _IsDeletedFromServer;
        [Ignore]
        [JsonProperty("isdeleted")]
        public bool IsDeletedFromServer
        {
            get { return _IsDeletedFromServer; }
            set
            {
                _IsDeletedFromServer = value;

                IsDeleted = value ? 1 : 0;
            }
        }
    }
}