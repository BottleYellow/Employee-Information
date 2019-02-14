using EIS.Entities.Address;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class CurrentAddressValidator : AbstractValidator<Current>
    {
        public CurrentAddressValidator()
        {
            RuleFor(x => x.Address).NotNull();
            RuleFor(x => x.City).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.State).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Country).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.PinCode).Matches("^[0-9]*$").NotNull();
        }
    }
}
