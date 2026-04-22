using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;

namespace DRLMobile.Core.Models.DataModels
{
    public class StyleData
    {
        [JsonProperty("styleid")]
        public int StyleId { get; set; }

        [JsonProperty("categoryid")]
        public int CatId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("stylename")]
        public string StyleName { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("langid")]
        public int LanguageId { get; set; }

        [JsonProperty("promoid")]
        public int PromoId { get; set; }


        public StyleUIModel CopyToUIModel()
        {
            var uiModel = new StyleUIModel()
            {
                StyleId = this.StyleId,
                StyleName = this.StyleName,
                CatId = this.CatId
            };
            return uiModel;

        }
    }
}