# Application Sitemap

This sitemap lists all currently accessible MVC endpoints in the application and maps each endpoint to its controller, action, and rendered view.

## Routing Overview

- Default conventional route: `{controller=Home}/{action=Index}/{id?}`
- Additional attribute routes are defined in:
  - `CarsController`
  - `CarTiresController`
  - `FuelExpensesController`
  - `TiresController`

## Endpoints

| Endpoint URL pattern(s) | Controller | Action | View used |
|---|---|---|---|
| `/` ; `/Home` ; `/Home/Index` | `HomeController` | `Index` | `Views/Home/Index.cshtml` |
| `/Home/Privacy` | `HomeController` | `Privacy` | `Views/Home/Privacy.cshtml` |
| `/Home/Error` | `HomeController` | `Error` | `Views/Shared/Error.cshtml` |
| `/auti/svi` | `CarsController` | `Index` | `Views/Cars/Index.cshtml` |
| `/auti/detalji/{id}` | `CarsController` | `Details` | `Views/Cars/Details.cshtml` |
| `/CarTires/Index` | `CarTiresController` | `Index` | `Views/CarTires/Index.cshtml` |
| `/CarTires/Details?id={id}` | `CarTiresController` | `Details` | `Views/CarTires/Details.cshtml` |
| `/ExpenseCategories` ; `/ExpenseCategories/Index` | `ExpenseCategoriesController` | `Index` | `Views/ExpenseCategories/Index.cshtml` |
| `/ExpenseCategories/Details/{id?}` | `ExpenseCategoriesController` | `Details` | `Views/ExpenseCategories/Details.cshtml` |
| `/Expenses` ; `/Expenses/Index` | `ExpensesController` | `Index` | `Views/Expenses/Index.cshtml` |
| `/Expenses/Details/{id?}` | `ExpensesController` | `Details` | `Views/Expenses/Details.cshtml` |
| `/troskovi-goriva/Index` | `FuelExpensesController` | `Index` | `Views/FuelExpenses/Index.cshtml` |
| `/troskovi-goriva/Details?id={id}` | `FuelExpensesController` | `Details` | `Views/FuelExpenses/Details.cshtml` |
| `/Insurances` ; `/Insurances/Index` | `InsurancesController` | `Index` | `Views/Insurances/Index.cshtml` |
| `/Insurances/Details/{id?}` | `InsurancesController` | `Details` | `Views/Insurances/Details.cshtml` |
| `/ServiceRecords` ; `/ServiceRecords/Index` | `ServiceRecordsController` | `Index` | `Views/ServiceRecords/Index.cshtml` |
| `/ServiceRecords/Details/{id?}` | `ServiceRecordsController` | `Details` | `Views/ServiceRecords/Details.cshtml` |
| `/gume/Index` | `TiresController` | `Index` | `Views/Tires/Index.cshtml` |
| `/gume/detalji/{id}` | `TiresController` | `Details` | `Views/Tires/Details.cshtml` |
| `/Users` ; `/Users/Index` | `UsersController` | `Index` | `Views/Users/Index.cshtml` |
| `/Users/Details/{id?}` | `UsersController` | `Details` | `Views/Users/Details.cshtml` |

## Notes

- All listed actions are currently implemented as read-only view-rendering actions.
- For actions where route pattern does not include `{id}` (for example `CarTires/Details` and `troskovi-goriva/Details`), `id` is passed as a query string parameter.