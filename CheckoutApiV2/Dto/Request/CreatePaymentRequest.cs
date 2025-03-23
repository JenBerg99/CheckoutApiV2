using CheckoutApiV2.Model;

namespace CheckoutApiV2.Dto.Request
{
    /// <summary>
    /// Represents a request to create a new payment for an order in the checkout system.
    /// </summary>
    public class CreatePaymentRequest
    {
        /// <summary>
        /// The unique identifier of the order associated with this payment.
        /// </summary>
        /// <remarks>
        /// This must be a valid order ID.
        /// </remarks>
        public long OrderId { get; set; }

        /// <summary>
        /// The amount of money to be paid for the order.
        /// </summary>
        /// <remarks>
        /// This must be a positive value representing the amount to be paid.
        /// </remarks>
        public decimal Amount { get; set; }

        /// <summary>
        /// The method of payment used for this transaction.
        /// </summary>
        /// <remarks>
        /// This must be one of the valid payment methods (e.g., CreditCard, DebitCard, etc.).
        /// </remarks>
        public PaymentMethod Method { get; set; }

        /// <summary>
        /// The status of the payment.
        /// </summary>
        /// <remarks>
        /// This can be a valid payment status such as Pending, Success, Failed, or Canceled.
        /// </remarks>
        public PaymentStatus Status { get; set; }
    }
}
