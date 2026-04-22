namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddCustomerDistributorModel : BaseModel
    {
        public int DistributorID { get; set; }
        public int CustomerDistributorID { get; set; }
        public string DeviceCustomerID { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string UpdatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public int DistributorPriority { get; set; }
        public int IsExported { get; set; }
        public int IsDeleted { get; set; }
    }
}
