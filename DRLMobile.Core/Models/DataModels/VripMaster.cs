using Newtonsoft.Json;

using SQLite;

using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class VripMaster
    {
        [PrimaryKey]
        [JsonProperty("vripid")]
        public int VripID { get; set; }

        [JsonProperty("vripname")]
        public string VripName { get; set; }
        public string CustomerID { get; set; }
        public string Cslyr { get; set; }
        public string Target { get; set; }
        public string Csytd { get; set; }

        [JsonProperty("qualified")]
        public string Qualified { get; set; }
        public string Rebate { get; set; }
        public string CSNeeded { get; set; }

        [JsonProperty("year")]
        public string Year { get; set; }

        public string CreatedDate { get; set; }

        [JsonProperty("updateddatetime")]
        public string UpdatedDate { get; set; }


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

        private int _cslyrFromServer;
        [Ignore]
        [JsonProperty("cslyr")]
        public int CslyrFromServer
        {
            get { return _cslyrFromServer; }
            set
            {
                _cslyrFromServer = value;

                Cslyr = Convert.ToString(value);
            }
        }

        private int _csNeededFromServer;
        [Ignore]
        [JsonProperty("csneeded")]
        public int CSNeededFromServer
        {
            get { return _csNeededFromServer; }
            set
            {
                _csNeededFromServer = value;

                CSNeeded = Convert.ToString(value);
            }
        }

        private int _targetFromServer;
        [Ignore]
        [JsonProperty("target")]
        public int TargetFromServer
        {
            get { return _targetFromServer; }
            set
            {
                _targetFromServer = value;

                Target = Convert.ToString(value);
            }
        }

        private float _csytdFromServer;
        [Ignore]
        [JsonProperty("csytd")]
        public float CsytdFromServer
        {
            get { return _csytdFromServer; }
            set
            {
                _csytdFromServer = value;

                Target = Convert.ToString(value);
            }
        }

        //private DateTime _createddatetimeFromServer;
        //[Ignore]
        //[JsonProperty("createddatetime")]
        //public DateTime CreatedDateFromServer
        //{
        //    get { return _createddatetimeFromServer; }
        //    set
        //    {
        //        _createddatetimeFromServer = value;

        //        CreatedDate = Convert.ToString(value);
        //    }
        //}
    }
}