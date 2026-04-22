using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;

namespace DRLMobile.Core.Models.DataModels
{
    public class CityMaster
    {
        [JsonProperty("cityid")]
        public int CityID { get; set; }

        [JsonProperty("cityname")]
        public string CityName { get; set; }

        [JsonProperty("stateid")]
        public int StateID { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }


        public CityMasterUIModel CopyToUIModel()
        {
            return new CityMasterUIModel()
            {
                CityID= this.CityID,
                StateID=this.StateID,
                CityName= this.CityName
            };
        }
    }
}