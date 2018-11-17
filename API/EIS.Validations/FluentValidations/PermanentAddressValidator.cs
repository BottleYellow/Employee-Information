using EIS.Entities.Address;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace EIS.Validations.FluentValidations
{
    public class PermanentAddressValidator : AbstractValidator<Permanent>
    {
        public PermanentAddressValidator()
        {
            RuleFor(x => x.Address).NotNull();
            RuleFor(x => x.City).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.State).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.Country).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.PinCode).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$");
        }
    }
}
