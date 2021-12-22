using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class PhishingMessageToOptionalTemplate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhishingMessage_PhishingTemplate_PhishingTemplateId",
                table: "PhishingMessage");

            migrationBuilder.AlterColumn<Guid>(
                name: "PhishingTemplateId",
                table: "PhishingMessage",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_PhishingMessage_PhishingTemplate_PhishingTemplateId",
                table: "PhishingMessage",
                column: "PhishingTemplateId",
                principalTable: "PhishingTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhishingMessage_PhishingTemplate_PhishingTemplateId",
                table: "PhishingMessage");

            migrationBuilder.AlterColumn<Guid>(
                name: "PhishingTemplateId",
                table: "PhishingMessage",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PhishingMessage_PhishingTemplate_PhishingTemplateId",
                table: "PhishingMessage",
                column: "PhishingTemplateId",
                principalTable: "PhishingTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
