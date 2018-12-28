using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EIS.WebAPI.Messanger
{
    public class EmailManager
    {
        public readonly IConfiguration configuration;
        public EmailManager(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public void SendEmail(string Subject, string Body, string To)
        {
            string  UserID, Password, SMTPPort, Host;
            UserID = configuration["appSettings:UserID"];
            Password = configuration["appSettings:Password"];
            SMTPPort = configuration["appSettings:SMTPPort"];
            Host = configuration["appSettings:Host"];
            MailMessage mail = new System.Net.Mail.MailMessage();
            mail.To.Add(To);
            mail.From = new MailAddress(UserID);
            mail.Subject = Subject;
            mail.Body = Body;
            SmtpClient smtp = new SmtpClient
            {
                Host = Host,
                Port = Convert.ToInt16(SMTPPort),
                Credentials = new NetworkCredential(UserID, Password),
                EnableSsl = true
            };
            smtp.Send(mail);
        }
    }
}
