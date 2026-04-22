using System;

namespace DRLMobile.Core.Models.UIModels
{
    public class VripUiModel : BaseModel
    {
        private int? _vripID;
        public int? VripID
        {
            get { return _vripID; }
            set { SetProperty(ref _vripID, value); }
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

        private string _vripName;
        public string VripName
        {
            get { return _vripName; }
            set { SetProperty(ref _vripName, value); }
        }

        private string _cslyr;
        public string Cslyr
        {
            get { return _cslyr; }
            set
            {
                _cslyr = value;
                CslyrToShow = ConvertToDisplayIntegerValue(value);
            }
        }

        private int _cslyrToShow;
        public int CslyrToShow
        {
            get { return _cslyrToShow; }
            set { SetProperty(ref _cslyrToShow, value); }
        }

        private string _target = "0";
        public string Target
        {
            get { return _target; }
            set 
            {
                _target = value;
                TargetToShow = ConvertToCommaSeparatedValue(value);
                TargetToShowInt = ConvertToDisplayIntegerValue(value);
            }
        }

        private string _csytd = "0";
        public string Csytd
        {
            get { return _csytd; }
            set 
            {
                _csytd = value;
                CsytdToShow = ConvertToCommaSeparatedValue(value);
                CsytdToShowInt = ConvertToDisplayIntegerValue(value);
            }
        }

        private string _csNeeded = "0";
        public string CSNeeded
        {
            get { return _csNeeded; }
            set 
            {
                _csNeeded = value;
                CSNeededToShow = ConvertToCommaSeparatedValue(value);
                CSNeededToShowInt = ConvertToDisplayIntegerValue(value);
            }
        }

        private string _qualified;
        public string Qualified
        {
            get { return _qualified; }
            set { SetProperty(ref _qualified, value); }
        }

        private string _rebate;
        public string Rebate
        {
            get { return _rebate; }
            set { SetProperty(ref _rebate, value); }
        }

        public string SearchDisplayPath
        {
            get { return CustomerName + " " + CustomerNumber + " " + Year + " " + CslyrToShow + " " + TargetToShowInt + " " + CsytdToShowInt + " " + CSNeededToShowInt + " " + Qualified + " " + Rebate; }
        }

        private string _targetToShow = "0";
        public string TargetToShow
        {
            get { return _targetToShow; }
            set { SetProperty(ref _targetToShow, value); }
        }

        private int _targetToShowInt;
        public int TargetToShowInt
        {
            get { return _targetToShowInt; }
            set { SetProperty(ref _targetToShowInt, value); }
        }

        private string _csytdToShow = "0";
        public string CsytdToShow
        {
            get { return _csytdToShow; }
            set { SetProperty(ref _csytdToShow, value); }
        }

        private int _csytdToShowInt;
        public int CsytdToShowInt
        {
            get { return _csytdToShowInt; }
            set { SetProperty(ref _csytdToShowInt, value); }
        }

        private string _csNeededToShow = "0";
        public string CSNeededToShow
        {
            get { return _csNeededToShow; }
            set { SetProperty(ref _csNeededToShow, value); }
        }

        private int _csNeededToShowInt;
        public int CSNeededToShowInt
        {
            get { return _csNeededToShowInt; }
            set { SetProperty(ref _csNeededToShowInt, value); }
        }

        //ToShow
        private string ConvertToCommaSeparatedValue(string value)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(value))
            {
                int number = 0;
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
