using CheckoutApiV2.Model;

namespace CheckoutApiV2.Dto.Response
{
    /// <summary>
    /// Represents the response data for retrieving an order from the checkout system.
    /// </summary>
    public class GetOrderResponse
    {
        /// <summary>
        /// The unique identifier of the order.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// A collection of articles (products) included in the order.
        /// </summary>
        public IEnumerable<OrderArticles> Articles { get; set; } = new List<OrderArticles>();

        /// <summary>
        /// The current status of the order.
        /// </summary>
        public OrderStatus Status { get; set; }

        /// <summary>
        /// The total cost of the order, including all articles.
        /// </summary>
        public decimal TotalCost { get; set; }
    }
}
