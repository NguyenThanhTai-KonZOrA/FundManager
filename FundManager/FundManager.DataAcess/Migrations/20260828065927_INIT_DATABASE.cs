using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FundManager.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class INIT_DATABASE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FileExtension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    PropertyId = table.Column<int>(type: "int", nullable: true),
                    OutletId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RequestPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WindowAccount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDeviceId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    PayloadJson = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    AttemptCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "System"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PermissionName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PermissionCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeRoles_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "ApplicationImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "FileExtension", "FilePath", "FileSize", "FileUrl", "IsActive", "IsDelete", "Name", "OutletId", "PropertyId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Ho Tram Logo", ".png", "/ApplicationImages/TheGrandHoTramLogo.png", 3100L, "/ApplicationImages/16831151-1e2c-4f27-bc56-1a26c3afef0f.jpg", true, false, "The Grand Ho Tram", null, null, (byte)2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Spa", ".png", "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", 13050L, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", true, false, "The Grand Spa Icon", 1, 2, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lotus Spa Logo", ".png", "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png", 11465L, "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png", true, false, "Lotus Spa Icon", 2, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "MAIA SPA Icon", ".png", "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png", 12176L, "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png", true, false, "MAIA SPA Icon", 3, 3, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Spa Image", ".png", "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png", 1003615L, "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png", true, false, "The Grand Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lotus Spa Image", ".png", "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", 551515L, "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", true, false, "Lotus Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Maia Spa Image", ".png", "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png", 844391L, "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png", true, false, "Maia Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "English Flag", ".svg", "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg", 1274L, "/ApplicationImages/bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg", true, false, "English Flag", null, null, (byte)4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Vietnam Flag", ".svg", "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\6640af6d-60a0-4cea-98c8-1871e7facb0e.svg", 1274L, "/ApplicationImages/6640af6d-60a0-4cea-98c8-1871e7facb0e.svg", true, false, "Vietnam Flag", null, null, (byte)4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Korean Flag", ".svg", "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\f4bb608f-5a36-4b58-9959-c3361228a7ee.svg", 1274L, "/ApplicationImages/f4bb608f-5a36-4b58-9959-c3361228a7ee.svg", true, false, "Korean Flag", null, null, (byte)4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 11, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "China Flag", ".svg", "D:\\IIS Publish\\DigitalDocumentPlatform\\FundManager.API\\ApplicationImages\\d350d454-b583-4725-b20f-7c1a0e03d51b.svg", 1274L, "/ApplicationImages/d350d454-b583-4725-b20f-7c1a0e03d51b.svg", true, false, "China Flag", null, null, (byte)4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "ApplicationSettings",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "DataType", "Description", "IsActive", "IsDelete", "Key", "UpdatedAt", "UpdatedBy", "Value" },
                values: new object[] { 1, "System", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Integer", "Delay duration after submission in minutes.", true, false, "DelayDurationAfterSubmitted", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "10" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Department", "Email", "EmployeeCode", "FullName", "IsActive", "IsDelete", "PhoneNumber", "Position", "UpdatedAt", "UpdatedBy", "WindowAccount" },
                values: new object[] { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "IT", "adminTemp@thegrandhotram.com", "admin", "System Administrator", true, false, "System", "Administrator", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "admin" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "Category", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDelete", "PermissionCode", "PermissionName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "View dashboard", true, false, "CAN_VIEW_DASHBOARD", "View Dashboard", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "View room check information", true, false, "CAN_CHECK_ROOM", "Room Check", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Update employee information", true, false, "CAN_UNCHECK", "Un-check", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "View history information", true, false, "CAN_VIEW_HISTORY", "History", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "View reports", true, false, "CAN_VIEW_REPORTS", "Reports", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, "Employee", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "View all reports", true, false, "CAN_VIEW_ALL_REPORTS", "ViewAllReports", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDelete", "RoleName", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Full system access", true, false, "Administrator", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Management level access", true, false, "OutletStaff", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EmployeeId", "IsActive", "IsDelete", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, true, false, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "IsActive", "IsDelete", "PermissionId", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 1, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 2, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 3, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 4, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 5, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", true, false, 6, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Entity",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Success",
                table: "AuditLogs",
                columns: new[] { "IsSuccess", "CreatedAt" },
                filter: "[IsSuccess] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserName",
                table: "AuditLogs",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_EmployeeId_RoleId",
                table: "EmployeeRoles",
                columns: new[] { "EmployeeId", "RoleId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRoles_RoleId",
                table: "EmployeeRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_EmployeeCode",
                table: "Employees",
                column: "EmployeeCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Pending",
                table: "Notifications",
                columns: new[] { "Status", "AttemptCount", "StaffDeviceId" },
                filter: "[Status] = 'Pending'")
                .Annotation("SqlServer:Include", new[] { "Id", "SessionId", "PayloadJson", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_SessionStaff",
                table: "Notifications",
                columns: new[] { "SessionId", "StaffDeviceId" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_StaffDeviceId",
                table: "Notifications",
                column: "StaffDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true,
                filter: "[PermissionCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationImages");

            migrationBuilder.DropTable(
                name: "ApplicationSettings");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "EmployeeRoles");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
