using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto.Response;
using CheckoutApiV2.Model;

namespace CheckoutApiV2.Interfaces
{
    /// <summary>
    /// Defines the contract for the order service, providing methods for creating, retrieving, updating, and deleting orders.
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Asynchronously creates a new order based on the provided request.
        /// </summary>
        /// <param name="createOrderRequest">The request containing the details of the order to create.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
        public Task<bool> PostOrderAsync(CreateOrderRequest createOrderRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously retrieves an order by its ID.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the ID of the order to retrieve.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing the order response or null if not found.</returns>
        public Task<GetOrderResponse?> GetOrderAsnyc(IdRequest getOrDeleteRequest, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously changes the status of an order.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the ID of the order to update.</param>
        /// <param name="orderStatus">The new status to set for the order.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
        public Task<bool> ChangeStatusAsync(IdRequest getOrDeleteRequest, OrderStatus orderStatus, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously deletes an order by its ID.
        /// </summary>
        /// <param name="getOrDeleteRequest">The request containing the ID of the order to delete.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation, containing a boolean indicating success or failure.</returns>
        public Task<bool> DeleteOrderAsync(IdRequest getOrDeleteRequest, CancellationToken cancellationToken);
    }
}
