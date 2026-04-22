using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class LnkRackItems
    {
        [Unique, NotNull]
        public int ID { get; set; }
        public int BrandId { get; set; }
        public int ProductID { get; set; }

        [JsonIgnore]
        public int IsDeleted { get; set; }

        [JsonIgnore]
        public int Status { get; set; }
        public int SortOrder { get; set; }


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

        private bool _StatusFromServer;
        [Ignore]
        [JsonProperty("Status")]
        public bool StatusFromServer
        {
            get { return _StatusFromServer; }
            set
            {
                _StatusFromServer = value;

                Status = value ? 1 : 0;
            }
        }
    }
}