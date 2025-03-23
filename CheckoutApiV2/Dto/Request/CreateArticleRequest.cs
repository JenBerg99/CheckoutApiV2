namespace CheckoutApiV2.Dto.Request
{
    /// <summary>
    /// Represents a request to create a new article (product) in the checkout system.
    /// </summary>
    public class CreateArticleRequest
    {
        /// <summary>
        /// The name of the article (product).
        /// </summary>
        /// <remarks>
        /// This field must not be empty or null.
        /// </remarks>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The price of the article (product).
        /// </summary>
        /// <remarks>
        /// This field represents the monetary value of the article and should be a positive number.
        /// </remarks>
        public decimal Price { get; set; }
    }
}
