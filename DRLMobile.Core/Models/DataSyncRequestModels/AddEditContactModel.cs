namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddEditContactModel
    {
        public string contactemail { get; set; }
        public string contactfax { get; set; }
        public int contactid { get; set; }
        public string contactname { get; set; }
        public string contactphone { get; set; }
        public string contactrole { get; set; }
        public string createdate { get; set; }
        public int deleted { get; set; }
        public string devicecontactid { get; set; }
        public string devicecustomerid { get; set; }
        public int importedfrom { get; set; }
        public int isexported { get; set; }
        public int positionid { get; set; }
        public int rankid { get; set; }
        public string updatedate { get; set; }
    }
}