using EIS.Entities.Leave;
using FluentValidation;

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
