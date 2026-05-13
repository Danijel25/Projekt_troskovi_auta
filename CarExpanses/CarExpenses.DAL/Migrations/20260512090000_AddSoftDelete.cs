using System;
using CarExpenses.DAL;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarExpenses.DAL.Migrations
{
    [DbContext(typeof(CarExpesesDbContext))]
    [Migration("20260512090000_AddSoftDelete")]
    public partial class AddSoftDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "ExpenseCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "Tires",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "CarTires",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "Expenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "FuelExpenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "Insurances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeleatedAt",
                table: "ServiceRecords",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "ExpenseCategories");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "Tires");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "CarTires");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "FuelExpenses");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "Insurances");

            migrationBuilder.DropColumn(
                name: "DeleatedAt",
                table: "ServiceRecords");
        }
    }
}
