using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBanck.Migrations
{
    /// <inheritdoc />
    public partial class fixIdentityId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdAccount",
                table: "Transactions",
                column: "IdAccount");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions",
                column: "IdAccount",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_IdAccount",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_IdAccount",
                table: "Transactions");
        }
    }
}
