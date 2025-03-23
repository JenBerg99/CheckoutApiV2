using CheckoutApiV2.Dto.Request;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating CreatePaymentRequest objects using FluentValidation.
    /// </summary>
    public class CreatePaymentRequestValidation : AbstractValidator<CreatePaymentRequest>
    {
        public CreatePaymentRequestValidation()
        {
            // Validation rule for OrderId - must be greater than zero
            RuleFor(x => x.OrderId).NotNull().NotEmpty().GreaterThan(0).WithMessage("The OrderId must be greater than zero.");

            // Validation rule for Amount - must be greater than or equal to zero
            RuleFor(x => x.Amount).NotNull().NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Amount cannot be negative.");

            // Validation rule for Method - must be a valid enum value
            RuleFor(x => x.Method).NotNull().NotEmpty().IsInEnum().WithMessage("Method must be a valid one and cannot be empty.");

            // Validation rule for Status - must be a valid enum value
            RuleFor(x => x.Status).NotNull().NotEmpty().IsInEnum().WithMessage("Status must be a valid one and cannot be empty.");
        }
    }
}
