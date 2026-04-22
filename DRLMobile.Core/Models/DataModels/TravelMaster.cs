using Newtonsoft.Json;
using SQLite;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class TravelMaster
    {
        [PrimaryKey]
        [JsonProperty("travelid")]
        public int TravelID { get; set; }

        [JsonProperty("travelername")]
        public string Name { get; set; }
        public string CustomerID { get; set; }
        public string NeededPoint { get; set; }
        public string EarnedPoints { get; set; }
        public string BonusPoints { get; set; }
        public string NetPoints { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        [JsonProperty("awards")]
        public string Awards { get; set; }

        [JsonProperty("next")]
        public string Next { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

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

        private int _netpointsFromServer;
        [Ignore]
        [JsonProperty("netpoints")]
        public int NetpointsFromServer
        {
            get { return _netpointsFromServer; }
            set
            {
                _netpointsFromServer = value;

                NetPoints = Convert.ToString(value);
            }
        }

        private int _bonuspointsFromServer;
        [Ignore]
        [JsonProperty("bonuspoints")]
        public int BonuspointsFromServer
        {
            get { return _bonuspointsFromServer; }
            set
            {
                _bonuspointsFromServer = value;

                BonusPoints = Convert.ToString(value);
            }
        }

        private int _earnedpointsFromServer;
        [Ignore]
        [JsonProperty("earnedpoints")]
        public int EarnedpointsFromServer
        {
            get { return _earnedpointsFromServer; }
            set
            {
                _earnedpointsFromServer = value;

                EarnedPoints = Convert.ToString(value);
            }
        }

        private int _neededpointFromServer;
        [Ignore]
        [JsonProperty("neededpoint")]
        public int NeededpointFromServer
        {
            get { return _neededpointFromServer; }
            set
            {
                _neededpointFromServer = value;

                NeededPoint = Convert.ToString(value);
            }
        }
    }
}