using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctoralManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixActivityDocumentsIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityDocument_OnePerEntity",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_ConferenceId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_DoctoralProjectId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_MobilityId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_PublicationId",
                table: "ActivityDocuments");

            migrationBuilder.DropColumn(
                name: "ActivityDocumentId",
                table: "DoctoralProjects");

            migrationBuilder.DropColumn(
                name: "ProposalDocumentPath",
                table: "DoctoralProjects");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_ConferenceId",
                table: "ActivityDocuments",
                column: "ConferenceId",
                unique: true,
                filter: "\"ConferenceId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_DoctoralProjectId",
                table: "ActivityDocuments",
                column: "DoctoralProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_MobilityId",
                table: "ActivityDocuments",
                column: "MobilityId",
                unique: true,
                filter: "\"MobilityId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityDocuments_PublicationId",
                table: "ActivityDocuments",
                column: "PublicationId",
                unique: true,
                filter: "\"PublicationId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_ConferenceId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_DoctoralProjectId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_MobilityId",
                table: "ActivityDocuments");

            migrationBuilder.DropIndex(
                name: "IX_ActivityDocuments_PublicationId",
                table: "ActivityDocuments");

            migrationBuilder.AddColumn<int>(
                name: "ActivityDocumentId",
                table: "DoctoralProjects",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProposalDocumentPath",
                table: "DoctoralProjects",
                type: "text",
                nullable: true);

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
        }
    }
}
