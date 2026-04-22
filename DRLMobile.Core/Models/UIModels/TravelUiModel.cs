using System;

namespace DRLMobile.Core.Models.UIModels
{
    public class TravelUiModel : BaseModel
    {
        private int? _travelID;
        public int? TravelID
        {
            get { return _travelID; }
            set { SetProperty(ref _travelID, value); }
        }

        private string _customerNumber;
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { SetProperty(ref _customerNumber, value); }
        }

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        private string _customerId;
        public string CustomerId
        {
            get { return _customerId; }
            set { SetProperty(ref _customerId, value); }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private string _netPoints = "0";
        public string NetPoints
        {
            get { return _netPoints; }
            set
            {
                _netPoints = value;
                NetPointsToShow = ConvertToCommaSeparatedValue(value);
                NetPointsToShowInt = ConvertToDisplayIntegerValue(value);
            }
        }
                
        private string _year;
        public string Year
        {
            get { return _year; }
            set
            {
                _year = value;
                YearToShow = Convert.ToInt32(value);
            }
        }

        private int _yearToShow;
        public int YearToShow
        {
            get { return _yearToShow; }
            set { SetProperty(ref _yearToShow, value); }
        }

        private string _awards = "-";
        public string Awards
        {
            get { return _awards; }
            set { SetProperty(ref _awards, value); }
        }

        private string _neededPoint = "0";
        public string NeededPoint
        {
            get { return _neededPoint; }
            set
            {
                _neededPoint = value;
                NeededPointToShow = ConvertToCommaSeparatedValue(value);
                NeededPointToShowInt = ConvertToDisplayIntegerValue(value);
            }
        }

        private string _next;
        public string Next
        {
            get { return _next; }
            set { SetProperty(ref _next, value); }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set { SetProperty(ref _city, value); }
        }

        private string _state;
        public string State
        {
            get { return _state; }
            set { SetProperty(ref _state, value); }
        }

        public string SearchDisplayPath
        {
            get { return CustomerName + " " + CustomerNumber + " " + YearToShow + " " + NetPointsToShow + " " + Awards + " " + NeededPointToShow + " " + Next + " " + City + " " + State; }
        }

        private string _earnedPoints = "0";
        public string EarnedPoints
        {
            get { return _earnedPoints; }
            set 
            {
                _earnedPoints = value;
                EarnedPointsToShow = ConvertToCommaSeparatedValue(value);
            }
        }

        private string _bonusPoints = "0";
        public string BonusPoints
        {
            get { return _bonusPoints; }
            set 
            {
                _bonusPoints = value;
                BonusPointsToShow = ConvertToCommaSeparatedValue(value);
            }
        }

        private string _netPointsToShow = "0";
        public string NetPointsToShow
        {
            get { return _netPointsToShow; }
            set { SetProperty(ref _netPointsToShow, value); }
        }

        private string _bonusPointsToShow = "0";
        public string BonusPointsToShow
        {
            get { return _bonusPointsToShow; }
            set { SetProperty(ref _bonusPointsToShow, value); }
        }

        private string _earnedPointsToShow = "0";
        public string EarnedPointsToShow
        {
            get { return _earnedPointsToShow; }
            set { SetProperty(ref _earnedPointsToShow, value); }
        }

        private string _neededPointToShow = "0";
        public string NeededPointToShow
        {
            get { return _neededPointToShow; }
            set { SetProperty(ref _neededPointToShow, value); }
        }

        private int _neededPointToShowInt;
        public int NeededPointToShowInt
        {
            get { return _neededPointToShowInt; }
            set { SetProperty(ref _neededPointToShowInt, value); }
        }

        private int _netPointsToShowInt;
        public int NetPointsToShowInt
        {
            get { return _netPointsToShowInt; }
            set { SetProperty(ref _netPointsToShowInt, value); }
        }

        private string ConvertToCommaSeparatedValue(string value)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                int number;
                int.TryParse(value, out number);
                result = string.Format("{0:n0}", number);
            }
            return result;
        }

        private int ConvertToDisplayIntegerValue(string value)
        {
            int result = 0;
            if (!string.IsNullOrEmpty(value))
            {
                int number;
                int.TryParse(value, out number);
                result = number;
            }
            return result;
        }
    }
}
