using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class ChangePinRequestModel
    {
        public int pin { get; set; }
        public int userid { get; set; }
        public string username { get; set; }
        public int newpin { get; set; }
    }
}
