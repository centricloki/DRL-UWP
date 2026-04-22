using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class RegionMasterUIModel : BaseModel
    {
        private int _regionID;
        public int RegionID
        {
            get { return _regionID; }
            set { SetProperty(ref _regionID , value); }
        }

        private string _regioname;
        public string Regioname
        {
            get { return _regioname; }
            set { SetProperty(ref _regioname, value); }
        }


        private int _zoneID;
        public int ZoneID
        {
            get { return _zoneID; }
            set { SetProperty(ref _zoneID, value); }
        }




    }
}
