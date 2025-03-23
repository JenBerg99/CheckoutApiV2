using CheckoutApiV2.Dto.Request;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating CreateArticleRequest objects using FluentValidation.
    /// </summary>
    public class CreateArticleRequestValidation : AbstractValidator<CreateArticleRequest>
    {
        public CreateArticleRequestValidation()
        {
            // Validation rule for the Name field - must not be null or empty
            RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Name is required.");

            // Validation rule for the Price field - must be greater than zero
            RuleFor(x => x.Price).NotNull().NotEmpty().GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
