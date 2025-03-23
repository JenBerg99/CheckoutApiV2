using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto.Response;

/// <summary>
/// Defines the contract for the article service, providing methods for retrieving and creating articles.
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// Asynchronously retrieves all articles.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable collection of articles.</returns>
    public Task<IEnumerable<GetArticleResponse>> GetArticlesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously retrieves articles by their IDs.
    /// </summary>
    /// <param name="multipleIdRequest">A request object containing multiple article IDs to fetch.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing an enumerable collection of articles.</returns>
    public Task<IEnumerable<GetArticleResponse>> GetArticlesByIdAsync(MultipleIdRequest multipleIdRequest, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates multiple articles based on the provided requests.
    /// </summary>
    /// <param name="articlePostRequests">A list of requests to create articles.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
    public Task<bool> PostMultipleArticleAsync(List<CreateArticleRequest> articlePostRequests, CancellationToken cancellationToken);

    /// <summary>
    /// Asynchronously creates a single article based on the provided request.
    /// </summary>
    /// <param name="articlePostRequest">A request to create a single article.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
    public Task<bool> PostArticleAsync(CreateArticleRequest articlePostRequest, CancellationToken cancellationToken);
}
