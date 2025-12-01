using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationAndActivityDocument : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Students",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityDocumentId",
                table: "Publications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Publications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActivityDocumentId",
                table: "Mobilities",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EctsPoints",
                table: "Mobilities",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Mobilities",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ActivityDocumentId",
                table: "DoctoralProjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActivityDocumentId",
                table: "ConferenceParticipations",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "ConferenceParticipations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ActivityDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ReviewComment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ReviewedBy = table.Column<int>(type: "integer", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UploadedBy = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PublicationId = table.Column<int>(type: "integer", nullable: true),
                    MobilityId = table.Column<int>(type: "integer", nullable: true),
                    ConferenceId = table.Column<int>(type: "integer", nullable: true),
                    DoctoralProjectId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivityDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivityDocuments_ConferenceParticipations_ConferenceId",
                        column: x => x.ConferenceId,
                        principalTable: "ConferenceParticipations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityDocuments_DoctoralProjects_DoctoralProjectId",
                        column: x => x.DoctoralProjectId,
                        principalTable: "DoctoralProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityDocuments_Mobilities_MobilityId",
                        column: x => x.MobilityId,
                        principalTable: "Mobilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivityDocuments_Publications_PublicationId",
                        column: x => x.PublicationId,
                        principalTable: "Publications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ApplicationId = table.Column<int>(type: "integer", nullable: false),
                    DocumentType = table.Column<int>(type: "integer", nullable: false),
                    FileName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    FilePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FileSize = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    UploadedBy = table.Column<int>(type: "integer", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationDocuments_Applications_ApplicationId",
                        column: x => x.ApplicationId,
                        principalTable: "Applications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocument_OnePerEntity",
                table: "ActivityDocuments",
                columns: new[] { "PublicationId", "MobilityId", "ConferenceId", "DoctoralProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_ConferenceId",
                table: "ActivityDocuments",
                column: "ConferenceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_DoctoralProjectId",
                table: "ActivityDocuments",
                column: "DoctoralProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_MobilityId",
                table: "ActivityDocuments",
                column: "MobilityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_PublicationId",
                table: "ActivityDocuments",
                column: "PublicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_ApplicationId_DocumentType",
                table: "ApplicationDocuments",
                columns: new[] { "ApplicationId", "DocumentType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivityDocuments");

            migrationBuilder.DropTable(
                name: "ApplicationDocuments");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ActivityDocumentId",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Publications");

            migrationBuilder.DropColumn(
                name: "ActivityDocumentId",
                table: "Mobilities");

            migrationBuilder.DropColumn(
                name: "EctsPoints",
                table: "Mobilities");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Mobilities");

            migrationBuilder.DropColumn(
                name: "ActivityDocumentId",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "ActivityDocumentId",
                table: "ConferenceParticipations");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "ConferenceParticipations");
        }
    }
}
