using CheckoutApiV2.Model;
using FluentValidation;

namespace CheckoutApiV2.Extensions.Validations
{
    /// <summary>
    /// Validator for validating OrderStatusDto objects using FluentValidation.
    /// Ensures that the provided order status is within the valid enum range.
    /// </summary>
    public class OrderStatusEnumValidation : AbstractValidator<OrderStatus>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OrderStatusEnumValidation"/> class.
        /// Defines validation rules for the OrderStatusDto.
        /// </summary>
        public OrderStatusEnumValidation()
        {
            // Rule to ensure that the Status value is a valid value from the OrderStatus enum.
            RuleFor(x => x).IsInEnum().WithMessage("Invalid order status.");
        }
    }
}