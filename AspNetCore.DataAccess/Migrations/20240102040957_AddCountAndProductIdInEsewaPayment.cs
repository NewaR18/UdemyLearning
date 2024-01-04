using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddCountAndProductIdInEsewaPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "EsewaPayment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "EsewaPayment",
                type: "int",
                nullable: false,
                defaultValue: 19);

            migrationBuilder.CreateIndex(
                name: "IX_EsewaPayment_ProductId",
                table: "EsewaPayment",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_EsewaPayment_Product_ProductId",
                table: "EsewaPayment",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EsewaPayment_Product_ProductId",
                table: "EsewaPayment");

            migrationBuilder.DropIndex(
                name: "IX_EsewaPayment_ProductId",
                table: "EsewaPayment");

            migrationBuilder.DropColumn(
                name: "Count",
                table: "EsewaPayment");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "EsewaPayment");
        }
    }
}
