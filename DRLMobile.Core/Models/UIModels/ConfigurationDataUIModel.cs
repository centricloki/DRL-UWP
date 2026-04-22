namespace DRLMobile.Core.Models.UIModels
{
    public class ConfigurationDataUIModel : BaseModel
    {
        public string FTPPath { get; set; }
        public string FTPUserName { get; set; }
        public string FTPPassword { get; set; }
        public string SRCZIPFilePath { get; set; }
        public string SRCZIPFileDate { get; set; }
    }
}