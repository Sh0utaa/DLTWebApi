using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DriversLicenseTestWebAPI.Migrations
{
    /// <inheritdoc />
    public partial class ExamSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCorrect",
                table: "UserAnswerSubmission",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionId",
                table: "UserAnswerSubmission",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ExamSession",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectAmount = table.Column<int>(type: "int", nullable: false),
                    IncorrectAmount = table.Column<int>(type: "int", nullable: false),
                    Failed = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamSession", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerSubmission_AnswerId",
                table: "UserAnswerSubmission",
                column: "AnswerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerSubmission_QuestionId",
                table: "UserAnswerSubmission",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswerSubmission_SessionId",
                table: "UserAnswerSubmission",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswerSubmission_Answers_AnswerId",
                table: "UserAnswerSubmission",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswerSubmission_ExamSession_SessionId",
                table: "UserAnswerSubmission",
                column: "SessionId",
                principalTable: "ExamSession",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswerSubmission_Questions_QuestionId",
                table: "UserAnswerSubmission",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswerSubmission_Answers_AnswerId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswerSubmission_ExamSession_SessionId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswerSubmission_Questions_QuestionId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropTable(
                name: "ExamSession");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswerSubmission_AnswerId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswerSubmission_QuestionId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropIndex(
                name: "IX_UserAnswerSubmission_SessionId",
                table: "UserAnswerSubmission");

            migrationBuilder.DropColumn(
                name: "IsCorrect",
                table: "UserAnswerSubmission");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "UserAnswerSubmission");
        }
    }
}
