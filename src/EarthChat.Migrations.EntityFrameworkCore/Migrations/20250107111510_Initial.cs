using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarthChat.Migrations.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user-oauth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Provider = table.Column<string>(type: "text", nullable: false, comment: "OAuth 提供者"),
                    ProviderUserId = table.Column<string>(type: "text", nullable: false, comment: "OAuth 提供者用户 ID"),
                    AccessToken = table.Column<string>(type: "text", nullable: false),
                    AccessTokenExpiresAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatableId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatableId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user-oauth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Avatar = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false, comment: "头像"),
                    Username = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false, comment: "用户名"),
                    Email = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "邮箱"),
                    Signature = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true, comment: "个性签名"),
                    PasswordHash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false, comment: "密码哈希值"),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Roles = table.Column<List<string>>(type: "text[]", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatableId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatableId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user-oauth_Provider_ProviderUserId",
                table: "user-oauth",
                columns: new[] { "Provider", "ProviderUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user-oauth_Provider_ProviderUserId_UserId",
                table: "user-oauth",
                columns: new[] { "Provider", "ProviderUserId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_user-oauth_UserId",
                table: "user-oauth",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_users_Email",
                table: "users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_Username",
                table: "users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user-oauth");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
