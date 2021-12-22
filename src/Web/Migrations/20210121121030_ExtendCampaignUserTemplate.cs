using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class ExtendCampaignUserTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Difficulty",
                table: "PhishingTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "FromTimeOfDay",
                table: "PhishingTemplate",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinSecondsBetweenMessages",
                table: "PhishingTemplate",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SendType",
                table: "PhishingTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ToTimeOfDay",
                table: "PhishingTemplate",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Birthdate",
                table: "ParticipantDisplayModel",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassTeacher",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Director",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InformaticsTeacher",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "School",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Birthdate",
                table: "AspNetUsers",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Difficulty",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "FromTimeOfDay",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "MinSecondsBetweenMessages",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "SendType",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "ToTimeOfDay",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ClassTeacher",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "Director",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "InformaticsTeacher",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "School",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "Gender",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthdate",
                table: "ParticipantDisplayModel",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Birthdate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }
    }
}
