using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyWorkReport.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionOrderRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RowVersion",
                table: "ProductionOrders",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "ProductionOrders");
        }
    }
}
