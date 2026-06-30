using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyWorkReport.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStandardCycleSecondsToDecimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "StandardCycleSeconds",
                table: "StandardWorkTimes",
                type: "TEXT",
                precision: 9,
                scale: 2,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StandardCycleSeconds",
                table: "StandardWorkTimes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "TEXT",
                oldPrecision: 9,
                oldScale: 2,
                oldNullable: true);
        }
    }
}
