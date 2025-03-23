using CheckoutApiV2.Dto.Request;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating IdRequest objects using FluentValidation.
    /// </summary>
    public class IdRequestValidation : AbstractValidator<IdRequest>
    {
        public IdRequestValidation()
        {
            // Validation rule for Id field - must be greater than zero
            RuleFor(x => x.Id).NotNull().NotEmpty().GreaterThan(0).WithMessage("The Id must be greater than zero.");
        }
    }
}
