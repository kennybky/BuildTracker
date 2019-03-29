using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        public virtual DbSet<Test> Tests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=MAPI-AKTOGUNL;Database=BuildTracker;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var RoleConverter = new ValueConverter<Role, string>(
              v => v.ToString(),
                v => (Role)Enum.Parse(typeof(Role), v)); 

            var BuildTypeConverter = new ValueConverter<BuildType, string>(
              v => v.ToString(),
                  v => (BuildType)Enum.Parse(typeof(BuildType), v));

            var TestTypeConverter = new ValueConverter<TestType, string>(
              v => v.ToString(),
                  v => (TestType)Enum.Parse(typeof(TestType), v));

            var PlatformConverter = new ValueConverter<Platform, string>(
              v => v.ToString(),
                  v => (Platform)Enum.Parse(typeof(Platform), v));


            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.UserName)
                    .HasName("IX_Username")
                    .IsUnique();
                entity.Property(e => e.UserName)
                    .IsRequired();

                entity.Property(e => e.PasswordHash)
                    .IsRequired();

                entity.Property(e => e.Role).HasConversion(RoleConverter);

                entity.Property(e => e.Role).HasDefaultValue(Role.USER);

                entity.Property(e => e.AccountConfirmed).HasDefaultValue(false).IsRequired();

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
                entity.HasOne(e => e.Product).WithMany(p => p.Builds).HasPrincipalKey(e => e.Name).HasForeignKey(e => e.ProductName).IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

                entity.Property(e => e.Platform).HasConversion(PlatformConverter);
                entity.Property(e => e.Type).HasConversion(BuildTypeConverter);

                entity.HasIndex(e => new { e.ProductName, e.Version, e.Platform }).IsUnique();

                entity.HasOne(e => e.BuildPerson).WithMany(u => u.Builds).IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.UpdatePerson).WithMany(u => u.BuildUpdates).IsRequired().OnDelete(DeleteBehavior.Restrict);


            });

            modelBuilder.Entity<ProductDeveloper>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.DeveloperId });

                entity.HasOne(d => d.Developer)
                    .WithMany(p => p.ProductDevelopers);

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductDevelopers);
            });

            modelBuilder.Entity<Test>(entity =>
            {
                entity.Property(e => e.Platform).HasConversion(PlatformConverter);
                entity.Property(e => e.Type).HasConversion(TestTypeConverter);
                entity.HasOne(t => t.Build).WithMany(b => b.Tests).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(t => t.TestPerson).WithMany(u=> u.Tests).IsRequired().OnDelete(DeleteBehavior.Restrict);
                entity.Property(t => t.TestDate).HasDefaultValueSql("(getutcdate())");

                entity.Property(t=> t.Comments).HasColumnType("text");
            });
        }
    }
}
