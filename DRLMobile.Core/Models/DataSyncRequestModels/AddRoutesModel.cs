using System.Collections.Generic;

namespace DRLMobile.Core.Models.DataSyncRequestModels
{
    public class AddRoutesModel : BaseModel
    {
        public string addressname { get; set; }
        public int cityid { get; set; }
        public string createdate { get; set; }
        public string devicerouteid { get; set; }
        public string discription { get; set; }
        public string enddate { get; set; }
        public string housenumber { get; set; }
        public int idAssignToTSM { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string planneddate { get; set; }
        public string routedescription { get; set; }
        public string routename { get; set; }
        public string routetype { get; set; }
        public string startdate { get; set; }
        public string streetname { get; set; }
        public int userid { get; set; }
        public string zipcode { get; set; }
        public List<int> customrdata { get; set; }
        public int isdeleted { get; set; }

    }
}
