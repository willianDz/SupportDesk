﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SupportDesk.Persistence.SupportDesk;

#nullable disable

namespace SupportDesk.Persistence.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("SupportDesk.Domain.Entities.Gender", b =>
                {
                    b.Property<int>("GenderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("GenderId"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(5)
                        .HasColumnType("character varying(5)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("GenderId");

                    b.ToTable("Genders", (string)null);

                    b.HasData(
                        new
                        {
                            GenderId = 1,
                            Abbreviation = "M",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9440),
                            Description = "Masculino",
                            IsActive = true
                        },
                        new
                        {
                            GenderId = 2,
                            Abbreviation = "F",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9443),
                            Description = "Femenino",
                            IsActive = true
                        });
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Request", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime?>("ApprovalRejectionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("character varying(800)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RequestStatusId")
                        .HasColumnType("integer");

                    b.Property<int>("RequestTypeId")
                        .HasColumnType("integer");

                    b.Property<Guid?>("ReviewerUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("StartReviewDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ZoneId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RequestStatusId");

                    b.HasIndex("RequestTypeId");

                    b.HasIndex("ReviewerUserId");

                    b.HasIndex("ZoneId");

                    b.ToTable("Requests", (string)null);
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestDocument", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DocumentUrl")
                        .IsRequired()
                        .HasMaxLength(800)
                        .HasColumnType("character varying(800)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RequestId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RequestId");

                    b.ToTable("RequestDocuments", (string)null);
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestStatus", b =>
                {
                    b.Property<int>("RequestStatusId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RequestStatusId"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("RequestStatusId");

                    b.ToTable("RequestStatuses", (string)null);

                    b.HasData(
                        new
                        {
                            RequestStatusId = 1,
                            Abbreviation = "NUE",
                            Description = "Nuevo"
                        },
                        new
                        {
                            RequestStatusId = 2,
                            Abbreviation = "ENR",
                            Description = "En Revision"
                        },
                        new
                        {
                            RequestStatusId = 3,
                            Abbreviation = "REC",
                            Description = "Rechazado"
                        },
                        new
                        {
                            RequestStatusId = 4,
                            Abbreviation = "APR",
                            Description = "Aprobado"
                        });
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestType", b =>
                {
                    b.Property<int>("RequestTypeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RequestTypeId"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("RequestTypeId");

                    b.ToTable("RequestTypes", (string)null);

                    b.HasData(
                        new
                        {
                            RequestTypeId = 1,
                            Abbreviation = "TI",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6340),
                            Description = "Tecnología de la información",
                            IsActive = true
                        },
                        new
                        {
                            RequestTypeId = 2,
                            Abbreviation = "RRHH",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6349),
                            Description = "Recursos Humanos",
                            IsActive = true
                        },
                        new
                        {
                            RequestTypeId = 3,
                            Abbreviation = "LEGA",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6350),
                            Description = "Legal",
                            IsActive = true
                        },
                        new
                        {
                            RequestTypeId = 4,
                            Abbreviation = "MANT",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6351),
                            Description = "Mantenimiento",
                            IsActive = true
                        },
                        new
                        {
                            RequestTypeId = 5,
                            Abbreviation = "MKT",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6352),
                            Description = "Mercadeo",
                            IsActive = true
                        });
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.TwoFactorAuthToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpirationDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("TwoFactorAuthToken");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int?>("GenderId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PhotoUrl")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GenderId");

                    b.ToTable("Users", (string)null);
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.UserRequestType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("RequestTypeId")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RequestTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRequestTypes");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.UserZone", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.Property<int>("ZoneId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("ZoneId");

                    b.ToTable("UserZones");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Zone", b =>
                {
                    b.Property<int>("ZoneId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("ZoneId"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("CreatedBy")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<string>("LastModifiedBy")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModifiedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("ZoneId");

                    b.ToTable("Zones", (string)null);

                    b.HasData(
                        new
                        {
                            ZoneId = 1,
                            Abbreviation = "ZN",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1916),
                            Description = "Zona Norte",
                            IsActive = true
                        },
                        new
                        {
                            ZoneId = 2,
                            Abbreviation = "ZS",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1920),
                            Description = "Zona Sur",
                            IsActive = true
                        },
                        new
                        {
                            ZoneId = 3,
                            Abbreviation = "ZC",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1922),
                            Description = "Zona Centro",
                            IsActive = true
                        },
                        new
                        {
                            ZoneId = 4,
                            Abbreviation = "ZO",
                            CreatedDate = new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1923),
                            Description = "Zona Occidente",
                            IsActive = true
                        });
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Request", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.RequestStatus", "RequestStatus")
                        .WithMany("Requests")
                        .HasForeignKey("RequestStatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SupportDesk.Domain.Entities.RequestType", "RequestType")
                        .WithMany("Requests")
                        .HasForeignKey("RequestTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SupportDesk.Domain.Entities.User", "ReviewerUser")
                        .WithMany("ReviewedRequests")
                        .HasForeignKey("ReviewerUserId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("SupportDesk.Domain.Entities.Zone", "Zone")
                        .WithMany("Requests")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("RequestStatus");

                    b.Navigation("RequestType");

                    b.Navigation("ReviewerUser");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestDocument", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.Request", "Request")
                        .WithMany("RequestDocuments")
                        .HasForeignKey("RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Request");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.TwoFactorAuthToken", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.User", "User")
                        .WithMany("TwoFactorAuthTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.User", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.Gender", "Gender")
                        .WithMany("Users")
                        .HasForeignKey("GenderId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.Navigation("Gender");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.UserRequestType", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.RequestType", "RequestType")
                        .WithMany("UserRequestTypes")
                        .HasForeignKey("RequestTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("SupportDesk.Domain.Entities.User", "User")
                        .WithMany("UserRequestTypes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("RequestType");

                    b.Navigation("User");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.UserZone", b =>
                {
                    b.HasOne("SupportDesk.Domain.Entities.User", "User")
                        .WithMany("UserZones")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SupportDesk.Domain.Entities.Zone", "Zone")
                        .WithMany("UserZones")
                        .HasForeignKey("ZoneId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("Zone");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Gender", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Request", b =>
                {
                    b.Navigation("RequestDocuments");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestStatus", b =>
                {
                    b.Navigation("Requests");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.RequestType", b =>
                {
                    b.Navigation("Requests");

                    b.Navigation("UserRequestTypes");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.User", b =>
                {
                    b.Navigation("ReviewedRequests");

                    b.Navigation("TwoFactorAuthTokens");

                    b.Navigation("UserRequestTypes");

                    b.Navigation("UserZones");
                });

            modelBuilder.Entity("SupportDesk.Domain.Entities.Zone", b =>
                {
                    b.Navigation("Requests");

                    b.Navigation("UserZones");
                });
#pragma warning restore 612, 618
        }
    }
}
