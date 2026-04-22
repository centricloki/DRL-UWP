namespace DRLMobile.Core.Models.UIModels
{
    public class CityMasterUIModel : BaseModel
    {
        private int _cityID;
        public int CityID
        {
            get { return _cityID; }
            set {SetProperty(ref _cityID , value); }
        }

        private string _cityName;
        public string CityName
        {
            get { return _cityName; }
            set { SetProperty(ref _cityName, value); }
        }

        private int _stateID;
        public int StateID
        {
            get { return _stateID; }
            set { SetProperty(ref _stateID, value); }
        }

        
    }
}
