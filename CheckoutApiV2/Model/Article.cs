namespace CheckoutApiV2.Model
{
    /// <summary>
    /// Represents an article (product) in the checkout system
    /// </summary>
    public class Article
    {
        /// <summary>
        /// Unique identifier for the article
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of the article
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Price of the article
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// A collection of order articles linking the article to orders
        /// </summary>
        public ICollection<OrderArticles>? OrderArticles { get; set; }
    }
}
