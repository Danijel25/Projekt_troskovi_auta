using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Security;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("lookup")]
public class LookupsController(
    IUserRepository userRepository,
    ICarRepository carRepository,
    ITireRepository tireRepository,
    IExpenseCategoryRepository expenseCategoryRepository) : Controller
{
    [HttpGet("{source}")]
    public async Task<IActionResult> Search(string source, string? query, int limit = 25)
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
            "users" => await GetUsers(term, max),
            "cars" => await GetCars(term, max),
            "tires" => await GetTires(term, max),
            "categories" => await GetCategories(term, max),
            _ => []
        };

        return Json(results);
    }

    private async Task<List<LookupItemViewModel>> GetUsers(string? term, int limit)
    {
        return (await userRepository.GetListAsync(new UserFilter { Search = term }))
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

    private async Task<List<LookupItemViewModel>> GetCars(string? term, int limit)
    {
        return (await carRepository.GetListAsync(new CarFilter { Search = term }))
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

    private async Task<List<LookupItemViewModel>> GetTires(string? term, int limit)
    {
        return (await tireRepository.GetListAsync(new TireFilter { Search = term }))
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

    private async Task<List<LookupItemViewModel>> GetCategories(string? term, int limit)
    {
        return (await expenseCategoryRepository.GetListAsync(new ExpenseCategoryFilter { Search = term }))
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
