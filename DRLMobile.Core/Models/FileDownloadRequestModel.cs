namespace DRLMobile.Core.Models
{
    public class FileDownloadRequestModel : BaseModel
    {
        public string filename { get; set; }
        public string filetype { get; set; }
        public string updatedate { get; set; }
    }
}