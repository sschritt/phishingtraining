using Microsoft.EntityFrameworkCore.Migrations;

namespace PhishingTraining.Web.Migrations
{
    public partial class MergeMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NumberOfMessagesPerParticipant",
                table: "Campaign",
                newName: "NumberOfWhatsAppMessagesPerParticipant");

            migrationBuilder.AddColumn<int>(
                name: "Incentive",
                table: "PhishingTemplate",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "SenderKnown",
                table: "PhishingTemplate",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "School",
                table: "Campaign",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InformaticsTeacher",
                table: "Campaign",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "Campaign",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassTeacher",
                table: "Campaign",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Campaign",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfEMailMessagesPerParticipant",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfInstagramMessagesPerParticipant",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSMSMessagesPerParticipant",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfSnapchatMessagesPerParticipant",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTikTokMessagesPerParticipant",
                table: "Campaign",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ShowHintsOnClick",
                table: "Campaign",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Incentive",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "SenderKnown",
                table: "PhishingTemplate");

            migrationBuilder.DropColumn(
                name: "NumberOfEMailMessagesPerParticipant",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "NumberOfInstagramMessagesPerParticipant",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "NumberOfSMSMessagesPerParticipant",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "NumberOfSnapchatMessagesPerParticipant",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "NumberOfTikTokMessagesPerParticipant",
                table: "Campaign");

            migrationBuilder.DropColumn(
                name: "ShowHintsOnClick",
                table: "Campaign");

            migrationBuilder.RenameColumn(
                name: "NumberOfWhatsAppMessagesPerParticipant",
                table: "Campaign",
                newName: "NumberOfMessagesPerParticipant");

            migrationBuilder.AlterColumn<string>(
                name: "School",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InformaticsTeacher",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Director",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ClassTeacher",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Class",
                table: "Campaign",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
