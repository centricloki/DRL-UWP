using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class CustomerPageUIModel : BaseModel
    {
        private readonly Object thisLock = new Object();

        private string _customerName;
        public string CustomerName
        {
            get { return _customerName; }
            set { SetProperty(ref _customerName, value); }
        }

        private string _customerNumber;
        public string CustomerNumber
        {
            get { return _customerNumber; }
            set { SetProperty(ref _customerNumber, value); }
        }

        private string _storeType;
        public string StoreType
        {
            get { return _storeType; }
            set { SetProperty(ref _storeType, value); }
        }


        private string _rank;
        public string Rank
        {
            get { return _rank; }
            set { SetProperty(ref _rank, value); }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set { SetProperty(ref _address, value); }
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

        private string _zipcode;
        public string ZipCode
        {
            get { return _zipcode; }
            set { SetProperty(ref _zipcode, value); }
        }


        private string _lastCallDate;
        public string LastCallDate
        {
            get { return _lastCallDate; }
            set { SetProperty(ref _lastCallDate, value); }
        }

        private string _lastCallDateTime;
        public string LastCallDateTime
        {
            get { return _lastCallDateTime; }
            set { SetProperty(ref _lastCallDateTime, value); GetCallDate(value); }
        }

        private DateTime? _callDate;
        public DateTime? CallDate
        {
            get { return _callDate; }
            set { SetProperty(ref _callDate, value); }
        }


        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        private string _territoryName;
        public string TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }

        private string _territoryNumber;
        public string TerritoryNumber
        {
            get { return _territoryNumber; }
            set { SetProperty(ref _territoryNumber, value); }
        }

        public string SearchDisplayPath
        {
            get { return (CustomerName + "\n" + CustomerNumber + "\n" + StoreType + "\n" + City + "\n" + State); }
        }

        public int CustomerId { get; set; }
        private bool _isEllipsisVisible;
        public bool IsEllipsisVisible
        {
            get { return _isEllipsisVisible; }
            set { SetProperty(ref _isEllipsisVisible, value); }
        }
        private byte _vripOrTravel;
        public byte VripOrTravel
        {
            get { return _vripOrTravel; }
            set { SetProperty(ref _vripOrTravel, value); }
        }

        public int AccountType { get; set; }
        public string DeviceCustomerId { get; set; }
        public int StateId { get; set; }
        public string ActivityComment { get; set; }
        public string ActivityCreator { get; set; }
        public string OrderDeliveryWeekDays { get; set; }
        public string TerritoryID { get; set; }
        private void GetCallDate(string value)
        {
            Parallel.Invoke(() =>
            {
                lock (thisLock)
                {
                    LastCallDate = Helpers.DateTimeHelper.ConvertStringDateToMM_DD_YYYY(value);
                    var isValidDate = DateTime.TryParse(value, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                    if (isValidDate)
                        CallDate = date;
                }
            });
        }

    }
}
