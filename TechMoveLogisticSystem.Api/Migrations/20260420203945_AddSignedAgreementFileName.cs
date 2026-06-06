using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TechMoveLogisticSystem.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSignedAgreementFileName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SignedAgreementFileName",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SignedAgreementFileName",
                table: "Contracts");
        }
    }
}
