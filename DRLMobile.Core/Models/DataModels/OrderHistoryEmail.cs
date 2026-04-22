using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class OrderHistoryEmail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DeviceOrderID { get; set; }
        public string EmailID { get; set; }
        public string MemoField { get; set; }

        [JsonIgnore]
        public int IsEmailSent { get; set; }
        public string UpdateDate { get; set; }
        public string DeviceOrderEmailID { get; set; }
        public int IsExported { get; set; }
        public int UserID { get; set; }

        private bool _IsEmailSentFromServer;
        [Ignore]
        [JsonProperty("IsEmailSent")]
        public bool IsEmailSentFromServer
        {
            get { return _IsEmailSentFromServer; }
            set 
            { 
                _IsEmailSentFromServer = value;

                IsEmailSent = value ? 1 : 0;
            }
        }
    }
}