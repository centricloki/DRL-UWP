using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.DataModels
{
    public class RackOrderCartItemModel : BaseModel
    {
        private int _productId;
        public int ProductID
        {
            get { return _productId; }
            set { SetProperty(ref _productId, value); }
        }

        private string _documentFileName;
        public string DocumentFileName
        {
            get { return _documentFileName; }
            set { SetProperty(ref _documentFileName, value); }
        }
        private string _documentType;
        public string DocumentType
        {
            get { return _documentType; }
            set { SetProperty(ref _documentType, value); }
        }

        private string _productName;
        public string ProductName
        {
            get { return _productName; }
            set { SetProperty(ref _productName, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private int _price;
        public int Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }
        public int ProductType { get; set; }
        public string ImportedFrom { get; set; }
        public string DistributionRecordedDate { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
        public int Status { get; set; }
        public int LangID { get; set; }
        public int Quantity { get; set; }
        public int isDistributed { get; set; }
        public int isTobbaco { get; set; }
        public int CatId { get; set; }
        public int BrandId { get; set; }
        public int StyleId { get; set; }
        public string UOM { get; set; }
        public int IsDeleted { get; set; }
        public int SRCHoneySellable { get; set; }
        public int SRCHoneyReturnable { get; set; }
        public int SRCCanIOrder { get; set; }
        [Ignore]
        public bool CartData { get; set; }
    }
}
