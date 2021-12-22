using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class PhishingTemplateUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "From",
                table: "PhishingTemplate",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HtmlTemplateFilename",
                table: "PhishingTemplate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "PhishingTemplate",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "To",
                table: "PhishingTemplate",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "From",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "HtmlTemplateFilename",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "To",
                table: "PhishingTemplate");
        }
    }
}
