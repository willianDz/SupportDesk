using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewerUserCommentsToRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("79d60d85-04e7-45c3-9c36-85f62a8d28f3"));

            migrationBuilder.AddColumn<string>(
                name: "ReviewerUserComments",
                table: "Requests",
                type: "character varying(800)",
                maxLength: 800,
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("35de8636-18b8-49c4-bbfb-728e543be598"));

            migrationBuilder.DropColumn(
                name: "ReviewerUserComments",
                table: "Requests");

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 606, DateTimeKind.Utc).AddTicks(4227));

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 606, DateTimeKind.Utc).AddTicks(4229));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1593));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1596));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1597));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1599));

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1600));

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PhotoUrl" },
                values: new object[] { new Guid("79d60d85-04e7-45c3-9c36-85f62a8d28f3"), null, null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(1573), "williandiaz0012@gmail.com", "Administrador", null, true, true, true, null, null, "Administrador", null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7238));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7246));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7247));

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7267));
        }
    }
}
