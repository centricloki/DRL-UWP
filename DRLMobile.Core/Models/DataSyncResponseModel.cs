using System.Collections.Generic;

namespace DRLMobile.Core.Models
{
    public class DataSyncResponseModel : BaseModel
    {
        public string responsestatus { get; set; }
        public string errormsg { get; set; }
        public List<string> success { get; set; }
        public List<string> error { get; set; }
    }
}
