using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore.Utilities.ManageBackgroundJobs
{
    public class ManageHangfireJobs
    {
        public Task AddJobToQueue()
        {
            //BackgroundJob.Enqueue<T>(x => x.Execute());
            return Task.CompletedTask;
        }
        public Task AddEmailJobToQueue(string email, string subject, string htmlMessage)
        {
            BackgroundJob.Enqueue<IEmailSender>(x => x.SendEmailAsync(email, subject, htmlMessage));
            return Task.CompletedTask;
        }
    }
}
