using DRLMobile.Core.Helpers;
using DRLMobile.ExceptionHandler;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    public class ActivityForAllCustomerUIModel : BaseModel
    {
        private readonly object thisLock = new object();

        private string _stateName;
        public string StateName
        {
            get { return _stateName; }
            set { SetProperty(ref _stateName, value); }
        }

        private string _distributorNo;
        public string DistributorNo
        {
            get { return _distributorNo; }
            set { SetProperty(ref _distributorNo, value); }
        }

        private string _createdDate;
        public string CreatedDate
        {
            get { return _createdDate; }
            set { SetProperty(ref _createdDate, value); }
        }

        private string _physicalAddressCityID;
        public string PhysicalAddressCityID
        {
            get { return _physicalAddressCityID; }
            set { SetProperty(ref _physicalAddressCityID, value); }
        }

        private string _physicalAddressStateID;
        public string PhysicalAddressStateID
        {
            get { return _physicalAddressStateID; }
            set { SetProperty(ref _physicalAddressStateID, value); }
        }

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

        private int _callActivityID;
        public int CallActivityID
        {
            get { return _callActivityID; }
            set { SetProperty(ref _callActivityID, value); }
        }

        private string _activityType;
        public string ActivityType
        {
            get { return _activityType; }
            set { SetProperty(ref _activityType, value); }
        }

        private string _customerCreatedDate;
        public string CustomerCreatedDate
        {
            get { return _customerCreatedDate; }
            set { SetProperty(ref _customerCreatedDate, value); }
        }

        private string _callDate;
        public string CallDate
        {
            get { return _callDate; }
            set { SetProperty(ref _callDate, value); PopulateDisplayDate(); }
        }

        private string _displayCallDate;
        public string DisplayCallDate
        {
            get { return _displayCallDate; }
            set { SetProperty(ref _displayCallDate, value); }
        }

        private string _salesPerson;
        public string SalesPerson
        {
            get { return _salesPerson; }
            set { SetProperty(ref _salesPerson, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        private string _userNameFull;
        public string UserNameFull
        {
            get { return _userNameFull; }
            set { SetProperty(ref _userNameFull, value); }
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

        private DateTime _lastcallDate;
        public DateTime LastcallDate
        {
            get { return _lastcallDate; }
            set { SetProperty(ref _lastcallDate, value); }
        }

        public int UserID { get; set; }
        public string CustomerID { get; set; }
        public int IsVoided { get; set; }
        public int IsVoidedSync { get; set; }
        public string WholesaleInvoiceFilePath { get; set; }
        public string Objective { get; set; }
        public string Result { get; set; }
        public string Comments { get; set; }
        public string UpdateDate { get; set; }
        public string OrderID { get; set; }
        public string GratisProduct { get; set; }
        public string CallActivityDeviceID { get; set; }
        public int IsExported { get; set; }
        public int isDeleted { get; set; }
        public int TerritoryID { get; set; }
        public string TerritoryName { get; set; }
        public string Hours { get; set; }

        public string SearchDisplayPath { get { return CustomerName + " " + CustomerNumber + " " + ActivityType + " " + UserName + " " + DistributorNo + " " + UserNameFull + " " + PhysicalAddressCityID + " " + StateName + " " + DisplayCallDate; } }

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
                               DateTime tempDate;

                               DateTime.TryParse(CallDate, new CultureInfo("en-US"), DateTimeStyles.None, out tempDate);

                               LastcallDate = tempDate;

                               DisplayCallDate = tempDate.Date.ToLongDateString();
                           }
                           catch (Exception)
                           {
                               var date = DateTime.Parse(CallDate, new CultureInfo("en-US"));
                               LastcallDate = date;
                               DisplayCallDate = date.ToString(DateTimeHelper.USDateFormat, CultureInfo.InvariantCulture);
                           }
                       }
                   });
            }
            catch (Exception ex)
            {
                ErrorLogger.WriteToErrorLog(nameof(ActivityForAllCustomerUIModel), "PopulateDisplayDate", ex.StackTrace);
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