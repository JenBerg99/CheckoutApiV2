using CheckoutApiV2.Dto.Request;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating MultipleIdRequest objects using FluentValidation.
    /// </summary>
    public class MultipleIdRequestValidation : AbstractValidator<MultipleIdRequest>
    {
        public MultipleIdRequestValidation()
        {
            // Validation rule for Ids - must not be null or empty
            RuleFor(x => x.Ids).NotNull().NotEmpty().WithMessage("At least one Id is required.");

            // Validation rule for each Id in the Ids list - must be greater than zero
            RuleForEach(x => x.Ids).NotNull().GreaterThan(0).WithMessage("The Id must be greater than zero.");
        }
    }
}