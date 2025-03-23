using CheckoutApiV2.Dto;
using FluentValidation;
using System.Net;
using System.Net.Mime;
using System.Text.Json;

namespace CheckoutApiV2.Middleware
{
    /// <summary>
    /// Middelware to have an Exception Handling in one Class
    /// </summary>
    /// <param name="next">The Request Object</param>
    /// <param name="logger">Loggger to Log Events</param>
    public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                //log all incoming Requests
                var request = context.Request;
                var route = request.Path;
                var queryString = request.QueryString.ToString();
                var method = request.Method;

                var parameters = string.Join(", ", request.Query.Select(q => $"{q.Key}: {q.Value}"));

                _logger.LogInformation("Incoming request: {Method} {Route} with parameters: {Parameters}", method, route, parameters);

                await _next(context);
            }
            catch (Exception exception)
            {
               await HandleExceptions(context, exception);
            }
        }

        /// <summary>
        /// Handles all Exceptions from this Project
        /// </summary>
        /// <param name="context">The content from the Request</param>
        /// <param name="exception">The Exception which was thrown</param>
        /// <returns></returns>
        private async Task HandleExceptions(HttpContext context, Exception exception)
        {
            //set some Options for the Json Serializer
            var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            context.Response.ContentType = MediaTypeNames.Application.Json;
            var response = context.Response;

            //create the error Response
            var errorResponse = new ErrorResponse("Internal server error!");
            switch (exception)
            {
                case ValidationException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = errorResponse with { Message = ex.Message, DetailMessage = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)) };
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = errorResponse with
                    {
                        DetailMessage = exception.Message,
                        StackTrace = exception.StackTrace
                    };
                    break;
            }

            //Log the Exceptions
            _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);
            var result = JsonSerializer.Serialize(errorResponse, serializeOptions);
            await context.Response.WriteAsync(result);
        }
    }
}
