using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models
{
    public class DataSyncCustomerDocumentZipResponseModel : BaseModel
    {
        public string responsestatus { get; set; }
        public string errormsg { get; set; }
        public string custdocfilename { get; set; }
    }

    public class DataSyncPartialSRCZipResponseModel : BaseModel
    {
        public string responsestatus { get; set; }
        public string errormsg { get; set; }
        public string partialsrcfilename { get; set; }
    }
}
