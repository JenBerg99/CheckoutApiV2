namespace CheckoutApiV2.Dto
{
    /// <summary>
    /// Represents an error response with details about the failure.
    /// </summary>
    /// <param name="Message">The error message.</param>
    /// <param name="DetailMessage">Optional additional details about the error.</param>
    /// <param name="StackTrace">Optional stack trace information for debugging purposes.</param>
    public record ErrorResponse(
        string Message,
        string? DetailMessage = null,
        string? StackTrace = null
    );
}
