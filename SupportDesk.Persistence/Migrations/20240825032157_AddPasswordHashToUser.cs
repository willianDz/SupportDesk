using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordHashToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("35de8636-18b8-49c4-bbfb-728e543be598"));

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("7e252744-fd43-4f0b-8420-0b01b18c32ee"));

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 581, DateTimeKind.Utc).AddTicks(1356));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 581, DateTimeKind.Utc).AddTicks(1363));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 583, DateTimeKind.Utc).AddTicks(6059));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 583, DateTimeKind.Utc).AddTicks(6065));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 583, DateTimeKind.Utc).AddTicks(6067));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 583, DateTimeKind.Utc).AddTicks(6068));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 583, DateTimeKind.Utc).AddTicks(6069));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PhotoUrl" },
                values: new object[] { new Guid("35de8636-18b8-49c4-bbfb-728e543be598"), null, null, new DateTime(2024, 8, 23, 23, 12, 52, 584, DateTimeKind.Utc).AddTicks(5177), "williandiaz0012@gmail.com", "Administrador", null, true, true, true, null, null, "Administrador", null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 584, DateTimeKind.Utc).AddTicks(9887));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 584, DateTimeKind.Utc).AddTicks(9896));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 584, DateTimeKind.Utc).AddTicks(9898));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 23, 12, 52, 584, DateTimeKind.Utc).AddTicks(9915));
        }
    }
}
