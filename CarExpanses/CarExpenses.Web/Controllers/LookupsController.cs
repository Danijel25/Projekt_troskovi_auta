using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("lookup")]
public class LookupsController(
    UserManager<User> userManager,
    ICarRepository carRepository,
    ITireRepository tireRepository,
    IExpenseCategoryRepository expenseCategoryRepository) : Controller
{
    [HttpGet("{source}")]
    public IActionResult Search(string source, string? query, int limit = 25)
    {
        var max = Math.Clamp(limit, 1, 50);
        var term = query?.Trim();
        var normalized = source.Trim().ToLowerInvariant();

        if (normalized == "users" && !User.IsInRole(AppRoles.Admin))
        {
            return Forbid();
        }

        var results = normalized switch
        {
            "users" => GetUsers(term, max),
            "cars" => GetCars(term, max),
            "tires" => GetTires(term, max),
            "categories" => GetCategories(term, max),
            _ => []
        };

        return Json(results);
    }

    private List<LookupItemViewModel> GetUsers(string? term, int limit)
    {
        var users = userManager.Users.AsQueryable();
        if (!string.IsNullOrWhiteSpace(term))
        {
            users = users.Where(user =>
                (user.UserName != null && user.UserName.Contains(term))
                || (user.Email != null && user.Email.Contains(term))
                || user.Id.ToString().Contains(term));
        }

        return users
            .OrderBy(user => user.UserName)
            .Take(limit)
            .Select(user => new LookupItemViewModel
            {
                Value = user.Id.ToString(),
                Label = user.UserName ?? string.Empty,
                Hint = user.Email ?? string.Empty
            })
            .ToList();
    }

    private List<LookupItemViewModel> GetCars(string? term, int limit)
    {
        var cars = carRepository.GetAll();
        var filtered = string.IsNullOrWhiteSpace(term)
            ? cars
            : cars.Where(car =>
                car.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Model.Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Year.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.FuelType.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || car.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));

        return filtered
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Take(limit)
            .Select(car => new LookupItemViewModel
            {
                Value = car.Id.ToString(),
                Label = $"{car.Brand} {car.Model} ({car.Year})",
                Hint = $"Fuel: {car.FuelType}"
            })
            .ToList();
    }

    private List<LookupItemViewModel> GetTires(string? term, int limit)
    {
        var tires = tireRepository.GetAll();
        var filtered = string.IsNullOrWhiteSpace(term)
            ? tires
            : tires.Where(tire =>
                tire.Brand.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Model.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Season.Contains(term, StringComparison.OrdinalIgnoreCase)
                || tire.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));

        return filtered
            .OrderBy(tire => tire.Brand)
            .ThenBy(tire => tire.Model)
            .Take(limit)
            .Select(tire => new LookupItemViewModel
            {
                Value = tire.Id.ToString(),
                Label = $"{tire.Brand} {tire.Model}",
                Hint = tire.Season
            })
            .ToList();
    }

    private List<LookupItemViewModel> GetCategories(string? term, int limit)
    {
        var categories = expenseCategoryRepository.GetAll();
        var filtered = string.IsNullOrWhiteSpace(term)
            ? categories
            : categories.Where(category =>
                category.Name.Contains(term, StringComparison.OrdinalIgnoreCase)
                || category.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase));

        return filtered
            .OrderBy(category => category.Name)
            .Take(limit)
            .Select(category => new LookupItemViewModel
            {
                Value = category.Id.ToString(),
                Label = category.Name,
                Hint = $"Id {category.Id}"
            })
            .ToList();
    }
}
