using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadContactDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddEditContactModel> contactdata { get; set; }
    }
}