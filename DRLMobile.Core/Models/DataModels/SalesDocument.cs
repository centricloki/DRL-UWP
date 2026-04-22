namespace DRLMobile.Core.Models.DataModels
{
    public class SalesDocument
    {
        public int SalesDocumentID { get; set; }
        public string OriginalFileName { get; set; }
        public string SalesDocumentName { get; set; }
        public string ImportedFrom { get; set; }
        public string UpdateDate { get; set; }
        public string SalesDocumentPath { get; set; }
        public string SalesDocumentDesc { get; set; }
        public int StyleId { get; set; }
        public int ProductId { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}