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
                name: "ApplicationSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
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
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Abrv2 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Abrv3 = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
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
                name: "FormTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AgreementText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NativeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FlagEmoji = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
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
                name: "Outlets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    MainColor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IconImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outlets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatronDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ConnectionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatronDevices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PatronTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorHex = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatronTypes", x => x.Id);
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
                name: "Properties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Properties", x => x.Id);
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
                name: "FormQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTemplateId = table.Column<int>(type: "int", nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false),
                    HasFollowUpText = table.Column<bool>(type: "bit", nullable: false),
                    FollowUpLabel = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FollowUpTriggerOption = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormQuestions_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormTemplateTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTemplateId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AgreementText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuestionsTranslation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTemplateTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormTemplateTranslations_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormTemplateVersionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTemplateId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    LogoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FooterText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AgreementText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QuestionsSnapshot = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormTemplateVersionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormTemplateVersionHistories_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
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
                    table.PrimaryKey("PK_DocumentTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTemplates_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MacAddress = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StaffUserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ConnectionId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    IsOnline = table.Column<bool>(type: "bit", nullable: false),
                    LastHeartbeat = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                    table.PrimaryKey("PK_StaffDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffDevices_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowDefinitions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OutletId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowDefinitions_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormTemplateId = table.Column<int>(type: "int", nullable: false),
                    TemplateVersion = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PatronDeviceId = table.Column<int>(type: "int", nullable: true),
                    SignatureSessionId = table.Column<int>(type: "int", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubmissions_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormSubmissions_PatronDevices_PatronDeviceId",
                        column: x => x.PatronDeviceId,
                        principalTable: "PatronDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Patron",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Birthday = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PlayerId = table.Column<int>(type: "int", nullable: true),
                    OutletId = table.Column<int>(type: "int", nullable: true),
                    RoomNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    PatronTypeId = table.Column<int>(type: "int", nullable: true),
                    CustomerType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patron", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Patron_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Patron_PatronTypes_PatronTypeId",
                        column: x => x.PatronTypeId,
                        principalTable: "PatronTypes",
                        principalColumn: "Id");
                });

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
                    table.ForeignKey(
                        name: "FK_ApplicationImages_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ApplicationImages_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PropertyOutlets",
                columns: table => new
                {
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    OutletId = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyOutlets", x => new { x.PropertyId, x.OutletId });
                    table.ForeignKey(
                        name: "FK_PropertyOutlets_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyOutlets_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.CreateTable(
                name: "FormQuestionOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormQuestionId = table.Column<int>(type: "int", nullable: false),
                    OptionText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormQuestionOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormQuestionOptions_FormQuestions_FormQuestionId",
                        column: x => x.FormQuestionId,
                        principalTable: "FormQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplateTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTemplateId = table.Column<int>(type: "int", nullable: false),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplateTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTemplateTranslations_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTemplateVersionHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentTemplateId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ChangeNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTemplateVersionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentTemplateVersionHistories_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeviceMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StaffDeviceId = table.Column<int>(type: "int", nullable: false),
                    PatronDeviceId = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastVerified = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StaffDeviceId1 = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceMappings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeviceMappings_PatronDevices_PatronDeviceId",
                        column: x => x.PatronDeviceId,
                        principalTable: "PatronDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceMappings_StaffDevices_StaffDeviceId",
                        column: x => x.StaffDeviceId,
                        principalTable: "StaffDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeviceMappings_StaffDevices_StaffDeviceId1",
                        column: x => x.StaffDeviceId1,
                        principalTable: "StaffDevices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkflowDefinitionId = table.Column<int>(type: "int", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepType = table.Column<int>(type: "int", nullable: false),
                    StepLabel = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FormTemplateId = table.Column<int>(type: "int", nullable: true),
                    DocumentTemplateId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_DocumentTemplates_DocumentTemplateId",
                        column: x => x.DocumentTemplateId,
                        principalTable: "DocumentTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_FormTemplates_FormTemplateId",
                        column: x => x.FormTemplateId,
                        principalTable: "FormTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkflowSteps_WorkflowDefinitions_WorkflowDefinitionId",
                        column: x => x.WorkflowDefinitionId,
                        principalTable: "WorkflowDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FormSubmissionAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormSubmissionId = table.Column<int>(type: "int", nullable: false),
                    FormQuestionId = table.Column<int>(type: "int", nullable: false),
                    AnswerValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    FollowUpText = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormSubmissionAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormSubmissionAnswers_FormQuestions_FormQuestionId",
                        column: x => x.FormQuestionId,
                        principalTable: "FormQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormSubmissionAnswers_FormSubmissions_FormSubmissionId",
                        column: x => x.FormSubmissionId,
                        principalTable: "FormSubmissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PatronSignature",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatronId = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<int>(type: "int", nullable: false),
                    DocumentPath = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SignatureData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeviceInfo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatronSignature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatronSignature_Patron_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patron",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SignatureSessions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatronId = table.Column<int>(type: "int", nullable: false),
                    StaffDeviceId = table.Column<int>(type: "int", nullable: false),
                    PatronDeviceId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignatureSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SignatureSessions_PatronDevices_PatronDeviceId",
                        column: x => x.PatronDeviceId,
                        principalTable: "PatronDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignatureSessions_Patron_PatronId",
                        column: x => x.PatronId,
                        principalTable: "Patron",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SignatureSessions_StaffDevices_StaffDeviceId",
                        column: x => x.StaffDeviceId,
                        principalTable: "StaffDevices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ApplicationImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "FileExtension", "FilePath", "FileSize", "FileUrl", "IsActive", "IsDelete", "Name", "OutletId", "PropertyId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Ho Tram Logo", ".png", "/ApplicationImages/TheGrandHoTramLogo.png", 3100L, "/ApplicationImages/16831151-1e2c-4f27-bc56-1a26c3afef0f.jpg", true, false, "The Grand Ho Tram", null, null, (byte)2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
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
                table: "Countries",
                columns: new[] { "Id", "Abrv2", "Abrv3", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDelete", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 4, "AF", "AFG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Afghanistan", true, false, "Afghanistan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 8, "AL", "ALB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Albania", true, false, "Albania", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 10, "AQ", "ATA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Antarctica", true, false, "Antarctica", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 12, "DZ", "DZA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Algeria", true, false, "Algeria", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 16, "AS", "ASM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "American Samoa", true, false, "American Samoa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 20, "AD", "AND", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Andorra", true, false, "Andorra", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 24, "AO", "AGO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Angola", true, false, "Angola", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 28, "AG", "ATG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Antigua and Barbuda", true, false, "Antigua and Barbuda", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 31, "AZ", "AZE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Azerbaijan", true, false, "Azerbaijan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 32, "AR", "ARG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Argentina", true, false, "Argentina", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 36, "AU", "AUS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Australia", true, false, "Australia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 40, "AT", "AUT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Austria", true, false, "Austria", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 44, "BS", "BHS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bahamas", true, false, "Bahamas", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 48, "BH", "BHR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bahrain", true, false, "Bahrain", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 50, "BD", "BGD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bangladesh", true, false, "Bangladesh", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 51, "AM", "ARM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Armenia", true, false, "Armenia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 52, "BB", "BRB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Barbados", true, false, "Barbados", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 56, "BE", "BEL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Belgium", true, false, "Belgium", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 60, "BM", "BMU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bermuda", true, false, "Bermuda", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 64, "BT", "BTN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bhutan", true, false, "Bhutan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 68, "BO", "BOL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bolivia", true, false, "Bolivia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 69, "BQ", "BES", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bonaire, Sint Eustatius and Saba", true, false, "Bonaire, Sint Eustatius and Saba", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 70, "BA", "BIH", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bosnia and Herzegovina", true, false, "Bosnia and Herzegovina", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 72, "BW", "BWA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Botswana", true, false, "Botswana", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 74, "BV", "BVT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bouvet Island", true, false, "Bouvet Island", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 76, "BR", "BRA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Brazil", true, false, "Brazil", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 84, "BZ", "BLZ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Belize", true, false, "Belize", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 86, "IO", "IOT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "British Indian Ocean Territory", true, false, "British Indian Ocean Territory", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 90, "SB", "SLB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Solomon Islands", true, false, "Solomon Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 92, "VG", "VGB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Virgin Islands (British)", true, false, "Virgin Islands (British)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 96, "BN", "BRN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Brunei Darussalam", true, false, "Brunei Darussalam", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 100, "BG", "BGR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Bulgaria", true, false, "Bulgaria", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 104, "MM", "MMR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Myanmar", true, false, "Myanmar", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 108, "BI", "BDI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Burundi", true, false, "Burundi", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 112, "BY", "BLR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Belarus", true, false, "Belarus", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 116, "KH", "KHM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cambodia", true, false, "Cambodia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 120, "CM", "CMR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cameroon", true, false, "Cameroon", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 124, "CA", "CAN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Canada", true, false, "Canada", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 132, "CV", "CPV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cabo Verde", true, false, "Cabo Verde", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 136, "KY", "CYM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cayman Islands", true, false, "Cayman Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 140, "CF", "CAF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Central African Republic", true, false, "Central African Republic", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 144, "LK", "LKA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sri Lanka", true, false, "Sri Lanka", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 148, "TD", "TCD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Chad", true, false, "Chad", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 152, "CL", "CHL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Chile", true, false, "Chile", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 156, "CN", "CHN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "China", true, false, "China", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 158, "TW", "TWN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Taiwan", true, false, "Taiwan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 162, "CX", "CXR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Christmas Island", true, false, "Christmas Island", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 166, "CC", "CCK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cocos (Keeling) Islands", true, false, "Cocos (Keeling) Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 170, "CO", "COL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Colombia", true, false, "Colombia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 174, "KM", "COM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Comoros", true, false, "Comoros", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 175, "YT", "MYT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mayotte", true, false, "Mayotte", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 178, "CG", "COG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Congo", true, false, "Congo", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 180, "CD", "COD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Congo (Democratic Republic of the)", true, false, "Congo (Democratic Republic of the)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 184, "CK", "COK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cook Islands", true, false, "Cook Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 188, "CR", "CRI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Costa Rica", true, false, "Costa Rica", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 191, "HR", "HRV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Croatia", true, false, "Croatia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 192, "CU", "CUB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cuba", true, false, "Cuba", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 193, "CW", "CUW", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Curacao", true, false, "Curacao", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 196, "CY", "CYP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cyprus", true, false, "Cyprus", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 203, "CZ", "CZE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Czechia", true, false, "Czechia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 204, "BJ", "BEN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Benin", true, false, "Benin", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 208, "DK", "DNK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Denmark", true, false, "Denmark", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 212, "DM", "DMA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Dominica", true, false, "Dominica", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 214, "DO", "DOM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Dominican Republic", true, false, "Dominican Republic", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 218, "EC", "ECU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ecuador", true, false, "Ecuador", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 222, "SV", "SLV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "El Salvador", true, false, "El Salvador", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 226, "GQ", "GNQ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Equatorial Guinea", true, false, "Equatorial Guinea", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 231, "ET", "ETH", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ethiopia", true, false, "Ethiopia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 232, "ER", "ERI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Eritrea", true, false, "Eritrea", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 233, "EE", "EST", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Estonia", true, false, "Estonia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 234, "FO", "FRO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Faroe Islands", true, false, "Faroe Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 238, "FK", "FLK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Falkland Islands", true, false, "Falkland Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 239, "GS", "SGS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "South Georgia and the South Sandwich Islands", true, false, "South Georgia and the South Sandwich Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 242, "FJ", "FJI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Fiji", true, false, "Fiji", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 246, "FI", "FIN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Finland", true, false, "Finland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 248, "AX", "ALA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Aland Islands", true, false, "Aland Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 250, "FR", "FRA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "France", true, false, "France", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 254, "GF", "GUF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "French Guiana", true, false, "French Guiana", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 258, "PF", "PYF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "French Polynesia", true, false, "French Polynesia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 260, "TF", "ATF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "French Southern Territories", true, false, "French Southern Territories", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 262, "DJ", "DJI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Djibouti", true, false, "Djibouti", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 266, "GA", "GAB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Gabon", true, false, "Gabon", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 268, "GE", "GEO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Georgia", true, false, "Georgia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 270, "GM", "GMB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Gambia", true, false, "Gambia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 275, "PS", "PSE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Palestine, State of", true, false, "Palestine, State of", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 276, "DE", "DEU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Germany", true, false, "Germany", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 288, "GH", "GHA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ghana", true, false, "Ghana", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 292, "GI", "GIB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Gibraltar", true, false, "Gibraltar", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 296, "KI", "KIR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Kiribati", true, false, "Kiribati", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 300, "GR", "GRC", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Greece", true, false, "Greece", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 304, "GL", "GRL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Greenland", true, false, "Greenland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 308, "GD", "GRD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Grenada", true, false, "Grenada", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 312, "GP", "GLP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guadeloupe", true, false, "Guadeloupe", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 316, "GU", "GUM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guam", true, false, "Guam", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 320, "GT", "GTM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guatemala", true, false, "Guatemala", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 324, "GN", "GIN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guinea", true, false, "Guinea", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 325, "GG", "GGY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guernsey", true, false, "Guernsey", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 328, "GY", "GUY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guyana", true, false, "Guyana", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 332, "HT", "HTI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Haiti", true, false, "Haiti", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 334, "HM", "HMD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Heard Island and McDonald Islands", true, false, "Heard Island and McDonald Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 336, "VA", "VAT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Holy See", true, false, "Holy See", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 340, "HN", "HND", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Honduras", true, false, "Honduras", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 344, "HK", "HKG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Hong Kong", true, false, "Hong Kong", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 348, "HU", "HUN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Hungary", true, false, "Hungary", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 352, "IS", "ISL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Iceland", true, false, "Iceland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 353, "IM", "IMN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Isle of Man", true, false, "Isle of Man", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 356, "IN", "IND", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "India", true, false, "India", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 360, "ID", "IDN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Indonesia", true, false, "Indonesia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 364, "IR", "IRN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Iran", true, false, "Iran", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 368, "IQ", "IRQ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Iraq", true, false, "Iraq", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 372, "IE", "IRL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ireland", true, false, "Ireland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 376, "IL", "ISR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Israel", true, false, "Israel", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 380, "IT", "ITA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Italy", true, false, "Italy", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 384, "CI", "CIV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Cote d'Ivoire", true, false, "Cote d'Ivoire", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 388, "JM", "JAM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Jamaica", true, false, "Jamaica", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 392, "JP", "JPN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Japan", true, false, "Japan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 393, "JE", "JEY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Jersey", true, false, "Jersey", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 398, "KZ", "KAZ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Kazakhstan", true, false, "Kazakhstan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 400, "JO", "JOR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Jordan", true, false, "Jordan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 404, "KE", "KEN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Kenya", true, false, "Kenya", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 408, "KP", "PRK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Korea (Democratic People's Republic of)", true, false, "Korea (Democratic People's Republic of)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 410, "KR", "KOR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Korea (Republic of)", true, false, "Korea (Republic of)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 414, "KW", "KWT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Kuwait", true, false, "Kuwait", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 417, "KG", "KGZ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Kyrgyzstan", true, false, "Kyrgyzstan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 418, "LA", "LAO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lao People's Democratic Republic", true, false, "Lao People's Democratic Republic", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 422, "LB", "LBN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lebanon", true, false, "Lebanon", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 426, "LS", "LSO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lesotho", true, false, "Lesotho", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 428, "LV", "LVA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Latvia", true, false, "Latvia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 430, "LR", "LBR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Liberia", true, false, "Liberia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 434, "LY", "LBY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Libya", true, false, "Libya", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 438, "LI", "LIE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Liechtenstein", true, false, "Liechtenstein", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 440, "LT", "LTU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lithuania", true, false, "Lithuania", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 442, "LU", "LUX", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Luxembourg", true, false, "Luxembourg", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 446, "MO", "MAC", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Macao", true, false, "Macao", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 450, "MG", "MDG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Madagascar", true, false, "Madagascar", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 454, "MW", "MWI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Malawi", true, false, "Malawi", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 458, "MY", "MYS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Malaysia", true, false, "Malaysia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 462, "MV", "MDV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Maldives", true, false, "Maldives", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 466, "ML", "MLI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mali", true, false, "Mali", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 470, "MT", "MLT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Malta", true, false, "Malta", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 474, "MQ", "MTQ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Martinique", true, false, "Martinique", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 478, "MR", "MRT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mauritania", true, false, "Mauritania", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 480, "MU", "MUS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mauritius", true, false, "Mauritius", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 484, "MX", "MEX", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mexico", true, false, "Mexico", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 492, "MC", "MCO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Monaco", true, false, "Monaco", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 496, "MN", "MNG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mongolia", true, false, "Mongolia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 498, "MD", "MDA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Moldova", true, false, "Moldova", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 500, "MS", "MSR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Montserrat", true, false, "Montserrat", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 501, "ME", "MNE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Montenegro", true, false, "Montenegro", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 504, "MA", "MAR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Morocco", true, false, "Morocco", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 508, "MZ", "MOZ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Mozambique", true, false, "Mozambique", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 512, "OM", "OMN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Oman", true, false, "Oman", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 516, "NaN", "NAM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Namibia", true, false, "Namibia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 520, "NR", "NRU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Nauru", true, false, "Nauru", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 524, "NP", "NPL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Nepal", true, false, "Nepal", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 528, "NL", "NLD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Netherlands", true, false, "Netherlands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 530, "AN", "ANT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Netherlands Antilles", true, false, "Netherlands Antilles", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 533, "AW", "ABW", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Aruba", true, false, "Aruba", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 540, "NC", "NCL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "New Caledonia", true, false, "New Caledonia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 548, "VU", "VUT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Vanuatu", true, false, "Vanuatu", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 554, "NZ", "NZL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "New Zealand", true, false, "New Zealand", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 558, "NI", "NIC", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Nicaragua", true, false, "Nicaragua", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 562, "NE", "NER", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Niger", true, false, "Niger", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 566, "NG", "NGA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Nigeria", true, false, "Nigeria", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 570, "NU", "NIU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Niue", true, false, "Niue", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 574, "NF", "NFK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Norfolk Island", true, false, "Norfolk Island", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 578, "NO", "NOR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Norway", true, false, "Norway", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 580, "MP", "MNP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Northern Mariana Islands", true, false, "Northern Mariana Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 581, "UM", "UMI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "United States Minor Outlying Islands", true, false, "United States Minor Outlying Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 583, "FM", "FSM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Micronesia", true, false, "Micronesia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 584, "MH", "MHL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Marshall Islands", true, false, "Marshall Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 585, "PW", "PLW", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Palau", true, false, "Palau", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 586, "PK", "PAK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pakistan", true, false, "Pakistan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 591, "PA", "PAN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Panama", true, false, "Panama", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 598, "PG", "PNG", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Papua New Guinea", true, false, "Papua New Guinea", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 600, "PY", "PRY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Paraguay", true, false, "Paraguay", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 604, "PE", "PER", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Peru", true, false, "Peru", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 608, "PH", "PHL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Philippines", true, false, "Philippines", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 612, "PN", "PCN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Pitcairn", true, false, "Pitcairn", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 616, "PL", "POL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Poland", true, false, "Poland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 620, "PT", "PRT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Portugal", true, false, "Portugal", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 624, "GW", "GNB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Guinea-Bissau", true, false, "Guinea-Bissau", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 626, "TL", "TLS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Timor-Leste", true, false, "Timor-Leste", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 630, "PR", "PRI", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Puerto Rico", true, false, "Puerto Rico", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 634, "QA", "QAT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Qatar", true, false, "Qatar", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 638, "RE", "REU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Reunion", true, false, "Reunion", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 642, "RO", "ROU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Romania", true, false, "Romania", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 643, "RU", "RUS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Russian Federation", true, false, "Russian Federation", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 646, "RW", "RWA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Rwanda", true, false, "Rwanda", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 647, "BL", "BLM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Barthelemy", true, false, "Saint Barthelemy", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 654, "SH", "SHN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Helena, Ascension and Tristan da Cunha", true, false, "Saint Helena, Ascension and Tristan da Cunha", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 655, "MF", "MAF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Martin", true, false, "Saint Martin", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 656, "SX", "SXM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sint Maarten", true, false, "Sint Maarten", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 659, "KN", "KNA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Kitts and Nevis", true, false, "Saint Kitts and Nevis", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 660, "AI", "AIA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Anguilla", true, false, "Anguilla", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 662, "LC", "LCA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Lucia", true, false, "Saint Lucia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 666, "PM", "SPM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Pierre and Miquelon", true, false, "Saint Pierre and Miquelon", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 670, "VC", "VCT", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saint Vincent and the Grenadines", true, false, "Saint Vincent and the Grenadines", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 674, "SM", "SMR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "San Marino", true, false, "San Marino", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 678, "ST", "STP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sao Tome and Principe", true, false, "Sao Tome and Principe", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 682, "SA", "SAU", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Saudi Arabia", true, false, "Saudi Arabia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 686, "SN", "SEN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Senegal", true, false, "Senegal", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 690, "SC", "SYC", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Seychelles", true, false, "Seychelles", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 694, "SL", "SLE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sierra Leone", true, false, "Sierra Leone", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 702, "SG", "SGP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Singapore", true, false, "Singapore", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 703, "SK", "SVK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Slovakia", true, false, "Slovakia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 704, "VN", "VNM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Viet Nam", true, false, "Viet Nam", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 705, "SI", "SVN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Slovenia", true, false, "Slovenia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 706, "SO", "SOM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Somalia", true, false, "Somalia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 710, "ZA", "ZAF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "South Africa", true, false, "South Africa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 716, "ZW", "ZWE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Zimbabwe", true, false, "Zimbabwe", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 724, "ES", "ESP", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Spain", true, false, "Spain", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 732, "EH", "ESH", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Western Sahara", true, false, "Western Sahara", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 736, "SD", "SDN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sudan", true, false, "Sudan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 740, "SR", "SUR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Suriname", true, false, "Suriname", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 744, "SJ", "SJM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Svalbard and Jan Mayen", true, false, "Svalbard and Jan Mayen", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 748, "SZ", "SWZ", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Eswatini", true, false, "Eswatini", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 752, "SE", "SWE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Sweden", true, false, "Sweden", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 756, "CH", "CHE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Switzerland", true, false, "Switzerland", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 760, "SY", "SYR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Syrian Arab Republic", true, false, "Syrian Arab Republic", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 762, "TJ", "TJK", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tajikistan", true, false, "Tajikistan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 764, "TH", "THA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Thailand", true, false, "Thailand", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 768, "TG", "TGO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Togo", true, false, "Togo", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 772, "TK", "TKL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tokelau", true, false, "Tokelau", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 776, "TO", "TON", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tonga", true, false, "Tonga", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 780, "TT", "TTO", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Trinidad and Tobago", true, false, "Trinidad and Tobago", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 784, "AE", "ARE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "United Arab Emirates", true, false, "United Arab Emirates", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 788, "TN", "TUN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tunisia", true, false, "Tunisia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 792, "TR", "TUR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Turkey", true, false, "Turkey", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 795, "TM", "TKM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Turkmenistan", true, false, "Turkmenistan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 796, "TC", "TCA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Turks and Caicos Islands", true, false, "Turks and Caicos Islands", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 798, "TV", "TUV", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tuvalu", true, false, "Tuvalu", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 800, "UG", "UGA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Uganda", true, false, "Uganda", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 804, "UA", "UKR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ukraine", true, false, "Ukraine", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 807, "MK", "MKD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Macedonia", true, false, "Macedonia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 818, "EG", "EGY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Egypt", true, false, "Egypt", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 826, "GB", "GBR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "United Kingdom", true, false, "United Kingdom", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 834, "TZ", "TZA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Tanzania, United Republic of", true, false, "Tanzania, United Republic of", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 840, "US", "USA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "United States of America", true, false, "United States", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 850, "VI", "VIR", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Virgin Islands (U.S.)", true, false, "Virgin Islands (U.S.)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 854, "BF", "BFA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Burkina Faso", true, false, "Burkina Faso", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 858, "UY", "URY", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Uruguay", true, false, "Uruguay", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 860, "UZ", "UZB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Uzbekistan", true, false, "Uzbekistan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 862, "VE", "VEN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Venezuela", true, false, "Venezuela", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 876, "WF", "WLF", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Wallis and Futuna", true, false, "Wallis and Futuna", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 882, "WS", "WSM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Samoa", true, false, "Samoa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 887, "YE", "YEM", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Yemen", true, false, "Yemen", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 891, "RS", "SRB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Serbia", true, false, "Serbia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 892, "SS", "SSD", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "South Sudan", true, false, "South Sudan", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 894, "ZM", "ZMB", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Zambia", true, false, "Zambia", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTemplates",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "Description", "DocumentType", "IsActive", "IsDelete", "OutletId", "Title", "UpdatedAt", "UpdatedBy", "Version" },
                values: new object[,]
                {
                    { 1, "<h2>Personal Data Processing Consent</h2>\r\n<p>I, the undersigned, hereby consent to The Grand Ho Tram Strip collecting, storing, and processing my personal data for the purpose of delivering hotel and spa services, loyalty programme management, and regulatory compliance.</p>\r\n<h3>Data collected</h3>\r\n<ul>\r\n  <li>Full name, date of birth, nationality</li>\r\n  <li>Contact information (email, phone number)</li>\r\n  <li>Health information relevant to spa treatments</li>\r\n  <li>Transaction and service history</li>\r\n</ul>\r\n<h3>Your rights</h3>\r\n<p>You have the right to access, correct, or request deletion of your personal data at any time by contacting our Data Protection Officer at <a href=\"mailto:dpo@thegrandhotram.com\">dpo@thegrandhotram.com</a>.</p>\r\n<p>By signing below you confirm that you have read and understood this consent form.</p>", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Consent form for collecting and processing personal data in accordance with applicable data protection regulations.", 1, true, false, null, "Personal Data Processing Consent (PDP)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 2, "<h2>Hotel Terms &amp; Policies</h2>\r\n<h3>Check-in / Check-out</h3>\r\n<p>Standard check-in time is 15:00 and check-out time is 12:00. Early check-in and late check-out are subject to availability and may incur additional charges.</p>\r\n<h3>Cancellation Policy</h3>\r\n<p>Reservations cancelled within 48 hours of arrival will be charged one night's room rate. No-shows will be charged the full reservation amount.</p>\r\n<h3>Property Rules</h3>\r\n<p>Smoking is prohibited in all indoor areas. Pets are not permitted on the property. Guests are responsible for any damage caused to hotel property during their stay.</p>\r\n<h3>Liability</h3>\r\n<p>The hotel is not responsible for the loss or damage of personal belongings. Guests are encouraged to use the in-room safe or the hotel's safety deposit box service.</p>\r\n<p>I confirm that I have read and agree to the above terms and policies.</p>", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Standard hotel terms and policies that guests must acknowledge upon check-in.", 2, true, false, null, "Hotel Terms & Policies (HTP)", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 3, "<h2>Spa Liability Release &amp; Acknowledgement</h2>\r\n<p>I acknowledge that treatments at The Grand Spa are non-medical. I confirm that I have accurately completed the spa consultation form and hereby release The Grand Spa, the hotel, and its employees from any liability or claims arising from my spa treatment.</p>\r\n<h3>Cancellation Policy</h3>\r\n<p>I understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.</p>\r\n<h3>Health Declaration</h3>\r\n<p>I declare that the health information provided in the spa consultation form is accurate and complete to the best of my knowledge. I will inform the therapist of any changes to my health status before each treatment.</p>\r\n<p>By signing below I confirm my understanding and acceptance of the above terms.</p>", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Liability release form for spa treatments. Patron acknowledges the non-medical nature of treatments and cancellation policy.", 4, true, false, null, "Spa Liability Release & Acknowledgement", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 }
                });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Department", "Email", "EmployeeCode", "FullName", "IsActive", "IsDelete", "PhoneNumber", "Position", "UpdatedAt", "UpdatedBy", "WindowAccount" },
                values: new object[] { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "IT", "adminTemp@thegrandhotram.com", "admin", "System Administrator", true, false, "System", "Administrator", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "admin" });

            migrationBuilder.InsertData(
                table: "FormTemplates",
                columns: new[] { "Id", "AgreementText", "CreatedAt", "CreatedBy", "Description", "FooterText", "IsActive", "IsDelete", "LogoUrl", "Title", "UpdatedAt", "UpdatedBy", "Version" },
                values: new object[,]
                {
                    { 1, "I agree to the Personal data processing notice", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:", "I acknowledge that treatments at The Grand Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Grand Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.", true, false, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", "THE GRAND SPA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 2, "I agree to the Personal data processing notice", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:", "I acknowledge that treatments at The Maia Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Maia Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.", true, false, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", "THE MAIA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 3, "I agree to the Personal data processing notice", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Please take a moment to complete this form carefully, as your answers will help us provide your treatment safely and effectively:", "I acknowledge that treatments at The Lotus Spa are non-medical. I confirm that I have accurately completed this consultation form and released The Lotus Spa, the hotel, and its employees, from any liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the cost of the selected treatment.", true, false, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", "THE LOTUS SPA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 }
                });

            migrationBuilder.InsertData(
                table: "Languages",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "FlagEmoji", "IsActive", "IsDelete", "Name", "NativeName", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "en", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "/ApplicationImages/bf6f69ff-9957-4faf-8524-6833eb96e3f9.svg", true, false, "English", "English", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "vi", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "/ApplicationImages/6640af6d-60a0-4cea-98c8-1871e7facb0e.svg", true, false, "Vietnamese", "Tiếng Việt", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, "ko", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "/ApplicationImages/f4bb608f-5a36-4b58-9959-c3361228a7ee.svg", true, false, "Korean", "한국어", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, "zh", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "/ApplicationImages/d350d454-b583-4725-b20f-7c1a0e03d51b.svg", true, false, "Chinese", "中文", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "Outlets",
                columns: new[] { "Id", "BackgroundImageUrl", "Code", "CreatedAt", "CreatedBy", "Description", "IconImageUrl", "IsActive", "IsDelete", "MainColor", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png", "THE_GRAND_SPA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Spa", "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", true, false, "#274549", "The Grand Spa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", "LOTUS_SPA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lotus Spa", "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png", true, false, "#384fc2", "Lotus Spa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png", "MAIA_SPA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Maia Spa", "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png", true, false, "#f07ace", "Maia Spa", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", "GLOBAL", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Global", "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", true, false, "#274549", "Global", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

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
                table: "Properties",
                columns: new[] { "Id", "Code", "Color", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDelete", "Name", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "IC", "#1976d2", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "InterContinental Ho Tram", true, false, "InterContinental", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "HI", "#388e3c", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Holiday Inn Ho Tram", true, false, "Holiday Inn", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, "IX", "#d32f2f", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Ixora Ho Tram", true, false, "Ixora", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
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
                table: "ApplicationImages",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "FileExtension", "FilePath", "FileSize", "FileUrl", "IsActive", "IsDelete", "Name", "OutletId", "PropertyId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Spa", ".png", "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", 13050L, "/ApplicationImages/ed572294-335b-46bd-8df8-99de63864904.png", true, false, "The Grand Spa Icon", 1, 2, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lotus Spa Logo", ".png", "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png", 11465L, "/ApplicationImages/70de369b-60ee-4aaf-85b0-04b8ad623991.png", true, false, "Lotus Spa Icon", 2, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "MAIA SPA Icon", ".png", "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png", 12176L, "/ApplicationImages/ede11902-62a8-4e76-859f-2312b0c24893.png", true, false, "MAIA SPA Icon", 3, 3, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "The Grand Spa Image", ".png", "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png", 1003615L, "/ApplicationImages/3cc52c88-5c58-481e-9f5a-533152911041.png", true, false, "The Grand Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Lotus Spa Image", ".png", "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", 551515L, "/ApplicationImages/ab90af7f-6576-4f63-bce8-bae4d687de0b.png", true, false, "Lotus Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Maia Spa Image", ".png", "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png", 844391L, "/ApplicationImages/be451c4b-e9d3-44d3-834c-28b94c352353.png", true, false, "Maia Spa Image", 1, 1, (byte)0, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "DocumentTemplateTranslations",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "Description", "DocumentTemplateId", "IsActive", "IsDelete", "LanguageCode", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "<h3>THÔNG BÁO XỬ LÝ DỮ LIỆU CÁ NHÂN</h3>\r\nThông báo xử lý dữ liệu cá nhân này quy định cách thức Công ty TNHH Dự án Hồ Tràm (\"HTP\") thu thập và xử lý dữ liệu cá nhân của Quý khách, bao gồm dữ liệu cá nhân liên quan đến sức khỏe mà Quý khách tự nguyện cung cấp, liên quan đến việc đặt dịch vụ, tư vấn, điều trị và sử dụng các dịch vụ Spa và chăm sóc sức khỏe tại The Grand Ho Tram. Thông báo này cũng nêu rõ các quyền của Quý khách với tư cách là chủ thể dữ liệu và các nghĩa vụ của HTP theo quy định của pháp luật về bảo vệ dữ liệu cá nhân hiện hành.\r\nCác thông tin dưới đây được cung cấp theo quy định của pháp luật về bảo vệ dữ liệu cá nhân hiện hành:\r\n <ol>\r\n  <li><strong>Mục đích xử lý (“Mục đích”):</strong> Đánh giá sự phù hợp của Quý khách đối với các liệu trình Spa; cung cấp dịch vụ Spa và chăm sóc sức khỏe một cách an toàn và hiệu quả; lưu trữ hồ sơ điều trị; quản lý lịch hẹn và thanh toán; tiếp nhận và xử lý yêu cầu của khách hàng; bảo đảm sức khỏe, an toàn và an ninh của khách hàng và nhân viên; bảo đảm an ninh, an toàn tại khuôn viên (bao gồm thông qua hệ thống camera giám sát (CCTV)); giải quyết khiếu nại; phục vụ công tác quản trị nội bộ và kiểm soát chất lượng; và tuân thủ các quy định pháp luật hiện hành cũng như các yêu cầu hợp pháp của cơ quan nhà nước có thẩm quyền.</li>\r\n  <li><strong>Dữ liệu cá nhân được xử lý:</strong> Dữ liệu cá nhân do Quý khách cung cấp hoặc được phát sinh liên quan đến việc đặt hoặc sử dụng dịch vụ Spa, bao gồm: họ và tên; ngày tháng năm sinh; quốc tịch; số giấy tờ định danh cá nhân hoặc số hộ chiếu; thông tin liên lạc (địa chỉ, số điện thoại, email); thông tin thanh toán và tài chính; số phòng; thông tin liên quan đến sức khỏe (chẳng hạn như tình trạng bệnh lý, dị ứng, tình trạng mang thai, các vấn đề về da và các thông tin khác có liên quan đến liệu trình điều trị) cần thiết để cung cấp dịch vụ Spa hoặc bảo đảm an toàn tại khuôn viên. Thông tin liên quan đến sức khỏe là dữ liệu cá nhân nhạy cảm theo quy định của pháp luật về bảo vệ dữ liệu cá nhân hiện hành và chỉ được xử lý nhằm mục đích đánh giá sự phù hợp của Quý khách đối với liệu trình điều trị, bảo đảm an toàn cho Quý khách và cung cấp dịch vụ Spa phù hợp, trên cơ sở sự đồng ý rõ ràng của Quý khách hoặc cơ sở pháp lý khác theo quy định của pháp luật hiện hành. Trường hợp Quý khách tự nguyện cung cấp dữ liệu liên quan đến bên thứ ba (như khách đi cùng), Quý khách xác nhận rằng cá nhân đó đã được thông báo và đồng ý đối với việc xử lý dữ liệu được mô tả trong Thông báo này.</li>\r\n  <li><strong>Phương thức xử lý:</strong> HTP xử lý dữ liệu cá nhân bằng cả phương pháp tự động và thủ công, bao gồm một hoặc nhiều hoạt động sau: thu thập, ghi, phân tích, lưu trữ, chỉnh sửa, tiết lộ, kết hợp, truy cập, truy xuất, thu hồi, mã hóa, giải mã, sao chép, chuyển giao, xóa, hủy dữ liệu cá nhân và các hành động khác có tác động đến dữ liệu cá nhân. Toàn bộ dữ liệu cá nhân của Khách hàng sẽ được lưu trữ trong hệ thống cơ sở dữ liệu khách hàng của HTP. Hệ thống này chỉ cho phép nhân sự được ủy quyền tiếp cận và chỉ phục vụ duy nhất cho các Mục đích nêu trên. Trong trường hợp dữ liệu cá nhân được chuyển giao biên giới để thực hiện một hoặc nhiều Mục đích nói trên, HTP cam kết tuân thủ đầy đủ các quy định pháp luật hiện hành về bảo vệ dữ liệu cá nhân.</li>\r\n  <li><strong>Tổ chức, cá nhân được phép xử lý dữ liệu cá nhân: </strong> HTP, các bộ phận chuyên môn và văn phòng của HTP; các cơ sở y tế và đơn vị cung cấp dịch vụ chăm sóc sức khỏe; các đơn vị xử lý thanh toán và các nhà cung cấp dịch vụ khác (bao gồm Công ty TNHH Tổng công ty Công nghệ và Giải pháp CMC); và các cơ quan nhà nước có thẩm quyền mà HTP và các văn phòng của HTP có nghĩa vụ cung cấp thông tin theo chức năng, nhiệm vụ và/hoặc quy định của pháp luật Việt Nam. Khi xử lý dữ liệu cá nhân của Khách hàng cho các Mục đích nêu trên, HTP sẽ đóng vai trò là Bên Kiểm soát dữ liệu cá nhân (trong trường hợp HTP quyết định mục đích và phương tiện xử lý dữ liệu cá nhân); hoặc Bên Kiểm soát đồng thời là Bên Xử lý dữ liệu cá nhân (trong trường hợp HTP vừa quyết định mục đích, phương tiện, vừa trực tiếp xử lý dữ liệu cá nhân).</li>\r\n  <li><strong>Rủi ro tiềm ẩn và hậu quả không mong muốn:</strong> Mặc dù chúng tôi đã áp dụng các biện pháp bảo mật nghiêm ngặt, xin Quý khách lưu ý rằng dữ liệu được truyền tải qua môi trường Internet có thể không an toàn tuyệt đối. HTP không chịu trách nhiệm đối với các hành vi truy cập trái phép, việc Quý khách tự nguyện chia sẻ thông tin, hoặc mất mát dữ liệu do các lỗi kỹ thuật nằm ngoài tầm kiểm soát hợp lý của chúng tôi.</li>\r\n  <li><strong>Quyền của Quý khách:</strong> Theo quy định của pháp luật hiện hành, Quý khách có các quyền sau: (i) Được biết về hoạt động xử lý dữ liệu cá nhân của mình; (ii) Truy cập dữ liệu cá nhân của mình đang do HTP lưu trữ; (iii) Yêu cầu đính chính dữ liệu không chính xác hoặc không đầy đủ; (iv) Yêu cầu xóa dữ liệu cá nhân của mình trong các trường hợp pháp luật cho phép; (v) Hạn chế hoặc phản đối việc xử lý dữ liệu cá nhân; (vi) Yêu cầu cung cấp một bản sao dữ liệu cá nhân của mình theo định dạng cấu trúc, thông dụng (quyền chuyển đổi dữ liệu); (vii) Rút lại sự đồng ý vào bất kỳ lúc nào (việc rút lại không ảnh hưởng đến tính hợp pháp của các hoạt động xử lý dữ liệu đã thực hiện trước khi rút lại); và (viii) Khiếu nại, tố cáo hoặc khởi kiện đến cơ quan có thẩm quyền. Để thực hiện bất kỳ quyền nào nêu trên, vui lòng liên hệ với HTP theo thông tin bên dưới.</li>\r\n  <li><strong>Thời hạn xử lý dữ liệu cá nhân:</strong> HTP sẽ lưu trữ dữ liệu cá nhân của Quý khách trong thời gian cần thiết để hoàn thành các Mục đích được mô tả trong Thông báo này, và trong một khoảng thời gian dài hơn theo yêu cầu hoặc sự cho phép của pháp luật hiện hành (bao gồm các quy định về thời hạn lưu trữ bắt buộc đối với hồ sơ thuế, kế toán, bản tư vấn liệu trình Spa và hồ sơ tuân thủ hoạt động). Sau khi hết thời hạn lưu trữ áp dụng, dữ liệu cá nhân của Quý khách sẽ được xóa bỏ hoặc mã hóa/vô danh hóa một cách an toàn.</li>\r\n</ol>\r\nĐể biết thêm thông tin chi tiết, vui lòng truy cập <a href=\"https://thegrandhotram.com/privacy-policy\" target=\"_blank\">https://thegrandhotram.com/privacy-policy</a> hoặc liên hệ với Cán bộ Bảo vệ Dữ liệu của chúng tôi qua email: <a href=\"data.privacy@thegrandhotram.com\" target=\"_blank\">data.privacy@thegrandhotram.com</a>.", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Thông báo xử lý dữ liệu cá nhân này quy định cách thức Công ty TNHH Dự án Hồ Tràm.", 1, true, false, "vi", "THÔNG BÁO XỬ LÝ DỮ LIỆU CÁ NHÂN", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "<h3>PERSONAL DATA PROCESSING NOTICE</h3>\r\nThis Personal Data Protection Notice sets out how Ho Tram Project Co., Ltd. (\"HTP\") collects and processes your personal data including health-related personal data that you voluntarily provide, in connection with your booking, consultation, treatment and use of Spa and wellness services at The Grand Ho Tram. It also outlines your rights as a data subject and HTP's obligations under applicable PDP Laws.\r\nThe following information is provided in accordance with applicable personal data protection laws:\r\n<ol>\r\n  <li><strong>Processing Purposes (“Purposes”):</strong> To assess your suitability for Spa treatments; provide Spa and wellness services safely and effectively; maintain treatment records; manage appointments and payments; respond to customer requests; ensure the health, safety and security of guests and employees; premises security and safety (including via CCTV surveillance); handle complaints; conduct internal administration and quality assurance; and comply with applicable laws and lawful requests from competent authorities.</li>\r\n  <li><strong>Personal Data Processed:</strong> Personal data that you provide or that is generated in connection with your booking or use of Spa services, including: full name; date of birth; nationality; identity document or passport number; contact information (address, telephone number, email); payment and financial information; room number; and, health-related information (such as medical conditions, allergies, pregnancy status, skin concerns and other information relevant to your treatment) necessary to provide Spa services or ensure safety on premises. Health-related information constitutes sensitive personal data under applicable personal data protection laws and will be processed only for the purpose of assessing your suitability for treatment, ensuring your safety, and providing appropriate Spa services, based on your explicit consent or other lawful basis permitted by applicable law. Where you voluntarily provide data relating to a third party (such as accompanying guests), you confirm that such person has been informed of and consented to the processing described in this Notice.</li>\r\n  <li><strong>Methods of Processing:</strong> HTP processes personal data using both automated and manual methods, including one or more of the following: collection, analysis, summary, encryption, decryption, modification, deletion, destruction, de-identification, provision, disclosure, transfer of personal data, and other activities impacting personal data. All personal data of Customers will be stored in HTP's Customer database system, which is accessible only to authorized personnel and solely for the Purposes. In the event that personal data is transferred across borders for one or more of the aforementioned Purposes, HTP commits to complying fully with all applicable personal data protection regulations.</li>\r\n  <li><strong>Organizations and Individuals Permitted to Process Personal Data:</strong> HTP, HTP's professional departments and offices; medical facilities and healthcare providers; payment processing units and other service providers (including CMC Technology and Solutions Corporation Limited); and competent state authorities to whom HTP and its offices are obligated to provide information in accordance with their duties and/or the laws of Vietnam. When processing the Customer's personal data for the aforementioned Purposes, HTP assumes the role of Personal Data Controller where HTP determines the purposes and means of processing personal data; or Personal Data Controller-cum-Processor where HTP determines the purposes, means, and directly processes personal data.</li>\r\n  <li><strong>Potential risks and unintended consequences:</strong> Although we have implemented robust security measures, please be advised that data transmitted over the Internet may not be entirely secure. HTP cannot be held liable for unauthorized access, your voluntary sharing, or data loss resulting from technical errors beyond our control.</li>\r\n  <li><strong>Your Rights:</strong> Subject to applicable law, you have the right to: (i) be informed of the processing of your personal data; (ii) access your personal data held by HTP; (iii) request correction of inaccurate or incomplete data; (iv) request deletion of your personal data where permitted by law; (v) restrict or object to the processing of your personal data; (vi) request a copy of your personal data in a structured, machine-readable format (data portability); (vii) withdraw your consent at any time (without affecting the lawfulness of processing prior to withdrawal); and (viii) lodge a complaint with the competent authority. To exercise any of the above rights, please contact HTP using the details below.</li>\r\n  <li><strong>Duration of personal data processing:</strong> HTP will retain your personal data for as long as necessary to fulfil the Purposes described in this Notice, and for such further period as may be required or permitted by applicable law (including statutory retention periods applicable to tax and accounting records, Spa consultation form and compliance records). Upon expiry of the applicable retention period, your personal data will be securely deleted or de-identified.</li>\r\n</ol>\r\n\r\nFor further information, please visit <a href=\"https://thegrandhotram.com/privacy-policy\" target=\"_blank\">https://thegrandhotram.com/privacy-policy</a> or contact our Data Protection Officer at <a href=\"https://thegrandhotram.com/privacy-policy\" target=\"_blank\">data.privacy@thegrandhotram.com</a>.", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "This Personal Data Protection Notice sets out how Ho Tram Project Co., Ltd. (\"HTP\") ", 1, true, false, "en", "PERSONAL DATA PROCESSING NOTICE", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "EmployeeRoles",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "EmployeeId", "IsActive", "IsDelete", "RoleId", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, true, false, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" });

            migrationBuilder.InsertData(
                table: "FormQuestions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "FollowUpLabel", "FollowUpTriggerOption", "FormTemplateId", "HasFollowUpText", "IsActive", "IsDelete", "IsRequired", "QuestionText", "QuestionType", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, true, "Have you experienced spa treatments before?", 2, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "How are you feeling right now?", 3, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "How do you want to feel after the treatment?", 3, 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "For massages: what treatment pressure do you prefer?", 3, 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:", "Yes", 1, true, true, false, false, "For a body massage, are there any specific areas we should focus on?", 2, 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "For massages, are there any areas we should avoid?", 1, 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "Have you ever, or are you suffering from any of the following?", 3, 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 1, false, true, false, false, "Are you sensitive or allergic to any of the following?", 3, 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, please briefly describe:", "Yes", 1, true, true, false, false, "Have you recently had an operation?", 2, 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, which trimester?", "Yes", 1, true, true, false, false, "For women: Are you pregnant?", 2, 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 11, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, true, "Have you experienced spa treatments before?", 2, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 12, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "How are you feeling right now?", 3, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 13, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "How do you want to feel after the treatment?", 3, 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 14, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "For massages: what treatment pressure do you prefer?", 3, 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 15, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:", "Yes", 2, true, true, false, false, "For a body massage, are there any specific areas we should focus on?", 2, 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 16, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "For massages, are there any areas we should avoid?", 1, 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 17, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "Have you ever, or are you suffering from any of the following?", 3, 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 18, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 2, false, true, false, false, "Are you sensitive or allergic to any of the following?", 3, 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 19, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, please briefly describe:", "Yes", 2, true, true, false, false, "Have you recently had an operation?", 2, 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 20, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, which trimester?", "Yes", 2, true, true, false, false, "For women: Are you pregnant?", 2, 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 21, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, true, "Have you experienced spa treatments before?", 2, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 22, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "How are you feeling right now?", 3, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 23, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "How do you want to feel after the treatment?", 3, 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 24, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "For massages: what treatment pressure do you prefer?", 3, 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 25, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "For facial massage: Do you have any special skin concerns related to your face? If yes, please briefly describe:", "Yes", 3, true, true, false, false, "For a body massage, are there any specific areas we should focus on?", 2, 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 26, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "For massages, are there any areas we should avoid?", 1, 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 27, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "Have you ever, or are you suffering from any of the following?", 3, 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 28, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, 3, false, true, false, false, "Are you sensitive or allergic to any of the following?", 3, 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 29, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, please briefly describe:", "Yes", 3, true, true, false, false, "Have you recently had an operation?", 2, 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 30, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "If yes, which trimester?", "Yes", 3, true, true, false, false, "For women: Are you pregnant?", 2, 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "FormTemplateTranslations",
                columns: new[] { "Id", "AgreementText", "CreatedAt", "CreatedBy", "Description", "FooterText", "FormTemplateId", "IsActive", "IsDelete", "LanguageCode", "QuestionsTranslation", "Title", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, "Tôi đồng ý với các điều khoản và điều kiện trên.", new DateTime(2026, 8, 6, 11, 54, 50, 33, DateTimeKind.Local).AddTicks(8915), "System", "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:", "Tôi xác nhận rằng các liệu trình tại The Grand Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Grand Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.", 1, true, false, "vi", "[\r\n  {\"questionId\":1,\"questionText\":\"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?\",\"options\":[{\"optionId\":1,\"optionText\":\"Có\"},{\"optionId\":2,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":2,\"questionText\":\"Hiện tại bạn đang cảm thấy thế nào?\",\"options\":[{\"optionId\":3,\"optionText\":\"Mệt Mỏi\"},{\"optionId\":4,\"optionText\":\"Căng Thẳng\"},{\"optionId\":5,\"optionText\":\"Nhức Cơ Bắp\"},{\"optionId\":6,\"optionText\":\"Bình Thường\"}]},\r\n  {\"questionId\":3,\"questionText\":\"Bạn muốn cảm thấy thế nào sau khi trị liệu?\",\"options\":[{\"optionId\":7,\"optionText\":\"Yên Bình\"},{\"optionId\":8,\"optionText\":\"Tươi Mới\"},{\"optionId\":9,\"optionText\":\"Phấn Khởi\"},{\"optionId\":10,\"optionText\":\"Đầy Năng Lượng\"}]},\r\n  {\"questionId\":4,\"questionText\":\"Với Massage: Bạn mong muốn dùng lực thế nào?\",\"options\":[{\"optionId\":11,\"optionText\":\"Mạnh\"},{\"optionId\":12,\"optionText\":\"Trung Bình\"},{\"optionId\":13,\"optionText\":\"Nhẹ\"},{\"optionId\":14,\"optionText\":\"Cần Thử Lực\"}]},\r\n  {\"questionId\":5,\"questionText\":\"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?\",\"followUpLabel\":\"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":15,\"optionText\":\"Có\"},{\"optionId\":16,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":6,\"questionText\":\"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?\",\"options\":[]},\r\n  {\"questionId\":7,\"questionText\":\"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?\",\"options\":[{\"optionId\":17,\"optionText\":\"Tiểu Đường\"},{\"optionId\":18,\"optionText\":\"Động Kinh\"},{\"optionId\":19,\"optionText\":\"Hen Suyễn\"},{\"optionId\":20,\"optionText\":\"Ngất Xỉu\"},{\"optionId\":21,\"optionText\":\"Đau Cơ\"},{\"optionId\":22,\"optionText\":\"Vấn Đề Tiêu Hóa\"},{\"optionId\":23,\"optionText\":\"Huyết Áp Cao/ Thấp\"},{\"optionId\":24,\"optionText\":\"Bệnh Về Da\"}]},\r\n  {\"questionId\":8,\"questionText\":\"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?\",\"options\":[{\"optionId\":25,\"optionText\":\"Thức Ăn\"},{\"optionId\":26,\"optionText\":\"Thuốc\"},{\"optionId\":27,\"optionText\":\"Tinh Dầu\"}]},\r\n  {\"questionId\":9,\"questionText\":\"Gần đây bạn có phẫu thuật không?\",\"followUpLabel\":\"Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":28,\"optionText\":\"Có\"},{\"optionId\":29,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":10,\"questionText\":\"Với Phụ Nữ: Bạn đang có thai không?\",\"followUpLabel\":\"Nếu có, Bạn đang ở quý thai kỳ nào?\",\"options\":[{\"optionId\":30,\"optionText\":\"Có\"},{\"optionId\":31,\"optionText\":\"Không\"}]}\r\n]", "TƯ VẤN SPA THE GRAND", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, "I agree to the above terms and conditions.", new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(263), "System", "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:", "I acknowledge that the treatments at The Grand Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Grand Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.", 1, true, false, "en", "[\r\n  {\"questionId\":1,\"questionText\":\"Have you had a Spa treatment experience before?\",\"options\":[{\"optionId\":1,\"optionText\":\"Yes\"},{\"optionId\":2,\"optionText\":\"No\"}]},\r\n  {\"questionId\":2,\"questionText\":\"How are you feeling right now?\",\"options\":[{\"optionId\":3,\"optionText\":\"Tired\"},{\"optionId\":4,\"optionText\":\"Stressed\"},{\"optionId\":5,\"optionText\":\"Muscle Aches\"},{\"optionId\":6,\"optionText\":\"Normal\"}]},\r\n  {\"questionId\":3,\"questionText\":\"How would you like to feel after the treatment?\",\"options\":[{\"optionId\":7,\"optionText\":\"Peaceful\"},{\"optionId\":8,\"optionText\":\"Refreshed\"},{\"optionId\":9,\"optionText\":\"Invigorated\"},{\"optionId\":10,\"optionText\":\"Energized\"}]},\r\n  {\"questionId\":4,\"questionText\":\"For Massage: How would you like the pressure to be?\",\"options\":[{\"optionId\":11,\"optionText\":\"Strong\"},{\"optionId\":12,\"optionText\":\"Medium\"},{\"optionId\":13,\"optionText\":\"Light\"},{\"optionId\":14,\"optionText\":\"Need to Test\"}]},\r\n  {\"questionId\":5,\"questionText\":\"For Body Massage: Are there any areas we should focus on?\",\"followUpLabel\":\"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:\",\"options\":[{\"optionId\":15,\"optionText\":\"Yes\"},{\"optionId\":16,\"optionText\":\"No\"}]},\r\n  {\"questionId\":6,\"questionText\":\"For massage: Are there any areas you would like to avoid?\",\"options\":[]},\r\n  {\"questionId\":7,\"questionText\":\"Have you ever had or are currently experiencing any of the following health issues?\",\"options\":[{\"optionId\":17,\"optionText\":\"Diabetes\"},{\"optionId\":18,\"optionText\":\"Epilepsy\"},{\"optionId\":19,\"optionText\":\"Asthma\"},{\"optionId\":20,\"optionText\":\"Fainting\"},{\"optionId\":21,\"optionText\":\"Muscle Pain\"},{\"optionId\":22,\"optionText\":\"Digestive Issues\"},{\"optionId\":23,\"optionText\":\"High/Low Blood Pressure\"},{\"optionId\":24,\"optionText\":\"Skin Conditions\"}]},\r\n  {\"questionId\":8,\"questionText\":\"Are you allergic or sensitive to any of the following?\",\"options\":[{\"optionId\":25,\"optionText\":\"Food\"},{\"optionId\":26,\"optionText\":\"Medication\"},{\"optionId\":27,\"optionText\":\"Essential Oils\"}]},\r\n  {\"questionId\":9,\"questionText\":\"Have you had any recent surgeries?\",\"followUpLabel\":\"If yes, please describe briefly:\",\"options\":[{\"optionId\":28,\"optionText\":\"Yes\"},{\"optionId\":29,\"optionText\":\"No\"}]},\r\n  {\"questionId\":10,\"questionText\":\"For Women: Are you currently pregnant?\",\"followUpLabel\":\"If yes, which trimester are you in?\",\"options\":[{\"optionId\":30,\"optionText\":\"Yes\"},{\"optionId\":31,\"optionText\":\"No\"}]}\r\n]", "THE GRAND SPA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, "Tôi đồng ý với các điều khoản và điều kiện trên.", new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(270), "System", "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:", "Tôi xác nhận rằng các liệu trình tại The Maia Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Maia Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.", 2, true, false, "vi", "[\r\n  {\"questionId\":11,\"questionText\":\"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?\",\"options\":[{\"optionId\":32,\"optionText\":\"Có\"},{\"optionId\":33,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":12,\"questionText\":\"Hiện tại bạn đang cảm thấy thế nào?\",\"options\":[{\"optionId\":34,\"optionText\":\"Mệt Mỏi\"},{\"optionId\":35,\"optionText\":\"Căng Thẳng\"},{\"optionId\":36,\"optionText\":\"Nhức Cơ Bắp\"},{\"optionId\":37,\"optionText\":\"Bình Thường\"}]},\r\n  {\"questionId\":13,\"questionText\":\"Bạn muốn cảm thấy thế nào sau khi trị liệu?\",\"options\":[{\"optionId\":38,\"optionText\":\"Yên Bình\"},{\"optionId\":39,\"optionText\":\"Tươi Mới\"},{\"optionId\":40,\"optionText\":\"Phấn Khởi\"},{\"optionId\":41,\"optionText\":\"Đầy Năng Lượng\"}]},\r\n  {\"questionId\":14,\"questionText\":\"Với Massage: Bạn mong muốn dùng lực thế nào?\",\"options\":[{\"optionId\":42,\"optionText\":\"Mạnh\"},{\"optionId\":43,\"optionText\":\"Trung Bình\"},{\"optionId\":44,\"optionText\":\"Nhẹ\"},{\"optionId\":45,\"optionText\":\"Cần Thử Lực\"}]},\r\n  {\"questionId\":15,\"questionText\":\"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?\",\"followUpLabel\":\"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":46,\"optionText\":\"Có\"},{\"optionId\":47,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":16,\"questionText\":\"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?\",\"options\":[]},\r\n  {\"questionId\":17,\"questionText\":\"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?\",\"options\":[{\"optionId\":48,\"optionText\":\"Tiểu Đường\"},{\"optionId\":49,\"optionText\":\"Động Kinh\"},{\"optionId\":50,\"optionText\":\"Hen Suyễn\"},{\"optionId\":51,\"optionText\":\"Ngất Xỉu\"},{\"optionId\":52,\"optionText\":\"Đau Cơ\"},{\"optionId\":53,\"optionText\":\"Vấn Đề Tiêu Hóa\"},{\"optionId\":54,\"optionText\":\"Huyết Áp Cao/ Thấp\"},{\"optionId\":55,\"optionText\":\"Bệnh Về Da\"}]},\r\n  {\"questionId\":18,\"questionText\":\"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?\",\"options\":[{\"optionId\":56,\"optionText\":\"Thức Ăn\"},{\"optionId\":57,\"optionText\":\"Thuốc\"},{\"optionId\":58,\"optionText\":\"Tinh Dầu\"}]},\r\n  {\"questionId\":19,\"questionText\":\"Gần đây bạn có phẫu thuật không?\",\"followUpLabel\":\"Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":59,\"optionText\":\"Có\"},{\"optionId\":60,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":20,\"questionText\":\"Với Phụ Nữ: Bạn đang có thai không?\",\"followUpLabel\":\"Nếu có, Bạn đang ở quý thai kỳ nào?\",\"options\":[{\"optionId\":61,\"optionText\":\"Có\"},{\"optionId\":62,\"optionText\":\"Không\"}]}\r\n]", "TƯ VẤN SPA THE MAIA", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, "I agree to the above terms and conditions.", new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(272), "System", "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:", "I acknowledge that the treatments at The Maia Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Maia Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.", 2, true, false, "en", "[\r\n {\"questionId\":11,\"questionText\":\"Have you had a Spa treatment experience before?\",\"options\":[{\"optionId\":32,\"optionText\":\"Yes\"},{\"optionId\":33,\"optionText\":\"No\"}]},\r\n {\"questionId\":12,\"questionText\":\"How are you feeling right now?\",\"options\":[{\"optionId\":34,\"optionText\":\"Tired\"},{\"optionId\":35,\"optionText\":\"Stressed\"},{\"optionId\":36,\"optionText\":\"Muscle Aches\"},{\"optionId\":37,\"optionText\":\"Normal\"}]},\r\n {\"questionId\":13,\"questionText\":\"How would you like to feel after the treatment?\",\"options\":[{\"optionId\":38,\"optionText\":\"Peaceful\"},{\"optionId\":39,\"optionText\":\"Refreshed\"},{\"optionId\":40,\"optionText\":\"Invigorated\"},{\"optionId\":41,\"optionText\":\"Energized\"}]},\r\n {\"questionId\":14,\"questionText\":\"For Massage: How would you like the pressure to be?\",\"options\":[{\"optionId\":42,\"optionText\":\"Strong\"},{\"optionId\":43,\"optionText\":\"Medium\"},{\"optionId\":44,\"optionText\":\"Light\"},{\"optionId\":45,\"optionText\":\"Need to Test\"}]},\r\n {\"questionId\":15,\"questionText\":\"For Body Massage: Are there any areas we should focus on?\",\"followUpLabel\":\"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:\",\"options\":[{\"optionId\":46,\"optionText\":\"Yes\"},{\"optionId\":47,\"optionText\":\"No\"}]},\r\n {\"questionId\":16,\"questionText\":\"For massage: Are there any areas you would like to avoid?\",\"options\":[]},\r\n {\"questionId\":17,\"questionText\":\"Have you ever had or are currently experiencing any of the following health issues?\",\"options\":[{\"optionId\":48,\"optionText\":\"Diabetes\"},{\"optionId\":49,\"optionText\":\"Epilepsy\"},{\"optionId\":50,\"optionText\":\"Asthma\"},{\"optionId\":51,\"optionText\":\"Fainting\"},{\"optionId\":52,\"optionText\":\"Muscle Pain\"},{\"optionId\":53,\"optionText\":\"Digestive Issues\"},{\"optionId\":54,\"optionText\":\"High/Low Blood Pressure\"},{\"optionId\":55,\"optionText\":\"Skin Conditions\"}]},\r\n {\"questionId\":18,\"questionText\":\"Are you allergic or sensitive to any of the following?\",\"options\":[{\"optionId\":56,\"optionText\":\"Food\"},{\"optionId\":57,\"optionText\":\"Medication\"},{\"optionId\":58,\"optionText\":\"Essential Oils\"}]},\r\n {\"questionId\":19,\"questionText\":\"Have you had any recent surgeries?\",\"followUpLabel\":\"If yes, please describe briefly:\",\"options\":[{\"optionId\":59,\"optionText\":\"Yes\"},{\"optionId\":60,\"optionText\":\"No\"}]},\r\n {\"questionId\":20,\"questionText\":\"For Women: Are you currently pregnant?\",\"followUpLabel\":\"If yes, which trimester are you in?\",\"options\":[{\"optionId\":61,\"optionText\":\"Yes\"},{\"optionId\":62,\"optionText\":\"No\"}]}\r\n]", "THE MAIA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, "Tôi đồng ý với các điều khoản và điều kiện trên.", new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(273), "System", "Vui lòng dành một chút thời gian để điền đầy đủ vào mẫu này, vì câu trả lời của bạn sẽ giúp chúng tôi cung cấp dịch vụ điều trị an toàn và hiệu quả:", "Tôi xác nhận rằng các liệu trình tại The Lotus Spa không mang tính y tế. Tôi xác nhận rằng tôi đã điền đầy đủ và chính xác vào mẫu tư vấn này và miễn trừ trách nhiệm cho The Lotus Spa, khách sạn và nhân viên của khách sạn đối với bất kỳ trách nhiệm pháp lý hoặc khiếu nại nào.\r\n\r\nTôi hiểu rằng việc hủy hoặc đổi lịch phải được thực hiện ít nhất 24 giờ trước để tránh bị tính phí 100% chi phí của liệu trình đã chọn.", 3, true, false, "vi", "[\r\n  {\"questionId\":21,\"questionText\":\"Bạn đã từng có trải nghiệm trị liệu Spa trước đây chưa?\",\"options\":[{\"optionId\":63,\"optionText\":\"Có\"},{\"optionId\":64,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":22,\"questionText\":\"Hiện tại bạn đang cảm thấy thế nào?\",\"options\":[{\"optionId\":65,\"optionText\":\"Mệt Mỏi\"},{\"optionId\":66,\"optionText\":\"Căng Thẳng\"},{\"optionId\":67,\"optionText\":\"Nhức Cơ Bắp\"},{\"optionId\":68,\"optionText\":\"Bình Thường\"}]},\r\n  {\"questionId\":23,\"questionText\":\"Bạn muốn cảm thấy thế nào sau khi trị liệu?\",\"options\":[{\"optionId\":69,\"optionText\":\"Yên Bình\"},{\"optionId\":70,\"optionText\":\"Tươi Mới\"},{\"optionId\":71,\"optionText\":\"Phấn Khởi\"},{\"optionId\":72,\"optionText\":\"Đầy Năng Lượng\"}]},\r\n  {\"questionId\":24,\"questionText\":\"Với Massage: Bạn mong muốn dùng lực thế nào?\",\"options\":[{\"optionId\":73,\"optionText\":\"Mạnh\"},{\"optionId\":74,\"optionText\":\"Trung Bình\"},{\"optionId\":75,\"optionText\":\"Nhẹ\"},{\"optionId\":76,\"optionText\":\"Cần Thử Lực\"}]},\r\n  {\"questionId\":25,\"questionText\":\"Với Massage thân thể: Có khu vực nào chúng tôi nên tập trung không?\",\"followUpLabel\":\"Với Massage mặt: Bạn có đang gặp phải bất kỳ vấn đề gì về da không? Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":77,\"optionText\":\"Có\"},{\"optionId\":78,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":26,\"questionText\":\"Với massage: Bạn có muốn tránh bất cứ khu vực nào không?\",\"options\":[]},\r\n  {\"questionId\":27,\"questionText\":\"Bạn đã bao giờ hay đang gặp phải các vấn đề về sức khỏe dưới đây?\",\"options\":[{\"optionId\":79,\"optionText\":\"Tiểu Đường\"},{\"optionId\":80,\"optionText\":\"Động Kinh\"},{\"optionId\":81,\"optionText\":\"Hen Suyễn\"},{\"optionId\":82,\"optionText\":\"Ngất Xỉu\"},{\"optionId\":83,\"optionText\":\"Đau Cơ\"},{\"optionId\":84,\"optionText\":\"Vấn Đề Tiêu Hóa\"},{\"optionId\":85,\"optionText\":\"Huyết Áp Cao/ Thấp\"},{\"optionId\":86,\"optionText\":\"Bệnh Về Da\"}]},\r\n  {\"questionId\":28,\"questionText\":\"Bạn có bị dị ứng hoặc nhạy cảm với các thứ dưới đây không?\",\"options\":[{\"optionId\":87,\"optionText\":\"Thức Ăn\"},{\"optionId\":88,\"optionText\":\"Thuốc\"},{\"optionId\":89,\"optionText\":\"Tinh Dầu\"}]},\r\n  {\"questionId\":29,\"questionText\":\"Gần đây bạn có phẫu thuật không?\",\"followUpLabel\":\"Nếu có, vui lòng miêu tả ngắn gọn:\",\"options\":[{\"optionId\":90,\"optionText\":\"Có\"},{\"optionId\":91,\"optionText\":\"Không\"}]},\r\n  {\"questionId\":30,\"questionText\":\"Với Phụ Nữ: Bạn đang có thai không?\",\"followUpLabel\":\"Nếu có, Bạn đang ở quý thai kỳ nào?\",\"options\":[{\"optionId\":92,\"optionText\":\"Có\"},{\"optionId\":93,\"optionText\":\"Không\"}]}\r\n]", "TƯ VẤN SPA THE LOTUS", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, "I agree to the above terms and conditions.", new DateTime(2026, 8, 6, 11, 54, 50, 34, DateTimeKind.Local).AddTicks(275), "System", "Please take a moment to fill out this form completely, as your answers will help us provide safe and effective treatment services:", "I acknowledge that the treatments at The Lotus Spa are not medical in nature. I confirm that I have completed this consultation form fully and accurately and release The Lotus Spa, the hotel, and the hotel's staff from any legal liability or claims.\r\n\r\nI understand that cancellations or rescheduling must be made at least 24 hours in advance to avoid being charged 100% of the selected treatment cost.", 3, true, false, "en", "[\r\n{\"questionId\":21,\"questionText\":\"Have you had a Spa treatment experience before?\",\"options\":[{\"optionId\":63,\"optionText\":\"Yes\"},{\"optionId\":64,\"optionText\":\"No\"}]},\r\n{\"questionId\":22,\"questionText\":\"How are you feeling right now?\",\"options\":[{\"optionId\":65,\"optionText\":\"Tired\"},{\"optionId\":66,\"optionText\":\"Stressed\"},{\"optionId\":67,\"optionText\":\"Muscle Aches\"},{\"optionId\":68,\"optionText\":\"Normal\"}]},\r\n{\"questionId\":23,\"questionText\":\"How would you like to feel after the treatment?\",\"options\":[{\"optionId\":69,\"optionText\":\"Peaceful\"},{\"optionId\":70,\"optionText\":\"Refreshed\"},{\"optionId\":71,\"optionText\":\"Invigorated\"},{\"optionId\":72,\"optionText\":\"Energized\"}]},\r\n{\"questionId\":24,\"questionText\":\"For Massage: How would you like the pressure to be?\",\"options\":[{\"optionId\":73,\"optionText\":\"Strong\"},{\"optionId\":74,\"optionText\":\"Medium\"},{\"optionId\":75,\"optionText\":\"Light\"},{\"optionId\":76,\"optionText\":\"Need to Test\"}]},\r\n{\"questionId\":25,\"questionText\":\"For Body Massage: Are there any areas we should focus on?\",\"followUpLabel\":\"For Facial Massage: Are you currently experiencing any skin concerns? If yes, please describe briefly:\",\"options\":[{\"optionId\":77,\"optionText\":\"Yes\"},{\"optionId\":78,\"optionText\":\"No\"}]},\r\n{\"questionId\":26,\"questionText\":\"For massage: Are there any areas you would like to avoid?\",\"options\":[]},\r\n{\"questionId\":27,\"questionText\":\"Have you ever had or are currently experiencing any of the following health issues?\",\"options\":[{\"optionId\":79,\"optionText\":\"Diabetes\"},{\"optionId\":80,\"optionText\":\"Epilepsy\"},{\"optionId\":81,\"optionText\":\"Asthma\"},{\"optionId\":82,\"optionText\":\"Fainting\"},{\"optionId\":83,\"optionText\":\"Muscle Pain\"},{\"optionId\":84,\"optionText\":\"Digestive Issues\"},{\"optionId\":85,\"optionText\":\"High/Low Blood Pressure\"},{\"optionId\":86,\"optionText\":\"Skin Conditions\"}]},\r\n{\"questionId\":28,\"questionText\":\"Are you allergic or sensitive to any of the following?\",\"options\":[{\"optionId\":87,\"optionText\":\"Food\"},{\"optionId\":88,\"optionText\":\"Medication\"},{\"optionId\":89,\"optionText\":\"Essential Oils\"}]},\r\n{\"questionId\":29,\"questionText\":\"Have you had any recent surgeries?\",\"followUpLabel\":\"If yes, please describe briefly:\",\"options\":[{\"optionId\":90,\"optionText\":\"Yes\"},{\"optionId\":91,\"optionText\":\"No\"}]},\r\n{\"questionId\":30,\"questionText\":\"For Women: Are you currently pregnant?\",\"followUpLabel\":\"If yes, which trimester are you in?\",\"options\":[{\"optionId\":92,\"optionText\":\"Yes\"},{\"optionId\":93,\"optionText\":\"No\"}]}\r\n]", "THE LOTUS SPA CONSULTATION", new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "PropertyOutlets",
                columns: new[] { "OutletId", "PropertyId", "CreatedAt", "CreatedBy", "Id", "IsActive", "IsDelete", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, true, false, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, true, false, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, true, false, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

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

            migrationBuilder.InsertData(
                table: "WorkflowDefinitions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Description", "IsActive", "IsDelete", "Name", "OutletId", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Complete spa intake flow: consultation form → liability signature → PDP consent.", true, false, "Default", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Complete spa intake flow: consultation form → liability signature → PDP consent.", true, false, "Spa Full Journey", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", "Abbreviated flow for returning guests: liability signature → PDP consent only.", false, false, "Spa Document-Only", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "FormQuestionOptions",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "FormQuestionId", "IsActive", "IsDelete", "OptionText", "SortOrder", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, true, false, "Tired", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, true, false, "Stressed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, true, false, "Muscle Tension", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2, true, false, "Calm", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, true, false, "Peaceful", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, true, false, "Refreshed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 9, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, true, false, "Vibrant", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 10, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, true, false, "Energized", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 11, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 4, true, false, "Strong", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 12, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 4, true, false, "Medium", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 13, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 4, true, false, "Light", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 14, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 4, true, false, "Don't know", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 15, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 5, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 16, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 5, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 17, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Diabetes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 18, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Epilepsy", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 19, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Asthma", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 20, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Fainting", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 21, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Muscle Aches", 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 22, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Digestive Problems", 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 23, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "High/ Low Blood Pressure", 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 24, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 7, true, false, "Skin Diseases", 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 25, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 8, true, false, "Food", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 26, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 8, true, false, "Medication", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 27, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 8, true, false, "Essential Oils", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 28, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 9, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 29, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 9, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 30, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 10, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 31, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 10, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 32, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 11, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 33, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 11, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 34, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 12, true, false, "Tired", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 35, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 12, true, false, "Stressed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 36, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 12, true, false, "Muscle Tension", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 37, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 12, true, false, "Calm", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 38, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 13, true, false, "Peaceful", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 39, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 13, true, false, "Refreshed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 40, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 13, true, false, "Vibrant", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 41, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 13, true, false, "Energized", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 42, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 14, true, false, "Strong", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 43, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 14, true, false, "Medium", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 44, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 14, true, false, "Light", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 45, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 14, true, false, "Don't know", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 46, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 15, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 47, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 15, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 48, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Diabetes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 49, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Epilepsy", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 50, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Asthma", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 51, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Fainting", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 52, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Muscle Aches", 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 53, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Digestive Problems", 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 54, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "High/ Low Blood Pressure", 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 55, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 17, true, false, "Skin Diseases", 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 56, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 18, true, false, "Food", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 57, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 18, true, false, "Medication", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 58, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 18, true, false, "Essential Oils", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 59, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 19, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 60, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 19, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 61, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 20, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 62, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 20, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 63, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 21, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 64, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 21, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 65, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 22, true, false, "Tired", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 66, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 22, true, false, "Stressed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 67, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 22, true, false, "Muscle Tension", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 68, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 22, true, false, "Calm", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 69, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 23, true, false, "Peaceful", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 70, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 23, true, false, "Refreshed", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 71, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 23, true, false, "Vibrant", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 72, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 23, true, false, "Energized", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 73, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 24, true, false, "Strong", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 74, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 24, true, false, "Medium", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 75, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 24, true, false, "Light", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 76, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 24, true, false, "Don't know", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 77, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 25, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 78, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 25, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 79, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Diabetes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 80, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Epilepsy", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 81, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Asthma", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 82, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Fainting", 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 83, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Muscle Aches", 5, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 84, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Digestive Problems", 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 85, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "High/ Low Blood Pressure", 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 86, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 27, true, false, "Skin Diseases", 8, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 87, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 28, true, false, "Food", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 88, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 28, true, false, "Medication", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 89, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 28, true, false, "Essential Oils", 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 90, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 29, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 91, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 29, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 92, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 30, true, false, "Yes", 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" },
                    { 93, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 30, true, false, "No", 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System" }
                });

            migrationBuilder.InsertData(
                table: "WorkflowSteps",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "DocumentTemplateId", "FormTemplateId", "IsActive", "IsDelete", "StepLabel", "StepOrder", "StepType", "UpdatedAt", "UpdatedBy", "WorkflowDefinitionId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, true, false, "Default Consultation Form", 1, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 3, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, null, true, false, "Personal Data Processing Consent", 2, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1 },
                    { 4, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, 1, true, false, "Spa Consultation Form", 1, 1, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2 },
                    { 6, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 1, null, true, false, "Personal Data Processing Consent", 2, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 2 },
                    { 7, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3, null, true, false, "Spa Liability Release", 1, 2, new DateTime(2026, 5, 19, 0, 0, 0, 0, DateTimeKind.Utc), "System", 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationImages_OutletId",
                table: "ApplicationImages",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationImages_PropertyId",
                table: "ApplicationImages",
                column: "PropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationSettings_Key",
                table: "ApplicationSettings",
                column: "Key",
                unique: true);

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
                name: "IX_DeviceMappings_PatronDeviceId_IsActive",
                table: "DeviceMappings",
                columns: new[] { "PatronDeviceId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMappings_StaffDeviceId_IsActive",
                table: "DeviceMappings",
                columns: new[] { "StaffDeviceId", "IsActive" },
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DeviceMappings_StaffDeviceId1",
                table: "DeviceMappings",
                column: "StaffDeviceId1");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplates_OutletId",
                table: "DocumentTemplates",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplateTranslations_DocumentTemplateId",
                table: "DocumentTemplateTranslations",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTemplateVersionHistories_DocumentTemplateId",
                table: "DocumentTemplateVersionHistories",
                column: "DocumentTemplateId");

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
                name: "IX_FormQuestionOptions_FormQuestionId",
                table: "FormQuestionOptions",
                column: "FormQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormQuestions_FormTemplateId",
                table: "FormQuestions",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissionAnswers_FormQuestionId",
                table: "FormSubmissionAnswers",
                column: "FormQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissionAnswers_FormSubmissionId",
                table: "FormSubmissionAnswers",
                column: "FormSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_FormTemplateId",
                table: "FormSubmissions",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormSubmissions_PatronDeviceId",
                table: "FormSubmissions",
                column: "PatronDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplateTranslations_FormTemplateId",
                table: "FormTemplateTranslations",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_FormTemplateVersionHistories_FormTemplateId",
                table: "FormTemplateVersionHistories",
                column: "FormTemplateId");

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
                name: "IX_Patron_OutletId",
                table: "Patron",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_Patron_PatronTypeId",
                table: "Patron",
                column: "PatronTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PatronDevices_ConnectionId",
                table: "PatronDevices",
                column: "ConnectionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatronDevices_DeviceName",
                table: "PatronDevices",
                column: "DeviceName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatronSignature_PatronId",
                table: "PatronSignature",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true,
                filter: "[PermissionCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyOutlets_OutletId",
                table: "PropertyOutlets",
                column: "OutletId");

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

            migrationBuilder.CreateIndex(
                name: "IX_SignatureSessions_PatronDeviceId",
                table: "SignatureSessions",
                column: "PatronDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_SignatureSessions_PatronId",
                table: "SignatureSessions",
                column: "PatronId");

            migrationBuilder.CreateIndex(
                name: "IX_SignatureSessions_StaffDeviceId",
                table: "SignatureSessions",
                column: "StaffDeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffDevices_MacAddress",
                table: "StaffDevices",
                column: "MacAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffDevices_OutletId",
                table: "StaffDevices",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowDefinitions_OutletId",
                table: "WorkflowDefinitions",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_DocumentTemplateId",
                table: "WorkflowSteps",
                column: "DocumentTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_FormTemplateId",
                table: "WorkflowSteps",
                column: "FormTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowSteps_WorkflowDefinitionId",
                table: "WorkflowSteps",
                column: "WorkflowDefinitionId");
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
                name: "Countries");

            migrationBuilder.DropTable(
                name: "DeviceMappings");

            migrationBuilder.DropTable(
                name: "DocumentTemplateTranslations");

            migrationBuilder.DropTable(
                name: "DocumentTemplateVersionHistories");

            migrationBuilder.DropTable(
                name: "EmployeeRoles");

            migrationBuilder.DropTable(
                name: "FormQuestionOptions");

            migrationBuilder.DropTable(
                name: "FormSubmissionAnswers");

            migrationBuilder.DropTable(
                name: "FormTemplateTranslations");

            migrationBuilder.DropTable(
                name: "FormTemplateVersionHistories");

            migrationBuilder.DropTable(
                name: "Languages");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "PatronSignature");

            migrationBuilder.DropTable(
                name: "PropertyOutlets");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "SignatureSessions");

            migrationBuilder.DropTable(
                name: "WorkflowSteps");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "FormQuestions");

            migrationBuilder.DropTable(
                name: "FormSubmissions");

            migrationBuilder.DropTable(
                name: "Properties");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Patron");

            migrationBuilder.DropTable(
                name: "StaffDevices");

            migrationBuilder.DropTable(
                name: "DocumentTemplates");

            migrationBuilder.DropTable(
                name: "WorkflowDefinitions");

            migrationBuilder.DropTable(
                name: "FormTemplates");

            migrationBuilder.DropTable(
                name: "PatronDevices");

            migrationBuilder.DropTable(
                name: "PatronTypes");

            migrationBuilder.DropTable(
                name: "Outlets");
        }
    }
}
