using EIS.Entities.Employee;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class LeavesValidator : AbstractValidator<Leaves>
    {
        public LeavesValidator()
        {
            RuleFor(x => x.LeavesAlloted).NotNull();
            RuleFor(x => x.LeavesAvailed).NotNull();
        }
    }
}
