using DRLMobile.Core.Models.DataModels;
using DRLMobile.ExceptionHandler;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace DRLMobile.Core.Models.UIModels
{
    public class CustomerDocumentUIModel : BaseModel
    {
        private readonly object thisLock = new object();

        public int? DocumentId { get; set; }

        private string _docName;
        public string DocName
        {
            get { return _docName; }
            set { SetProperty(ref _docName, value); }
        }

        private string _docUrl;
        public string DocUrl
        {
            get { return _docUrl; }
            set { SetProperty(ref _docUrl, value); }
        }

        private string _displayDocUrl;
        public string DisplayDocUrl
        {
            get { return _displayDocUrl; }
            set { SetProperty(ref _displayDocUrl, value); }
        }

        private string _docDesc;
        public string DocDesc
        {
            get { return _docDesc; }
            set { SetProperty(ref _docDesc, value); }
        }

        private string _docType;
        public string DocType
        {
            get { return _docType; }
            set { SetProperty(ref _docType, value); }
        }

        private bool _isPublishToChildren;
        public bool IsPublishToChildren
        {
            get { return _isPublishToChildren; }
            set { SetProperty(ref _isPublishToChildren, value); }
        }

        private bool _isPreviewIconVisible;
        public bool IsPreviewIconVisible
        {
            get { return _isPreviewIconVisible; }
            set { SetProperty(ref _isPreviewIconVisible, value); }
        }

        private bool _isDownloadIconVisible;
        public bool IsDownloadIconVisible
        {
            get { return _isDownloadIconVisible; }
            set { SetProperty(ref _isDownloadIconVisible, value); }
        }

        public CustomerDocument CustomerDocument { get; set; }

        public int CustomerId { get; set; }

        private bool _isProgressVisible;
        public bool IsProgressVisible
        {
            get { return _isProgressVisible; }
            set { SetProperty(ref _isProgressVisible, value); }
        }

        private DateTime _documentDateTime;
        public DateTime DocumentDateTime
        {
            get { return _documentDateTime; }
            set { SetProperty(ref _documentDateTime, value); }
        }

        public void CopyToDeleteObject()
        {
            CustomerDocument.UpdateDateTime = Helpers.DateTimeHelper.ConvertToDbInsertDateTimeFormat(DateTime.Now);
            CustomerDocument.IsDelete = "1";
            CustomerDocument.IsExported = 0;
        }
    }
}
