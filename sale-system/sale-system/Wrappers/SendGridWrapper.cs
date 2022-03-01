using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Wrappers
{
    public class SendGridWrapper
    {
        private readonly IConfiguration _configuration;
        public SendGridWrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMails(List<EmailAddress> tos)
        {
            var section = _configuration.GetSection("SendGrid");
            string apiKey = section.GetValue<string>("SendGridKey");
            string senderMail = section.GetValue<string>("Email");
            string senderName = section.GetValue<string>("Name");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderMail, senderName);

            var subject = "SaleSystem Newsletter";
            var htmlContent = "<strong>New post with category that you subsribe!</strong>";
            var displayRecipients = false;
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, "", htmlContent, displayRecipients);
            await client.SendEmailAsync(msg);
        }
    }
}
