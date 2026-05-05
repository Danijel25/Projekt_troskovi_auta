using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CarExpenses.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ExpenseCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Car washing" },
                    { 2, "Wiper fluid refill" },
                    { 3, "Parking and tolls" }
                });

            migrationBuilder.InsertData(
                table: "Tires",
                columns: new[] { "Id", "Brand", "Model", "Price", "Season" },
                values: new object[,]
                {
                    { 1, "Michelin", "Pilot Sport 5", 145m, "Summer" },
                    { 2, "Continental", "PremiumContact 6", 138m, "Summer" },
                    { 3, "Bridgestone", "Blizzak LM005", 152m, "Winter" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "marko.pavic@example.com", "Pass123!", "marko92" },
                    { 2, "ivana.horvat@example.com", "Pass123!", "ivana87" },
                    { 3, "petra.kovac@example.com", "Pass123!", "petra95" }
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "CurrentMilage", "EngineVolume", "FuelType", "Model", "PurchaseDate", "PurchasePrice", "UserId", "Year" },
                values: new object[,]
                {
                    { 1, "Toyota", 68500, 1.8, 3, "Corolla", new DateTime(2021, 3, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 19850m, 1, 2021 },
                    { 2, "BMW", 112300, 2.0, 1, "320d", new DateTime(2019, 7, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 27900m, 2, 2019 },
                    { 3, "Tesla", 43100, 0.0, 2, "Model 3", new DateTime(2022, 11, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 43990m, 3, 2022 }
                });

            migrationBuilder.InsertData(
                table: "CarTires",
                columns: new[] { "Id", "CarId", "InstalledDate", "TireId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 2, new DateTime(2025, 4, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 3, new DateTime(2025, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "CarId", "CategoryId", "Date", "Description" },
                values: new object[,]
                {
                    { 1, 18m, 1, 1, new DateTime(2026, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Hand wash and interior cleaning" },
                    { 2, 7.5m, 2, 2, new DateTime(2026, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Winter windshield washer fluid" },
                    { 3, 62m, 3, 3, new DateTime(2026, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Monthly garage parking pass" }
                });

            migrationBuilder.InsertData(
                table: "FuelExpenses",
                columns: new[] { "Id", "CarId", "FuelExpenseDate", "Kilometars", "Liters", "PricePerLiter" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 68940, 42.50m, 1.63m },
                    { 2, 2, new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 113020, 55.10m, 1.58m },
                    { 3, 3, new DateTime(2026, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 43580, 0m, 0m }
                });

            migrationBuilder.InsertData(
                table: "Insurances",
                columns: new[] { "Id", "CarId", "Company", "EndDate", "InsuranceType", "Price", "StartDate" },
                values: new object[,]
                {
                    { 1, 1, "Allianz", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comprehensive", 640m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, "Generali", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comprehensive", 760m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, "Wiener Osiguranje", new DateTime(2026, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comprehensive", 890m, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ServiceRecords",
                columns: new[] { "Id", "CarId", "Cost", "Description", "Mileage", "ServiceDate", "ServiceType" },
                values: new object[,]
                {
                    { 1, 1, 180m, "Oil and filter change", 69210, new DateTime(2026, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Regular maintenance" },
                    { 2, 2, 320m, "Front brake pads replacement", 113450, new DateTime(2026, 2, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "Brake service" },
                    { 3, 3, 95m, "High-voltage system diagnostic", 43820, new DateTime(2026, 2, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Battery check" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CarTires",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarTires",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CarTires",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Expenses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "FuelExpenses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FuelExpenses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FuelExpenses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Insurances",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Insurances",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Insurances",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ServiceRecords",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ServiceRecords",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ServiceRecords",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ExpenseCategories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Tires",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
