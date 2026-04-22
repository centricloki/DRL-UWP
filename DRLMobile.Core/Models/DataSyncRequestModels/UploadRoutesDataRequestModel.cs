using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadRoutesDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public List<AddRoutesModel> routedata { get; set; }
    }
}
