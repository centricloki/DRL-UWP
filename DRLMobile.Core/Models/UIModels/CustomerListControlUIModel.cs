using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class CustomerListControlUIModel : CustomerPageUIModel
    {
        private bool _showKAMColumn;
        public bool ShowKAMColumn
        {
            get => _showKAMColumn;
            set => SetProperty(ref _showKAMColumn, value);
        }

        private string _kamUserName;
        public string KAMUserName
        {
            get => _kamUserName;
            set => SetProperty(ref _kamUserName, value);
        }

        // Default constructor (optional but good practice)
        public CustomerListControlUIModel()
        {
        }

        // 🔑 Copy constructor: creates a new CustomerListControlUIModel from a CustomerPageUIModel
        public CustomerListControlUIModel(CustomerPageUIModel source) : base()
        {
            if (source == null)
                return;

            // Copy all properties from source (parent class properties)
            this.CustomerName = source.CustomerName;
            this.CustomerNumber = source.CustomerNumber;
            this.StoreType = source.StoreType;
            this.Rank = source.Rank;
            this.Address = source.Address;
            this.City = source.City;
            this.State = source.State;
            this.ZipCode = source.ZipCode;
            this.LastCallDate = source.LastCallDate;
            this.LastCallDateTime = source.LastCallDateTime; // This will also trigger CallDate parsing
            this.CallDate = source.CallDate;
            this.IsSelected = source.IsSelected;
            this.TerritoryName = source.TerritoryName;
            this.TerritoryNumber = source.TerritoryNumber;
            this.IsEllipsisVisible = source.IsEllipsisVisible;
            this.VripOrTravel = source.VripOrTravel;
            this.CustomerId = source.CustomerId;
            this.AccountType = source.AccountType;
            this.DeviceCustomerId = source.DeviceCustomerId;
            this.StateId = source.StateId;
            this.ActivityComment = source.ActivityComment;
            this.ActivityCreator = source.ActivityCreator;
            this.OrderDeliveryWeekDays = source.OrderDeliveryWeekDays;
            this.TerritoryID = source.TerritoryID;
            // Initialize the new property (optional — defaults to false if not set)
            this.ShowKAMColumn = false; // or true, based on your default logic
        }
    }
}
