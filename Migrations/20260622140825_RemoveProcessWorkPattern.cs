using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyWorkReport.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProcessWorkPattern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProcessWorkPatterns");

            migrationBuilder.AlterColumn<int>(
                name: "StandardCycleSeconds",
                table: "StandardWorkTimes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StandardCycleSeconds",
                table: "StandardWorkTimes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProcessWorkPatterns",
                columns: table => new
                {
                    ProcessId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkPatternId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessWorkPatterns", x => new { x.ProcessId, x.WorkPatternId });
                    table.ForeignKey(
                        name: "FK_ProcessWorkPatterns_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcessWorkPatterns_WorkPatterns_WorkPatternId",
                        column: x => x.WorkPatternId,
                        principalTable: "WorkPatterns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProcessWorkPatterns_WorkPatternId",
                table: "ProcessWorkPatterns",
                column: "WorkPatternId");
        }
    }
}
