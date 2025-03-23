using CheckoutApiV2.Dto;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating OrderArticleDto objects using FluentValidation.
    /// </summary>
    public class OrderArticleDtoValidation : AbstractValidator<OrderArticleDto>
    {
        public OrderArticleDtoValidation()
        {
            // Validation rule for ArticleId - must be greater than zero
            RuleFor(x => x.ArticleId).NotNull().NotEmpty().GreaterThan(0).WithMessage("ArticleId must be greater than zero.");

            // Validation rule for Ammount - must be greater than or equal to zero
            RuleFor(x => x.Ammount).NotNull().NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");
        }
    }
}
