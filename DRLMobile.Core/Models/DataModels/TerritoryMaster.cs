using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;
using SQLite;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class TerritoryMaster
    {
        [PrimaryKey]
        [JsonProperty("territoryid")]
        public int TerritoryID { get; set; }

        [JsonProperty("territoryname")]
        public string TerritoryName { get; set; }

        [JsonProperty("regionid")]
        public int RegionID { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("bdid")]
        public Nullable<int> BDID { get; set; }

        [JsonProperty("isactive")]
        public bool IsActive { get; set; }

        [JsonProperty("isdeleted")]
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public string TerritoryNumber { get; set; }


        public TerritoryMasterUIModel CopyToUIModel()
        {
            return new TerritoryMasterUIModel()
            {
                TerritoryName = this.TerritoryName,
                TerritoryID = this.TerritoryID,
                RegionID = this.RegionID
            };
        }

    }
}