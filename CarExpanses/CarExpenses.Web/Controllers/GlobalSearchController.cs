using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class GlobalSearchController(
    ICarRepository carRepository,
    ITireRepository tireRepository,
    ICarTireRepository carTireRepository,
    IFuelExpenseRepository fuelExpenseRepository,
    IServiceRecordRepository serviceRecordRepository,
    IInsuranceRepository insuranceRepository,
    IExpenseCategoryRepository expenseCategoryRepository,
    IExpenseRepository expenseRepository,
    IUserRepository userRepository) : Controller
{
    private const int PageLimit = 12;
    private const int ItemLimit = 6;

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var term = query?.Trim();
        var groups = new List<GlobalSearchGroup>();

        var pages = BuildPageResults(term);
        AddGroup(groups, "Pages", pages);

        if (!string.IsNullOrWhiteSpace(term))
        {
            var cars = await SearchCars(term);
            AddGroup(groups, "Cars", cars);

            var tires = await SearchTires(term);
            AddGroup(groups, "Tires", tires);

            var carTires = await SearchCarTires(term);
            AddGroup(groups, "Car Tires", carTires);

            var fuelExpenses = await SearchFuelExpenses(term);
            AddGroup(groups, "Fuel Expenses", fuelExpenses);

            var serviceRecords = await SearchServiceRecords(term);
            AddGroup(groups, "Service Records", serviceRecords);

            var insurances = await SearchInsurances(term);
            AddGroup(groups, "Insurances", insurances);

            var categories = await SearchCategories(term);
            AddGroup(groups, "Expense Categories", categories);

            var expenses = await SearchExpenses(term);
            AddGroup(groups, "Expenses", expenses);

            if (User.IsInRole(AppRoles.Admin))
            {
                AddGroup(groups, "Users", await SearchUsers(term));
            }
        }

        return Json(new { groups });
    }

    private IReadOnlyList<GlobalSearchItem> BuildPageResults(string? term)
    {
        var pages = GetPageCatalog();
        var results = new List<GlobalSearchItem>();

        foreach (var page in pages)
        {
            if (page.RequiredRole is not null && !User.IsInRole(page.RequiredRole))
            {
                continue;
            }

            if (!string.IsNullOrWhiteSpace(term) && !MatchesPage(page, term))
            {
                continue;
            }

            var url = Url.Action(page.Action, page.Controller);
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            results.Add(new GlobalSearchItem(page.Label, url, page.Hint));
            if (results.Count >= PageLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchCars(string term)
    {
        var results = new List<GlobalSearchItem>();
        var cars = await carRepository.GetListAsync(new CarFilter() { Search = term });
        foreach (var car in cars)
        {
            var url = Url.Action("Details", "Cars", new { id = car.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = $"{car.Brand} {car.Model} ({car.Year})";
            var hint = $"Car #{car.Id} | Fuel: {car.FuelType}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchTires(string term)
    {
        var results = new List<GlobalSearchItem>();
        var tires = await tireRepository.GetListAsync(new TireFilter() { Search = term });
        foreach (var tire in tires)
        {           
            var url = Url.Action("Details", "Tires", new { id = tire.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = $"{tire.Brand} {tire.Model}";
            var hint = $"{tire.Season} | Tire #{tire.Id}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchCarTires(string term)
    {
        var results = new List<GlobalSearchItem>();
        var carTires = await carTireRepository.GetListAsync(new CarTireFilter() { Search = term });
        foreach (var assignment in carTires)
        {
            var url = Url.Action("Details", "CarTires", new { id = assignment.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var carLabel = assignment.Car is null ? "Car" : $"{assignment.Car.Brand} {assignment.Car.Model}";
            var tireLabel = assignment.Tire is null ? "Tire" : $"{assignment.Tire.Brand} {assignment.Tire.Model}";
            var label = $"{carLabel} - {tireLabel}";
            var hint = $"Installed {assignment.InstalledDate:yyyy-MM-dd} | Assignment #{assignment.Id}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchFuelExpenses(string term)
    {
        var results = new List<GlobalSearchItem>();
        var fuleExpenses = await fuelExpenseRepository.GetListAsync(new FuelExpenseFilter() { Search = term });
        foreach (var expense in fuleExpenses)
        {
            var url = Url.Action("Details", "FuelExpenses", new { id = expense.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var carLabel = expense.Car is null ? "Car" : $"{expense.Car.Brand} {expense.Car.Model}";
            var label = $"{carLabel} fuel";
            var hint = $"{expense.FuelExpenseDate:yyyy-MM-dd} | Total {expense.TotalCost:0.##}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchServiceRecords(string term)
    {
        var results = new List<GlobalSearchItem>();

        var serviceRecords = await serviceRecordRepository.GetListAsync(new ServiceRecordFilter() { Search = term });
        foreach (var record in serviceRecords)
        {
            var url = Url.Action("Details", "ServiceRecords", new { id = record.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var carLabel = record.Car is null ? "Car" : $"{record.Car.Brand} {record.Car.Model}";
            var label = $"{carLabel} service";
            var hint = $"{record.ServiceType} | {record.ServiceDate:yyyy-MM-dd}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchInsurances(string term)
    {
        var results = new List<GlobalSearchItem>();

        var insurances = await insuranceRepository.GetListAsync(new InsuranceFilter() { Search = term });
        foreach (var insurance in insurances)
        {
            var url = Url.Action("Details", "Insurances", new { id = insurance.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = $"{insurance.Company} {insurance.InsuranceType}";
            var hint = $"{insurance.StartDate:yyyy-MM-dd} to {insurance.EndDate:yyyy-MM-dd}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchCategories(string term)
    {
        var results = new List<GlobalSearchItem>();

        var categories = await expenseCategoryRepository.GetListAsync(new ExpenseCategoryFilter() { Search = term });
        foreach (var category in categories)
        {
            var url = Url.Action("Details", "ExpenseCategories", new { id = category.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = category.Name;
            var hint = $"Category #{category.Id}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchExpenses(string term)
    {
        var results = new List<GlobalSearchItem>();

        var expenses = await expenseRepository.GetListAsync(new ExpenseFilter() { Search = term });
        foreach (var expense in expenses)
        {
            var url = Url.Action("Details", "Expenses", new { id = expense.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = expense.Description;
            var hint = $"{expense.Date:yyyy-MM-dd} | {expense.Amount:0.##}";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private async Task<IReadOnlyList<GlobalSearchItem>> SearchUsers(string term)
    {
        var lowered = term.ToLowerInvariant();
        var users = await userRepository.GetListAsync(new UserFilter() { Search = term });

        var results = new List<GlobalSearchItem>();
        foreach (var user in users)
        {
            var url = Url.Action("Details", "Users", new { id = user.Id });
            if (string.IsNullOrWhiteSpace(url))
            {
                continue;
            }

            var label = user.UserName ?? $"User {user.Id}";
            var hint = user.Email ?? "";
            results.Add(new GlobalSearchItem(label, url, hint));

            if (results.Count >= ItemLimit)
            {
                break;
            }
        }

        return results;
    }

    private static void AddGroup(List<GlobalSearchGroup> groups, string title, IReadOnlyList<GlobalSearchItem> items)
    {
        if (items.Count == 0)
        {
            return;
        }

        groups.Add(new GlobalSearchGroup(title, items));
    }

    private static bool Matches(string? source, string term)
    {
        return !string.IsNullOrWhiteSpace(source)
            && source.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private static bool MatchesPage(SearchPage page, string term)
    {
        if (Matches(page.Label, term) || Matches(page.Hint, term))
        {
            return true;
        }

        foreach (var keyword in page.Keywords)
        {
            if (Matches(keyword, term))
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<SearchPage> GetPageCatalog() =>
        new List<SearchPage>
        {
            new("Dashboard", "Dashboard", "Home", "Overview and totals", new[] { "home", "dashboard", "overview" }),
            new("Cars", "Index", "Cars", "All vehicles", new[] { "cars", "vehicles", "garage", "auti" }),
            new("Add Car", "Create", "Cars", "Create a new vehicle", new[] { "cars", "create", "new", "add", "vehicle" }),
            new("Tires", "Index", "Tires", "All tires", new[] { "tires", "gume" }),
            new("Add Tire", "Create", "Tires", "Create a new tire", new[] { "tires", "create", "new", "add" }),
            new("Car Tires", "Index", "CarTires", "Car tire assignments", new[] { "car tires", "assignments", "mounts" }),
            new("Add Car Tire", "Create", "CarTires", "Assign tire to car", new[] { "car tire", "assign", "create", "new" }),
            new("Fuel Expenses", "Index", "FuelExpenses", "Fuel log", new[] { "fuel", "expenses", "troskovi" }),
            new("Add Fuel Expense", "Create", "FuelExpenses", "Log a fuel expense", new[] { "fuel", "expense", "create", "new" }),
            new("Service Records", "Index", "ServiceRecords", "Service history", new[] { "service", "records" }),
            new("Add Service Record", "Create", "ServiceRecords", "Log a service record", new[] { "service", "record", "create", "new" }),
            new("Insurances", "Index", "Insurances", "Insurance policies", new[] { "insurance", "policies" }),
            new("Add Insurance", "Create", "Insurances", "Create a policy", new[] { "insurance", "create", "new" }),
            new("Expense Categories", "Index", "ExpenseCategories", "Category list", new[] { "categories", "expense" }),
            new("Add Expense Category", "Create", "ExpenseCategories", "Create a category", new[] { "categories", "expense", "create", "new" }, AppRoles.Admin),
            new("Expenses", "Index", "Expenses", "Expense ledger", new[] { "expenses", "ledger" }),
            new("Add Expense", "Create", "Expenses", "Log an expense", new[] { "expenses", "create", "new" }),
            new("Users", "Index", "Users", "User management", new[] { "users", "admin" }, AppRoles.Admin)
        };

    private sealed record SearchPage(
        string Label,
        string Action,
        string Controller,
        string Hint,
        IReadOnlyList<string> Keywords,
        string? RequiredRole = null);

    private sealed record GlobalSearchGroup(string Title, IReadOnlyList<GlobalSearchItem> Items);

    private sealed record GlobalSearchItem(string Label, string Url, string Hint);
}
