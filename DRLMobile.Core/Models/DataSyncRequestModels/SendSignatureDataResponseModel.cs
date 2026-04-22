using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class SendSignatureDataResponseModel
    {
        public string deviceorderid { get; set; }
        public string filename { get; set; }
        public string filestream { get; set; }
        public int filetype { get; set; }
        public string username { get; set; }
        public int pin { get; set; }
        public string updatedate { get; set; }
    }
}
