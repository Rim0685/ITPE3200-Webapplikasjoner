using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizApi.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardDeck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeckId",
                table: "Flashcards",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FlashcardDecks",
                columns: table => new
                {
                    FlashcardDeckId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlashcardDecks", x => x.FlashcardDeckId);
                    table.ForeignKey(
                        name: "FK_FlashcardDecks_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_DeckId",
                table: "Flashcards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_FlashcardDecks_UserId",
                table: "FlashcardDecks",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_FlashcardDecks_DeckId",
                table: "Flashcards",
                column: "DeckId",
                principalTable: "FlashcardDecks",
                principalColumn: "FlashcardDeckId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_FlashcardDecks_DeckId",
                table: "Flashcards");

            migrationBuilder.DropTable(
                name: "FlashcardDecks");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_DeckId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "DeckId",
                table: "Flashcards");
        }
    }
}
