using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdatingEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7e252744-fd43-4f0b-8420-0b01b18c32ee"));

            migrationBuilder.AddColumn<string>(
                name: "TwoFactorCode",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TwoFactorCodeExpiration",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 408, DateTimeKind.Utc).AddTicks(7256));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 408, DateTimeKind.Utc).AddTicks(7260));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 410, DateTimeKind.Utc).AddTicks(8927));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 410, DateTimeKind.Utc).AddTicks(8929));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 410, DateTimeKind.Utc).AddTicks(8930));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 410, DateTimeKind.Utc).AddTicks(8931));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 410, DateTimeKind.Utc).AddTicks(8932));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PasswordHash", "PhotoUrl", "TwoFactorCode", "TwoFactorCodeExpiration" },
                values: new object[] { new Guid("cd9ad7b9-a45f-40df-b29f-6a6d3ab28555"), null, null, new DateTime(2024, 8, 25, 23, 37, 57, 411, DateTimeKind.Utc).AddTicks(7544), "williandiaz0012@gmail.com", "Administrador", null, true, true, true, null, null, "Administrador", "", null, null, null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 412, DateTimeKind.Utc).AddTicks(1876));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 412, DateTimeKind.Utc).AddTicks(1880));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 412, DateTimeKind.Utc).AddTicks(1881));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 23, 37, 57, 412, DateTimeKind.Utc).AddTicks(1891));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cd9ad7b9-a45f-40df-b29f-6a6d3ab28555"));

            migrationBuilder.DropColumn(
                name: "TwoFactorCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TwoFactorCodeExpiration",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 532, DateTimeKind.Utc).AddTicks(7127));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 532, DateTimeKind.Utc).AddTicks(7131));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 535, DateTimeKind.Utc).AddTicks(7609));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 535, DateTimeKind.Utc).AddTicks(7612));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 535, DateTimeKind.Utc).AddTicks(7614));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 535, DateTimeKind.Utc).AddTicks(7615));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 535, DateTimeKind.Utc).AddTicks(7616));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PasswordHash", "PhotoUrl" },
                values: new object[] { new Guid("7e252744-fd43-4f0b-8420-0b01b18c32ee"), null, null, new DateTime(2024, 8, 25, 3, 21, 56, 536, DateTimeKind.Utc).AddTicks(7894), "williandiaz0012@gmail.com", "Administrador", null, true, true, true, null, null, "Administrador", "", null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 537, DateTimeKind.Utc).AddTicks(4049));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 537, DateTimeKind.Utc).AddTicks(4052));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 537, DateTimeKind.Utc).AddTicks(4054));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 25, 3, 21, 56, 537, DateTimeKind.Utc).AddTicks(4065));
        }
    }
}
