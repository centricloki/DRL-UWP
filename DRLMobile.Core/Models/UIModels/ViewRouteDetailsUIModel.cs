using System;
using System.Collections.Generic;
using System.Linq;

namespace DRLMobile.Core.Models.UIModels
{
    public class ViewRouteDetailsUIModel : BaseModel
    {
        public string Longitude { get; set; }
        public string Latitude { get; set; }

        public string PhysicalAddressZipCode { get; set; }

        public string PhysicalAddressCityID { get; set; }

        public int PhysicalAddressStateID { get; set; }

        public string PhysicalAddress { get; set; }

        public int CustomerID { get; set; }
        public string CustomerNumber { get; set; }

        public string DeviceCustomerID { get; set; }

        public string CustomerName { get; set; }

        public int AccountType { get; set; }

        public string StateName { get; set; }

        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }

        private int _listIndex;
        public int ListIndex
        {
            get { return _listIndex; }
            set { SetProperty(ref _listIndex, value); }
        }
        // public int ListIndex { get; set; }

        public string SearchDisplayPath
        {
            get { return CustomerName + " " + PhysicalAddress; }
        }

        public string CustomerAddress
        {
            get
            {
                var addressParts = new List<string> { PhysicalAddress, PhysicalAddressCityID, StateName };

                // Only add ZIP code if it's a valid 5-digit code (not "0" or empty)
                if ( 
                    !string.IsNullOrWhiteSpace(PhysicalAddressZipCode) &&
                    !string.Equals(PhysicalAddressZipCode, "0", StringComparison.OrdinalIgnoreCase) &&
                    PhysicalAddressZipCode.Length == 5 &&
                    PhysicalAddressZipCode.All(char.IsDigit))
                {
                    addressParts.Add(PhysicalAddressZipCode);
                }

                return string.Join(", ", addressParts);
            }
        }
    }
}