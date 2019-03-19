using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BuildTrackerApi.Models
{
    public partial class BuildTrackerContext : DbContext
    {
        public BuildTrackerContext()
        {
        }

        public BuildTrackerContext(DbContextOptions<BuildTrackerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Build> Builds { get; set; }
        public virtual DbSet<ProductDeveloper> ProductDevelopers { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=MAPI-AKTOGUNL;Database=BuildTracker;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName)
                    .HasName("IX_Username")
                    .IsUnique();


                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Role).HasDefaultValue(Role.USER).IsRequired();

                entity.Property(e => e.PasswordSalt)
                    .IsRequired();

            });

            modelBuilder.HasAnnotation("ProductVersion", "2.2.2-servicing-10034");
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasIndex(e => e.Name)
                    .HasName("IX_Products")
                    .IsUnique();
            });

            modelBuilder.Entity<Build>(entity =>
            {
                entity.Property(e => e.BuildDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.LastUpdate).HasDefaultValueSql("(getutcdate())");
                entity.HasOne(e => e.Product).WithMany(p => p.Builds).HasPrincipalKey(e=> e.Name)
                .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BuildPerson).WithMany().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.UpdatePerson).WithMany().OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<ProductDeveloper>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.DeveloperId });

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.ProductDevelopers);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDevelopers);
            });
        }
    }
}
