using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBanck.Migrations
{
    /// <inheritdoc />
    public partial class cambioFKAccountClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_ClientId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Accounts");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_IdClient",
                table: "Accounts",
                column: "IdClient");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_IdClient",
                table: "Accounts",
                column: "IdClient",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Clients_IdClient",
                table: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_IdClient",
                table: "Accounts");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ClientId",
                table: "Accounts",
                column: "ClientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Clients_ClientId",
                table: "Accounts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
