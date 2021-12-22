using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class AddPhishingMessageSurveyResultfields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ClickSituation",
                table: "PhishingMessage",
                newName: "ClickCompany");

            migrationBuilder.AddColumn<string>(
                name: "ClickActivity",
                table: "PhishingMessage",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClickActivity",
                table: "PhishingMessage");

            migrationBuilder.RenameColumn(
                name: "ClickCompany",
                table: "PhishingMessage",
                newName: "ClickSituation");
        }
    }
}
