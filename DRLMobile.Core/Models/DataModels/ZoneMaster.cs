using DRLMobile.Core.Models.UIModels;

using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ZoneMaster
    {
        [JsonProperty("zoneid")]
        public int ZoneID { get; set; }

        [JsonProperty("zonename")]
        public string ZoneName { get; set; }

        // [JsonProperty("updateddate")]
        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("avpid")]
        public Nullable<int> AVPID { get; set; }

        [JsonProperty("isactive")]
        public bool IsActive { get; set; }

        [JsonProperty("isdeleted")]
        public bool IsDeleted { get; set; }


        public ZoneMasterUIModel CopyToUIModel()
        {
            return new ZoneMasterUIModel()
            {   
                ZoneID = this.ZoneID,
                ZoneName = this.ZoneName
            };
        }


    }
}