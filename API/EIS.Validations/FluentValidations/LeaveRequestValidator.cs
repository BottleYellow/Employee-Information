using EIS.Entities.Leave;
using EIS.Repositories.IRepository;
using FluentValidation;
using System;
using System.Linq;

namespace EIS.Validations.FluentValidations
{
    public class LeaveRequestValidator : AbstractValidator<LeaveRequest>
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public LeaveRequestValidator(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }
      
    }

}
