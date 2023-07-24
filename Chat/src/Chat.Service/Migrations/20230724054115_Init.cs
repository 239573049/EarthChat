using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Chat.Service.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Cotnent = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Extends = table.Column<Dictionary<string, string>>(type: "hstore", nullable: false),
                    Creator = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<int>(type: "integer", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Account = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Avatar = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Extends = table.Column<string>(type: "text", nullable: false),
                    Creator = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Modifier = table.Column<Guid>(type: "uuid", nullable: false),
                    ModificationTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "Extends", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("ff66b799-bc8b-4ff2-bc30-2f08cab7e3cc"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 7, 24, 5, 41, 15, 247, DateTimeKind.Utc).AddTicks(958), new Guid("00000000-0000-0000-0000-000000000000"), "{}", new DateTime(2023, 7, 24, 5, 41, 15, 247, DateTimeKind.Utc).AddTicks(962), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "123456" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_Id",
                table: "ChatMessages",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Account",
                table: "Users",
                column: "Account",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
