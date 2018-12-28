using EIS.Entities.Leave;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class LeaveMasterValidator : AbstractValidator<LeaveMaster>
    {
        public LeaveMasterValidator()
        {
            RuleFor(x => x.LeaveType).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Days).Must(GreaterThanZero).NotNull();
            RuleFor(x => x.ValidFrom).NotNull().WithMessage("Please enter valid date");
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
