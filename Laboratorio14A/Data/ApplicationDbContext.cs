using Laboratorio14A.Models;
using Microsoft.EntityFrameworkCore;

namespace Laboratorio14A.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Detail> Details { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Invoice>()
                .Property(i => i.Total)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Detail>()
                .Property(d => d.Price)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Detail>()
                .Property(d => d.SubTotal)
                .HasPrecision(12, 2);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Customer)
                .WithMany(c => c.Invoices)
                .HasForeignKey(i => i.IdCustomer)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Detail>()
                .HasOne(d => d.Product)
                .WithMany(p => p.Details)
                .HasForeignKey(d => d.IdProduct)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Detail>()
                .HasOne(d => d.Invoice)
                .WithMany(i => i.Details)
                .HasForeignKey(d => d.IdInvoice)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.DocumentNumber)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.InvoiceNumber)
                .IsUnique();
        }
    }
}