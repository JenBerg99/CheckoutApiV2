using CheckoutApiV2.Dto;
using CheckoutApiV2.Dto.Request;
using CheckoutApiV2.Model;
using CheckoutApiV2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Testcontainers.PostgreSql;

namespace CheckoutApiV2.Tests
{
    [SetUpFixture]
    public class TestSetup
    {
        public static PostgreSqlContainer Container;
        public static CheckoutContext Context;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            Container = new PostgreSqlBuilder()
                .WithDatabase("checkoutdb")
                .WithUsername("postgres")
                .WithPassword("password")
                .Build();

            await Container.StartAsync();

            var options = new DbContextOptionsBuilder<CheckoutContext>()
                .UseNpgsql(Container.GetConnectionString())
                .Options;

            Context = new CheckoutContext(options);
            await Context.Database.EnsureCreatedAsync();
        }

        [OneTimeTearDown]
        public async Task GlobalTeardown()
        {
            await Container.DisposeAsync();
            await Context.DisposeAsync();
        }
    }

    [TestFixture]
    public class ArticleServiceIntegrationTests
    {
        private ArticleService _articleService;
        private ValidationService _validationService;

        [SetUp]
        public void SetUp()
        {
            var loggerMock = new Mock<ILogger<OrderService>>();
            var validationLoggerMock = new Mock<ILogger<ValidationService>>();

            _validationService = new ValidationService(validationLoggerMock.Object);
            _articleService = new ArticleService(TestSetup.Context, _validationService, loggerMock.Object);
        }

        [Test]
        public async Task GetArticlesAsync_ShouldReturnEmptyList_WhenNoArticlesExist()
        {
            var result = await _articleService.GetArticlesAsync(CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task PostArticleAsync_ShouldCreateArticle()
        {
            var createArticleRequest = new CreateArticleRequest
            {
                Name = "Test Article",
                Price = 10.00m
            };

            var result = await _articleService.PostArticleAsync(createArticleRequest, CancellationToken.None);

            Assert.That(result, Is.True);

            var articles = await _articleService.GetArticlesAsync(CancellationToken.None);
            Assert.That(articles.Count(), Is.EqualTo(1));
            Assert.That(articles.First().Name, Is.EqualTo("Test Article"));
        }

        [Test]
        public async Task PostMultipleArticleAsync_ShouldReturnFalse_WhenEmptyListProvided()
        {
            var result = await _articleService.PostMultipleArticleAsync(new List<CreateArticleRequest>(), CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostMultipleArticleAsync_ShouldCreateArticles()
        {
            var articles = new List<CreateArticleRequest>
            {
                new CreateArticleRequest { Name = "Article 1", Price = 5.00m },
                new CreateArticleRequest { Name = "Article 2", Price = 15.00m }
            };

            var result = await _articleService.PostMultipleArticleAsync(articles, CancellationToken.None);

            Assert.That(result, Is.True);

            var allArticles = await _articleService.GetArticlesAsync(CancellationToken.None);
            Assert.That(allArticles.Count(), Is.EqualTo(3));
        }
    }

    [TestFixture]
    public class OrderServiceIntegrationTests
    {
        private OrderService _orderService;
        private ArticleService _articleService;
        private ValidationService _validationService;

        [SetUp]
        public void SetUp()
        {
            var loggerMock = new Mock<ILogger<OrderService>>();
            var orderLoggerMock = new Mock<ILogger<OrderService>>();
            var validationLoggerMock = new Mock<ILogger<ValidationService>>();

            _validationService = new ValidationService(validationLoggerMock.Object);
            _articleService = new ArticleService(TestSetup.Context, _validationService, orderLoggerMock.Object);
            _orderService = new OrderService(TestSetup.Context, _articleService, _validationService, loggerMock.Object);
        }

        [Test]
        public async Task GetOrderAsync_ShouldReturnNull_WhenOrderDoesNotExist()
        {
            var result = await _orderService.GetOrderAsnyc(new IdRequest { Id = 999 }, CancellationToken.None);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task ChangeStatusAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var result = await _orderService.ChangeStatusAsync(new IdRequest { Id = 999 }, OrderStatus.Complete, CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostOrderAsync_ShouldReturnFalse_WhenArticleDoesNotExist()
        {
            var createOrderRequest = new CreateOrderRequest
            {
                Articles = new List<OrderArticleDto> { new OrderArticleDto { ArticleId = 999, Ammount = 2 } }
            };

            var result = await _orderService.PostOrderAsync(createOrderRequest, CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostOrderAsync_ShouldReturnFalse_WhenNoArticlesProvided()
        {
            var createOrderRequest = new CreateOrderRequest { Articles = new List<OrderArticleDto>() };

            try
            {
                var result = await _orderService.PostOrderAsync(createOrderRequest, CancellationToken.None);
            }
            catch (FluentValidation.ValidationException ex)
            {
                Assert.That(ex.Message == "Validation failed for CreateOrderRequest.");
                Assert.Pass();                
            }
            
            Assert.Fail();            
        }

        [Test]
        public async Task ChangeStatusAsync_ShouldUpdateStatus_WhenOrderExists()
        {
            var createArticleRequest = new CreateArticleRequest
            {
                Name = "Test Article",
                Price = 10.00m
            };

            await _articleService.PostArticleAsync(createArticleRequest, CancellationToken.None);

            var articles = await _articleService.GetArticlesAsync(CancellationToken.None);

            var createOrderRequest = new CreateOrderRequest
            {
                Articles = articles.Select(a => new OrderArticleDto { ArticleId = a.Id, Ammount = 1 }).ToList()
            };

            await _orderService.PostOrderAsync(createOrderRequest, CancellationToken.None);

            var orderId = TestSetup.Context.Orders.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

            var result = await _orderService.ChangeStatusAsync(new IdRequest { Id = orderId }, OrderStatus.Canceled, CancellationToken.None);

            Assert.That(result, Is.True);

            var updatedOrder = await _orderService.GetOrderAsnyc(new IdRequest { Id = orderId }, CancellationToken.None);

            Assert.That(updatedOrder?.Status, Is.EqualTo(OrderStatus.Canceled));
        }

        [Test]
        public async Task DeleteOrderAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var result = await _orderService.DeleteOrderAsync(new IdRequest { Id = 999 }, CancellationToken.None);

            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteOrderAsync_ShouldRemoveOrder_WhenOrderExists()
        {
            var createArticleRequest = new CreateArticleRequest
            {
                Name = "Test Article",
                Price = 10.00m
            };

            await _articleService.PostArticleAsync(createArticleRequest, CancellationToken.None);

            var articles = await _articleService.GetArticlesAsync(CancellationToken.None);

            var createOrderRequest = new CreateOrderRequest
            {
                Articles = articles.Select(a => new OrderArticleDto { ArticleId = a.Id, Ammount = 1 }).ToList()
            };

            await _orderService.PostOrderAsync(createOrderRequest, CancellationToken.None);

            var orderId = TestSetup.Context.Orders.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();

            var result = await _orderService.DeleteOrderAsync(new IdRequest { Id = orderId }, CancellationToken.None);

            Assert.That(result, Is.True);

            var deletedOrder = await _orderService.GetOrderAsnyc(new IdRequest { Id = orderId }, CancellationToken.None);

            Assert.That(deletedOrder, Is.Null);
        }
    }

    [TestFixture]
    public class PaymentServiceIntegrationTests
    {
        private PaymentService _paymentService;
        private OrderService _orderService;
        private ArticleService _articleService;
        private ValidationService _validationService;

        [SetUp]
        public void SetUp()
        {
            var loggerMock = new Mock<ILogger<PaymentService>>();
            var orderLoggerMock = new Mock<ILogger<OrderService>>();
            var validationLoggerMock = new Mock<ILogger<ValidationService>>();

            _validationService = new ValidationService(validationLoggerMock.Object);
            _articleService = new ArticleService(TestSetup.Context, _validationService, orderLoggerMock.Object);
            _orderService = new OrderService(TestSetup.Context, _articleService, _validationService, orderLoggerMock.Object);
            _paymentService = new PaymentService(TestSetup.Context, _orderService, _validationService, loggerMock.Object);
        }

        [Test]
        public async Task GetPaymentByIdAsync_ShouldReturnNull_WhenPaymentDoesNotExist()
        {
            var result = await _paymentService.GetPaymentByIdAsync(new IdRequest { Id = 999 }, CancellationToken.None);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task PostPaymentAsync_ShouldReturnFalse_WhenOrderDoesNotExist()
        {
            var createPaymentRequest = new CreatePaymentRequest
            {
                OrderId = 999,
                Amount = 20.00m,
                Method = PaymentMethod.CreditCard,
                Status = PaymentStatus.Success
            };

            var result = await _paymentService.PostPaymentAsync(createPaymentRequest, CancellationToken.None);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task PostPaymentAsync_ShouldCreatePayment()
        {
            var createArticleRequest = new CreateArticleRequest { Name = "Test Article", Price = 20.00m };
            await _articleService.PostArticleAsync(createArticleRequest, CancellationToken.None);

            var articles = await _articleService.GetArticlesAsync(CancellationToken.None);

            var createOrderRequest = new CreateOrderRequest
            {
                Articles = articles.Select(a => new OrderArticleDto { ArticleId = a.Id, Ammount = 1 }).ToList()
            };

            await _orderService.PostOrderAsync(createOrderRequest, CancellationToken.None);

            var orderId = TestSetup.Context.Orders.OrderByDescending(x => x.Id).Select(x => x.Id).FirstOrDefault();           

            var createPaymentRequest = new CreatePaymentRequest
            {
                OrderId = orderId,
                Amount = 20.00m,
                Method = PaymentMethod.CreditCard,
                Status = PaymentStatus.Success
            };

            var result = await _paymentService.PostPaymentAsync(createPaymentRequest, CancellationToken.None);
            Assert.That(result, Is.True);

            var payment = await _paymentService.GetPaymentByIdAsync(new IdRequest { Id = 1 }, CancellationToken.None);
            Assert.That(payment, Is.Not.Null);
            Assert.That(payment.Amount, Is.EqualTo(20.00m));
            Assert.That(payment.Method, Is.EqualTo(PaymentMethod.CreditCard));
            Assert.That(payment.Status, Is.EqualTo(PaymentStatus.Success));
        }
    }
}
