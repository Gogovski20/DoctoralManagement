using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommitteeNotes",
                table: "DoctoralProjects",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DoctoralProjects",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DecisionAt",
                table: "DoctoralProjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposalDocumentPath",
                table: "DoctoralProjects",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "DoctoralProjects",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmittedAt",
                table: "DoctoralProjects",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ConferenceParticipations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<int>(type: "integer", nullable: false),
                    ConferenceName = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    EctsAwarded = table.Column<int>(type: "integer", nullable: false),
                    EvidencePath = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConferenceParticipations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConferenceParticipations_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ThesisDefenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DoctoralProjectId = table.Column<int>(type: "integer", nullable: false),
                    ScheduledAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Room = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CommitteeMemberIds = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ResultNotes = table.Column<string>(type: "text", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ArchiveNumber = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThesisDefenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ThesisDefenses_DoctoralProjects_DoctoralProjectId",
                        column: x => x.DoctoralProjectId,
                        principalTable: "DoctoralProjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConferenceParticipations_StudentId",
                table: "ConferenceParticipations",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ThesisDefenses_DoctoralProjectId",
                table: "ThesisDefenses",
                column: "DoctoralProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConferenceParticipations");

            migrationBuilder.DropTable(
                name: "ThesisDefenses");

            migrationBuilder.DropColumn(
                name: "CommitteeNotes",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "DecisionAt",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "ProposalDocumentPath",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "SubmittedAt",
                table: "DoctoralProjects");
        }
    }
}
