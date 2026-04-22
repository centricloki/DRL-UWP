namespace DRLMobile.Core.Models.UIModels
{
    public class PromotionUiModel : BaseModel
    {
        private int? _promotionID;
        public int? PromotionID
        {
            get { return _promotionID; }
            set { SetProperty(ref _promotionID, value); }
        }
        private string _promotionPlanType;

        public string PromotionPlanType
        {
            get { return _promotionPlanType; }
            set { SetProperty(ref _promotionPlanType, value); }
        }
        private string _startDate;

        public string StartDate
        {
            get { return _startDate; }
            set { SetProperty(ref _startDate, value); }
        }
        private string _endDate;

        public string EndDate
        {
            get { return _endDate; }
            set { SetProperty(ref _endDate, value); }
        }
        private string _firstPaymentID;

        public string FirstPaymentID
        {
            get { return _firstPaymentID; }
            set { SetProperty(ref _firstPaymentID, value); }
        }
        private string _firstPaymentAmount;

        public string FirstPaymentAmount
        {
            get { return _firstPaymentAmount; }
            set { SetProperty(ref _firstPaymentAmount, value); }
        }
        private string _secondPaymentID;

        public string SecondPaymentID
        {
            get { return _secondPaymentID; }
            set { SetProperty(ref _secondPaymentID, value); }
        }
        private string _secondPaymentAmount;

        public string SecondPaymentAmount
        {
            get { return _secondPaymentAmount; }
            set { SetProperty(ref _secondPaymentAmount, value); }
        }
        private string _customerID;

        public string CustomerID
        {
            get { return _customerID; }
            set { SetProperty(ref _customerID, value); }
        }
        public string SearchDisplayPath
        {
            get { return (PromotionID + " " + PromotionPlanType + " " + StartDate + " " + EndDate + " " + FirstPaymentID + " " + FirstPaymentAmount + " " + SecondPaymentID + " " + SecondPaymentAmount); }
        }
    }
}
