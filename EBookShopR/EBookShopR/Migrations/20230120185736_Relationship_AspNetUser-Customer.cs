﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    public partial class Relationship_AspNetUserCustomer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Customers_AspNetUsers_CustomerID",
                table: "Customers",
                column: "CustomerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id", 
                onDelete: ReferentialAction.Restrict
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Customers_AspNetUsers_CustomerID",
                table: "Customers"
                );
        }
    }
}
