using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using TechMoveLogisticSystem.Models;

namespace TechMoveLogisticSystem.Data
{
    // this class is my database bridge - it connects my app to SQL Server
    public class AppDbContext : DbContext
    {
        // constructor - used by EF Core to pass connection settings
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // these represent my tables in the database
        public DbSet<Client> Clients { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // making sure money values are stored properly (no weird rounding issues)
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.Cost)
                .HasPrecision(18, 2);

            // defining relationships clearly 

            // Client to Contracts
            modelBuilder.Entity<Contract>()
                .HasOne(c => c.Client)
                .WithMany(c => c.Contracts)
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Contract to ServiceRequests
            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Contract)
                .WithMany(c => c.ServiceRequests)
                .HasForeignKey(sr => sr.ContractId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}