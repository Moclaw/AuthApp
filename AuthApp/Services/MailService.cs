using AuthApp.Models;
using AuthApp.Models.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AuthApp.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        protected IConfiguration Configuration { get; }
        public MailService(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _mailSettings = mailSettings.Value;
            Configuration = configuration;
        }
        public bool SendEmailAsync(string toEmail)
        {
            MailMessage message = new MailMessage(
                   from: Configuration["MailSettings:DefaultFromAddress"],
                   to: toEmail,
                   subject: "moclaw-app",
                   body: $"<h1>Your account login!!!</h1>"
               );

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;
            message.ReplyToList.Add(new MailAddress(Configuration["MailSettings:DefaultFromAddress"]));
            message.Sender = new MailAddress(Configuration["MailSettings:DefaultFromAddress"]);
            using (SmtpClient smtp = new SmtpClient(Configuration["MailSettings:SmtpHost"], 587))
            {
                smtp.Credentials = new NetworkCredential(
                   Configuration["MailSettings:DefaultFromAddress"],
                    Configuration["MailSettings:SmtpPassword"]
                );
                smtp.EnableSsl = true;
                smtp.Send(message);
                return true;
            }
        }
    }
}
