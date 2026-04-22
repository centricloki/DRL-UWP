using Newtonsoft.Json;

using SQLite;

using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class RankMaster
    {
        [PrimaryKey]
        public int RankID { get; set; }
        public string Rank { get; set; }
        [JsonProperty("CreatedDateTime")]
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        [JsonProperty("UpdatedDateTime")]
        public string UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
    }
}