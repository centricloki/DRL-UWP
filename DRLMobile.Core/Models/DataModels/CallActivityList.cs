using SQLite;
using System;
using Newtonsoft.Json;

namespace DRLMobile.Core.Models.DataModels
{
    public class CallActivityList
    {
        [PrimaryKey]
        [JsonProperty("callactivityid")]
        public int? CallActivityID { get; set; }

        [JsonProperty("customerdeviceid")]
        public string CustomerID { get; set; }

        [JsonProperty("userid")]
        public int UserID { get; set; }

        [JsonProperty("activitytype")]
        public string ActivityType { get; set; }
        public string Objective { get; set; }
        public string Result { get; set; }

        [JsonProperty("comments")]
        public string Comments { get; set; }

        [JsonProperty("createdate")]
        public string CreatedDate { get; set; }

        [JsonProperty("updatedate")]
        public string UpdateDate { get; set; }
        public string CallDate { get; set; }

        [JsonProperty("deviceorderid")]
        public string OrderID { get; set; }
        public string SalesPerson { get; set; }
        public string GratisProduct { get; set; }

        [JsonProperty("devicecallactivityid")]
        public string CallActivityDeviceID { get; set; }

        [JsonProperty("isexported")]
        public int IsExported { get; set; }
        public int isDeleted { get; set; }
        public string CustomerIDERP { get; set; }
        public string Hours { get; set; }
        public int TerritoryID { get; set; }

        [JsonProperty("territoryname")]
        public string TerritoryName { get; set; }

        [JsonProperty("IsVoided")]
        public int IsVoided { get; set; }
        public string WholesaleInvoiceFilePath { get; set; }
        public bool IsDownload { get; set; }

        [JsonProperty("IsVoidedSync")]
        public int IsVoidedSync { get; set; }


        public string CarSaleSold { get; set; }
        public decimal GrandTotal { get; set; }

        private int _customeridFromServer;
        [Ignore]
        [JsonProperty("customerid")]
        public int CustomeridFromServer
        {
            get { return _customeridFromServer; }
            set
            {
                _customeridFromServer = value;

                CustomerID = Convert.ToString(value);
            }
        }

        private int _hoursFromServer;
        [Ignore]
        [JsonProperty("hours")]
        public int HoursFromServer
        {
            get { return _hoursFromServer; }
            set
            {
                _hoursFromServer = value;

                Hours = Convert.ToString(value);
            }
        }

        private string _territoryidFromServer;
        [Ignore]
        [JsonProperty("territoryid")]
        public string TerritoryidFromServer
        {
            get { return _territoryidFromServer; }
            set
            {
                _territoryidFromServer = value;

                TerritoryID = string.IsNullOrEmpty(value) ? 0 : Convert.ToInt32(value);
            }
        }

        private bool _IsDeletedFromServer;
        [Ignore]
        [JsonProperty("isdeleted")]
        public bool IsDeletedFromServer
        {
            get { return _IsDeletedFromServer; }
            set
            {
                _IsDeletedFromServer = value;

                isDeleted = value ? 1 : 0;
            }
        }

        private string _calldateFromServer;
        [Ignore]
        [JsonProperty("calldate")]
        public string CallDateFromServer
        {
            get { return _calldateFromServer; }
            set
            {
                _calldateFromServer = value;

                CallDate = value;
            }
        }
                
        public string ConsumerActivationEngagement { get; set; }
        public string MarketsVisited { get; set; }
        public string CallsMadeVsGoal { get; set; }
        public string NewCustomerAcquisitions { get; set; }
        public string KeyWinsSummary { get; set; }
        public string ChallengesAndFeedback { get; set; }
        public string NextCyclePlan { get; set; }
        public string NextWeekPlan { get; set; }
    }
}