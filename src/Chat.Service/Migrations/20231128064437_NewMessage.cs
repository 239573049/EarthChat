using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Service.Migrations
{
    public partial class NewMessage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"), new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303") });

            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"), new Guid("99c5439e-b481-4ae8-b6f5-3b29c112d731") });

            migrationBuilder.DeleteData(
                table: "ChatGroups",
                keyColumn: "Id",
                keyValue: new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("99c5439e-b481-4ae8-b6f5-3b29c112d731"));

            migrationBuilder.AddColumn<string>(
                name: "NewMessage",
                table: "Friends",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NewMessage",
                table: "ChatGroups",
                type: "text",
                nullable: true);

            migrationBuilder.InsertData(
                table: "ChatGroups",
                columns: new[] { "Id", "Avatar", "CreationTime", "Creator", "Default", "Description", "ModificationTime", "Modifier", "Name", "NewMessage" },
                values: new object[] { new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"), "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(323), new Guid("00000000-0000-0000-0000-000000000000"), true, "世界频道，所有人默认加入的频道", new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(323), new Guid("00000000-0000-0000-0000-000000000000"), "世界频道", null });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303"),
                columns: new[] { "CreationTime", "ModificationTime", "Password" },
                values: new object[] { new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(306), new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(307), "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "GiteeId", "GithubId", "Ip", "Location", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("5365f9af-f99c-4415-a4ac-428d6c7f8805"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(245), new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, new DateTime(2023, 11, 28, 14, 44, 36, 912, DateTimeKind.Local).AddTicks(262), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "ChatGroupInUsers",
                columns: new[] { "ChatGroupId", "UserId", "Id" },
                values: new object[,]
                {
                    { new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"), new Guid("5365f9af-f99c-4415-a4ac-428d6c7f8805"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"), new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"), new Guid("5365f9af-f99c-4415-a4ac-428d6c7f8805") });

            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"), new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303") });

            migrationBuilder.DeleteData(
                table: "ChatGroups",
                keyColumn: "Id",
                keyValue: new Guid("ceff724a-9bf7-45c7-a722-08ef28b786c3"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5365f9af-f99c-4415-a4ac-428d6c7f8805"));

            migrationBuilder.DropColumn(
                name: "NewMessage",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "NewMessage",
                table: "ChatGroups");

            migrationBuilder.InsertData(
                table: "ChatGroups",
                columns: new[] { "Id", "Avatar", "CreationTime", "Creator", "Default", "Description", "ModificationTime", "Modifier", "Name" },
                values: new object[] { new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"), "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3929), new Guid("00000000-0000-0000-0000-000000000000"), true, "世界频道，所有人默认加入的频道", new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3929), new Guid("00000000-0000-0000-0000-000000000000"), "世界频道" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303"),
                columns: new[] { "CreationTime", "ModificationTime", "Password" },
                values: new object[] { new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3918), new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3918), "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "GiteeId", "GithubId", "Ip", "Location", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("99c5439e-b481-4ae8-b6f5-3b29c112d731"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3863), new Guid("00000000-0000-0000-0000-000000000000"), null, null, null, null, new DateTime(2023, 10, 19, 18, 58, 56, 755, DateTimeKind.Local).AddTicks(3880), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "ChatGroupInUsers",
                columns: new[] { "ChatGroupId", "UserId", "Id" },
                values: new object[,]
                {
                    { new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"), new Guid("6d53f694-4221-4e87-b8b2-2f54e8929303"), new Guid("00000000-0000-0000-0000-000000000000") },
                    { new Guid("3b887aac-8655-4e00-9d25-8925c75a8143"), new Guid("99c5439e-b481-4ae8-b6f5-3b29c112d731"), new Guid("00000000-0000-0000-0000-000000000000") }
                });
        }
    }
}
