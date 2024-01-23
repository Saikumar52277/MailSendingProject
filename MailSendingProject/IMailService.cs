using MailSendingProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailSendingProject
{
     public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
