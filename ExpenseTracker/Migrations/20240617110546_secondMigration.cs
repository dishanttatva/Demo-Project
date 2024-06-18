using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class secondMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Expenses_Category_Id",
                table: "Expenses",
                column: "Category_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_User_Id",
                table: "Expenses",
                column: "User_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Categories_Category_Id",
                table: "Expenses",
                column: "Category_Id",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Users_User_Id",
                table: "Expenses",
                column: "User_Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Categories_Category_Id",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Users_User_Id",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_Category_Id",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_User_Id",
                table: "Expenses");
        }
    }
}
