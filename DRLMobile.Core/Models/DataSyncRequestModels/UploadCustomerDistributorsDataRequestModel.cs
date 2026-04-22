using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadCustomerDistributorsDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddCustomerDistributorModel> customerdistributordata { get; set; }
    }
}
