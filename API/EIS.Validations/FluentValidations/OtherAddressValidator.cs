using EIS.Entities.Address;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class OtherAddressValidator : AbstractValidator<Other>
    {
        public OtherAddressValidator()
        {
            RuleFor(x => x.FirstName).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.LastName).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Address).NotNull();
            RuleFor(x => x.City).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.State).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.Country).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.PinCode).Length(6).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.PhoneNumber).Length(10,15).Matches("^[0-9]*$");
        }
    }
}
