using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class KhaltiModelUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "expires_at",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "expires_in",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "fee",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "payment_url",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "refunded",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "transaction_id",
                table: "PaymentKhalti");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "PaymentKhalti",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "pidx",
                table: "PaymentKhalti",
                newName: "Pidx");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "PaymentKhalti",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "total_amount",
                table: "PaymentKhalti",
                newName: "Amount");

            migrationBuilder.AddColumn<string>(
                name: "MobileNo",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderId",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderName",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TxnId",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileNo",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderName",
                table: "PaymentKhalti");

            migrationBuilder.DropColumn(
                name: "TxnId",
                table: "PaymentKhalti");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "PaymentKhalti",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Pidx",
                table: "PaymentKhalti",
                newName: "pidx");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "PaymentKhalti",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "PaymentKhalti",
                newName: "total_amount");

            migrationBuilder.AddColumn<DateTime>(
                name: "expires_at",
                table: "PaymentKhalti",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "expires_in",
                table: "PaymentKhalti",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "fee",
                table: "PaymentKhalti",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "payment_url",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "refunded",
                table: "PaymentKhalti",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "transaction_id",
                table: "PaymentKhalti",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
