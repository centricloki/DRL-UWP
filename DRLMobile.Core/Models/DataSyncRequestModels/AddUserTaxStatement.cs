namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddUserTaxStatement : BaseModel
    {
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string Description { get; set; }
        public string DeviceUserTaxStatementID { get; set; }
        public int IsDeleted { get; set; }
        public int IsExported { get; set; }
        public string Title { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int UserID { get; set; }
        public int UserTaxStatementID { get; set; }
    }
}