namespace DRLMobile.Core.Models.UIModels
{
    public class UserTaxStatementPageUiModel : BaseModel
    {
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
    }
}
