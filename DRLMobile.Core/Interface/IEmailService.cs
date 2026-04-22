using DRLMobile.Core.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DRLMobile.Core.Interface
{
    public interface IEmailService
    {
        Task<bool> SendMailFromOutlook(EmailModel email);
    }
}
