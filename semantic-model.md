# Semantic Model

This document summarizes the current domain entities, database tables, main properties, and relationships used in the project.

## Models/Classes and Tables

| Model/Class | Table | Primary Key | Main Properties |
|---|---|---|---|
| `User` | `Users` | `Id` | `Username`, `Email`, `Password` |
| `Car` | `Cars` | `Id` | `UserId`, `Brand`, `Model`, `Year`, `EngineVolume`, `CurrentMilage`, `PurchasePrice`, `PurchaseDate`, `FuelType` |
| `Tire` | `Tires` | `Id` | `Brand`, `Model`, `Season`, `Price` |
| `CarTire` | `CarTires` | `Id` | `CarId`, `TireId`, `InstalledDate` |
| `FuelExpense` | `FuelExpenses` | `Id` | `FuelExpenseDate`, `Liters`, `PricePerLiter`, `Kilometars`, `CarId`, computed `TotalCost` |
| `ServiceRecord` | `ServiceRecords` | `Id` | `ServiceType`, `Description`, `Cost`, `ServiceDate`, `Mileage`, `CarId` |
| `Insurance` | `Insurances` | `Id` | `Company`, `InsuranceType`, `Price`, `StartDate`, `EndDate`, `CarId` |
| `ExpenseCategory` | `ExpenseCategories` | `Id` | `Name` |
| `Expense` | `Expenses` | `Id` | `Description`, `Amount`, `Date`, `CategoryId`, nullable `CarId` (shadow FK in EF model) |

## Supporting Types

| Type | Kind | Values/Usage |
|---|---|---|
| `FuelType` | Enum | `Petrol`, `Diesel`, `Electric`, `Hybrid` (stored as int in `Cars.FuelType`) |

## Connections Between Tables/Classes

1. `User` (1) -> (many) `Car`
   - FK: `Cars.UserId` -> `Users.Id`
   - Required relationship, cascade delete.

2. `Car` (1) -> (many) `FuelExpense`
   - FK: `FuelExpenses.CarId` -> `Cars.Id`
   - Required relationship, cascade delete.

3. `Car` (1) -> (many) `ServiceRecord`
   - FK: `ServiceRecords.CarId` -> `Cars.Id`
   - Required relationship, cascade delete.

4. `Car` (1) -> (many) `Insurance`
   - FK: `Insurances.CarId` -> `Cars.Id`
   - Required relationship, cascade delete.

5. `ExpenseCategory` (1) -> (many) `Expense`
   - FK: `Expenses.CategoryId` -> `ExpenseCategories.Id`
   - Required relationship, cascade delete.

6. `Car` (1) -> (many) `Expense`
   - FK: `Expenses.CarId` -> `Cars.Id`
   - Optional relationship (`CarId` is nullable in the database model).

7. `Car` (many) <-> (many) `Tire` through `CarTire`
   - Bridge table/class: `CarTires`
   - `CarTires.CarId` -> `Cars.Id` (required, cascade delete)
   - `CarTires.TireId` -> `Tires.Id` (required, cascade delete)

## Quick Relationship Graph

- `User` -> `Car`
- `Car` -> `FuelExpense`
- `Car` -> `ServiceRecord`
- `Car` -> `Insurance`
- `Car` -> `Expense` (optional)
- `ExpenseCategory` -> `Expense`
- `Car` -> `CarTire` <- `Tire`
