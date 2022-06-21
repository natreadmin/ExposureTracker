﻿// <auto-generated />
using ExposureTracker.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ExposureTracker.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ExposureTracker.Models.Insured", b =>
                {
                    b.Property<string>("policyno")
                        .HasColumnType("text");

                    b.Property<string>("benefittype")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("bordereauxfilename")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("bordereauxyear")
                        .HasColumnType("integer");

                    b.Property<string>("cedantcode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("cedingcompany")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("certificate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("clientid")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("currency")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("dateofbirth")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("firstname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("fullName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("gender")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("identifier")
                        .HasColumnType("text");

                    b.Property<string>("lastname")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("middlename")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("mortalityrating")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("plan")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("planeffectivedate")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("reinsurednetamountatrisk")
                        .HasColumnType("numeric");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("sumassured")
                        .HasColumnType("numeric");

                    b.Property<string>("typeofbusiness")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("policyno");

                    b.ToTable("dbLifeData");
                });

            modelBuilder.Entity("ExposureTracker.Models.TranslationTables", b =>
                {
                    b.Property<string>("plancode")
                        .HasColumnType("text");

                    b.Property<string>("benefitcov")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("cedingcompany")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("insuredprod")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("plancode");

                    b.ToTable("dbTranslationTable");
                });
#pragma warning restore 612, 618
        }
    }
}
