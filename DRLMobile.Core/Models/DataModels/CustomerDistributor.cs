using Newtonsoft.Json;
using SQLite;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class CustomerDistributor
    {
        [PrimaryKey]
        public int? CustomerDistributorID { get; set; }
        public string DeviceCustomerID { get; set; }

        [JsonIgnore]
        public string DistributorID { get; set; }
        public int IsExported { get; set; }

        [JsonIgnore]
        public int IsDeleted { get; set; }
        public string CreatedDate { get; set; }

        [JsonIgnore]
        public string CreatedBy { get; set; }
        public string UpdatedDate { get; set; }

        [JsonIgnore]
        public string UpdatedBy { get; set; }
        public int DistributorPriority { get; set; }


        private int _DistributorIDFromServer;
        [Ignore]
        [JsonProperty("DistributorID")]
        public int DistributorIDFromServer
        {
            get { return _DistributorIDFromServer; }
            set
            {
                _DistributorIDFromServer = value;

                DistributorID = Convert.ToString(value);
            }
        }

        private bool _IsDeletedFromServer;
        [Ignore]
        [JsonProperty("IsDeleted")]
        public bool IsDeletedFromServer
        {
            get { return _IsDeletedFromServer; }
            set
            {
                _IsDeletedFromServer = value;

                IsDeleted = value ? 1 : 0;
            }
        }

        private int _CreatedByFromServer;
        [Ignore]
        [JsonProperty("CreatedBy")]
        public int CreatedByFromServer
        {
            get { return _CreatedByFromServer; }
            set
            {
                _CreatedByFromServer = value;

                CreatedBy = Convert.ToString(value);
            }
        }

        private int _UpdatedByFromServer;
        [Ignore]
        [JsonProperty("UpdatedBy")]
        public int UpdatedByFromServer
        {
            get { return _UpdatedByFromServer; }
            set
            {
                _UpdatedByFromServer = value;

                UpdatedBy = Convert.ToString(value);
            }
        }
    }
}