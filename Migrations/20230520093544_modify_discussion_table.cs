using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DigitalMarketing2.Migrations
{
    /// <inheritdoc />
    public partial class modify_discussion_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussion_Lesson_LessonId",
                table: "Discussion");

            migrationBuilder.RenameColumn(
                name: "LessonId",
                table: "Discussion",
                newName: "ModuleId");

            migrationBuilder.RenameIndex(
                name: "IX_Discussion_LessonId",
                table: "Discussion",
                newName: "IX_Discussion_ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussion_Module_ModuleId",
                table: "Discussion",
                column: "ModuleId",
                principalTable: "Module",
                principalColumn: "ModuleId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Discussion_Module_ModuleId",
                table: "Discussion");

            migrationBuilder.RenameColumn(
                name: "ModuleId",
                table: "Discussion",
                newName: "LessonId");

            migrationBuilder.RenameIndex(
                name: "IX_Discussion_ModuleId",
                table: "Discussion",
                newName: "IX_Discussion_LessonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Discussion_Lesson_LessonId",
                table: "Discussion",
                column: "LessonId",
                principalTable: "Lesson",
                principalColumn: "LessonId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
