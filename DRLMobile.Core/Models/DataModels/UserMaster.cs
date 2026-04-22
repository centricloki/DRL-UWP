using Newtonsoft.Json;

using SQLite;

using System;
using System.Runtime.Serialization;

namespace DRLMobile.Core.Models.DataModels
{
    public class UserMaster
    {
        [JsonProperty("userid")]
        public int UserId { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        [JsonProperty("lastname")]
        public string LastName { get; set; }

        [JsonProperty("emailid")]
        public string EmailID { get; set; }

        [JsonProperty("contactno")]
        public string ContactNo { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        public int PIN { get; set; }

        [JsonProperty("roleid")]
        public int RoleID { get; set; }

        [JsonProperty("territoryid")]
        public string TerritoryID { get; set; }

        [JsonProperty("managerid")]
        public int ManagerID { get; set; }


        public string ImportedFrom { get; set; }

        [JsonProperty("sellerrepresentativetobaccopermitno")]
        public string SellerRepresentativeTobaccoPermitNo { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }

        [JsonProperty("zoneid")]
        public int ZoneId { get; set; }

        [JsonProperty("regionid")]
        public int RegionId { get; set; }

        public int IsExported { get; set; }

        [JsonProperty("defterritoryid")]
        public int defterritoryid { get; set; }

        [JsonProperty("bdid")]
        public int BDID { get; set; }

        [JsonProperty("avpid")]
        public int AVPID { get; set; }

        [JsonProperty("isinactive")]
        public int IsInActive { get; set; }

        [JsonProperty("isdeleted")]
        public int IsDeleted { get; set; }

        private int _pinFromServer;
        [Ignore]
        [JsonProperty("pin")]
        public int pinFromServer
        {
            get { return _pinFromServer; }
            set
            {
                _pinFromServer = value;

                PIN = Convert.ToInt32(value);
            }
        }

        private int _importedfromFromServer;
        [Ignore]
        [JsonProperty("importedfrom")]
        public int importedfromFromServer
        {
            get { return _importedfromFromServer; }
            set
            {
                _importedfromFromServer = value;

                ImportedFrom = Convert.ToString(value);
            }
        }

        private int _CreatedByFromServer;
        [Ignore]
        [JsonProperty("createdby")]
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
        [JsonProperty("updatedby")]
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