using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Service.Migrations
{
    public partial class countermand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("eb0549f8-074e-42b0-b727-63316666138b"), new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2") });

            migrationBuilder.DeleteData(
                table: "ChatGroups",
                keyColumn: "Id",
                keyValue: new Guid("eb0549f8-074e-42b0-b727-63316666138b"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2"));

            migrationBuilder.AddColumn<bool>(
                name: "Countermand",
                table: "ChatMessages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "ChatGroups",
                columns: new[] { "Id", "Avatar", "CreationTime", "Creator", "Default", "Description", "ModificationTime", "Modifier", "Name" },
                values: new object[] { new Guid("4a78d446-155d-4f4f-8eb8-535347148047"), "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 6, 23, 54, 34, 673, DateTimeKind.Local).AddTicks(1598), new Guid("00000000-0000-0000-0000-000000000000"), true, "世界频道，所有人默认加入的频道", new DateTime(2023, 10, 6, 23, 54, 34, 673, DateTimeKind.Local).AddTicks(1598), new Guid("00000000-0000-0000-0000-000000000000"), "世界频道" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "GiteeId", "GithubId", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("392e0e36-e072-4a6c-a096-64467f8a89d5"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 6, 23, 54, 34, 673, DateTimeKind.Local).AddTicks(1477), new Guid("00000000-0000-0000-0000-000000000000"), null, null, new DateTime(2023, 10, 6, 23, 54, 34, 673, DateTimeKind.Local).AddTicks(1489), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "ChatGroupInUsers",
                columns: new[] { "ChatGroupId", "UserId", "Id" },
                values: new object[] { new Guid("4a78d446-155d-4f4f-8eb8-535347148047"), new Guid("392e0e36-e072-4a6c-a096-64467f8a89d5"), new Guid("00000000-0000-0000-0000-000000000000") });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("4a78d446-155d-4f4f-8eb8-535347148047"), new Guid("392e0e36-e072-4a6c-a096-64467f8a89d5") });

            migrationBuilder.DeleteData(
                table: "ChatGroups",
                keyColumn: "Id",
                keyValue: new Guid("4a78d446-155d-4f4f-8eb8-535347148047"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("392e0e36-e072-4a6c-a096-64467f8a89d5"));

            migrationBuilder.DropColumn(
                name: "Countermand",
                table: "ChatMessages");

            migrationBuilder.InsertData(
                table: "ChatGroups",
                columns: new[] { "Id", "Avatar", "CreationTime", "Creator", "Default", "Description", "ModificationTime", "Modifier", "Name" },
                values: new object[] { new Guid("eb0549f8-074e-42b0-b727-63316666138b"), "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3760), new Guid("00000000-0000-0000-0000-000000000000"), true, "世界频道，所有人默认加入的频道", new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3761), new Guid("00000000-0000-0000-0000-000000000000"), "世界频道" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "GiteeId", "GithubId", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3597), new Guid("00000000-0000-0000-0000-000000000000"), null, null, new DateTime(2023, 10, 6, 22, 55, 18, 917, DateTimeKind.Local).AddTicks(3612), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "ChatGroupInUsers",
                columns: new[] { "ChatGroupId", "UserId", "Id" },
                values: new object[] { new Guid("eb0549f8-074e-42b0-b727-63316666138b"), new Guid("cf7531f0-ac7d-427d-8393-426cf442b1f2"), new Guid("00000000-0000-0000-0000-000000000000") });
        }
    }
}
