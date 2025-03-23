using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Model;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutApiV2.Controllers
{

    /// <summary>
    /// Controller for managing payments. Provides endpoints for retrieving and creating payments.
    /// </summary>
    [ApiController]
    [Route("payment")]
    public class PaymentController : ControllerBase
    {
        private readonly ILogger<PaymentController> _logger;
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PaymentController"/> class.
        /// </summary>
        /// <param name="paymentService">The service for handling payment-related operations.</param>
        /// <param name="logger">The logger for logging information and errors.</param>
        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a payment based on the provided payment ID.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the payment ID to retrieve.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `GET /payment?Id=1`  
        /// </remarks>
        /// <response code="200">Returns the requested payment.</response>
        /// <response code="400">Bad request if payment ID is invalid.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>The requested payment wrapped in an OK response.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(Payment), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Get([FromQuery] IdRequest getOrDeleteRequest, CancellationToken cancellationToken = default)
        {
            var response = await _paymentService.GetPaymentByIdAsync(getOrDeleteRequest, cancellationToken);
            return Ok(response);
        }

        /// <summary>
        /// Creates a new payment based on the provided payment request.
        /// </summary>
        /// <param name="paymentRequest">The request containing payment details.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <remarks>
        /// **Sample Request:**  
        /// `POST /payment`  
        /// 
        /// **Payment Statuses:**  
        /// - `1` = Success – Payment was successful.  
        /// - `2` = Pending – Payment is currently pending.  
        /// - `3` = Failed – Payment has failed.  
        /// - `4` = Canceled – Payment was canceled by the user or the system.  
        /// 
        /// **Payment Methods:**  
        /// - `1` = CreditCard – Payment made using a credit card.  
        /// - `2` = DebitCard – Payment made using a debit card.  
        /// - `3` = Cash – Payment made in cash.  
        /// - `4` = BankTransfer – Payment made via a bank transfer.  
        /// - `5` = ApplePay – Payment made using Apple Pay.  
        /// - `6` = GooglePay – Payment made using Google Pay.  
        /// 
        /// **Sample Body:**  
        /// ```json
        /// {
        ///   "orderId": 1,
        ///   "amount": 10,
        ///   "method": 1,
        ///   "status": 1
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">Returns success if the payment is created successfully.</response>
        /// <response code="400">Bad request if payment data is invalid.</response>
        /// <response code="500">Unexpected error occurred.</response>
        /// <returns>A response indicating success or failure of the payment creation process.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] CreatePaymentRequest paymentRequest, CancellationToken cancellationToken = default)
        {
            var response = await _paymentService.PostPaymentAsync(paymentRequest, cancellationToken);
            return Ok(response);
        }
    }
}