using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class VripTravelData
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty("programid")]
        public int ProgramID { get; set; }

        [JsonProperty("programname")]
        public string ProgramName { get; set; }

        [JsonProperty("programyear")]
        public string ProgramYear { get; set; }

        [JsonProperty("programtype")]
        public string ProgramType { get; set; }
    }
}