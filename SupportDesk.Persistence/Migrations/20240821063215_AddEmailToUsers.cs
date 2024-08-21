using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9440));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9443));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6340));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6349));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6350));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6351));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6352));

            migrationBuilder.InsertData(
                table: "Zones",
                columns: new[] { "ZoneId", "Abbreviation", "CreatedBy", "CreatedDate", "Description", "IsActive", "LastModifiedBy", "LastModifiedDate" },
                values: new object[,]
                {
                    { 1, "ZN", null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1916), "Zona Norte", true, null, null },
                    { 2, "ZS", null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1920), "Zona Sur", true, null, null },
                    { 3, "ZC", null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1922), "Zona Centro", true, null, null },
                    { 4, "ZO", null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1923), "Zona Occidente", true, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 324, DateTimeKind.Utc).AddTicks(5587));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 324, DateTimeKind.Utc).AddTicks(5593));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 327, DateTimeKind.Utc).AddTicks(7731));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 327, DateTimeKind.Utc).AddTicks(7735));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 327, DateTimeKind.Utc).AddTicks(7737));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 327, DateTimeKind.Utc).AddTicks(7738));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 20, 19, 5, 11, 327, DateTimeKind.Utc).AddTicks(7740));
        }
    }
}
