using EIS.Entities.Address;
using FluentValidation;

namespace EIS.Validations.FluentValidations
{
    public class EmergencyAddressValidator : AbstractValidator<Emergency>
    {
        public EmergencyAddressValidator()
        {
            RuleFor(x => x.FirstName).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.LastName).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.Address).NotNull();
            RuleFor(x => x.City).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.State).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.Country).Matches("^[a-zA-Z ]*$");
            RuleFor(x => x.PinCode).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.Relation).Matches("^[a-zA-Z ]*$").NotNull();
            RuleFor(x => x.MobileNumber).Matches("^[0-9]*$").NotNull();
            RuleFor(x => x.PhoneNumber).Matches("^[0-9]*$");
        }
    }
}
