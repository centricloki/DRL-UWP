using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadCallActivityDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddCallActivityModel> callactivitydata { get; set; }
    }
}