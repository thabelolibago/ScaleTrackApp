using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScaleTrackAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveApprovalStatusFromAuditTrail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "AuditTrails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApprovalStatus",
                table: "AuditTrails",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
