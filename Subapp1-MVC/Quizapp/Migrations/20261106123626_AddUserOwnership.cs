using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quizapp.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOwnership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuizAttempts",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Flashcards",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_AspNetUsers_UserId",
                table: "Flashcards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizAttempts_AspNetUsers_UserId",
                table: "QuizAttempts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_AspNetUsers_UserId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_QuizAttempts_AspNetUsers_UserId",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_QuizAttempts_UserId",
                table: "QuizAttempts");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuizAttempts");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Flashcards");
        }
    }
}
