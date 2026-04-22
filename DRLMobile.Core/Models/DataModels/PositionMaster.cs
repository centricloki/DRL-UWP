using Newtonsoft.Json;

using SQLite;

using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class PositionMaster
    {
        [PrimaryKey]
        public int PositionID { get; set; }
        public string Position { get; set; }
        //[JsonIgnore]
        [JsonProperty("createddatetime")]
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        //[JsonIgnore]
        [JsonProperty("updateddatetime")]
        public string UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }

        //private DateTime _UpdatedDateFromServer;
        //[Ignore]
        //[JsonProperty("UpdatedDate")]
        //public DateTime UpdatedDateFromServer
        //{
        //    get { return _UpdatedDateFromServer; }
        //    set
        //    {
        //        _UpdatedDateFromServer = value;

        //        UpdatedDate = Convert.ToString(value);
        //    }
        //}

        //private DateTime _CreatedDateFromServer;
        //[Ignore]
        //[JsonProperty("CreatedDate")]
        //public DateTime CreatedDateFromServer
        //{
        //    get { return _CreatedDateFromServer; }
        //    set
        //    {
        //        _CreatedDateFromServer = value;

        //        CreatedDate = Convert.ToString(value);
        //    }
        //}

    }
}