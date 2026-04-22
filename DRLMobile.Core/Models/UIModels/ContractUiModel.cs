namespace DRLMobile.Core.Models.UIModels
{
    public class ContractUiModel : BaseModel
    {
        private int? _contractID;

        public int? ContractID
        {
            get { return _contractID; }
            set { SetProperty(ref _contractID, value); }
        }
        private string _contractPlanType;

        public string ContractPlanType
        {
            get { return _contractPlanType; }
            set { SetProperty(ref _contractPlanType, value); }
        }
        private string _contractYear;

        public string ContractYear
        {
            get { return _contractYear; }
            set { SetProperty(ref _contractYear, value); }
        }
        private string _numberOfPayments;

        public string NumberOfPayments
        {
            get { return _numberOfPayments; }
            set { SetProperty(ref _numberOfPayments, value); }
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
        private string _thirdPaymentID;

        public string ThirdPaymentID
        {
            get { return _thirdPaymentID; }
            set { SetProperty(ref _thirdPaymentID, value); }
        }
        private string _thirdPaymentAmount;

        public string ThirdPaymentAmount
        {
            get { return _thirdPaymentAmount; }
            set { SetProperty(ref _thirdPaymentAmount, value); }
        }
        private string _fourthPaymentID;

        public string FourthPaymentID
        {
            get { return _fourthPaymentID; }
            set { SetProperty(ref _fourthPaymentID, value); }
        }
        private string _fourthPaymentAmount;

        public string FourthPaymentAmount
        {
            get { return _fourthPaymentAmount; }
            set { SetProperty(ref _fourthPaymentAmount, value); }
        }
        private string _customerID;

        public string CustomerID
        {
            get { return _customerID; }
            set { SetProperty(ref _customerID, value); }
        }
        public string SearchDisplayPath
        {
            get { return (ContractID + " " + ContractPlanType + " " + ContractYear + " " + NumberOfPayments + " " + FirstPaymentID + " " + FirstPaymentAmount + " " + SecondPaymentID + " " + SecondPaymentAmount + " " + ThirdPaymentID + " " + ThirdPaymentAmount + " " + FourthPaymentID + " " + FourthPaymentAmount); }
        }
    }
}
