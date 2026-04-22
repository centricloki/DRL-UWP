using DRLMobile.Core.Models.UIModels;
using SQLite;
using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ProductMaster : BaseModel
    {
        [JsonProperty("productid")]
        public int ProductID { get; set; }

        [JsonProperty("productname")]
        public string ProductName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        public string ImportedFrom { get; set; }

        public int Price { get; set; }

        public string DistributionRecordedDate { get; set; }

        [JsonProperty("documenttype")]
        public int ProductType { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("langid")]
        public int LangID { get; set; }

        public int Quantity { get; set; }
        public int isDistributed { get; set; }
        public int isTobbaco { get; set; }

        [JsonProperty("catid")]
        public int CatId { get; set; }

        [JsonProperty("brandid")]
        public int BrandId { get; set; }

        [JsonProperty("styleid")]
        public int StyleId { get; set; }

        [JsonProperty("uom")]
        public string UOM { get; set; }
        public int IsDeleted { get; set; }

        [JsonIgnore]
        public int SRCHoneySellable { get; set; }

        [JsonIgnore]
        public int SRCHoneyReturnable { get; set; }

        [JsonIgnore]
        public int SRCCanIOrder { get; set; }

        [Ignore]
        public string categoryName { get; set; }

        [Ignore]
        public bool FavoriteData { get; set; }

        [Ignore]
        public bool CartData { get; set; }
        
        [Ignore]
        public int CartQuantity { get; set; }


        private int _importedfromFromServer;
        [Ignore]
        [JsonProperty("importedfrom")]
        public int ImportedfromFromServer
        {
            get { return _importedfromFromServer; }
            set
            {
                _importedfromFromServer = value;

                ImportedFrom = Convert.ToString(value);
            }
        }

        private bool _istobaccoproductFromServer;
        [Ignore]
        [JsonProperty("istobaccoproduct")]
        public bool IsTobaccoProductFromServer
        {
            get { return _istobaccoproductFromServer; }
            set
            {
                _istobaccoproductFromServer = value;

                isTobbaco = value ? 1 : 0;
            }
        }

        private bool _SRCHoneySellableFromServer;
        [Ignore]
        [JsonProperty("SRCHoneySellable")]
        public bool SRCHoneySellableFromServer
        {
            get { return _SRCHoneySellableFromServer; }
            set
            {
                _SRCHoneySellableFromServer = value;

                SRCHoneySellable = value ? 1 : 0;
            }
        }

        private bool _SRCHoneyReturnableFromServer;
        [Ignore]
        [JsonProperty("SRCHoneyReturnable")]
        public bool SRCHoneyReturnableFromServer
        {
            get { return _SRCHoneyReturnableFromServer; }
            set
            {
                _SRCHoneyReturnableFromServer = value;

                SRCHoneyReturnable = value ? 1 : 0;
            }
        }

        private bool _SRCCanIOrderFromServer;
        [Ignore]
        [JsonProperty("SRCCanIOrder")]
        public bool SRCCanIOrderFromServer
        {
            get { return _SRCCanIOrderFromServer; }
            set
            {
                _SRCCanIOrderFromServer = value;

                SRCCanIOrder = value ? 1 : 0;
            }
        }

        private decimal _priceFromServer;
        [Ignore]
        [JsonProperty("price")]
        public decimal PriceFromServer
        {
            get { return _priceFromServer; }
            set
            {
                _priceFromServer = value;

                Price = Convert.ToInt32(value);
            }
        }

        [Ignore]
        [JsonIgnore]
        public string SearchDisplayPath
        {
            get { return ProductName + Description; }
        }

        [JsonProperty("customerid")]
        public long? CustomerID { get; set; }

        public ProductMasterUIModel CopyToMasterUIModel()
        {
            return new ProductMasterUIModel()
            {
                ProductID = this.ProductID,
                ProductName = this.ProductName,
                Description = this.Description
            };
        }

        public SRCProductUIModel CopyToUIModel()
        {
            var uiModel = new SRCProductUIModel()
            {
                ItemNumber = this.ProductName,
                ItemDescription = this.Description,
                IsDistributed = this.isDistributed,
                DistributionRecordedDate = Helpers.DateTimeHelper.ConvertEmptyStringDateToMM_DD_YYYY(this.DistributionRecordedDate),
                CatId = this.CatId,
                BrandId = this.BrandId,
                StyleId = this.StyleId,
                Link = this.Description,
                ProductType = this.ProductType,
                UOM = this.UOM,
                IsDeleted = this.IsDeleted,
                SRCHoneySellable = this.SRCHoneySellable,
                SRCHoneyReturnable = this.SRCHoneyReturnable,
                SRCCanIOrder = this.SRCCanIOrder,
                ProductID = this.ProductID,
                isTobbaco = this.isTobbaco,
                IsFavorite = this.FavoriteData,
                IsAddedToCart = this.CartData,
                Quantity = this.CartQuantity
            };
            return uiModel;
        }
    }
}