using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadOrdersDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddOrderModel> orderdata { get; set; }
    }
}
