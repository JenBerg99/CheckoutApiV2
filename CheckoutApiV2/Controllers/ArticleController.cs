using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto.Response;
using CheckoutApiV2.Dto;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutApiV2.Controllers
{

    /// <summary>
    /// Controller for managing articles. Provides endpoints for retrieving and creating articles.
    /// </summary>
    [ApiController]
    [Route("articles")]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;
        private readonly IArticleService _articleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArticleController"/> class.
        /// </summary>
        /// <param name="logger">The logger for logging information and errors.</param>
        /// <param name="articleService">The service for handling article-related operations.</param>
        public ArticleController(ILogger<ArticleController> logger, IArticleService articleService)
        {
            _logger = logger;
            _articleService = articleService;
        }

        /// <summary>
        /// Retrieves a list of all articles.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `GET /articles`  
        /// </remarks>
        /// <response code="200">Returns a list of articles.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>A list of articles wrapped in an OK response.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetArticleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get(CancellationToken cancellationToken = default)
        {
            var response = await _articleService.GetArticlesAsync(cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Retrieves articles based on the provided article IDs.
        /// </summary>
        /// <param name="multipleIdRequest">The request containing the list of article IDs to fetch.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `GET /articles/getByIds?ids=1
        /// </remarks>
        /// <response code="200">Returns the list of articles matching the provided IDs.</response>
        /// <response code="400">Bad request if no article IDs are provided.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>A list of articles wrapped in an OK response.</returns>
        [Route("getByIds")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<GetArticleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromQuery] MultipleIdRequest multipleIdRequest, CancellationToken cancellationToken = default)
        {
            if (multipleIdRequest?.Ids == null || !multipleIdRequest.Ids.Any())
            {
                return BadRequest("At least one article ID is required.");
            }

            var response = await _articleService.GetArticlesByIdAsync(multipleIdRequest, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Creates multiple articles based on the provided request data.
        /// </summary>
        /// <param name="articlePostRequests">The list of article creation requests.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `POST /articles`  
        /// 
        /// **Sample Body:**  
        /// ```json
        /// [
        ///   {
        ///     "name": "Wasser",
        ///     "price": 3.99
        ///   },
        ///   {
        ///     "name": "Cola",
        ///     "price": 4.99
        ///   },
        ///   {
        ///     "name": "Kaugummi",
        ///     "price": 1.99
        ///   }
        /// ]
        /// ```  
        /// </remarks>
        /// <response code="200">Indicates success of the article creation process.</response>
        /// <response code="400">Bad request if the article creation data is invalid or empty.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>A response indicating success or failure of the creation process.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] List<CreateArticleRequest> articlePostRequests, CancellationToken cancellationToken = default)
        {
            if (articlePostRequests == null || !articlePostRequests.Any())
            {
                return BadRequest("Article creation data cannot be empty.");
            }

            var response = await _articleService.PostMultipleArticleAsync(articlePostRequests, cancellationToken);
            return Ok(response);
        }
    }
}