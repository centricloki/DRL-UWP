using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Text;

namespace DRLMobile.Core.Models.FedExAddressValidationModels
{
    public class FedExAddressContentDialog
    {
        public string StreetLines { get; set; }
        public string City { get; set; }
        public string StateOrProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string AddressType { get; set; }
        public bool IsResolved { get; set; }
    }
}
