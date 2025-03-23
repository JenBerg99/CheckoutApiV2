namespace CheckoutApiV2.Dto
{
    /// <summary>
    /// Represents the details of an article included in an order, including its ID and the quantity.
    /// </summary>
    public class OrderArticleDto
    {
        /// <summary>
        /// The unique identifier of the article (product).
        /// </summary>
        /// <remarks>
        /// This must be a valid article ID.
        /// </remarks>
        public long ArticleId { get; set; }

        /// <summary>
        /// The quantity of the article (product) included in the order.
        /// </summary>
        /// <remarks>
        /// This must be a non-negative integer and represents the number of units of the article.
        /// </remarks>
        public int Ammount { get; set; }
    }
}
