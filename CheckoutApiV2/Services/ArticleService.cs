using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto.Response;
using CheckoutApiV2.Extensions.Validations;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Model;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CheckoutApiV2.Services
{
    /// <summary>
    /// Implementation of the ArticleService
    /// </summary>
    /// <param name="checkoutContext">The DB Context</param>
    /// <param name="validationService">Validation Service to check the Input-Parameter</param>
    /// <param name="logger">Loggger to Log Events</param>
    public class ArticleService(CheckoutContext checkoutContext, IValidationService validationService, ILogger<OrderService> logger) : IArticleService
    {
        private readonly IValidationService _validationService = validationService;
        private readonly CheckoutContext _checkoutContext = checkoutContext;
        private readonly ILogger<OrderService> _logger = logger;

        /// <inheritdoc />
        public async Task<IEnumerable<GetArticleResponse>> GetArticlesAsync(CancellationToken cancellationToken)
        {
            var allArticles = await _checkoutContext
                .Articles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {ArticleCount} Items", allArticles.Count);

            var response = allArticles.Select(a => new GetArticleResponse
            {
                Id = a.Id,
                Name = a.Name,  
                Price = a.Price
            });

            return response;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<GetArticleResponse>> GetArticlesByIdAsync(MultipleIdRequest multipleIdRequest, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(multipleIdRequest, new MultipleIdRequestValidation(), cancellationToken);

            var searchedArticles = await _checkoutContext
                .Articles
                .AsNoTracking()
                .Where(a => multipleIdRequest.Ids.ToHashSet().Contains(a.Id))
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Found {ArticleCount} Article Items", searchedArticles.Count);

            var response = searchedArticles.Select(a => new GetArticleResponse
            {
                Id = a.Id,
                Name = a.Name,
                Price = a.Price
            });

            return response;
        }

        /// <inheritdoc />
        public async Task<bool> PostMultipleArticleAsync(List<CreateArticleRequest> createArticleRequests, CancellationToken cancellationToken)
        {
            //If no Paramter was provided - return false
            if (createArticleRequests.Count == 0)
            {
                _logger.LogInformation($"The List for ArticlePostRequest was Empty!");
                return false;
            }

            // Validate the for each Input-Parameter from the List
            foreach (var articleRequest in createArticleRequests)
            {
                await _validationService.ValidateAsync(articleRequest, new CreateArticleRequestValidation(), cancellationToken);
            }

            //create the new Article
            var newArticles = createArticleRequests.Select(ap => new Article()
            {
                Name = ap.Name,
                Price = ap.Price
            });

            await _checkoutContext.AddRangeAsync(newArticles, cancellationToken);
            return await _checkoutContext.SaveChangesAsync(cancellationToken) > 0;
        }

        /// <inheritdoc />
        public async Task<bool> PostArticleAsync(CreateArticleRequest createArticleRequest, CancellationToken cancellationToken)
        {
            //call PostMultipleArticle and return the response
            return await PostMultipleArticleAsync([createArticleRequest], cancellationToken);
        }
    }
}
