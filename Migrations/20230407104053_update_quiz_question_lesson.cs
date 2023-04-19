using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketing2.Migrations
{
    /// <inheritdoc />
    public partial class update_quiz_question_lesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Module_LessonModuleId",
                table: "QuizQuestion");

            migrationBuilder.RenameColumn(
                name: "LessonModuleId",
                table: "QuizQuestion",
                newName: "LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestion_LessonModuleId",
                table: "QuizQuestion",
                newName: "IX_QuizQuestion_LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Lesson_LessonId",
                table: "QuizQuestion",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuizQuestion_Lesson_LessonId",
                table: "QuizQuestion");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "QuizQuestion",
                newName: "LessonModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_QuizQuestion_LessonId",
                table: "QuizQuestion",
                newName: "IX_QuizQuestion_LessonModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuizQuestion_Module_LessonModuleId",
                table: "QuizQuestion",
                column: "LessonModuleId",
                principalTable: "Module",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
