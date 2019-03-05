using EIS.Entities.SP;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Controllers;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace EIS.WebAPI.Utilities
{
    public class EmailManager:BaseController
    {
        public readonly IConfiguration _configuration;
        public EmailManager(IConfiguration configuration, IRepositoryWrapper repository): base(repository)
        {
            _configuration = configuration;
        }

        public void SendEmail(string Subject, string Body, string To, string fileAttachment)
        {
            MailConfiguration mailConfiguration = _repository.Users.GetMailConfiguration();           
            MailMessage mail = new MailMessage();
            mail.To.Add(To);
            mail.From = new MailAddress(mailConfiguration.UserId);
            mail.Subject = Subject;
            mail.Body = Body;
            SmtpClient smtp = new SmtpClient
            {
                Host = mailConfiguration.Host,
                Port = Convert.ToInt16(mailConfiguration.SMTPPort),
                Credentials = new NetworkCredential(mailConfiguration.UserId, mailConfiguration.Password),
                EnableSsl = true
            };
        if(!string.IsNullOrEmpty(fileAttachment))
        {
            Attachment data = new Attachment(fileAttachment, MediaTypeNames.Application.Octet);
            mail.Attachments.Add(data);
            smtp.Send(mail);
            data.Dispose();
        }
        else
        { 
            smtp.SendAsync(mail,"test");
        }
        }

        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "0123456789abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            Random randNum = new Random();
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;
            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)((_allowedChars.Length) * randNum.NextDouble())];
            }
            return new string(chars);
        }
    }
}
