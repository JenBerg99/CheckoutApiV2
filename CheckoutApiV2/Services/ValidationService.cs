using CheckoutApiV2.Interfaces;
using FluentValidation;

namespace CheckoutApiV2.Services
{
    /// <summary>
    /// Service to Validate different Input Parameter by Fluent Validation
    /// </summary>
    /// <param name="logger">Loggger to Log Events</param>
    public class ValidationService(ILogger<ValidationService> logger) : IValidationService
    {
        private readonly ILogger<ValidationService> _logger = logger;

        /// <inheritdoc />
        public async Task ValidateAsync<T>(T request, IValidator<T> validator, CancellationToken cancellationToken = default)
        {
            //validate the input with the validator
            var result = await validator.ValidateAsync(request, cancellationToken);

            //if an issue was found - Log and throw a new Exception
            if (!result.IsValid)
            {
                _logger.LogError("Validation failed for {Name}: {Errors}", typeof(T).Name, string.Join(", ", result.Errors));
                throw new ValidationException($"Validation failed for {typeof(T).Name}.", result.Errors);
            }
        }
    }
}
