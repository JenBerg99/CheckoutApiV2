using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Extensions.Validations;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Model;
using Microsoft.EntityFrameworkCore;

namespace CheckoutApiV2.Services
{
    /// <summary>
    /// Implementation of the PaymentService
    /// </summary>
    /// <param name="checkoutContext">The DB Context</param>
    /// <param name="orderService">Order Service to get informations about Orders</param>
    /// <param name="validationService">Validation Service to check the Input-Parameter</param>
    /// <param name="logger">Loggger to Log Events</param>
    public class PaymentService(CheckoutContext checkoutContext, IOrderService orderService, IValidationService validationService, ILogger<PaymentService> logger) : IPaymentService
    {
        private readonly CheckoutContext _checkoutContext = checkoutContext;
        private readonly IOrderService _orderService = orderService;
        private readonly IValidationService _validationService = validationService;
        private readonly ILogger<PaymentService> _logger = logger;

        /// <inheritdoc />
        public async Task<Payment?> GetPaymentByIdAsync(IdRequest idRequest, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(idRequest, new IdRequestValidation(), cancellationToken);

            //searching for the payment by the Id from the Parameter 
            var searchedPayment = await _checkoutContext
                .Payments
                .FirstOrDefaultAsync(p => p.Id == idRequest.Id, cancellationToken);

            return searchedPayment;
        }

        /// <inheritdoc />
        public async Task<bool> PostPaymentAsync(CreatePaymentRequest createPaymentRequest, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(createPaymentRequest, new CreatePaymentRequestValidation(), cancellationToken);

            //get the order which is requested by the Parameter
            var orderResponse = await _orderService
                .GetOrderAsnyc(new IdRequest() { Id = createPaymentRequest.OrderId }, cancellationToken);

            //if no order could be found - return false
            if(orderResponse == null)
            {
                _logger.LogWarning("There is no Order with the Id {OrderId}", createPaymentRequest.OrderId);
                return false;
            }

            //create a new payment
            var newPayment = new Payment()
            {
                Amount = createPaymentRequest.Amount,
                Method = createPaymentRequest.Method,
                OrderId = createPaymentRequest.OrderId,
                Status = createPaymentRequest.Status
            };

            await _checkoutContext.Payments.AddAsync(newPayment, cancellationToken);
            return await _checkoutContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
