using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadProductDistributionDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddProductDistributionModel> customerproduct { get; set; }
    }
}
