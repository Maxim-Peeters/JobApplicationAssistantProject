using API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DAL.Data
{
    public class JobApplicationContext : DbContext
    {

        public JobApplicationContext()
        {
        }

        public JobApplicationContext(DbContextOptions<JobApplicationContext> options) : base(options)
        { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Application> Applications { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>().ToTable("Job");
            modelBuilder.Entity<Company>().ToTable("Company");
            modelBuilder.Entity<Application>().ToTable("Application");

            modelBuilder.Entity<Company>()
                .HasMany(c => c.Jobs)
                .WithOne(j => j.Company)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany()
                .HasForeignKey(a => a.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure the Salary property precision
            modelBuilder.Entity<Job>(entity =>
            {
                entity.Property(e => e.Salary)
                      .HasPrecision(18, 4); // Example: precision 18, scale 4
            });
        }

    }
}
