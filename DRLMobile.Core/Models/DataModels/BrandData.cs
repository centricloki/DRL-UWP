using DRLMobile.Core.Models.UIModels;
using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class BrandData
    {
        [PrimaryKey, AutoIncrement]
        [JsonProperty("brandid")]
        public int BrandId { get; set; }

        [JsonProperty("categoryid")]
        public int CatId { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }

        [JsonProperty("imagefilename")]
        public string ImageFileName { get; set; }

        [JsonProperty("brandname")]
        public string BrandName { get; set; }

        [JsonProperty("langid")]
        public int LanguageId { get; set; }

        [JsonProperty("priority")]
        public int Priority { get; set; }

        [JsonProperty("promoid")]
        public int PromoId { get; set; }

        [JsonProperty("localfilepath")]
        public string LocalFilePath { get; set; }

        [JsonProperty("isdownload")]
        public int IsDownload { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public int IsDeleted { get; set; }
        public int IsPopOrder { get; set; }

        [JsonProperty("sortorder")]
        public int SortOrder { get; set; }

        private bool _IsDeletedFromServer;
        [Ignore]
        [JsonProperty("isdeleted")]
        public bool IsDeletedFromServer
        {
            get { return _IsDeletedFromServer; }
            set
            {
                _IsDeletedFromServer = value;

                IsDeleted = value ? 1 : 0;
            }
        }

        private bool? _IsPopOrderFromServer;
        [Ignore]
        [JsonProperty("ispoporder")]
        public bool? IsPopOrderFromServer
        {
            get { return _IsPopOrderFromServer; }
            set
            {
                _IsPopOrderFromServer = value;

                if (value != null)
                {
                    IsPopOrder = (bool)value ? 1 : 0;
                }
                else
                {
                    IsPopOrder = 0;
                }
            }
        }

        public BrandUIModel CopyToUIModel()
        {
            var uiModel = new BrandUIModel()
            {
                BrandId = this.BrandId,
                BrandName = this.BrandName,
                BrandImage = this.ImageFileName,
                CatId = this.CatId
            };
            return uiModel;
        }
    }
}