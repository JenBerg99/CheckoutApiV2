using CheckoutApiV2.Dto.Request;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating CreateOrderRequest objects using FluentValidation.
    /// </summary>
    public class CreateOrderRequestValidation : AbstractValidator<CreateOrderRequest>
    {
        public CreateOrderRequestValidation()
        {
            // Validation rule for Articles field - must not be null or empty
            RuleFor(x => x.Articles).NotNull().NotEmpty().WithMessage("At least one Article is required.");

            // Validation rule for each article in the Articles list
            RuleForEach(x => x.Articles).SetValidator(new OrderArticleDtoValidation());
        }
    }
}
