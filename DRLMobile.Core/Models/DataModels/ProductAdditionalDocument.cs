using Newtonsoft.Json;
using SQLite;
using System;

namespace DRLMobile.Core.Models.DataModels
{
    public class ProductAdditionalDocument
    {
        [JsonProperty("productadditionaldocumentid")]
        public string ProductAdditionalDocumentID { get; set; }

        [JsonProperty("productid")]
        public int ProductID { get; set; }

        [JsonProperty("documentfilename")]
        public string DocumentFileName { get; set; }

        [JsonProperty("importedfrom")]
        public string ImportedFrom { get; set; }

        [JsonProperty("documenttype")]
        public string DocumentType { get; set; }

        [JsonProperty("langid")]
        public string LangID { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("updatedate")]
        public string UpdatedDate { get; set; }
        public string LocalFilePath { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("isdownload")]
        public int IsDownload { get; set; }
    }
}
