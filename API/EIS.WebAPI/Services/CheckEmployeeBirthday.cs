using EIS.Data.Context;
using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using EIS.WebAPI.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIS.WebAPI.Services
{
    public interface ICheckEmployeeBirthday
    {
        void EmailSentForBirthday();
    }
    public class CheckEmployeeBirthday : ICheckEmployeeBirthday
    {
        public readonly IRepositoryWrapper _repository;
        public readonly IConfiguration _configuration;
        private ApplicationDbContext _dbContext;
        public CheckEmployeeBirthday(IRepositoryWrapper repository,IConfiguration configuration,ApplicationDbContext dbcontext)
        {
            _repository = repository;
            _configuration = configuration;
            _dbContext = dbcontext;
        }
        public void EmailSentForBirthday()
        {
            int day = DateTime.Now.Day;
            int month = DateTime.Now.Month;
            string emailBody = "Today is birthday of ";
            var rootPath = @"C:\Temp\BirthdayImage\Birthday.jpg";
            List<Person> person = _repository.Employee.FindAllByCondition(x => x.DateOfBirth.Day == day && x.DateOfBirth.Month == month).ToList();
            if (person.Count > 0)
            {
                foreach (Person p in person)
                {
                    emailBody += p.FullName + ",";
                    string To = p.EmailAddress;
                    string subject = "Birthday Wishes From Aadyam Consultant";
                  
                  
                    Image img = Image.FromFile(rootPath);
                    string body = "Dear " + p.FullName + ", \n" + "Aadyam Consultant wishes you Happy Birthday\n";

                    new EmailManager(_configuration, _repository).SendEmail(subject, body, To,null);
                }
                emailBody = emailBody.Remove(emailBody.Length - 1, 1) + ".";



                var results = _dbContext.Person.Include(x => x.Role).Where(x => x.Role.Name.ToUpper() == "HR")
                .Select(p => new
                {
                    p.EmailAddress
                }).ToList();

                foreach (var p in results)
                {
                    string To = p.EmailAddress;
                    string subject = "Birthday Reminder";
                    string body = "Dear HR, \n" + emailBody;
                    new EmailManager(_configuration, _repository).SendEmail(subject, body, To, null);
                }
            }
        }
    }
}
