namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadCustomerDocumentsRequestModel
    {
        public int customerid { get; set; }
        public string devicecustomerid { get; set; }
        public string devicedocumentid { get; set; }
        public string description { get; set; }
        public bool IsPublishedToChild { get; set; }
        public string documenttype { get; set; }
        public string filename { get; set; }
        public string filestream { get; set; }
        public int filetype { get; set; }
        public int pin { get; set; }
        public string username { get; set; }
        public string updatedate { get; set; }
        public string versionnumber { get; set; }
        public string isdelete { get; set; }
    }

    public class UploadActivityRequestModel
    {
        public string DeviceCallActivityID { get; set; }
        public string documenttype { get; set; }
        public string filename { get; set; }
        public string filestream { get; set; }
        public int filetype { get; set; }
        public int pin { get; set; }
        public string username { get; set; }
        public string updatedate { get; set; }
        public string versionnumber { get; set; }
    }
}
