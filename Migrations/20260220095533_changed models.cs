using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_trial.Migrations
{
    /// <inheritdoc />
    public partial class changedmodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_Decision",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Decision",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ReviewedDate",
                table: "Idea");

            migrationBuilder.AlterColumn<string>(
                name: "ReviewedByUserName",
                table: "Idea",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReviewComment",
                table: "Idea",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewComment",
                table: "Idea");

            migrationBuilder.AddColumn<string>(
                name: "Decision",
                table: "Review",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "Review",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReviewedByUserName",
                table: "Idea",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedDate",
                table: "Idea",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Review_Decision",
                table: "Review",
                column: "Decision");
        }
    }
}
