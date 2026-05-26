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
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var insurances = repository.Query(new InsuranceFilter { Search = query }).ToList();
        return PartialView("_InsuranceList", insurances);
    }

    public IActionResult Details(int id)
    {
        var insurance = repository.GetById(id);
        return insurance is null ? NotFound() : View(insurance);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(InsuranceFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (carRepository.GetById(formModel.CarId) is null)
        {
            ModelState.AddModelError(nameof(formModel.CarId), "Car not found.");
            return View("Form", BuildFormModel(formModel));
        }

        repository.Add(new Insurance
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
    public IActionResult Edit(int id)
    {
        var insurance = repository.GetById(id);
        return insurance is null ? NotFound() : View("Form", BuildFormModel(insurance));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, InsuranceFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        if (carRepository.GetById(formModel.CarId) is null)
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

        if (!repository.Update(insurance))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var insurance = repository.GetById(id);
        return insurance is null ? NotFound() : View(insurance);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        if (!repository.Delete(id))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    private InsuranceFormViewModel BuildFormModel(Insurance? insurance = null)
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
            CarOptions = carRepository.GetAll()
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private InsuranceFormViewModel BuildFormModel(InsuranceFormViewModel formModel)
    {
        formModel.CarOptions = carRepository.GetAll()
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


