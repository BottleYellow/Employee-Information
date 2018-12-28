using EIS.Entities.Employee;
using EIS.Entities.Leave;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class LeaveRequestValidator : AbstractValidator<LeaveRequest>
    {
        public LeaveRequestValidator()
        {
            RuleFor(x => x.Reason).NotNull().WithMessage("Please give reason for leave");
        }
    }
}
