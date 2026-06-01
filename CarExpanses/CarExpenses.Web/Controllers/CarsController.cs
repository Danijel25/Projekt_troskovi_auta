using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
[Route("auti")]
public class CarsController(ICarRepository carRepository, UserManager<User> userManager, ICurrentUserService currentUserService) : Controller
{
    [Route("svi")]
    public async Task<IActionResult> Index()
    {
        return View(await carRepository.GetAllAsync());
    }

    [HttpGet]
    [Route("pretraga")]
    public async Task<IActionResult> Search(string? query)
    {
        var cars = await carRepository.GetListAsync(new CarFilter { Search = query });
        return PartialView("_CarList", cars);
    }

    [Route("detalji/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var car = await carRepository.GetByIdAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(car);
    }

    [HttpGet]
    [Route("novi")]
    public IActionResult Create()
    {
        return View(BuildFormModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("novi")]
    public async Task<IActionResult> Create(CarFormViewModel formModel)
    {
        if (!User.IsInRole(AppRoles.Admin))
        {
            var currentUserId = currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Forbid();
            }

            formModel.UserId = currentUserId.Value;
        }

        if (!ModelState.IsValid)
        {
            return View(BuildFormModel(formModel));
        }

        if (User.IsInRole(AppRoles.Admin) && !userManager.Users.Any(user => user.Id == formModel.UserId))
        {
            ModelState.AddModelError(nameof(formModel.UserId), "User not found.");
            return View(BuildFormModel(formModel));
        }

        var car = new Car
        {
            UserId = formModel.UserId,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Year = formModel.Year,
            EngineVolume = formModel.EngineVolume,
            CurrentMilage = formModel.CurrentMilage,
            PurchasePrice = formModel.PurchasePrice,
            PurchaseDate = formModel.PurchaseDate,
            FuelType = formModel.FuelType
        };

        await carRepository.AddAsync(car);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("uredi/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        var car = await carRepository.GetByIdAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(BuildFormModel(car));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Route("uredi/{id}")]
    public async Task<IActionResult> Edit(int id, CarFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!User.IsInRole(AppRoles.Admin))
        {
            var currentUserId = currentUserService.UserId;
            if (!currentUserId.HasValue)
            {
                return Forbid();
            }

            formModel.UserId = currentUserId.Value;
        }

        if (!ModelState.IsValid)
        {
            return View(BuildFormModel(formModel));
        }

        if (User.IsInRole(AppRoles.Admin) && !userManager.Users.Any(user => user.Id == formModel.UserId))
        {
            ModelState.AddModelError(nameof(formModel.UserId), "User not found.");
            return View(BuildFormModel(formModel));
        }

        var car = new Car
        {
            Id = formModel.Id,
            UserId = formModel.UserId,
            Brand = formModel.Brand,
            Model = formModel.Model,
            Year = formModel.Year,
            EngineVolume = formModel.EngineVolume,
            CurrentMilage = formModel.CurrentMilage,
            PurchasePrice = formModel.PurchasePrice,
            PurchaseDate = formModel.PurchaseDate,
            FuelType = formModel.FuelType
        };

        if (!await carRepository.UpdateAsync(car))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Route("obrisi/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await carRepository.GetByIdAsync(id);

        if (car is null)
        {
            return NotFound();
        }

        return View(car);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Route("obrisi/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await carRepository.DeleteAsync(id))
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private CarFormViewModel BuildFormModel(Car? car = null)
    {
        var formModel = new CarFormViewModel
        {
            Id = car?.Id ?? 0,
            UserId = car?.UserId ?? 0,
            Brand = car?.Brand ?? string.Empty,
            Model = car?.Model ?? string.Empty,
            Year = car?.Year ?? DateTime.UtcNow.Year,
            EngineVolume = car?.EngineVolume ?? 0,
            CurrentMilage = car?.CurrentMilage ?? 0,
            PurchasePrice = car?.PurchasePrice ?? 0,
            PurchaseDate = car?.PurchaseDate ?? DateTime.Today,
            FuelType = car?.FuelType ?? FuelType.Petrol
        };

        if (User.IsInRole(AppRoles.Admin))
        {
            formModel.UserOptions = userManager.Users
                .OrderBy(user => user.UserName)
                .Select(user => new SelectListItem($"{user.UserName} ({user.Email})", user.Id.ToString()))
                .ToList();
        }
        else
        {
            var currentUserId = currentUserService.UserId;
            if (currentUserId.HasValue)
            {
                formModel.UserId = currentUserId.Value;
            }

            formModel.UserOptions = Array.Empty<SelectListItem>();
        }

        return formModel;
    }

    private CarFormViewModel BuildFormModel(CarFormViewModel formModel)
    {
        if (User.IsInRole(AppRoles.Admin))
        {
            formModel.UserOptions = userManager.Users
                .OrderBy(user => user.UserName)
                .Select(user => new SelectListItem($"{user.UserName} ({user.Email})", user.Id.ToString()))
                .ToList();
        }
        else
        {
            var currentUserId = currentUserService.UserId;
            if (currentUserId.HasValue)
            {
                formModel.UserId = currentUserId.Value;
            }

            formModel.UserOptions = Array.Empty<SelectListItem>();
        }

        return formModel;
    }

}


