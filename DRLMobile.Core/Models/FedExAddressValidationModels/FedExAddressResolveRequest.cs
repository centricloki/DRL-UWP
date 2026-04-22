using Newtonsoft.Json;

namespace DRLMobile.Core.Models.FedExAddressValidationModels
{
    public partial class FedExAddressResolveRequest
    {
        [JsonProperty("addressesToValidate")] 
        public FedExAddressesToValidate[] AddressesToValidate { get; set; }
    }

    public partial class FedExAddressesToValidate
    {
        [JsonProperty("address")]
        public FedExAddress Address { get; set; }
    }

    public partial class FedExAddress
    {
        [JsonProperty("streetLines")]
        public string[] StreetLines { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("stateOrProvinceCode")]
        public string StateOrProvinceCode { get; set; }

        [JsonProperty("postalCode")]
        public long PostalCode { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }
    }
}
