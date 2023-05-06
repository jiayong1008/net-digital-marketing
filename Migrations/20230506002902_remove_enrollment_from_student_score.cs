using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketing2.Migrations
{
    /// <inheritdoc />
    public partial class remove_enrollment_from_student_score : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_Enrollment_EnrollmentId",
                table: "StudentScore");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId",
                table: "StudentScore");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuizQuestion_StudentQuizQuestionId",
                table: "StudentScore");

            migrationBuilder.DropIndex(
                name: "IX_StudentScore_EnrollmentId",
                table: "StudentScore");

            migrationBuilder.DropColumn(
                name: "EnrollmentId",
                table: "StudentScore");

            migrationBuilder.RenameColumn(
                name: "StudentQuizQuestionId",
                table: "StudentScore",
                newName: "AnswerId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentScore_StudentQuizQuestionId",
                table: "StudentScore",
                newName: "IX_StudentScore_AnswerId");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "StudentScore",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<int>(
                name: "QuizQuestionId",
                table: "StudentScore",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuizQuestionId1",
                table: "StudentScore",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "StudentScore",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_QuizQuestionId1",
                table: "StudentScore",
                column: "QuizQuestionId1");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_UserId",
                table: "StudentScore",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_AspNetUsers_UserId",
                table: "StudentScore",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuestionOption_AnswerId",
                table: "StudentScore",
                column: "AnswerId",
                principalTable: "QuestionOption",
                principalColumn: "QuestionOptionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId",
                table: "StudentScore",
                column: "QuizQuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId1",
                table: "StudentScore",
                column: "QuizQuestionId1",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_AspNetUsers_UserId",
                table: "StudentScore");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuestionOption_AnswerId",
                table: "StudentScore");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId",
                table: "StudentScore");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId1",
                table: "StudentScore");

            migrationBuilder.DropIndex(
                name: "IX_StudentScore_QuizQuestionId1",
                table: "StudentScore");

            migrationBuilder.DropIndex(
                name: "IX_StudentScore_UserId",
                table: "StudentScore");

            migrationBuilder.DropColumn(
                name: "QuizQuestionId1",
                table: "StudentScore");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentScore");

            migrationBuilder.RenameColumn(
                name: "AnswerId",
                table: "StudentScore",
                newName: "StudentQuizQuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentScore_AnswerId",
                table: "StudentScore",
                newName: "IX_StudentScore_StudentQuizQuestionId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudentScore",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "QuizQuestionId",
                table: "StudentScore",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentId",
                table: "StudentScore",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_EnrollmentId",
                table: "StudentScore",
                column: "EnrollmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_Enrollment_EnrollmentId",
                table: "StudentScore",
                column: "EnrollmentId",
                principalTable: "Enrollment",
                principalColumn: "EnrollmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuizQuestion_QuizQuestionId",
                table: "StudentScore",
                column: "QuizQuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentScore_QuizQuestion_StudentQuizQuestionId",
                table: "StudentScore",
                column: "StudentQuizQuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
