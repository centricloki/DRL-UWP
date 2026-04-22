namespace DRLMobile.Core.Models.DataModels
{
    public class SupplyChain
    {
        public int supplychainid { get; set; }
        public int customerid { get; set; }
        public int customerparentid { get; set; }
        public string customername { get; set; }
        public string createdate { get; set; }
        public string updatedate { get; set; }
        public string distributorid { get; set; }

    }
}