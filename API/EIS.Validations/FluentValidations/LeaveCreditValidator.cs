using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class LeaveCreditValidator : AbstractValidator<LeaveCredit>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public LeaveCreditValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.AllotedDays).Must(GreaterThanZero).WithMessage("Value must be greater than 0");
            RuleFor(x => x.ValidFrom).NotNull().WithMessage("Please enter valid date");
            RuleFor(m => m.ValidTo).NotNull().WithMessage("Please enter valid date");
            RuleFor(m => m.ValidTo).GreaterThan(m => m.ValidFrom).WithMessage("value must be greater than 'Valid From' date")
                    .When(m => m.ValidFrom != null);
            RuleFor(x => x.LeaveId).Must(UniqueForPerson).WithMessage("This Policy Credits are already credited to selected employee.");
        }
        public bool GreaterThanZero(float n)
        {
            if (n > 0)
                return true;
            else
                return false;
        }

        public bool UniqueForPerson(LeaveCredit obj, int LeaveId)
        {
            var credit = _repositoryWrapper.LeaveCredit.FindByCondition(x => x.LeaveId == LeaveId && x.PersonId == obj.PersonId && x.IsActive == true);
            if (credit == null)
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
