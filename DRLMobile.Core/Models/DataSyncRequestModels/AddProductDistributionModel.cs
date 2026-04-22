namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddProductDistributionModel
    {
        public int devicecustomerproductid { get; set; }
        public int customerid { get; set; }
        public string devicecustomerid { get; set; }
        public int productid { get; set; }
        public string lastdistributionrecorddate { get; set; }

        public int isdeleted { get; set; }
    }
}