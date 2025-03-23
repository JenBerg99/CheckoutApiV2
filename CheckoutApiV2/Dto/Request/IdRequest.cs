namespace CheckoutApiV2.Dto.Request
{
    /// <summary>
    /// Represents a request containing an ID for identifying a specific resource in the checkout system.
    /// </summary>
    public class IdRequest
    {
        /// <summary>
        /// The unique identifier of the resource.
        /// </summary>
        /// <remarks>
        /// This ID must be a positive value greater than zero.
        /// </remarks>
        public long Id { get; set; }
    }
}
