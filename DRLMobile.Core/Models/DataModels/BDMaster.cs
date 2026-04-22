using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class BDMaster
    {
        [JsonProperty("bdid")]
        public int BDID { get; set; }

        [JsonProperty("bdname")]
        public string BDName { get; set; }

        [JsonProperty("updatedate")]
        public System.DateTime UpdateDate { get; set; }

        [JsonProperty("isactive")]
        public bool IsActive { get; set; }

        [JsonProperty("isdeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("approver")]
        public Nullable<int> Approver { get; set; }
    }
}
