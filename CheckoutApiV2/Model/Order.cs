namespace CheckoutApiV2.Model
{
    /// <summary>
    /// Represents an order in the checkout API
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Unique identifier for the order
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The status of the order, defaulting to 'Open'
        /// </summary>
        public OrderStatus Status { get; set; } = OrderStatus.Open;

        /// <summary>
        /// A collection of articles (products) that are part of the order
        /// </summary>
        public ICollection<OrderArticles>? OrderArticles { get; set; }
    }

    /// <summary>
    /// Enum representing possible order statuses
    /// </summary>
    public enum OrderStatus
    {
        Complete = 1,
        Failed = 2,
        Open = 3,
        Active = 4,
        Canceled = 5
    }
}
