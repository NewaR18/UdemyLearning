using AspNetCore.Models.ViewModel;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.EmailConfigurations
{
    public class EmailSender : IEmailSender
    {
        private readonly IOptions<MailDetailsViewModel> _options;
        public EmailSender(IOptions<MailDetailsViewModel> options) 
        { 
            _options = options;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            //Way 1 -- It is obsolete according to Microsoft official website
            //https://learn.microsoft.com/en-us/dotnet/api/system.net.mail.smtpclient?view=netframework-4.7.1
            /*MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress("sudipshrestha960@gmail.com");
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new System.Net.Mail.SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("sudipshrestha960@gmail.com", "lucdxlvshdjpqzdy");
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return Task.CompletedTask;*/

            //Way 2 -- recommended By Microsoft
            var emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse(_options.Value.Email));
            emailToSend.To.Add(MailboxAddress.Parse(email));
            emailToSend.Subject = subject;
            emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };
            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                emailClient.Connect(_options.Value.Host, _options.Value.Port, MailKit.Security.SecureSocketOptions.StartTls);
                emailClient.Authenticate(_options.Value.Username, _options.Value.Password);
                emailClient.Send(emailToSend);
                emailClient.Disconnect(true);
            }
            return Task.CompletedTask;
        }
    }
}
