using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConsolidatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Genders",
                columns: table => new
                {
                    GenderId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genders", x => x.GenderId);
                });

            migrationBuilder.CreateTable(
                name: "RequestStatuses",
                columns: table => new
                {
                    RequestStatusId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestStatuses", x => x.RequestStatusId);
                });

            migrationBuilder.CreateTable(
                name: "RequestTypes",
                columns: table => new
                {
                    RequestTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTypes", x => x.RequestTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Abbreviation = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.ZoneId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GenderId = table.Column<int>(type: "integer", nullable: true),
                    PhotoUrl = table.Column<string>(type: "text", nullable: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false),
                    IsSupervisor = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    TwoFactorCode = table.Column<string>(type: "text", nullable: true),
                    TwoFactorCodeExpiration = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Genders_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Genders",
                        principalColumn: "GenderId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Requests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ReviewerUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestTypeId = table.Column<int>(type: "integer", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    Comments = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    StartReviewDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ApprovalRejectionDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestStatusId = table.Column<int>(type: "integer", nullable: false),
                    ReviewerUserComments = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requests_RequestStatuses_RequestStatusId",
                        column: x => x.RequestStatusId,
                        principalTable: "RequestStatuses",
                        principalColumn: "RequestStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_RequestTypes_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestTypes",
                        principalColumn: "RequestTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Users_ReviewerUserId",
                        column: x => x.ReviewerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Requests_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TwoFactorAuthToken",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwoFactorAuthToken", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TwoFactorAuthToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRequestTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestTypeId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRequestTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRequestTypes_RequestTypes_RequestTypeId",
                        column: x => x.RequestTypeId,
                        principalTable: "RequestTypes",
                        principalColumn: "RequestTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRequestTypes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ZoneId = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserZones_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserZones_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "ZoneId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RequestId = table.Column<int>(type: "integer", nullable: false),
                    DocumentUrl = table.Column<string>(type: "character varying(800)", maxLength: 800, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestDocuments_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Genders",
                columns: new[] { "GenderId", "Abbreviation", "CreatedBy", "CreatedDate", "Description", "IsActive", "LastModifiedBy", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, "M", null, new DateTime(2024, 8, 26, 0, 23, 37, 889, DateTimeKind.Utc).AddTicks(8914), "Masculino", true, null, null },
                    { 2, "F", null, new DateTime(2024, 8, 26, 0, 23, 37, 889, DateTimeKind.Utc).AddTicks(8919), "Femenino", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "RequestStatuses",
                columns: new[] { "RequestStatusId", "Abbreviation", "Description" },
                values: new object[,]
                {
                    { 1, "NUE", "Nuevo" },
                    { 2, "ENR", "En Revision" },
                    { 3, "REC", "Rechazado" },
                    { 4, "APR", "Aprobado" }
                });

            migrationBuilder.InsertData(
                table: "RequestTypes",
                columns: new[] { "RequestTypeId", "Abbreviation", "CreatedBy", "CreatedDate", "Description", "IsActive", "LastModifiedBy", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, "TI", null, new DateTime(2024, 8, 26, 0, 23, 37, 893, DateTimeKind.Utc).AddTicks(718), "Tecnología de la información", true, null, null },
                    { 2, "RRHH", null, new DateTime(2024, 8, 26, 0, 23, 37, 893, DateTimeKind.Utc).AddTicks(721), "Recursos Humanos", true, null, null },
                    { 3, "LEGA", null, new DateTime(2024, 8, 26, 0, 23, 37, 893, DateTimeKind.Utc).AddTicks(727), "Legal", true, null, null },
                    { 4, "MANT", null, new DateTime(2024, 8, 26, 0, 23, 37, 893, DateTimeKind.Utc).AddTicks(728), "Mantenimiento", true, null, null },
                    { 5, "MKT", null, new DateTime(2024, 8, 26, 0, 23, 37, 893, DateTimeKind.Utc).AddTicks(729), "Mercadeo", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "ZoneId", "Abbreviation", "CreatedBy", "CreatedDate", "Description", "IsActive", "LastModifiedBy", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, "ZN", null, new DateTime(2024, 8, 26, 0, 23, 37, 894, DateTimeKind.Utc).AddTicks(5507), "Zona Norte", true, null, null },
                    { 2, "ZS", null, new DateTime(2024, 8, 26, 0, 23, 37, 894, DateTimeKind.Utc).AddTicks(5509), "Zona Sur", true, null, null },
                    { 3, "ZC", null, new DateTime(2024, 8, 26, 0, 23, 37, 894, DateTimeKind.Utc).AddTicks(5510), "Zona Centro", true, null, null },
                    { 4, "ZO", null, new DateTime(2024, 8, 26, 0, 23, 37, 894, DateTimeKind.Utc).AddTicks(5511), "Zona Occidente", true, null, null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PasswordHash", "PhotoUrl", "TwoFactorCode", "TwoFactorCodeExpiration" },
                values: new object[] { new Guid("0ddbe7e3-c913-4e02-9db3-51b86d5bd6b4"), null, null, new DateTime(2024, 8, 26, 0, 23, 37, 894, DateTimeKind.Utc).AddTicks(952), "williandiaz0012@gmail.com", "Administrador", 1, true, true, true, null, null, "Administrador", "$2a$11$9tnjQB0OK4fm6CyM.MHlseAVS3VmpUqhLk0.oS4.fH3qcY2YKPSdW", null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_RequestDocuments_RequestId",
                table: "RequestDocuments",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestStatusId",
                table: "Requests",
                column: "RequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestTypeId",
                table: "Requests",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ReviewerUserId",
                table: "Requests",
                column: "ReviewerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ZoneId",
                table: "Requests",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_TwoFactorAuthToken_UserId",
                table: "TwoFactorAuthToken",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRequestTypes_RequestTypeId",
                table: "UserRequestTypes",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRequestTypes_UserId",
                table: "UserRequestTypes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_GenderId",
                table: "Users",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UserZones_UserId",
                table: "UserZones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserZones_ZoneId",
                table: "UserZones",
                column: "ZoneId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDocuments");

            migrationBuilder.DropTable(
                name: "TwoFactorAuthToken");

            migrationBuilder.DropTable(
                name: "UserRequestTypes");

            migrationBuilder.DropTable(
                name: "UserZones");

            migrationBuilder.DropTable(
                name: "Requests");

            migrationBuilder.DropTable(
                name: "RequestStatuses");

            migrationBuilder.DropTable(
                name: "RequestTypes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Genders");
        }
    }
}
