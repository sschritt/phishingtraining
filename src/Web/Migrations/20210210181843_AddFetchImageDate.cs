using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class AddFetchImageDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FetchImageDate",
                table: "PhishingMessage",
                type: "datetimeoffset",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FetchImageDate",
                table: "PhishingMessage");
        }
    }
}
