using CheckoutApiV2.Model;
using Microsoft.EntityFrameworkCore;

namespace CheckoutApiV2.Extensions
{
    /// <summary>
    /// Extension methods for managing database migrations in the application.
    /// </summary>
    public static class MigrationExtensions
    {
        /// <summary>
        /// Applies pending database migrations to ensure the database is up to date.
        /// </summary>
        /// <param name="app">The application builder used to create the service scope.</param>
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            // Create a scope for the application services
            using IServiceScope scope = app.ApplicationServices.CreateScope();

            // Get the CheckoutContext from the service provider
            using CheckoutContext dbContext =
                scope.ServiceProvider.GetRequiredService<CheckoutContext>();

            // Apply any pending migrations to the database
            dbContext.Database.Migrate();
        }
    }
}
