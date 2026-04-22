using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;

namespace DRLMobile.Core.Models.DataModels
{
    public class RegionMaster
    {
        [JsonProperty("regionid")]
        public int RegionID { get; set; }

        [JsonProperty("regionname")]
        public string Regioname { get; set; }

        [JsonProperty("zoneid")]
        public int ZoneID { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        public RegionMasterUIModel CopyToUIModel()
        {
            return new RegionMasterUIModel()
            {
                Regioname = this.Regioname,
                RegionID = this.RegionID,
                ZoneID = this.ZoneID
            };
        }

    }
}