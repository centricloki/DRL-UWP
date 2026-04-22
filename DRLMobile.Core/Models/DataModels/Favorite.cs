using DRLMobile.Core.Models.UIModels;
using SQLite;
using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class Favorite
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int StyleId { get; set; }
        public string StyleName { get; set; }
        [PrimaryKey]
        [JsonProperty("favoriteid")]
        public string FavoriteID { get; set; }

        [JsonProperty("productid")]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        [JsonIgnore]
        public string DistributionRecord { get; set; }
        public int Quantity { get; set; }

        [JsonIgnore]
        public int Price { get; set; }
        public int isTobbaco { get; set; }

        [JsonIgnore]
        public string CreatedDate { get; set; }

        [JsonIgnore]
        public string UpdatedDate { get; set; }

        [JsonProperty("userid")]
        public int UserId { get; set; }
        public string LangId { get; set; }
        public int ProductType { get; set; }

        [JsonProperty("isexported")]
        public int IsExported { get; set; }

        [JsonProperty("isdeleted")]
        public bool isDeleted { get; set; }

        private bool _IsTobbacoFromServer;
        [Ignore]
        [JsonProperty("IsTobbaco")]
        public bool IsTobbacoFromServer
        {
            get { return _IsTobbacoFromServer; }
            set
            {
                _IsTobbacoFromServer = value;

                isTobbaco = value ? 1 : 0;
            }
        }

        private float _priceFromServer;
        [Ignore]
        [JsonProperty("Price")]
        public float PriceFromServer
        {
            get { return _priceFromServer; }
            set
            {
                _priceFromServer = value;

                Price = Convert.ToInt32(value);
            }
        }

        private DateTime _distributionRecordFromServer;
        [Ignore]
        [JsonProperty("DistributionRecord")]
        public DateTime DistributionRecordFromServer
        {
            get { return _distributionRecordFromServer; }
            set
            {
                _distributionRecordFromServer = value;

                DistributionRecord = Convert.ToString(value);
            }
        }

        private DateTime _createddatetimeFromServer;
        [Ignore]
        [JsonProperty("CreatedDate")]
        public DateTime CreatedDateFromServer
        {
            get { return _createddatetimeFromServer; }
            set
            {
                _createddatetimeFromServer = value;

                CreatedDate = Convert.ToString(value);
            }
        }

        private DateTime _UpdatedDateFromServer;
        [Ignore]
        [JsonProperty("UpdatedDate")]
        public DateTime UpdatedDateFromServer
        {
            get { return _UpdatedDateFromServer; }
            set
            {
                _UpdatedDateFromServer = value;

                UpdatedDate = Convert.ToString(value);
            }
        }

        public FavoriteUiModel FavoriteDataToUiModel()
        {
            var uiModel = new FavoriteUiModel()
            {
                ItemNumber = this.ProductName,
                ItemDescription = this.ProductDescription,
                CategoryId = this.CategoryId,
                CategoryName = this.CategoryName,
                BrandId = this.BrandId,
                BrandName = this.BrandName,
                StyleId = this.StyleId,
                StyleName = this.StyleName,
                ProductId = this.ProductId,
                isTobbaco = this.isTobbaco,
                Quantity = this.Quantity,
                Price = this.Price,
                FavoriteData = new Favorite { FavoriteID = this.FavoriteID },
            };
            return uiModel;
        }
    }
}