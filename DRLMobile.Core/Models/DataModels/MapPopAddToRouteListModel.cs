using System.Linq;

namespace DRLMobile.Core.Models.DataModels
{
    public class MapPopAddToRouteListModel : BaseModel
    {
        private string _routeName;
        public string RouteName
        {
            get { return _routeName; }
            set { SetProperty(ref _routeName, value); }
        }

        private string _streetName;
        public string StreetName
        {
            get { return _streetName; }
            set { SetProperty(ref _streetName, value); }
        }

        private int _routeId;
        public int RouteId
        {
            get { return _routeId; }
            set { SetProperty(ref _routeId, value); }
        }

        private string _startDate;
        public string StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); PopulateDisplayStartDate(); }
        }



        private string _endDate;
        public string EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); PopulateDisplayEndDate(); }
        }


        private string _displayStartDate;
        public string DisplayStartDate
        {
            get { return _displayStartDate; }
            set { SetProperty(ref _displayStartDate, value); }
        }

        private string _displayEndDate;
        public string DisplayEndDate
        {
            get { return _displayEndDate; }
            set { SetProperty(ref _displayEndDate, value); }
        }

        public string UpdatedDate { get; set; }
        public int UserId { get; set; }

        public string Longitude { get; set; }

        public string Latitude { get; set; }

        public string Zipcode { get; set; }

        public string AddressName { get; set; }

        public string RouteType { get; set; }

        public int CityId { get; set; }

        public string RouteBrief { get; set; }

        public string HouseNo { get; set; }

        public string City { get; set; }

        public string StateProvinceRegion { get; set; }

        public string Country { get; set; }

        public int IsExported { get; set; }

        public string CreatedDate { get; set; }

        public string CreatedBy { get; set; }

        public string ImportedFrom { get; set; }

        public string UpdatedBy { get; set; }

        public int CustomerId { get; set; }

        public string DeviceRouteId { get; set; }

        public int IsDeleted { get; set; }

        public int idAssignToTSM { get; set; }

        public string username { get; set; }
        public string TerritoryName { get; set; }
        public string CreatorName { get; set; }

        private void PopulateDisplayStartDate()
        {
            DisplayStartDate = StartDate?.Split(' ')?.FirstOrDefault();
        }

        private void PopulateDisplayEndDate()
        {
            DisplayEndDate = EndDate?.Split(' ')?.FirstOrDefault();
        }


        private string _buttonText;
        public string ButtonText
        {
            get { return _buttonText; }
            set { SetProperty(ref _buttonText, value); }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected , value);  SetButtonText(); }
        }

        private void SetButtonText()
        {
            ButtonText = IsSelected ? "Remove" : "Add";
        }


        public MapPopAddToRouteListModel()
        {
            IsSelected = false;
        }
    }
}
