using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage;

namespace DRLMobile.Core.Models.DataModels
{
    public class EmailModel
    {
        public string Subject { get; set; }
        public ICollection<string> To { get; set; }
        public ICollection<string> Bcc { get; set; }
        public ICollection<string> Cc { get; set; }
        public string BodyText { get; set; }
        public string BodyHtml { get; set; }
        public ICollection<string> AttachmentListByPath { get; set; }
        public ICollection<IStorageFile> AttachmentListByFile { get; set; }
    }
}
