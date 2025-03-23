namespace CheckoutApiV2.Model
{
    /// <summary>
    /// Represents the payment for an order in the checkout system
    /// </summary>
    public class Payment
    {
        /// <summary>
        /// Unique identifier for the payment
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The unique identifier for the order associated with the payment
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// The amount of money being paid in this transaction
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The payment method used for the transaction
        /// </summary>
        public PaymentMethod Method { get; set; }

        /// <summary>
        /// The status of the payment, defaulting to 'Pending'
        /// </summary>
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    }

    /// <summary>
    /// Enum representing possible payment statuses.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Payment was successful.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Payment is currently pending.
        /// </summary>
        Pending = 2,

        /// <summary>
        /// Payment has failed.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Payment was canceled by the user or the system.
        /// </summary>
        Canceled = 4
    }

    /// <summary>
    /// Enum representing possible payment methods.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>
        /// Payment made using a credit card.
        /// </summary>
        CreditCard = 1,

        /// <summary>
        /// Payment made using a debit card.
        /// </summary>
        DebitCard = 2,

        /// <summary>
        /// Payment made in cash.
        /// </summary>
        Cash = 3,

        /// <summary>
        /// Payment made via a bank transfer.
        /// </summary>
        BankTransfer = 4,

        /// <summary>
        /// Payment made using Apple Pay.
        /// </summary>
        ApplePay = 5,

        /// <summary>
        /// Payment made using Google Pay.
        /// </summary>
        GooglePay = 6
    }

}
