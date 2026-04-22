using DRLMobile.ExceptionHandler;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    public class ActivityForIndividualCustomerUIModel : BaseModel
    {
        private readonly object thisLock = new object();

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

        private string _activityType;
        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType, value); }
        }

        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }

        private string _callDate;
        public string CallDate
        {
            get { return _callDate; }
            set { SetProperty(ref _callDate, value); PopulateDisplayDate(); }
        }

        private string _date;
        public string Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }

        private string _orderID;
        public string OrderID
        {
            get { return _orderID; }
            set { SetProperty(ref _orderID, value); }
        }

        private string _territoryName;
        public string TerritoryName
        {
            get { return _territoryName; }
            set { SetProperty(ref _territoryName, value); }
        }


        private int _territoryID;
        public int TerritoryID
        {
            get { return _territoryID; }
            set { SetProperty(ref _territoryID, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }


        private string _displayTerritoryName;
        public string DisplayTerritoryName
        {
            get { return _displayTerritoryName; }
            set { SetProperty(ref _displayTerritoryName, value); }
        }

        private string _sales;
        public string Sales
        {
            get { return _sales; }
            set { SetProperty(ref _sales, value); }
        }

        private string _grandTotal;
        public string GrandTotal
        {
            get { return _grandTotal; }
            set { SetProperty(ref _grandTotal, value); SetSalesValue(value); }

        }

        public int CallActivityID { get; set; }
        public string CustomerID { get; set; }
        public int UserID { get; set; }
        public string WholesaleInvoiceFilePath { get; set; }
        public string Objective { get; set; }
        public string Result { get; set; }
        public string Comments { get; set; }
        public string SalesPerson { get; set; }
        public string GratisProduct { get; set; }
        public string CallActivityDeviceID { get; set; }
        public int IsExported { get; set; }
        public int isDeleted { get; set; }
        public int IsVoided { get; set; }
        public int IsVoidedSync { get; set; }
        public string Hours { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UpdateDate { get; set; }
        public DateTime _callActivityDate { get; set; }

        public string PhysicalAddressStateID { get; set; }
        public string PhysicalAddressCityID { get; set; }

        public string SearchDisplayPath 
        { 
            get  { return ActivityType + " " + UserName + " " + TerritoryID + " " + TerritoryName + " " + OrderID; }
        }

        public string UserFullName { get { return FirstName + " " + LastName; } }

        private void PopulateDisplayDate()
        {
            try
            {
                Parallel.Invoke(() =>
                {
                    lock (thisLock)
                    {
                        try
                        {
                            DateTime.TryParse(CallDate, new CultureInfo("en-US"), DateTimeStyles.None, out DateTime date);
                            _callActivityDate = date;
                            Date = date.ToString("MMM dd,yyyy", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            var date = DateTime.Parse(CallDate, new CultureInfo("en-US"));
                            _callActivityDate = date;
                            Date = date.ToString("MMM dd,yyyy", CultureInfo.InvariantCulture);
                        }
                    }
                }
                   );
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivityForAllCustomerUIModel), nameof(PopulateDisplayDate), ex.StackTrace);
            }
        }

        private void SetSalesValue(string value)
        {
            switch (ActivityType)
            {
                case "Credit Card Sales":
                    double.TryParse(value, out double valCCS);
                    Sales = string.Format("${0:0.00}", valCCS);
                    break;
                case "Cash Sales Initiative":
                    double.TryParse(value, out double valCSI);
                    Sales = string.Format("${0:0.00}", valCSI);
                    break;
                case "Cash Sale":
                    double.TryParse(value, out double valCS);
                    Sales = string.Format("${0:0.00}", valCS);
                    break;
                default:
                    double.TryParse(value, out double valDefault);
                    Sales = string.Format("${0:0.00}", valDefault);
                    break;
            }
        }
    }
}
