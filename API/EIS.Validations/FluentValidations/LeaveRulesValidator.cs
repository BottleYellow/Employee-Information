using EIS.Entities.Leave;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class LeaveRulesValidator : AbstractValidator<LeaveRules>
    {
        public LeaveRulesValidator()
        {
            RuleFor(x => x.LeaveType).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Validity).Must(GreaterThanZero).NotNull();
            RuleFor(x => x.ValidFrom).NotNull().WithMessage("Please enter valid date");
            RuleFor(m => m.ValidTo)
                    .NotNull().WithMessage("Please enter valid date")
                    .GreaterThan(m => m.ValidFrom).WithMessage("'Valid To' date must after 'Valid From' date")
                    .When(m => m.ValidFrom != null);
        }
        public bool GreaterThanZero(int n)
        {
            if (n > 0)
                return true;
            else
                return false;
        }
    }
}
