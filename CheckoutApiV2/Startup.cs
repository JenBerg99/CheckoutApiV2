using CheckoutApiV2.Extensions;
using CheckoutApiV2.Interfaces;
using CheckoutApiV2.Middleware;
using CheckoutApiV2.Model;
using CheckoutApiV2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

namespace CheckoutApiV2
{
    /// <summary>
    /// Startup of the Project
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configuration defined in the appsettings
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Constructor of Class Startup
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configure services for Dependency Injection
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //get the host from the EnvironmentVariable due to differences between Docker Compose and AWS
            var host = Environment.GetEnvironmentVariable("DB_HOST") ?? "checkout.database";

            var databaseSettings = Configuration.GetSection("DatabaseConnection");
            var connectionString = $"Host={host};Port={databaseSettings["Port"]};Database={databaseSettings["DatabaseName"]};Username={databaseSettings["Username"]};Password={databaseSettings["Password"]};Include Error Detail=true";

            services.AddDbContext<CheckoutContext>(options =>
                options.UseNpgsql(connectionString));

            //Health check for AWS
            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy("API is working"));

            // Registering transient services
            services.AddTransient<IValidationService, ValidationService>();
            services.AddTransient<IOrderService, OrderService>();
            services.AddTransient<IArticleService, ArticleService>();
            services.AddTransient<IPaymentService, PaymentService>();

            // Register controllers and Swagger for API documentation
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Checkout Api Jens",
                    Version = "v0.2.0",
                    Description = "Checkout API for Example Code"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
                foreach (var filePath in xmlFiles) s.IncludeXmlComments(filePath);
            });

            services.AddSwaggerDocument(doc =>
            {
                doc.PostProcess = d =>
                {
                    d.Info.Title = "Test";
                    d.Info.Version = "0.2.0";
                };
            });

            // Logging configuration for console and debug output
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConsole();
                loggingBuilder.AddDebug();
            });
        }

        /// <summary>
        /// Configure the HTTP request pipeline (middleware)
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Use custom middleware for exception handling
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Enable Swagger UI and apply Migrations automaticly
            //if (env.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            app.ApplyMigrations();
            //}

            // Enable HTTPS redirection
            app.UseHttpsRedirection();

            // Routing middleware
            app.UseRouting();
            app.UseOpenApi();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication2 v1")); ;
            app.UseAuthorization();

            // Map controllers to their routes
            app.UseEndpoints(ep =>
            {
                ep.MapControllers().AllowAnonymous();
                ep.MapHealthChecks("/health").AllowAnonymous();
            });
        }
    }
}
