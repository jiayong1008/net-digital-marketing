using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketing2.Migrations
{
    /// <inheritdoc />
    public partial class fix_student_score_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId1",
                table: "StudentScore");

            migrationBuilder.DropIndex(
                name: "IX_StudentScore_QuizQuestionId1",
                table: "StudentScore");

            migrationBuilder.DropColumn(
                name: "QuizQuestionId1",
                table: "StudentScore");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QuizQuestionId1",
                table: "StudentScore",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_QuizQuestionId1",
                table: "StudentScore",
                column: "QuizQuestionId1");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId1",
                table: "StudentScore",
                column: "QuizQuestionId1",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId");
        }
    }
}
