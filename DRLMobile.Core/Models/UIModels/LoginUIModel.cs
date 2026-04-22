namespace DRLMobile.Core.Models.UIModels
{
    public class LoginUIModel : BaseModel
    {
        public string lastsyncutcdate { get; set; }
        public string dbfilename { get; set; }
        public int userid { get; set; }
        public ConfigurationDataUIModel configurationdata { get; set; }
        public string responsestatus { get; set; }
        public string errormsg { get; set; }
        public int roleid { get; set; }
    }
}