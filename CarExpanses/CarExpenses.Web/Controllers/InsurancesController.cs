using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class InsurancesController(IInsuranceRepository repository, ICarRepository carRepository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var insurances = await repository.GetListAsync(new InsuranceFilter { Search = query });
        return PartialView("_InsuranceList", insurances);
    }

    public async Task<IActionResult> Details(int id)
    {
        var insurance = await repository.GetByIdAsync(id);
        return insurance is null ? NotFound() : View(insurance);
    }

    [HttpGet]
    public async Task<IActionResult> Create() => View("Form", await BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InsuranceFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        await repository.AddAsync(new Insurance
        {
            Company = formModel.Company,
            InsuranceType = formModel.InsuranceType,
            Price = formModel.Price,
            StartDate = formModel.StartDate,
            EndDate = formModel.EndDate,
            CarId = formModel.CarId
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var insurance = await repository.GetByIdAsync(id);
        return insurance is null ? NotFound() : View("Form", await BuildFormModel(insurance));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InsuranceFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (await carRepository.GetByIdAsync(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        var insurance = new Insurance
        {
            Id = formModel.Id,
            Company = formModel.Company,
            InsuranceType = formModel.InsuranceType,
            Price = formModel.Price,
            StartDate = formModel.StartDate,
            EndDate = formModel.EndDate,
            CarId = formModel.CarId
        };

        if (!await repository.UpdateAsync(insurance))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var insurance = await repository.GetByIdAsync(id);
        return insurance is null ? NotFound() : View(insurance);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (!await repository.DeleteAsync(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<InsuranceFormViewModel> BuildFormModel(Insurance? insurance = null)
    {
        return new InsuranceFormViewModel
        {
            Id = insurance?.Id ?? 0,
            Company = insurance?.Company ?? string.Empty,
            InsuranceType = insurance?.InsuranceType ?? string.Empty,
            Price = insurance?.Price ?? 0,
            StartDate = insurance?.StartDate ?? DateTime.Today,
            EndDate = insurance?.EndDate ?? DateTime.Today,
            CarId = insurance?.CarId ?? 0,
            CarOptions = (await carRepository.GetAllAsync())
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private async Task<InsuranceFormViewModel> BuildFormModel(InsuranceFormViewModel formModel)
    {
        formModel.CarOptions = (await carRepository.GetAllAsync())
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


