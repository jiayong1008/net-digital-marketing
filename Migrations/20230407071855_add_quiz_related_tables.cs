using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketing2.Migrations
{
    /// <inheritdoc />
    public partial class add_quiz_related_tables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "QuestionOption",
                columns: table => new
                {
                    QuestionOptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Option = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizQuestionId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOption", x => x.QuestionOptionId);
                });

            migrationBuilder.CreateTable(
                name: "QuizQuestion",
                columns: table => new
                {
                    QuizQuestionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Question = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    QuizOrder = table.Column<int>(type: "int", nullable: false),
                    LessonModuleId = table.Column<int>(type: "int", nullable: false),
                    AnswerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuizQuestion", x => x.QuizQuestionId);
                    table.ForeignKey(
                        name: "FK_QuizQuestion_Module_LessonModuleId",
                        column: x => x.LessonModuleId,
                        principalTable: "Module",
                        principalColumn: "ModuleId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuizQuestion_QuestionOption_AnswerId",
                        column: x => x.AnswerId,
                        principalTable: "QuestionOption",
                        principalColumn: "QuestionOptionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentScore",
                columns: table => new
                {
                    StudentScoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    StudentQuizQuestionId = table.Column<int>(type: "int", nullable: false),
                    QuizQuestionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentScore", x => x.StudentScoreId);
                    table.ForeignKey(
                        name: "FK_StudentScore_Enrollment_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollment",
                        principalColumn: "EnrollmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentScore_QuizQuestion_QuizQuestionId",
                        column: x => x.QuizQuestionId,
                        principalTable: "QuizQuestion",
                        principalColumn: "QuizQuestionId");
                    table.ForeignKey(
                        name: "FK_StudentScore_QuizQuestion_StudentQuizQuestionId",
                        column: x => x.StudentQuizQuestionId,
                        principalTable: "QuizQuestion",
                        principalColumn: "QuizQuestionId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOption_QuizQuestionId",
                table: "QuestionOption",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestion_AnswerId",
                table: "QuizQuestion",
                column: "AnswerId",
                unique: true,
                filter: "[AnswerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_QuizQuestion_LessonModuleId",
                table: "QuizQuestion",
                column: "LessonModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_EnrollmentId",
                table: "StudentScore",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_QuizQuestionId",
                table: "StudentScore",
                column: "QuizQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentScore_StudentQuizQuestionId",
                table: "StudentScore",
                column: "StudentQuizQuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOption_QuizQuestion_QuizQuestionId",
                table: "QuestionOption",
                column: "QuizQuestionId",
                principalTable: "QuizQuestion",
                principalColumn: "QuizQuestionId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOption_QuizQuestion_QuizQuestionId",
                table: "QuestionOption");

            migrationBuilder.DropTable(
                name: "StudentScore");

            migrationBuilder.DropTable(
                name: "QuizQuestion");

            migrationBuilder.DropTable(
                name: "QuestionOption");
        }
    }
}
