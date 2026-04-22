using Newtonsoft.Json;

namespace DRLMobile.Core.Models.DataModels
{
    public class StateMaster
    {
        [JsonProperty("stateid")]
        public int StateID { get; set; }

        [JsonProperty("statename")]
        public string StateName { get; set; }

        [JsonProperty("customstatement")]
        public string CustomStatement { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }
    }
}