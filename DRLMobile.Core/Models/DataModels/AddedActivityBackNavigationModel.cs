using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class AddedActivityBackNavigationModel
    {
        public CallActivityList Activity { get; set; }
        public UserMaster LoggedInUser { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public List<TerritoryMaster> TerritoryList { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public bool IsUserActivity { get; set; }
    }
}
