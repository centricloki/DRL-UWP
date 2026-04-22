using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.UIModels
{
    public class TerritoryMasterUIModel: BaseModel
    {
        private int _territoryID;
        public int TerritoryID
        {
            get { return _territoryID; }
            set { SetProperty(ref _territoryID , value); }
        }


        private string _territoryName;
        public string TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }

        private int _regionID;
        public int RegionID
        {
            get { return _regionID; }
            set { SetProperty(ref _regionID, value); }
        }

    }
}
