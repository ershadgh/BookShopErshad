using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookShop.Migrations
{
    public partial class UpdateCityTB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provices_ProviceProvinceID",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ProviceProvinceID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ProviceProvinceID",
                table: "Cities");

            migrationBuilder.AddColumn<int>(
                name: "ProvinceID",
                table: "Cities",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "BookInfo",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "select Convert(datetime,GetDate())");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProvinceID",
                table: "Cities",
                column: "ProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provices_ProvinceID",
                table: "Cities",
                column: "ProvinceID",
                principalTable: "Provices",
                principalColumn: "ProvinceID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cities_Provices_ProvinceID",
                table: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_Cities_ProvinceID",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "ProvinceID",
                table: "Cities");

            migrationBuilder.AddColumn<int>(
                name: "ProviceProvinceID",
                table: "Cities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PublishDate",
                table: "BookInfo",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "select Convert(datetime,GetDate())",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cities_ProviceProvinceID",
                table: "Cities",
                column: "ProviceProvinceID");

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Provices_ProviceProvinceID",
                table: "Cities",
                column: "ProviceProvinceID",
                principalTable: "Provices",
                principalColumn: "ProvinceID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
