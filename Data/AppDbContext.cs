using Microsoft.EntityFrameworkCore;
using VehicleManagementAPI.Models;

namespace VehicleManagementAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<VendorItem> VendorItems { get; set; }
        public DbSet<Part> Parts { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
        public DbSet<PurchaseItem> PurchaseItems { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesItem> SalesItems { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships if necessary, though Data Annotations handled most
            
            // Example of configuring decimal precision
            modelBuilder.Entity<Part>()
                .Property(p => p.Part_Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<VendorItem>()
                .Property(v => v.Part_Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseInvoice>()
                .Property(p => p.Total_Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<PurchaseItem>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesInvoice>()
                .Property(p => p.Total_Amount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<SalesItem>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);
        }
    }
}
