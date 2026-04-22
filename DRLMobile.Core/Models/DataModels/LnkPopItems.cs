using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class LnkPopItems
    {
        [Unique,NotNull]
        public int ID { get; set; }
        public int BrandId { get; set; }
        public int ProductID { get; set; }

        [JsonIgnore]
        public int IsDeleted { get; set; }
        public int Hierarchy1 { get; set; }
        public int Hierarchy2 { get; set; }
        public int Hierarchy3 { get; set; }
        public int Hierarchy4 { get; set; }
        public int Hierarchy5 { get; set; }

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
    }
}