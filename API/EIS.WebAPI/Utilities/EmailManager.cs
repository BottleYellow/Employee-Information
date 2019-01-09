using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;

namespace EIS.WebAPI.Utilities
{
    public class EmailManager
    {
        public readonly IConfiguration _configuration;
        public EmailManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public void SendEmail(string Subject, string Body, string To)
        {
            string  UserID, Password, SMTPPort, Host;
            UserID = _configuration["appSettings:UserID"];
            Password = _configuration["appSettings:Password"];
            SMTPPort = _configuration["appSettings:SMTPPort"];
            Host = _configuration["appSettings:Host"];
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
