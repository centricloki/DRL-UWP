namespace DRLMobile.Core.Models.DataModels
{
    public class PromotionMaster
    {
        public int PromotionID { get; set; }
        public string PromotionPlanType { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string FirstPaymentID { get; set; }
        public string FirstPaymentAmount { get; set; }
        public string SecondPaymentID { get; set; }
        public string SecondPaymentAmount { get; set; }
        public string CustomerID { get; set; }
    }
}