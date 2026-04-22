using Newtonsoft.Json;
using SQLite;

namespace DRLMobile.Core.Models.DataModels
{
    public class ContractMaster
    {
        public int ContractID { get; set; }
        public string ContractPlanType { get; set; }
        public string ContractYear { get; set; }
        public string NumberOfPayments { get; set; }
        public string FirstPaymentID { get; set; }
        public string FirstPaymentAmount { get; set; }
        public string SecondPaymentID { get; set; }
        public string SecondPaymentAmount { get; set; }
        public string ThirdPaymentID { get; set; }
        public string ThirdPaymentAmount { get; set; }
        public string FourthPaymentID { get; set; }
        public string FourthPaymentAmount { get; set; }
        public string CustomerID { get; set; }
    }
}