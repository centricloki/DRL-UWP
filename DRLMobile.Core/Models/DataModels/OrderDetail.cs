using DRLMobile.Core.Models.UIModels;
using SQLite;
using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class OrderDetail
    {
        [PrimaryKey]
        [JsonProperty("orderdetailid")]
        public int? OrderDetailId { get; set; }

        [JsonProperty("orderid")]
        public int? OrderId { get; set; }

        [JsonProperty("categoryid")]
        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        [JsonProperty("brandstyleid")]
        public int BrandId { get; set; }
        public string BrandName { get; set; }

        [JsonProperty("styleid")]
        public int StyleId { get; set; }
        public string StyleName { get; set; }

        [JsonProperty("productid")]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int isTobbaco { get; set; }
        public string ProductDescription { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }

        [JsonProperty("units")]
        public string Unit { get; set; }
        public string Total { get; set; }

        [JsonProperty("deviceorderid")]
        public string DeviceOrderID { get; set; }
        public string CreditRequest { get; set; }

        private decimal _priceFromServer;
        [Ignore]
        [JsonProperty("price")]
        public decimal PriceFromServer
        {
            get { return _priceFromServer; }
            set
            {
                _priceFromServer = value;

                Price = Convert.ToString(value);
            }
        }
    }
}