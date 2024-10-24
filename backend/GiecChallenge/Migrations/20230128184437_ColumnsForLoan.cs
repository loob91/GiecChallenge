using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GiecChallenge.Migrations
{
    /// <inheritdoc />
    public partial class ColumnsForLoan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarbonLoans_Products_productid",
                table: "CarbonLoans");

            migrationBuilder.RenameColumn(
                name: "productid",
                table: "CarbonLoans",
                newName: "productPurchaseid");

            migrationBuilder.RenameIndex(
                name: "IX_CarbonLoans_productid",
                table: "CarbonLoans",
                newName: "IX_CarbonLoans_productPurchaseid");

            migrationBuilder.AddColumn<Guid>(
                name: "initialPurchaseid",
                table: "Purchases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CO2Cost",
                table: "ProductPurchases",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Purchases_initialPurchaseid",
                table: "Purchases",
                column: "initialPurchaseid");

            migrationBuilder.AddForeignKey(
                name: "FK_CarbonLoans_ProductPurchases_productPurchaseid",
                table: "CarbonLoans",
                column: "productPurchaseid",
                principalTable: "ProductPurchases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Purchases_Purchases_initialPurchaseid",
                table: "Purchases",
                column: "initialPurchaseid",
                principalTable: "Purchases",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarbonLoans_ProductPurchases_productPurchaseid",
                table: "CarbonLoans");

            migrationBuilder.DropForeignKey(
                name: "FK_Purchases_Purchases_initialPurchaseid",
                table: "Purchases");

            migrationBuilder.DropIndex(
                name: "IX_Purchases_initialPurchaseid",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "initialPurchaseid",
                table: "Purchases");

            migrationBuilder.DropColumn(
                name: "CO2Cost",
                table: "ProductPurchases");

            migrationBuilder.RenameColumn(
                name: "productPurchaseid",
                table: "CarbonLoans",
                newName: "productid");

            migrationBuilder.RenameIndex(
                name: "IX_CarbonLoans_productPurchaseid",
                table: "CarbonLoans",
                newName: "IX_CarbonLoans_productid");

            migrationBuilder.AddForeignKey(
                name: "FK_CarbonLoans_Products_productid",
                table: "CarbonLoans",
                column: "productid",
                principalTable: "Products",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
