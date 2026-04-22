using DRLMobile.Core.Interface;
using DRLMobile.Core.Models.DataModels;

using MsgKit;

using OpenMcdf;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.UI.Popups;

using Task = System.Threading.Tasks.Task;

namespace DRLMobile.Uwp.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _msgFilepath;
        private static readonly object _lock = new object();
        private static EmailService instance = null;
        private const string MsgFilename = "tmp.msg";
        private readonly StorageFolder LocalFolder = ApplicationData.Current.LocalCacheFolder;
        private EmailService()
        {
            _msgFilepath = Path.Combine(LocalFolder.Path, MsgFilename);
        }

        public static EmailService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                    {
                        instance = new EmailService();
                    }
                    return instance;
                }
            }
        }


        private EmailModel EmailModel = new EmailModel();

        public async Task<bool> SendMailFromOutlook(EmailModel email)
        {
            EmailModel = email;
            if (!await AssembleMail())
            {
                return false;
            }
            return await OpenOutlookAsync();
        }

        private async Task<bool> OpenOutlookAsync()
        {
            var msgFile = await StorageFile.GetFileFromPathAsync(_msgFilepath);
            var result = await Windows.System.Launcher.LaunchFileAsync(msgFile);

            if (!result)
            {
                await new MessageDialog(Helpers.ResourceExtensions.GetLocalized("SetupMailAccountErrorMsg")).ShowAsync().AsTask().ConfigureAwait(false);
                return false;
            }
            return true;
        }

        private async Task<bool> AssembleMail()
        {
            var mailSender = new Sender(string.Empty, string.Empty);
            var mail = new Email(mailSender, EmailModel?.Subject, true);

            if (EmailModel?.To != null)
            {
                foreach (var receipent in EmailModel?.To)
                {
                    mail.Recipients.AddTo(receipent);
                }
            }


            if (EmailModel?.Cc != null)
            {
                foreach (var receipent in EmailModel?.Cc)
                {
                    mail.Recipients.AddCc(receipent);
                }
            }


            if (EmailModel?.Bcc != null)
            {
                foreach (var receipent in EmailModel?.Bcc)
                {
                    mail.Recipients.AddBcc(receipent);
                }
            }

            if (!string.IsNullOrWhiteSpace(EmailModel?.BodyHtml))
            {
                mail.BodyHtml = EmailModel?.BodyHtml;
            }
            else if (!string.IsNullOrWhiteSpace(EmailModel?.BodyText))
            {
                mail.BodyText = EmailModel?.BodyText;
            }
            var filepath = Path.Combine(LocalFolder.Path, MsgFilename);

            if (EmailModel.AttachmentListByPath != null)
            {
                foreach (var attachment in EmailModel?.AttachmentListByPath)
                {
                    if (!string.IsNullOrWhiteSpace(attachment))
                        mail.Attachments.Add(attachment);
                }
            }

            if (EmailModel.AttachmentListByFile != null)
            {
                foreach (var attachment in EmailModel?.AttachmentListByFile)
                {
                    mail.Attachments.Add(attachment.Path);
                }
            }

            try
            {
                await Task.Run(() => mail.Save(filepath));
            }
            catch (CFException)
            {
                await new MessageDialog("An Outlook instance is already opened.").ShowAsync().AsTask().ConfigureAwait(false);
                return false;
            }

            mail.Dispose();

            return true;
        }
    }
}
