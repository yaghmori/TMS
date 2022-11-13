using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TMS.Infrastructure.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsReadOnly = table.Column<bool>(type: "bit", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CultureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedCultureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDatePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongDatePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LongTimePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortTimePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullDateTimePattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateSeparator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeSeparator = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    YearMonthPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MonthDayPattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstDayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    RightToLeft = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NormalizedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DBProvider = table.Column<int>(type: "int", nullable: true),
                    ConnectionString = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProfileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailVerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EmailVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumberVerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhoneNumberVerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PasswordResetedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FatherName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NationalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EconomicalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthIdentityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BirthIdentitySerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalInsuranceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InsuranceHistory = table.Column<int>(type: "int", nullable: true),
                    Children = table.Column<int>(type: "int", nullable: true),
                    FamilyCount = table.Column<int>(type: "int", nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BirthStatusDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MarriageDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EducationalField = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FieldOfActivity = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Persons_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuildVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccessToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SessionIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSessions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DefaultTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Theme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RightToLeft = table.Column<bool>(type: "bit", nullable: false),
                    DarkMode = table.Column<bool>(type: "bit", nullable: false),
                    Culture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_Tenant_DefaultTenantId",
                        column: x => x.DefaultTenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserSettings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTenants_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTenants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedIpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserToken_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AppSettings",
                columns: new[] { "Id", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "Description", "IsReadOnly", "Key", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "Value" },
                values: new object[] { new Guid("225ab570-cfa8-4ee5-baaa-7d934d7fa1b3"), new DateTime(2022, 11, 13, 20, 50, 19, 339, DateTimeKind.Utc).AddTicks(1779), null, null, null, null, "DefaultConnectionString", null, null, null, "Server=.;Database={0};Integrated Security = True;" });

            migrationBuilder.InsertData(
                table: "Cultures",
                columns: new[] { "Id", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "CultureName", "DateSeparator", "DisplayName", "FirstDayOfWeek", "FullDateTimePattern", "Image", "IsDefault", "LongDatePattern", "LongTimePattern", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "MonthDayPattern", "NormalizedCultureName", "NormalizedDisplayName", "RightToLeft", "ShortDatePattern", "ShortTimePattern", "TimeSeparator", "YearMonthPattern" },
                values: new object[,]
                {
                    { new Guid("746a9237-cf40-49da-8bb1-12fcff344bb1"), new DateTime(2022, 11, 13, 20, 50, 18, 763, DateTimeKind.Utc).AddTicks(4029), null, null, "fa-IR", "/", "فارسی", 6, "dddd, MMMM dd, yyyy h:mm:ss tt", "_content/TMS.RootComponents/assets/media/flags/iran.svg", false, "dddd, MMMM dd, yyyy ", "HH:mm:ss", null, null, null, "MMMM dd", "FA-IR", "فارسی", true, "yyyy/MM/dd", "HH:mm", ":", "MMMM, yyyy" },
                    { new Guid("c3c4fbe8-d03d-4cec-b6ed-654c9ea2f39f"), new DateTime(2022, 11, 13, 20, 50, 18, 763, DateTimeKind.Utc).AddTicks(82), null, null, "en-US", "/", "English", 1, "dddd, MMMM dd, yyyy h:mm:ss tt", "_content/TMS.RootComponents/assets/media/flags/usa.svg", true, "dddd, MMMM dd, yyyy", "HH:mm:ss", null, null, null, "MMMM dd", "EN-US", "ENGLISH", false, "yyyy/MM/dd", "HH:mm", ":", "MMMM, yyyy" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2"), null, new DateTime(2022, 11, 13, 20, 50, 18, 754, DateTimeKind.Utc).AddTicks(1098), null, null, null, null, null, "sysadmin", "SYSADMIN" },
                    { new Guid("b9afe837-7566-4516-93fa-cdc3d0f9289d"), null, new DateTime(2022, 11, 13, 20, 50, 18, 762, DateTimeKind.Utc).AddTicks(8427), null, null, null, null, null, "admin", "ADMIN" },
                    { new Guid("c3c1384e-9822-49f1-97c5-0d065426329b"), null, new DateTime(2022, 11, 13, 20, 50, 18, 762, DateTimeKind.Utc).AddTicks(8457), null, null, null, null, null, "user", "USER" },
                    { new Guid("fe299f1c-4a8c-4f2b-8c9e-52091391781e"), null, new DateTime(2022, 11, 13, 20, 50, 18, 762, DateTimeKind.Utc).AddTicks(8460), null, null, null, null, null, "owner", "OWNER" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "Description", "Email", "EmailConfirmed", "EmailTokenExpires", "EmailVerificationToken", "EmailVerifiedAt", "FirstName", "Image", "IsActive", "LastName", "LockoutEnabled", "LockoutEnd", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PasswordResetToken", "PasswordResetedAt", "PasswordTokenExpires", "PhoneNumber", "PhoneNumberConfirmed", "PhoneNumberTokenExpires", "PhoneNumberVerificationToken", "PhoneNumberVerifiedAt", "ProfileName", "SecurityStamp", "SettingsId", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("dc206917-2b17-45ca-9929-72cc08ad2f1d"), 0, "deccfa19-fd07-465f-9763-7035c8f846c3", null, null, null, "", "sysadmin@tms.com", true, null, null, null, "sys", "", true, "admin", false, null, null, null, null, "SYSADMIN@TMS.COM", "SYSADMIN", "AQAAAAIAAYagAAAAEObbwkOKs64vSYqAxteyPEQAXDQRm6Qj1+pMhRyJ8GoRlOInlzVziXjVarcortxJ0w==", null, null, null, "", true, null, null, null, "System Admin", "f5320257-95df-47c5-9419-3786e96992e5", new Guid("00000000-0000-0000-0000-000000000000"), false, "sysadmin" });

            migrationBuilder.InsertData(
                table: "RoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "RoleId" },
                values: new object[,]
                {
                    { 1, "permission", "SensorHistorie.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 2, "permission", "SensorHistorie.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 3, "permission", "SensorHistorie.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 4, "permission", "SensorHistorie.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 5, "permission", "SensorHistorie.Export", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 6, "permission", "SensorHistorie.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 7, "permission", "ProjectSetting.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 8, "permission", "ProjectSetting.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 9, "permission", "UserClaim.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 10, "permission", "UserClaim.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 11, "permission", "Client.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 12, "permission", "Client.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 13, "permission", "Client.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 14, "permission", "Client.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 15, "permission", "Client.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 16, "permission", "Client.AddOrRemoveUsers", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 17, "permission", "Client.Configuration", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 18, "permission", "User.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 19, "permission", "User.ViewSessions", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 20, "permission", "User.TerminateSession", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 21, "permission", "User.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 22, "permission", "User.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 23, "permission", "User.AddOrRemovePermissions", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 24, "permission", "User.AddOrRemoveRoles", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 25, "permission", "User.AddOrRemoveTenant", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 26, "permission", "User.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 27, "permission", "User.Export", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 28, "permission", "User.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 29, "permission", "Role.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 30, "permission", "Role.AddOrRemoveUsers", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 31, "permission", "Role.AddOrRemoveClaims", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 32, "permission", "Role.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 33, "permission", "Role.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 34, "permission", "Role.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 35, "permission", "Role.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 36, "permission", "RoleClaim.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 37, "permission", "RoleClaim.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 38, "permission", "RoleClaim.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 39, "permission", "RoleClaim.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 40, "permission", "RoleClaim.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 41, "permission", "Communication.SendMessage", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 42, "permission", "Communication.Chat", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 43, "permission", "Preference.ChangeLanguage", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 44, "permission", "Dashboard.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 45, "permission", "AdminDashboard.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 46, "permission", "AuditTrail.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 47, "permission", "AuditTrail.Export", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 48, "permission", "AuditTrail.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 49, "permission", "IdentityManagement.Menu", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 50, "permission", "IdentityManagement.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 51, "permission", "UserProfile.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 52, "permission", "UserProfile.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 53, "permission", "AppSetting.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 54, "permission", "AppSetting.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 55, "permission", "AppSetting.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 56, "permission", "AppSetting.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 57, "permission", "AppSetting.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 58, "permission", "UserSetting.View", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 59, "permission", "UserSetting.Create", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 60, "permission", "UserSetting.Edit", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 61, "permission", "UserSetting.Delete", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") },
                    { 62, "permission", "UserSetting.Search", null, null, null, null, null, null, new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId" },
                values: new object[] { new Guid("1024cb20-6df9-4ecb-86c8-96a8897e00b2"), new Guid("dc206917-2b17-45ca-9929-72cc08ad2f1d"), null, null, null, null, null, null });

            migrationBuilder.InsertData(
                table: "UserSettings",
                columns: new[] { "Id", "CreatedDate", "CreatedIpAddress", "CreatedUserId", "Culture", "DarkMode", "DefaultTenantId", "ModifiedDate", "ModifiedIpAddress", "ModifiedUserId", "RightToLeft", "Theme", "UserId" },
                values: new object[] { new Guid("4c98fef7-0fab-4c27-84cf-45b1e88ff1e3"), new DateTime(2022, 11, 13, 20, 50, 19, 339, DateTimeKind.Utc).AddTicks(1652), null, null, "en-US", false, null, null, null, null, false, "", new Guid("dc206917-2b17-45ca-9929-72cc08ad2f1d") });

            migrationBuilder.CreateIndex(
                name: "IX_Persons_UserId",
                table: "Persons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserSessions_UserId",
                table: "UserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_DefaultTenantId",
                table: "UserSettings",
                column: "DefaultTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTenants_TenantId",
                table: "UserTenants",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenants_UserId",
                table: "UserTenants",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "Cultures");

            migrationBuilder.DropTable(
                name: "Persons");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "UserSettings");

            migrationBuilder.DropTable(
                name: "UserTenants");

            migrationBuilder.DropTable(
                name: "UserToken");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Tenant");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
