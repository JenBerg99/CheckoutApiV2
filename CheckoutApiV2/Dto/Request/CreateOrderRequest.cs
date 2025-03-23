namespace CheckoutApiV2.Dto.Request
{
    /// <summary>
    /// Represents a request to create a new order in the checkout system.
    /// </summary>
    public class CreateOrderRequest
    {
        /// <summary>
        /// A collection of articles (products) included in the order.
        /// </summary>
        /// <remarks>
        /// This property must contain at least one article in the order.
        /// </remarks>
        public IEnumerable<OrderArticleDto> Articles { get; set; } = new List<OrderArticleDto>();
    }
}
