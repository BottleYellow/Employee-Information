using EIS.Entities.Employee;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class AttendanceValidator : AbstractValidator<Attendance>
    {
        public AttendanceValidator()
        {
            RuleFor(x => x.DateIn).NotNull();
            RuleFor(x => x.TimeIn).NotNull();
            RuleFor(x => x.DateOut).NotNull();
            RuleFor(x => x.TimeOut).NotNull();
        }
    }
}
