using Newtonsoft.Json;

using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class Classification
    {
        [PrimaryKey]
        [JsonProperty("accountclassificationid")]
        public int AccountClassificationId { get; set; }
        [JsonProperty("accountclassificationname")]
        public string AccountClassificationName { get; set; }
        [JsonProperty("customertype")]
        public int CustomerType { get; set; }
    }
}