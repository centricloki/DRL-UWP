using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class Configuration
    {
        [PrimaryKey, AutoIncrement]
        public int ConfigID { get; set; }
        public string KEYName { get; set; }
        public string KEYValue { get; set; }
    }
}