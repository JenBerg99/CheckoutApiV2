using CheckoutApiV2.Model;

namespace CheckoutApiV2.Dto.Response
{
    /// <summary>
    /// Represents the response data for a retrieved article.
    /// </summary>
    public class GetArticleResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the article.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the article.
        /// Defaults to an empty string if not provided.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the price of the article.
        /// </summary>
        public decimal Price { get; set; }
    }
}
