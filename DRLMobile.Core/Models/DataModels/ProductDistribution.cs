using SQLite;
using Newtonsoft.Json;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ProductDistribution
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string CustomerProductID { get; set; }

        [JsonProperty("productid")]
        public int ProductId { get; set; }

        [JsonProperty("lastdistributionrecordDate")]
        public string DistributionDate { get; set; }

        [JsonProperty("customerid")]
        public int CustomerId { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }
      
        [JsonProperty("isdeleted")]
        public int IsDeleted { get; set; }
        public int isExported { get; set; }


        private int _customerproductidFromServer;
        [Ignore]
        [JsonProperty("customerproductid")]
        public int CustomerproductidFromServer
        {
            get { return _customerproductidFromServer; }
            set
            {
                _customerproductidFromServer = value;

                CustomerProductID = Convert.ToString(value);
            }
        }
    }
}