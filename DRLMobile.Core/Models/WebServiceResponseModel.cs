using System.Net;

namespace DRLMobile.Core.Models
{
    public class WebServiceResponseModel : BaseModel
    {
        public string ServerResponse { get; set; }
        public string ErrorMessage { get; set; }
        public HttpStatusCode ResponseSatusCode { get; set; }
        public string HttpsResponseSatusDescription { get; set; }
    }
}