using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class RemoveUnusedFieldsFromCampagin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessagesClicked",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "MessagesPlanned",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "MessagesSent",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ParticipantsCount",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "TemplateUsageCount",
                table: "Campaign");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MessagesClicked",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagesPlanned",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MessagesSent",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParticipantsCount",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TemplateUsageCount",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
