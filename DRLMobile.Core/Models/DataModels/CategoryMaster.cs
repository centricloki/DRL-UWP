using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class CategoryMaster
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty("categoryid")]
        public int CategoryID { get; set; }

        [JsonProperty("categoryname")]
        public string CategoryName { get; set; }

        [JsonProperty("parentcategoryid")]
        public int ParentCategoryID { get; set; }

        [JsonProperty("status")]
        public int CategoryStatus { get; set; }

        [JsonProperty("imagefilename")]
        public string ImageFileName { get; set; }

        [JsonProperty("promoid")]
        public int PromoID { get; set; }

        [JsonProperty("langid")]
        public int LangID { get; set; }

        [JsonProperty("prioriry")]
        public int Priority { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("erpcategoryid")]
        public int ERPCategoryId { get; set; }

        [JsonProperty("localfilepath")]
        public string LocalFilePath { get; set; }

        [JsonProperty("isdownload")]
        public int IsDownload { get; set; }

        public CategoryUIModel CopyToUIModel()
        {
            var uiModel = new CategoryUIModel()
            {
                CategoryId = this.CategoryID,
                CategoryName = this.CategoryName,
                IsSelected = false,
                CategoryImage = "ms-appx:///Assets/SRCProduct/category_unselected.png",
                ERPCategoryId = this.ERPCategoryId
            };
            return uiModel;
        }
    }
}