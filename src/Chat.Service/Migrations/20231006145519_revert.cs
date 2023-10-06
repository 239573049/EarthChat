using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Service.Migrations
{
    public partial class revert : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages");

            migrationBuilder.DeleteData(
                table: "ChatGroupInUsers",
                keyColumns: new[] { "ChatGroupId", "UserId" },
                keyValues: new object[] { new Guid("b41a5f0a-d0e2-47f0-8aef-eb6f083bd720"), new Guid("2ba08fb3-5cc4-4342-8e15-862cfee9b429") });

            migrationBuilder.DeleteData(
                table: "ChatGroups",
                keyColumn: "Id",
                keyValue: new Guid("b41a5f0a-d0e2-47f0-8aef-eb6f083bd720"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("2ba08fb3-5cc4-4342-8e15-862cfee9b429"));

            migrationBuilder.AddColumn<Guid>(
                name: "RevertId",
                table: "ChatMessages",
                type: "uuid",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "RevertId",
                table: "ChatMessages");

            migrationBuilder.InsertData(
                table: "ChatGroups",
                columns: new[] { "Id", "Avatar", "CreationTime", "Creator", "Default", "Description", "ModificationTime", "Modifier", "Name" },
                values: new object[] { new Guid("b41a5f0a-d0e2-47f0-8aef-eb6f083bd720"), "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 9, 29, 1, 28, 35, 463, DateTimeKind.Local).AddTicks(936), new Guid("00000000-0000-0000-0000-000000000000"), true, "世界频道，所有人默认加入的频道", new DateTime(2023, 9, 29, 1, 28, 35, 463, DateTimeKind.Local).AddTicks(937), new Guid("00000000-0000-0000-0000-000000000000"), "世界频道" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "Avatar", "CreationTime", "Creator", "GiteeId", "GithubId", "ModificationTime", "Modifier", "Name", "Password" },
                values: new object[] { new Guid("2ba08fb3-5cc4-4342-8e15-862cfee9b429"), "admin", "https://avatars.githubusercontent.com/u/17716615?v=4", new DateTime(2023, 9, 29, 1, 28, 35, 463, DateTimeKind.Local).AddTicks(804), new Guid("00000000-0000-0000-0000-000000000000"), null, null, new DateTime(2023, 9, 29, 1, 28, 35, 463, DateTimeKind.Local).AddTicks(816), new Guid("00000000-0000-0000-0000-000000000000"), "管理员", "3786F993CB0AF43E" });

            migrationBuilder.InsertData(
                table: "ChatGroupInUsers",
                columns: new[] { "ChatGroupId", "UserId", "Id" },
                values: new object[] { new Guid("b41a5f0a-d0e2-47f0-8aef-eb6f083bd720"), new Guid("2ba08fb3-5cc4-4342-8e15-862cfee9b429"), new Guid("00000000-0000-0000-0000-000000000000") });

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_UserId",
                table: "ChatMessages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
