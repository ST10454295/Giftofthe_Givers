using GiftOfTheGivers.Models;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> Users { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Donor> Donors { get; set; }

        public DbSet<Donation> Donations { get; set; }

        public DbSet<DonationSchedule> DonationSchedules { get; set; }

        public DbSet<TaxCertificate> TaxCertificates { get; set; }

        public DbSet<Volunteer> Volunteers { get; set; }

        public DbSet<ReliefProject> ReliefProjects { get; set; }

        public DbSet<ProjectUpdate> ProjectUpdates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");

            modelBuilder.Entity<Donor>()
                .ToTable("Donors");

            modelBuilder.Entity<Donation>()
                .ToTable("Donations");

            modelBuilder.Entity<DonationSchedule>()
                .ToTable("DonationSchedules");

            modelBuilder.Entity<TaxCertificate>()
                .ToTable("TaxCertificates");

            modelBuilder.Entity<Volunteer>()
                .ToTable("Volunteers");

            modelBuilder.Entity<ReliefProject>()
                .ToTable("ReliefProjects");

            modelBuilder.Entity<ProjectUpdate>()
                .ToTable("ProjectUpdates");

            modelBuilder.Entity<ProjectUpdate>()
                .HasKey(u => u.UpdateId);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Donor)
                .WithOne(d => d.User)
                .HasForeignKey<Donor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donor>()
                .HasMany(d => d.Donations)
                .WithOne(d => d.Donor)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Donation>()
                .HasMany(d => d.DonationSchedules)
                .WithOne(s => s.Donation)
                .HasForeignKey(s => s.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Donation>()
                .HasMany(d => d.TaxCertificates)
                .WithOne(t => t.Donation)
                .HasForeignKey(t => t.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ReliefProjects)
                .WithOne(p => p.CreatedByEmployee)
                .HasForeignKey(p => p.CreatedByEmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ReliefProject>()
                .HasMany(p => p.ProjectUpdates)
                .WithOne(u => u.Project)
                .HasForeignKey(u => u.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.ProjectUpdates)
                .WithOne(u => u.Employee)
                .HasForeignKey(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}