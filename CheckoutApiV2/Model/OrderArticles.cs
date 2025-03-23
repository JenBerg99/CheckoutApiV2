using System.Text.Json.Serialization;

namespace CheckoutApiV2.Model
{
    /// <summary>
    /// Represents the linking table between orders and articles, containing quantity information
    /// </summary>
    public class OrderArticles
    {
        /// <summary>
        /// The unique identifier for the order
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// The order object (navigation property), ignored during serialization
        /// </summary>
        [JsonIgnore]
        public Order? Order { get; set; }

        /// <summary>
        /// The unique identifier for the article
        /// </summary>
        public long ArticleId { get; set; }

        /// <summary>
        /// The article object (navigation property), ignored during serialization
        /// </summary>
        [JsonIgnore]
        public Article? Article { get; set; }

        /// <summary>
        /// The amount of the article in the order
        /// </summary>
        public int Ammount { get; set; }
    }
}
