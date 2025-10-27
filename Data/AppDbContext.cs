using Microsoft.EntityFrameworkCore;
using Daraz_CloneAgain.Models;

namespace Daraz_CloneAgain.Data
{
    public class AppDbContext : DbContext
    {
        // Add this constructor
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<ProductColors> ProductColors { get; set; }
        public DbSet<ProductStorageOptions> ProductStorageOptions { get; set; }
        public DbSet<ProductDelivery> ProductDelivery { get; set; }
        public DbSet<ProductWarranty> ProductWarranty { get; set; }
        public DbSet<ProductSeller> ProductSeller { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .Property(p => p.CurrentPrice)
                .HasColumnName("current_price");

            modelBuilder.Entity<Product>()
                .Property(p => p.OriginalPrice)
                .HasColumnName("original_price");

            modelBuilder.Entity<Product>()
                .Property(p => p.ColorFamily)
                .HasColumnName("color_family");

            modelBuilder.Entity<Product>()
                .Property(p => p.StorageCapacity)
                .HasColumnName("storage_capacity");

            modelBuilder.Entity<ProductImages>()
                .Property(i => i.ImageUrl)
                .HasColumnName("image_url");

            modelBuilder.Entity<ProductColors>()
                .Property(c => c.ColorName)
                .HasColumnName("color_name");
            modelBuilder.Entity<ProductColors>()
                .Property(c => c.ColorCode)
                .HasColumnName("color_code");

            modelBuilder.Entity<ProductStorageOptions>()
                .Property(s => s.StorageOption)
                .HasColumnName("storage_option");

            modelBuilder.Entity<ProductDelivery>()
                .Property(d => d.StandardDeliveryText)
                .HasColumnName("standard_delivery_text");
            modelBuilder.Entity<ProductDelivery>()
                .Property(d => d.StandardDeliveryTime)
                .HasColumnName("standard_delivery_time");
            modelBuilder.Entity<ProductDelivery>()
                .Property(d => d.StandardDeliveryPrice)
                .HasColumnName("standard_delivery_price");
            modelBuilder.Entity<ProductDelivery>()
                .Property(d => d.CashOnDelivery)
                .HasColumnName("cash_on_delivery");

            modelBuilder.Entity<ProductWarranty>()
                .Property(w => w.EasyReturn)
                .HasColumnName("easy_return");
            modelBuilder.Entity<ProductWarranty>()
                .Property(w => w.BrandWarranty)
                .HasColumnName("brand_warranty");

            modelBuilder.Entity<ProductSeller>()
                .Property(s => s.SellerName)
                .HasColumnName("seller_name");
            modelBuilder.Entity<ProductSeller>()
                .Property(s => s.SellerType)
                .HasColumnName("seller_type");
            modelBuilder.Entity<ProductSeller>()
                .Property(s => s.ChatAvailable)
                .HasColumnName("chat_available");

            base.OnModelCreating(modelBuilder);
        }
    }
}
