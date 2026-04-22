using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class AVPMaster
    {
        [JsonProperty("avpid")]
        [PrimaryKey, AutoIncrement]
        public int AVPID { get; set; }

        [JsonProperty("avpname")]
        public string AVPName { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("isdeleted")]
        public bool IsDeleted { get; set; }

        [JsonProperty("isactive")]
        public bool IsActive { get; set; }
    }
}