using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class ZoneMasterUIModel : BaseModel
    {
        private int _zoneId;
        public int ZoneID
        {
            get { return _zoneId; }
            set { SetProperty(ref _zoneId, value); }
        }

        private string _zoneName;
        public string ZoneName
        {
            get { return _zoneName; }
            set { SetProperty(ref _zoneName, value); }
        }


    }
}
