using DRLMobile.Core.Models.UIModels;
using SQLite;
using System;
using System.Diagnostics;
using Newtonsoft.Json;
using Windows.UI.Xaml;
using System.Globalization;
using DRLMobile.Core.Helpers;

namespace DRLMobile.Core.Models.DataModels
{
    public class CustomerDocument
    {
        [PrimaryKey]
        [JsonProperty("customerdocumentid")]
        public int? CustomerDocumentID { get; set; }

        [JsonProperty("customerid")]
        public int CustomerID { get; set; }

        [JsonProperty("originalfilename")]
        public string OriginalFileName { get; set; }

        [JsonProperty("customerdocumentname")]
        public string CustomerDocumentName { get; set; }

        [JsonProperty("importedfrom")]
        public int ImportedFrom { get; set; }
        public int IsExported { get; set; }

        [JsonProperty("createdatetime")]
        public string CreateDateTime { get; set; }

        [JsonProperty("updatedatetime")]
        public string UpdateDateTime { get; set; }

        [JsonProperty("description")]
        public string CustomerDocDesc { get; set; }

        [JsonProperty("documenttype")]
        public string CustomerDocType { get; set; }

        [JsonProperty("devicedocumentid")]
        public string DeviceDocID { get; set; }

        [JsonProperty("isdelete")]
        public string IsDelete { get; set; }

        [JsonProperty("isdownload")]
        public int IsDownload { get; set; }

        [JsonProperty("ispublishedtochild")]
        public string IsPublishedToChild { get; set; }

        private bool _isexportedFromServer;
        [Ignore]
        [JsonProperty("isexported")]
        public bool IsExportedFromServer
        {
            get { return _isexportedFromServer; }
            set
            {
                _isexportedFromServer = value;

                IsExported = Convert.ToInt32(value);
            }
        }

        [Ignore]
        public DateTime CreatedDateFromServer => DateTimeHelper.ConvertToDBDateTime(CreateDateTime);

        public CustomerDocumentUIModel CopyTo()
        {
            var uimodel = new CustomerDocumentUIModel();

            uimodel.DocDesc = CustomerDocDesc;
            uimodel.DocumentId = CustomerDocumentID;
            uimodel.DocUrl = OriginalFileName;
            uimodel.DisplayDocUrl = GetTheImageThumblineAsPerDocType(OriginalFileName); //OriginalFileName.ToUpper().StartsWith("HTTP") ?  string.Empty : OriginalFileName;
            uimodel.DocName = Helpers.HelperMethods.GetNameFromURL(OriginalFileName);
            uimodel.DocType = CustomerDocType;
            uimodel.IsPreviewIconVisible = !OriginalFileName.ToUpper().StartsWith("HTTP");
            uimodel.IsDownloadIconVisible = OriginalFileName.ToUpper().StartsWith("HTTP");
            uimodel.CustomerId = CustomerID;
            uimodel.CustomerDocument = this;
            uimodel.DocumentDateTime = DateTime.Parse(CreateDateTime, CultureInfo.InvariantCulture);

            return uimodel;
        }

        private string GetTheImageThumblineAsPerDocType(string path)
        {
            //".jpg", ".jpeg", ".png", ".bmp"
            var returnPath = "";
            if (path.ToLower().Contains(".jpg") || path.ToLower().Contains(".jpeg") || path.ToLower().Contains(".png") || path.ToLower().Contains(".bmp"))
            {
                returnPath = OriginalFileName.ToUpper().StartsWith("HTTP") ? string.Empty : OriginalFileName;
            }
            else if (path.ToLower().Contains(".pdf"))
            {
                returnPath = (string)Application.Current.Resources["DocumentIconImage"];
            }
            else
            {
                returnPath = (string)Application.Current.Resources["DocumentIcon"];
            }

            return returnPath;
        }
    }
}