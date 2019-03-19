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
    [Migration("20190313202222_MigrateCodeFirst")]
    partial class MigrateCodeFirst
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

                    b.Property<string>("Version");

                    b.HasKey("Id");

                    b.HasIndex("BuildPersonId");

                    b.HasIndex("ProductName");

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

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired();

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired();

                    b.Property<int>("Role")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(3);

                    b.Property<string>("Username")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasName("IX_Username");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BuildTrackerApi.Models.Build", b =>
                {
                    b.HasOne("BuildTrackerApi.Models.User", "BuildPerson")
                        .WithMany()
                        .HasForeignKey("BuildPersonId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("BuildTrackerApi.Models.Product", "Product")
                        .WithMany("Builds")
                        .HasForeignKey("ProductName")
                        .HasPrincipalKey("Name")
                        .OnDelete(DeleteBehavior.Cascade);
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
