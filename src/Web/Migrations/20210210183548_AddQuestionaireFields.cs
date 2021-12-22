using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class AddQuestionaireFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducationalInformation",
                table: "PhishingTemplate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClickLocation",
                table: "PhishingMessage",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClickSituation",
                table: "PhishingMessage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducationalInformation",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "ClickLocation",
                table: "PhishingMessage");

            migrationBuilder.DropColumn(
                name: "ClickSituation",
                table: "PhishingMessage");
        }
    }
}
