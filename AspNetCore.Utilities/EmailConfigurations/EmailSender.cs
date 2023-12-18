using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.EmailConfigurations
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress("sudipshrestha960@gmail.com");
            mail.Subject = subject;
            mail.Body = htmlMessage;
            mail.IsBodyHtml = true;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new System.Net.NetworkCredential("sudipshrestha960@gmail.com", "lucdxlvshdjpqzdy");
            smtp.EnableSsl = true;
            smtp.Send(mail);
            return Task.CompletedTask;
        }
    }
}
