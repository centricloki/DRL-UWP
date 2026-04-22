namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadSignatureRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public string deviceorderid { get; set; }
        public string filename { get; set; }
        public string filestream { get; set; }
        public int filetype { get; set; }
        public string updatedate { get; set; }
        public string versionnumber { get; set; }
    }
}
