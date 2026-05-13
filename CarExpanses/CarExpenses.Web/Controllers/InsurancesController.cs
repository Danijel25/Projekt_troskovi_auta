using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

public class InsurancesController(IInsuranceRepository repository, ICarRepository carRepository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

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

        dbContext.Insurances.Add(new Insurance
        {
            Company = formModel.Company,
            InsuranceType = formModel.InsuranceType,
            Price = formModel.Price,
            StartDate = formModel.StartDate,
            EndDate = formModel.EndDate,
            CarId = formModel.CarId
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var insurance = dbContext.Insurances.AsNoTracking().FirstOrDefault(item => item.Id == id);
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

        var insurance = dbContext.Insurances.FirstOrDefault(item => item.Id == id);
        if (insurance is null)
        {
            return NotFound();
        }

        insurance.Company = formModel.Company;
        insurance.InsuranceType = formModel.InsuranceType;
        insurance.Price = formModel.Price;
        insurance.StartDate = formModel.StartDate;
        insurance.EndDate = formModel.EndDate;
        insurance.CarId = formModel.CarId;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var insurance = dbContext.Insurances.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        return insurance is null ? NotFound() : View(insurance);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var insurance = dbContext.Insurances.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        if (insurance is null)
        {
            return NotFound();
        }

        dbContext.Insurances.Remove(insurance);
        dbContext.SaveChanges();
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


