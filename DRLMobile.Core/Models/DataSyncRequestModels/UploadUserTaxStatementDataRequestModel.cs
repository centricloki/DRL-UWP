using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class UploadUserTaxStatementDataRequestModel
    {
        public int pin { get; set; }
        public string username { get; set; }
        public string versionnumber { get; set; }
        public List<AddUserTaxStatement> UserTaxStatement { get; set; }
    }
}