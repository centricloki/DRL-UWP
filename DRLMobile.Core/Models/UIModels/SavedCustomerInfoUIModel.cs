namespace DRLMobile.Core.Models.UIModels
{
    public class SavedCustomerInfoUIModel : BaseModel
    {
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

        private string _physicalAddress;
        public string PhysicalAddress
        {
            get { return _physicalAddress; }
            set { SetProperty(ref _physicalAddress, value); }
        }

        private int _accountType;
        public int AccountType
        {
            get { return _accountType; }
            set { SetProperty(ref _accountType, value); }
        }

        private string _physicalAddressCityID;
        public string PhysicalAddressCityID
        {
            get { return _physicalAddressCityID; }
            set { SetProperty(ref _physicalAddressCityID, value); }
        }

        private string _subAddressText;
        public string SubAddressText
        {
            get { return _subAddressText; }
            set { SetProperty(ref _subAddressText, value); }
        }

        private int _shippingAddressStateID;
        public int ShippingAddressStateID
        {
            get { return _shippingAddressStateID; }
            set { SetProperty(ref _shippingAddressStateID, value); }
        }

        private string _shippingAddressStateName;
        public string ShippingAddressStateName
        {
            get { return _shippingAddressStateName; }
            set { SetProperty(ref _shippingAddressStateName, value); }
        }
    }
}