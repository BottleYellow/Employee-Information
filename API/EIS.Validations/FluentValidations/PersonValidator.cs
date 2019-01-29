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
            RuleFor(x => x.MobileNumber).Length(10).Matches("^[0-9]*$").NotNull().Must(UniqueMobileNumber).WithMessage("Mobile Number already exists"); ;
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.EmployeeCode).NotNull().Must(UniqueCode).WithMessage("Employee Code already exists");
            RuleFor(x => x.EmailAddress).EmailAddress().NotNull().Must(UniqueEmail).WithMessage("Email Id already exists");
            RuleFor(x => x.AadharCard).Must(UniqueAadhar).WithMessage("Aadhar No already exists");
            RuleFor(x => x.PanCard).Must(UniquePan).WithMessage("Pan Card No already exists");
        }
        public bool UniqueCode(Person obj, string EmployeeCode)
        {
            var person = _repositoryWrapper.Employee.FindByCondition(x => x.EmployeeCode == EmployeeCode);
            if (person == null)
            {
                return true;
            }
            else
            {
                return person.Id == obj.Id;
            }
        }
        public bool UniqueEmail(Person obj,string email)
        {
            var person = _repositoryWrapper.Employee.FindByCondition(x => x.EmailAddress == email);
                if (person == null)
                {
                    return true;
                }
                else
                {
                    return person.Id == obj.Id;
                }  
        }
        public bool UniqueMobileNumber(Person obj,string mobile)
        {
            var person = _repositoryWrapper.Employee.FindByCondition(x=>x.MobileNumber==mobile);
            if (person == null)
            {
                return true;
            }
            else
            {
                return person.Id == obj.Id;
            }
        }
        public bool UniqueAadhar(Person obj,string aadhar)
        {
            var person = _repositoryWrapper.Employee.FindByCondition(x=>x.AadharCard==aadhar);
            if (person == null || string.IsNullOrEmpty(person.AadharCard))
            {
                return true;
            }
            else
            {
                return person.Id == obj.Id;
            }
        }
        public bool UniquePan(Person obj,string pan)
        {
            var person = _repositoryWrapper.Employee.FindByCondition(x=>x.PanCard==pan);
            
            if (person == null || string.IsNullOrEmpty(person.PanCard))
            {
                return true;
            }
            else
            {
                return person.Id == obj.Id;
            }
        }
    }
}
