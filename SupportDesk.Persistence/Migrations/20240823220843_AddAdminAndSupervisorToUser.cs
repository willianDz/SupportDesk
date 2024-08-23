using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SupportDesk.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminAndSupervisorToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Zones",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Zones",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "UserZones",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "UserZones",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Users",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSupervisor",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "UserRequestTypes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "UserRequestTypes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "RequestTypes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "RequestTypes",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "RequestDocuments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "RequestDocuments",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LastModifiedBy",
                table: "Genders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "Genders",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 606, DateTimeKind.Utc).AddTicks(4227), null });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 606, DateTimeKind.Utc).AddTicks(4229), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1593), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1596), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1597), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1599), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 609, DateTimeKind.Utc).AddTicks(1600), null });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BirthDate", "CreatedBy", "CreatedDate", "Email", "FirstName", "GenderId", "IsActive", "IsAdmin", "IsSupervisor", "LastModifiedBy", "LastModifiedDate", "LastName", "PhotoUrl" },
                values: new object[] { new Guid("79d60d85-04e7-45c3-9c36-85f62a8d28f3"), null, null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(1573), "williandiaz0012@gmail.com", "Administrador", null, true, true, true, null, null, "Administrador", null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7238), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7246), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7247), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 23, 22, 8, 42, 610, DateTimeKind.Utc).AddTicks(7267), null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("79d60d85-04e7-45c3-9c36-85f62a8d28f3"));

            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsSupervisor",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Zones",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "UserZones",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserZones",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "UserRequestTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "UserRequestTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "RequestTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "RequestTypes",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Requests",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Requests",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "RequestDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "RequestDocuments",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastModifiedBy",
                table: "Genders",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "Genders",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9440), null });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "GenderId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 348, DateTimeKind.Utc).AddTicks(9443), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6340), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6349), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6350), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6351), null });

            migrationBuilder.UpdateData(
                table: "RequestTypes",
                keyColumn: "RequestTypeId",
                keyValue: 5,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 351, DateTimeKind.Utc).AddTicks(6352), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 1,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1916), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 2,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1920), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 3,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1922), null });

            migrationBuilder.UpdateData(
                table: "Zones",
                keyColumn: "ZoneId",
                keyValue: 4,
                columns: new[] { "CreatedBy", "CreatedDate", "LastModifiedBy" },
                values: new object[] { null, new DateTime(2024, 8, 21, 6, 32, 14, 353, DateTimeKind.Utc).AddTicks(1923), null });
        }
    }
}
