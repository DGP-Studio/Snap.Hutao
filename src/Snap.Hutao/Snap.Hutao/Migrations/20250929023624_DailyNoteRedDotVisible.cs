using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Snap.Hutao.Migrations
{
    /// <inheritdoc />
    public partial class DailyNoteRedDotVisible : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DailyTaskDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ExpeditionDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HomeCoinDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ResinDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TransformerDotVisible",
                table: "daily_notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailyTaskDotVisible",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "ExpeditionDotVisible",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "HomeCoinDotVisible",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "ResinDotVisible",
                table: "daily_notes");

            migrationBuilder.DropColumn(
                name: "TransformerDotVisible",
                table: "daily_notes");
        }
    }
}
