using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalDocumentPlatform.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Update_ValueLength_ApplicationSettings_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApplicationSettings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(7937));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(9151));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(9156));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(9158));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(9160));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 19, 9, 34, 19, 113, DateTimeKind.Local).AddTicks(9162));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "ApplicationSettings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 33, DateTimeKind.Local).AddTicks(8915));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(263));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(270));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(272));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(273));

            migrationBuilder.UpdateData(
                table: "FormTemplateTranslations",
                keyColumn: "Id",
                keyValue: 6,
                column: "CreatedAt",
                value: new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(275));
        }
    }
}
