using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend_trial.Migrations
{
    /// <inheritdoc />
    public partial class ideaNotitfication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Idea_IdeaId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_ReviewerId",
                table: "Notification");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Idea_IdeaId",
                table: "Notification",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "IdeaId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_ReviewerId",
                table: "Notification",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Idea_IdeaId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_ReviewerId",
                table: "Notification");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Idea_IdeaId",
                table: "Notification",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "IdeaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_ReviewerId",
                table: "Notification",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "UserId");
        }
    }
}
