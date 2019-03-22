﻿// <auto-generated />
using System;
using BuildTrackerApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BuildTrackerApi.Migrations
{
    [DbContext(typeof(BuildTrackerContext))]
    [Migration("20190321233314_AddBuildUnique")]
    partial class AddBuildUnique
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BuildTrackerApi.Models.Build", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("BuildDate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("BuildNumber")
                        .IsRequired();

                    b.Property<int>("BuildPersonId");

                    b.Property<DateTime>("LastUpdate")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("(getutcdate())");

                    b.Property<string>("Platform")
                        .IsRequired();

                    b.Property<string>("ProductName")
                        .IsRequired();

                    b.Property<int>("Type");

                    b.Property<int>("UpdatePersonId");

                    b.Property<string>("Version")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("BuildPersonId");

                    b.HasIndex("UpdatePersonId");

                    b.HasIndex("ProductName", "Version", "Platform")
                        .IsUnique();

                    b.ToTable("Builds");
                });

            modelBuilder.Entity("BuildTrackerApi.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_Products");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("BuildTrackerApi.Models.ProductDeveloper", b =>
                {
                    b.Property<int>("ProductId");

                    b.Property<int>("DeveloperId");

                    b.HasKey("ProductId", "DeveloperId");

                    b.HasIndex("DeveloperId");

                    b.ToTable("ProductDevelopers");
                });

            modelBuilder.Entity("BuildTrackerApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash")
                        .IsRequired();

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int?>("Role")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(0);

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserName")
                        .IsUnique()
                        .HasName("IX_Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BuildTrackerApi.Models.Build", b =>
                {
                    b.HasOne("BuildTrackerApi.Models.User", "BuildPerson")
                        .WithMany()
                        .HasForeignKey("BuildPersonId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BuildTrackerApi.Models.Product", "Product")
                        .WithMany("Builds")
                        .HasForeignKey("ProductName")
                        .HasPrincipalKey("Name")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("BuildTrackerApi.Models.User", "UpdatePerson")
                        .WithMany()
                        .HasForeignKey("UpdatePersonId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("BuildTrackerApi.Models.ProductDeveloper", b =>
                {
                    b.HasOne("BuildTrackerApi.Models.User", "Developer")
                        .WithMany("ProductDevelopers")
                        .HasForeignKey("DeveloperId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BuildTrackerApi.Models.Product", "Product")
                        .WithMany("ProductDevelopers")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
