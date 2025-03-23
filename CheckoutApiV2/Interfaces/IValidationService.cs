using FluentValidation;

namespace CheckoutApiV2.Interfaces
{
    /// <summary>
    /// Defines the contract for the validation service, providing a method to validate requests using FluentValidation.
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Asynchronously validates a request object using a specified validator.
        /// </summary>
        /// <param name="request">The request object to validate.</param>
        /// <param name="validator">The validator to use for the validation process.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests (optional).</param>
        /// <returns>A task that represents the asynchronous operation, indicating the validation process completion.</returns>
        Task ValidateAsync<T>(T request, IValidator<T> validator, CancellationToken cancellationToken = default);
    }
}
