﻿using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class LeaveRulesValidator : AbstractValidator<LeaveRules>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public LeaveRulesValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.LeaveType).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Description).NotNull();
            RuleFor(x => x.Validity).Must(GreaterThanZero).WithMessage("Value must be greater than 0").When(x => x.IsPaid == true);
            RuleFor(x => x.ValidFrom).NotNull().WithMessage("Please enter valid date");
            RuleFor(m => m.ValidTo).NotNull().WithMessage("Please enter valid date");
            RuleFor(m => m.ValidTo).GreaterThan(m => m.ValidFrom).WithMessage("value must be greater than 'Valid From' date")
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
