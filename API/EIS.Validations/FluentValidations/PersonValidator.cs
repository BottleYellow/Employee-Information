using EIS.Entities.Employee;
using EIS.Repositories.IRepository;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;

namespace EIS.Validations.FluentValidations
{
    public class PersonValidator : AbstractValidator<Person>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public PersonValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.PanCard).Length(10);
            RuleFor(x => x.AadharCard).Length(12).Matches("^[0-9]*$");
            RuleFor(x => x.FirstName).MaximumLength(50).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.MiddleName).MaximumLength(50).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.LastName).MaximumLength(50).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.JoinDate).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.MobileNumber).Length(10, 15).Matches("^[0-9]*$").NotNull().Must(UniqueMobileNumber).WithMessage("Mobile Number already exists"); ;
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.EmailAddress).EmailAddress().NotNull().Must(UniqueEmail).WithMessage("Email Id already exists");
            RuleFor(x => x.EmailAddress).EmailAddress().NotNull().Must(UniqueEmail).WithMessage("Email Id already exists");
            RuleFor(x => x.AadharCard).NotNull().Must(UniqueAadhar).WithMessage("Aadhar No already exists");
            RuleFor(x => x.PanCard).NotNull().WithMessage("Please enter Pan Card Number").Must(UniquePan).WithMessage("Pan Card No already exists");
        }
        public bool UniqueEmail(string email)
        {
            var persons = _repositoryWrapper.Employee.FindAll();
            var person = persons.Where(x => x.EmailAddress == email).FirstOrDefault();
            if (person == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UniqueMobileNumber(string mobile)
        {
            var persons = _repositoryWrapper.Employee.FindAll();
            var person = persons.Where(x => x.MobileNumber == mobile).FirstOrDefault();
            if (person == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UniqueAadhar(string aadhar)
        {
            var persons = _repositoryWrapper.Employee.FindAll();
            var person = persons.Where(x => x.AadharCard == aadhar).FirstOrDefault();
            if (person == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool UniquePan(string pan)
        {
            var persons = _repositoryWrapper.Employee.FindAll();
            var person = persons.Where(x => x.PanCard == pan).FirstOrDefault();
            if (person == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
