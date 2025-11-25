using Microsoft.EntityFrameworkCore;
using UniversityPaymentSystem.Domain.Entities;

namespace UniversityPaymentSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        
        public DbSet<Student> Students { get; set; }
        public DbSet<TuitionPayment> TuitionPayments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNo)
                .IsUnique();

            
            modelBuilder.Entity<TuitionPayment>()
                .HasOne(tp => tp.Student) 
                .WithMany(s => s.Payments) 
                .HasForeignKey(tp => tp.StudentId); 
        }
    }
}