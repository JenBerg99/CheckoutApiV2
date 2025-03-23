using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CheckoutApiV2.Model
{
    /// <summary>
    /// Represents the DbContext for the checkout system, managing the database connection and the entities
    /// </summary>
    public class CheckoutContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the CheckoutContext with the provided DbContext options
        /// </summary>
        /// <param name="options">The options to configure the context</param>
        public CheckoutContext(DbContextOptions options) : base(options)
        {
        }

        /// <summary>
        /// Parameterless constructor for Testing
        /// </summary>
        public CheckoutContext() { }

        /// <summary>
        /// DbSet representing the orders in the database
        /// </summary>
        public virtual DbSet<Order> Orders { get; set; }

        /// <summary>
        /// DbSet representing the payments in the database
        /// </summary>
        public virtual DbSet<Payment> Payments { get; set; }

        /// <summary>
        /// DbSet representing the articles (products) in the database
        /// </summary>
        public virtual DbSet<Article> Articles { get; set; }

        /// <summary>
        /// DbSet representing the many-to-many relationship between orders and articles in the database
        /// </summary>
        public virtual DbSet<OrderArticles> OrderArticles { get; set; }

        /// <summary>
        /// Configures the relationships and keys for the entities in the model
        /// </summary>
        /// <param name="modelBuilder">The model builder to configure the entity relationships</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuring the composite primary key for the OrderArticles entity using OrderId and ArticleId
            modelBuilder.Entity<OrderArticles>()
                .HasKey(oa => new { oa.OrderId, oa.ArticleId });

            // Configuring the relationship between OrderArticles and Order
            modelBuilder.Entity<OrderArticles>()
                .HasOne(oa => oa.Order)
                .WithMany(o => o.OrderArticles)
                .HasForeignKey(oa => oa.OrderId);

            // Configuring the relationship between OrderArticles and Article
            modelBuilder.Entity<OrderArticles>()
                .HasOne(oa => oa.Article)
                .WithMany(a => a.OrderArticles)
                .HasForeignKey(oa => oa.ArticleId);

            // Calling the base class OnModelCreating method to apply configurations
            base.OnModelCreating(modelBuilder);
        }
    }
}
