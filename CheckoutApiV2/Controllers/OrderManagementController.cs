using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Model;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutApiV2.Controllers
{

    /// <summary>
    /// Controller for managing orders. Provides endpoints for retrieving, creating, and updating orders.
    /// </summary>
    [ApiController]
    [Route("order")]
    public class OrderManagementController : ControllerBase
    {
        private readonly ILogger<OrderManagementController> _logger;
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderManagementController"/> class.
        /// </summary>
        /// <param name="orderService">The service for handling order-related operations.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public OrderManagementController(IOrderService orderService, ILogger<OrderManagementController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves an order based on the provided ID.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the ID of the order to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `GET /order?Id=1`  
        /// </remarks>
        /// <response code="200">Returns the requested order.</response>
        /// <response code="400">Bad request if ID is invalid.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>The requested order wrapped in an OK response.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromQuery] IdRequest getOrDeleteRequest, CancellationToken cancellationToken = default)
        {
            var response = await _orderService.GetOrderAsnyc(getOrDeleteRequest, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the order ID to update.</param>
        /// <param name="orderStatus">The new status to set for the order.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `PUT /order?Id=1`  
        /// 
        /// **Order Statuses:**  
        /// - `1` = Complete – Order was successfully completed.  
        /// - `2` = Failed – Order processing failed.  
        /// - `3` = Open – Order is open and awaiting further action.  
        /// - `4` = Active – Order is currently being processed.  
        /// - `5` = Canceled – Order was canceled by the user or the system.  
        /// </remarks>
        /// <response code="200">Returns success if the status is updated successfully.</response>
        /// <response code="400">Bad request if ID or status is invalid.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>A response indicating success or failure of the update process.</returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Put([FromQuery] IdRequest getOrDeleteRequest, [FromBody] OrderStatus orderStatus, CancellationToken cancellationToken = default)
        {
            var response = await _orderService.ChangeStatusAsync(getOrDeleteRequest, orderStatus, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Creates a new order based on the provided order request.
        /// </summary>
        /// <param name="createOrderRequest">The request containing order creation data.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `POST /order`  
        /// 
        /// **Sample Body:**  
        /// ```json
        /// {
        ///   "articles": [
        ///     {
        ///       "articleId": 1,
        ///       "amount": 10
        ///     },
        ///     {
        ///       "articleId": 3,
        ///       "amount": 4
        ///     },
        ///     {
        ///       "articleId": 4,
        ///       "amount": 5
        ///     }
        ///   ]
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Returns success if the order is created successfully.</response>
        /// <response code="400">Bad request if the order data is invalid.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>The created order wrapped in an OK response.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] CreateOrderRequest createOrderRequest, CancellationToken cancellationToken = default)
        {
            if (createOrderRequest == null || !createOrderRequest.Articles.Any())
            {
                return BadRequest("Order creation data cannot be empty.");
            }

            var response = await _orderService.PostOrderAsync(createOrderRequest, cancellationToken);
            return Ok(response);
        }
    }
}