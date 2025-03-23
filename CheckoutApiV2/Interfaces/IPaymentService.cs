using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Model;

namespace CheckoutApiV2.Interfaces
{
    /// <summary>
    /// Defines the contract for the payment service, providing methods for retrieving and creating payments.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Asynchronously retrieves a payment by its ID.
        /// </summary>
        /// <param name="idRequest">The request containing the ID of the payment to retrieve.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the payment object or null if not found.</returns>
        public Task<Payment?> GetPaymentByIdAsync(IdRequest idRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously creates a new payment based on the provided request.
        /// </summary>
        /// <param name="createPaymentRequest">The request containing the payment details to create.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
        public Task<bool> PostPaymentAsync(CreatePaymentRequest createPaymentRequest, CancellationToken cancellationToken);
    }
}
