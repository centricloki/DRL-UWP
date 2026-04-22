using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadCustomerDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddEditCustomerModel> customerdata { get; set; }
    }
}
