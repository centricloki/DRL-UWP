using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Uwp.Helpers.CustomerPageGridHelper
{
    public class CustomerFilter
    {
        public CustomerFilter(string customerName = null, string customerNumber = null,
            string storeType = null, string rank = null,
            string address = null, string city = null, string state = null, string lastCallDate = null, DateTime? callDate = null)
        {
            CustomerName = customerName;
            CustomerNumber = customerNumber;
            StoreType = storeType;
            Rank = rank;
            Address = address;
            City = city;
            State = state;
            LastCallDate = lastCallDate;
            CallDate = callDate.Value;
        }
        public string CustomerName { get; private set; }
        public string CustomerNumber { get; private set; }
        public string StoreType { get; private set; }
        public string Rank { get; private set; }
        public string Address { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string LastCallDate { get; private set; }
        public DateTime CallDate { get; private set; }
    }
}
