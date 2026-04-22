namespace DRLMobile.Core.Models.DataModels
{
    public class CartAndFav
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string BrandName { get; set; }
        public int BrandId { get; set; }
        public string StyleName { get; set; }
        public int StyleId { get; set; }
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public bool IsDistribution { get; set; }
        public string DistributionRecordDate { get; set; }
        public int TableType { get; set; }
        public int ProductType { get; set; }
    }
}