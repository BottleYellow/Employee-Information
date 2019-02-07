using EIS.Entities.Employee;
using EIS.Entities.Hoildays;
using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using FluentValidation;
using System;

namespace EIS.Validations.FluentValidations
{
    public class LocationValidator : AbstractValidator<Locations>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public LocationValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
            RuleFor(x => x.LocationName)
                .NotEmpty().WithMessage("Please enter name for location")
                .Must(Unique).WithMessage("Location already exists.");
        }

        public bool Unique(Locations obj, string name)
        {
            var location = _repositoryWrapper.Locations.FindByCondition(x => x.LocationName == name);
            if (location == null)
            {
                return true;
            }
            else
            {
                return location.Id == obj.Id;
            }
        }
    }
}
