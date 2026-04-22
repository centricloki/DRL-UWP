using DRLMobile.ExceptionHandler;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class MapCustomerData : BaseModel
    {
        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string AccountClassificationName { get; set; }
        public string PhysicalAddress { get; set; }
        public string PhysicalAddressCityID { get; set; }
        public string PhysicalAddressStateID { get; set; }
        public string PhysicalAddressZipCode { get; set; }
        public string AccountClassification { get; set; }
        public string Rank { get; set; }
        public string AccountType { get; set; }
        //public string CustomerID { get; set; }
        public string DeviceCustomerID { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TotalAmount { get; set; }


        private int _customerId;
        public int CustomerID
        {
            get { return _customerId; }
            set { SetProperty(ref _customerId , value); }
        }


        private string _lastCallActivityDate;
        public string LastCallActivityDate
        {
            get { return _lastCallActivityDate; }
            set { _lastCallActivityDate = value; SetCallDate(); }
        }
        public DateTime? CallActivityDate { get; set; }

        public int Tag { get; set; }

        public double GrandTotalNumber { get; set; }

        public int Sold { get; set; }

        public string SalesType { get; set; }

        private void SetCallDate()
        {
            try
            {
                var isDateParsed = DateTime.TryParse(LastCallActivityDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                if (isDateParsed)
                    CallActivityDate = date;
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(MapCustomerData), nameof(SetCallDate), ex.StackTrace);
            }
        }

        public void SetAmountValue()
        {
            if (!string.IsNullOrWhiteSpace(TotalAmount))
            {
                var isConverted = double.TryParse(TotalAmount, out double amount);
                GrandTotalNumber = isConverted ? amount : 0;

                if (GrandTotalNumber > 0.00 && GrandTotalNumber <= 100.00)
                    Tag = 1;
                else if (GrandTotalNumber > 100.00 && GrandTotalNumber <= 500.00)
                    Tag = 2;
                else if (GrandTotalNumber > 500)
                    Tag = 3;
                else
                    Tag = 4;
            }
            else
            {
                Tag = 4;
            }
        }
    }
}
