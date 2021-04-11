using Microsoft.EntityFrameworkCore.Migrations;

namespace StackOverflow.Data.Migrations
{
    public partial class answerRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuestionId",
                table: "Answer",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Answer_Questions_QuestionId",
                table: "Answer",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Answer_Questions_QuestionId",
                table: "Answer");

            migrationBuilder.DropIndex(
                name: "IX_Answer_QuestionId",
                table: "Answer");

            migrationBuilder.DropColumn(
                name: "QuestionId",
                table: "Answer");
        }
    }
}
