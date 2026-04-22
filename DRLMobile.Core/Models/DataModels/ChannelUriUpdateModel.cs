using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class ChannelUriUpdateModel
    {
        public string applicationversion { get; set; }
        public string devicetoken { get; set; }
        public string deviceuniqueid { get; set; }
        public string iosversion { get; set; }
        public string updatetype { get; set; }
        public string userid { get; set; }
    }
}
