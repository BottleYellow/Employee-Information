using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using FluentValidation;
using System;

namespace EIS.Validations.FluentValidations
{
    public class HolidayValidator : AbstractValidator<Holiday>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public HolidayValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.Date).NotEmpty().WithMessage("Please select date")
                .Must(UniqueForLocation).WithMessage("Holiday already added with this date for given location");
            RuleFor(x => x.Vacation).NotEmpty().WithMessage("Please enter vacation for holiday");
        }
        public bool UniqueForLocation(Holiday obj,DateTime date)
        {
            var holiday = _repositoryWrapper.Holidays.FindByCondition(x => x.Date == date.Date && x.LocationId == obj.LocationId && x.IsActive==true);
            if (holiday == null)
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
