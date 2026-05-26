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
    UserManager<User> userManager) : Controller
{
    private const int PageLimit = 12;
    private const int ItemLimit = 6;

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var term = query?.Trim();
        var groups = new List<GlobalSearchGroup>();

        var pages = BuildPageResults(term);
        AddGroup(groups, "Pages", pages);

        if (!string.IsNullOrWhiteSpace(term))
        {
            AddGroup(groups, "Cars", SearchCars(term));
            AddGroup(groups, "Tires", SearchTires(term));
            AddGroup(groups, "Car Tires", SearchCarTires(term));
            AddGroup(groups, "Fuel Expenses", SearchFuelExpenses(term));
            AddGroup(groups, "Service Records", SearchServiceRecords(term));
            AddGroup(groups, "Insurances", SearchInsurances(term));
            AddGroup(groups, "Expense Categories", SearchCategories(term));
            AddGroup(groups, "Expenses", SearchExpenses(term));

            if (User.IsInRole(AppRoles.Admin))
            {
                AddGroup(groups, "Users", SearchUsers(term));
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

    private IReadOnlyList<GlobalSearchItem> SearchCars(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var car in carRepository.GetAll())
        {
            if (!Matches(car.Brand, term)
                && !Matches(car.Model, term)
                && !Matches(car.FuelType.ToString(), term)
                && !Matches(car.Year.ToString(), term)
                && !Matches(car.EngineVolume.ToString(), term)
                && !Matches(car.CurrentMilage.ToString(), term)
                && !Matches(car.Id.ToString(), term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchTires(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var tire in tireRepository.GetAll())
        {
            if (!Matches(tire.Brand, term)
                && !Matches(tire.Model, term)
                && !Matches(tire.Season, term)
                && !Matches(tire.Price.ToString(), term)
                && !Matches(tire.Id.ToString(), term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchCarTires(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var assignment in carTireRepository.GetAll())
        {
            if (!Matches(assignment.CarId.ToString(), term)
                && !Matches(assignment.TireId.ToString(), term)
                && !Matches(assignment.InstalledDate.ToString("yyyy-MM-dd"), term)
                && !Matches(assignment.Id.ToString(), term)
                && !Matches(assignment.Car?.Brand, term)
                && !Matches(assignment.Car?.Model, term)
                && !Matches(assignment.Tire?.Brand, term)
                && !Matches(assignment.Tire?.Model, term)
                && !Matches(assignment.Tire?.Season, term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchFuelExpenses(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var expense in fuelExpenseRepository.GetAll())
        {
            if (!Matches(expense.CarId.ToString(), term)
                && !Matches(expense.TotalCost.ToString(), term)
                && !Matches(expense.Liters.ToString(), term)
                && !Matches(expense.PricePerLiter.ToString(), term)
                && !Matches(expense.Kilometars.ToString(), term)
                && !Matches(expense.FuelExpenseDate.ToString("yyyy-MM-dd"), term)
                && !Matches(expense.Id.ToString(), term)
                && !Matches(expense.Car?.Brand, term)
                && !Matches(expense.Car?.Model, term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchServiceRecords(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var record in serviceRecordRepository.GetAll())
        {
            if (!Matches(record.ServiceType, term)
                && !Matches(record.Description, term)
                && !Matches(record.Cost.ToString(), term)
                && !Matches(record.ServiceDate.ToString("yyyy-MM-dd"), term)
                && !Matches(record.Mileage.ToString(), term)
                && !Matches(record.CarId.ToString(), term)
                && !Matches(record.Id.ToString(), term)
                && !Matches(record.Car?.Brand, term)
                && !Matches(record.Car?.Model, term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchInsurances(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var insurance in insuranceRepository.GetAll())
        {
            if (!Matches(insurance.Company, term)
                && !Matches(insurance.InsuranceType, term)
                && !Matches(insurance.Price.ToString(), term)
                && !Matches(insurance.StartDate.ToString("yyyy-MM-dd"), term)
                && !Matches(insurance.EndDate.ToString("yyyy-MM-dd"), term)
                && !Matches(insurance.CarId.ToString(), term)
                && !Matches(insurance.Id.ToString(), term)
                && !Matches(insurance.Car?.Brand, term)
                && !Matches(insurance.Car?.Model, term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchCategories(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var category in expenseCategoryRepository.GetAll())
        {
            if (!Matches(category.Name, term) && !Matches(category.Id.ToString(), term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchExpenses(string term)
    {
        var results = new List<GlobalSearchItem>();

        foreach (var expense in expenseRepository.GetAll())
        {
            if (!Matches(expense.Description, term)
                && !Matches(expense.Amount.ToString(), term)
                && !Matches(expense.Date.ToString("yyyy-MM-dd"), term)
                && !Matches(expense.Date.ToString("MM/dd"), term)
                && !Matches(expense.Id.ToString(), term)
                && !Matches(expense.Category?.Name, term))
            {
                continue;
            }

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

    private IReadOnlyList<GlobalSearchItem> SearchUsers(string term)
    {
        var lowered = term.ToLowerInvariant();
        var users = userManager.Users
            .Where(user =>
                (user.UserName != null && user.UserName.ToLower().Contains(lowered))
                || (user.Email != null && user.Email.ToLower().Contains(lowered))
                || user.Id.ToString().Contains(lowered))
            .OrderBy(user => user.UserName)
            .Take(ItemLimit)
            .ToList();

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
