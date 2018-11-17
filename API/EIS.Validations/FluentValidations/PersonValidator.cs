using EIS.Entities.Employee;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class PersonValidator : AbstractValidator<Person>
    {
        public PersonValidator()
        {
            RuleFor(x => x.IdCard).MaximumLength(15).NotNull();
            RuleFor(x => x.PanCard).Length(10);
            RuleFor(x => x.AadharCard).Length(12).Matches("^[0-9]*$");
            RuleFor(x => x.Image);
            RuleFor(x => x.Designation).NotNull();
            RuleFor(x => x.FirstName).MaximumLength(50).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.MiddleName).MaximumLength(50).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.LastName).MaximumLength(50).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.JoinDate).NotNull();
            RuleFor(x => x.Gender).NotNull();
            RuleFor(x => x.MobileNumber).Length(10, 15).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.DateOfBirth).NotNull();
            RuleFor(x => x.EmailAddress).EmailAddress().NotNull();
        }
    }
}
