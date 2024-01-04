﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspNetCore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class PaymentKhalti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "PaymentKhalti",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pidx = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    payment_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expires_in = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transaction_id = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    refunded = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentKhalti", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentKhalti");

            migrationBuilder.CreateTable(
                name: "KhaltiPayment",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    expires_in = table.Column<int>(type: "int", nullable: false),
                    fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    payment_url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    pidx = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    refunded = table.Column<bool>(type: "bit", nullable: false),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    transaction_id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhaltiPayment", x => x.id);
                });
        }
    }
}
