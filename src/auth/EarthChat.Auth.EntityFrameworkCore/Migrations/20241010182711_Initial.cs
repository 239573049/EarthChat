using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EarthChat.Auth.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "openiddictapplications",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    applicationtype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    clientid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    clientsecret = table.Column<string>(type: "text", nullable: true),
                    clienttype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    concurrencytoken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    consenttype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    displayname = table.Column<string>(type: "text", nullable: true),
                    displaynames = table.Column<string>(type: "text", nullable: true),
                    jsonwebkeyset = table.Column<string>(type: "text", nullable: true),
                    permissions = table.Column<string>(type: "text", nullable: true),
                    postlogoutredirecturis = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redirecturis = table.Column<string>(type: "text", nullable: true),
                    requirements = table.Column<string>(type: "text", nullable: true),
                    settings = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddictapplications", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddictscopes",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    concurrencytoken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    descriptions = table.Column<string>(type: "text", nullable: true),
                    displayname = table.Column<string>(type: "text", nullable: true),
                    displaynames = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    resources = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddictscopes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "openiddictauthorizations",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    applicationid = table.Column<string>(type: "text", nullable: true),
                    concurrencytoken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    scopes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddictauthorizations", x => x.id);
                    table.ForeignKey(
                        name: "FK_openiddictauthorizations_openiddictapplications_application~",
                        column: x => x.applicationid,
                        principalTable: "openiddictapplications",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "openiddicttokens",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    applicationid = table.Column<string>(type: "text", nullable: true),
                    authorizationid = table.Column<string>(type: "text", nullable: true),
                    concurrencytoken = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    expirationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    payload = table.Column<string>(type: "text", nullable: true),
                    properties = table.Column<string>(type: "text", nullable: true),
                    redemptiondate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    referenceid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    subject = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: true),
                    type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_openiddicttokens", x => x.id);
                    table.ForeignKey(
                        name: "FK_openiddicttokens_openiddictapplications_applicationid",
                        column: x => x.applicationid,
                        principalTable: "openiddictapplications",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_openiddicttokens_openiddictauthorizations_authorizationid",
                        column: x => x.authorizationid,
                        principalTable: "openiddictauthorizations",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_openiddictapplications_clientid",
                table: "openiddictapplications",
                column: "clientid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_openiddictauthorizations_applicationid_status_subject_type",
                table: "openiddictauthorizations",
                columns: new[] { "applicationid", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "IX_openiddictscopes_name",
                table: "openiddictscopes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_openiddicttokens_applicationid_status_subject_type",
                table: "openiddicttokens",
                columns: new[] { "applicationid", "status", "subject", "type" });

            migrationBuilder.CreateIndex(
                name: "IX_openiddicttokens_authorizationid",
                table: "openiddicttokens",
                column: "authorizationid");

            migrationBuilder.CreateIndex(
                name: "IX_openiddicttokens_referenceid",
                table: "openiddicttokens",
                column: "referenceid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "openiddictscopes");

            migrationBuilder.DropTable(
                name: "openiddicttokens");

            migrationBuilder.DropTable(
                name: "openiddictauthorizations");

            migrationBuilder.DropTable(
                name: "openiddictapplications");
        }
    }
}
