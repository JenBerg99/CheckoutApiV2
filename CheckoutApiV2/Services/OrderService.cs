using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Dto.Response;
using CheckoutApiV2.Extensions.Validations;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Model;
using Microsoft.EntityFrameworkCore;

namespace CheckoutApiV2.Services
{
    /// <summary>
    /// Implementation of the OrderService
    /// </summary>
    /// <param name="checkoutContext">The DB Context</param>
    /// <param name="articleService">Article Service to get informations about Articles</param>
    /// <param name="validationService">Validation Service to check the Input-Parameter</param>
    /// <param name="logger">Loggger to Log Events</param>
    public class OrderService(CheckoutContext checkoutContext, IArticleService articleService, IValidationService validationService, ILogger<OrderService> logger) : IOrderService
    {
        private readonly CheckoutContext _checkoutContext = checkoutContext;
        private readonly IArticleService _articleService = articleService;
        private readonly IValidationService _validationService = validationService;
        private readonly ILogger<OrderService> _logger = logger;

        /// <inheritdoc />
        public async Task<GetOrderResponse?> GetOrderAsnyc(IdRequest idRequest, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(idRequest, new IdRequestValidation(), cancellationToken);

            //searching for the order by the Id from the Parameter
            var searchedOrderItem = await _checkoutContext
                .Orders
                .AsNoTracking()
                .Include(o => o.OrderArticles)                
                .FirstOrDefaultAsync(o => o.Id == idRequest.Id, cancellationToken);

            //if no order was found - return null
            if (searchedOrderItem == null)
            {
                _logger.LogInformation("Can not find an Item with the OrderId {OrderId}", idRequest.Id);
                return null;
            }
            
            _logger.LogInformation("Found an Item with the OrderId {OrderId}", idRequest.Id);

            //check if the order has any Articles - if not return without Article Information
            if(searchedOrderItem.OrderArticles == null || searchedOrderItem.OrderArticles.Count == 0)
                return new GetOrderResponse()
                {
                    Id = searchedOrderItem.Id,
                    Status = searchedOrderItem.Status,
                    Articles = [],
                    TotalCost = 0
                };


            //get the Article informations
            var articlesInDatabase = await _articleService.GetArticlesByIdAsync(
                new MultipleIdRequest
                {
                    Ids = searchedOrderItem.OrderArticles.Select(x => x.ArticleId)
                },
                cancellationToken
                );

            var totalCost = searchedOrderItem.OrderArticles
                .Join(
                    articlesInDatabase,
                    quantity => quantity.ArticleId,
                    price => price.Id,
                    (quantity, price) => quantity.Ammount * price.Price
                )
                .Sum();

            //create a response based on the Orders Items
            var response = new GetOrderResponse()
            {
                Id = searchedOrderItem.Id,
                Status = searchedOrderItem.Status,
                Articles = searchedOrderItem.OrderArticles ?? [],
                TotalCost = Math.Round(totalCost, 2)
            };

            return response;
        }

        /// <inheritdoc />
        public async Task<bool> ChangeStatusAsync(IdRequest idRequest, OrderStatus orderStatus, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(idRequest, new IdRequestValidation(), cancellationToken);
            await _validationService.ValidateAsync(orderStatus, new OrderStatusEnumValidation(), cancellationToken);

            //searching for the order by the Id from the Parameter
            var searchedOrderItem = await _checkoutContext
                .Orders                                
                .FirstOrDefaultAsync(o => o.Id == idRequest.Id, cancellationToken);            

            //if no Order was found - return false
            if (searchedOrderItem == null)
                return false;

            //change Status
            searchedOrderItem.Status = orderStatus;            
            return await _checkoutContext.SaveChangesAsync(cancellationToken) > 0;            
        }

        /// <inheritdoc />
        public async Task<bool> PostOrderAsync(CreateOrderRequest createOrderRequest, CancellationToken cancellationToken)
        {
            // Validate the Input-Parameter
            await _validationService.ValidateAsync(createOrderRequest, new CreateOrderRequestValidation(), cancellationToken);

            //get all the ArticleIds from the Request
            var allArticleIds = createOrderRequest
                .Articles
                .Select(x => x.ArticleId)
                .ToHashSet();

            //check which requested Article we can find in the Database
            var articlesInDatabase = await _articleService.GetArticlesByIdAsync(new MultipleIdRequest() { Ids = allArticleIds }, cancellationToken);

            //if we found zero Orders - return false
            if (articlesInDatabase.Count() == 0)
            {
                _logger.LogWarning("No Articles found for the provided Ids: {ArticleIds}", string.Join(", ", allArticleIds));
                return false;
            }

            //create new Order
            var newOrderItem = new Order();            
            await _checkoutContext.Orders.AddAsync(newOrderItem, cancellationToken);
            await _checkoutContext.SaveChangesAsync(cancellationToken);

            //create a dictionary with the Articles we found in the Database and the requested Ammount from the Parameter
            var orderArticleDict = createOrderRequest
                .Articles
                .Where(a => articlesInDatabase.Select(ad => ad.Id).Contains(a.ArticleId))
                .ToDictionary(a => a.ArticleId, a => a.Ammount);

            //create the combination-Items from Order and Article
            var orderArticles = articlesInDatabase.Select(at => new OrderArticles()
            {
                OrderId = newOrderItem.Id,                
                ArticleId = at.Id,
                Ammount = orderArticleDict[at.Id]
            });

            await _checkoutContext.OrderArticles.AddRangeAsync(orderArticles, cancellationToken);
            return await _checkoutContext.SaveChangesAsync(cancellationToken) > 0;            
        }

        /// <inheritdoc />
        public async Task<bool> DeleteOrderAsync(IdRequest idRequest, CancellationToken cancellationToken)
        {
            //validate the Input-Parameter
            await _validationService.ValidateAsync(idRequest, new IdRequestValidation(), cancellationToken);

            //searching for the requested Order
            var searchedOrderItem = await _checkoutContext
                .Orders
                .FirstOrDefaultAsync(o => o.Id == idRequest.Id, cancellationToken);

            //if no Order exist - return false
            if (searchedOrderItem == null)
                return false;

            //remove order
            _checkoutContext.Orders.Remove(searchedOrderItem);
            return await _checkoutContext.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
