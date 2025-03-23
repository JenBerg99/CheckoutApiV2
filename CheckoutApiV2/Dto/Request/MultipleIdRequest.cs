namespace CheckoutApiV2.Dto.Request
{
    /// <summary>
    /// Represents a request containing multiple IDs for identifying multiple resources in the checkout system.
    /// </summary>
    public class MultipleIdRequest
    {
        /// <summary>
        /// A collection of unique identifiers for the resources.
        /// </summary>
        /// <remarks>
        /// This property must contain at least one ID and all IDs must be positive values greater than zero.
        /// </remarks>
        public IEnumerable<long> Ids { get; set; } = new List<long>();
    }
}
