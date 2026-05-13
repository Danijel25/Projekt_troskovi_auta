using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

public class ServiceRecordsController(IServiceRecordRepository repository, ICarRepository carRepository) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

    [HttpGet]
    public IActionResult Search(string? query)
    {
        var serviceRecords = repository.GetAll();

        if (string.IsNullOrWhiteSpace(query))
        {
            return PartialView("_ServiceRecordList", serviceRecords);
        }

        var term = query.Trim();
        var filtered = serviceRecords
            .Where(record =>
                record.ServiceType.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Description.Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Cost.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.ServiceDate.ToString("yyyy-MM-dd").Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Mileage.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.CarId.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || record.Id.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                || (record.Car?.Brand?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false)
                || (record.Car?.Model?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();

        return PartialView("_ServiceRecordList", filtered);
    }

    public IActionResult Details(int id)
    {
        var serviceRecord = repository.GetById(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
    }

    [HttpGet]
    public IActionResult Create() => View("Form", BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(ServiceRecordFormViewModel formModel)
    {
        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        repository.Add(new ServiceRecord
        {
            ServiceType = formModel.ServiceType,
            Description = formModel.Description,
            Cost = formModel.Cost,
            ServiceDate = formModel.ServiceDate,
            Mileage = formModel.Mileage,
            CarId = formModel.CarId
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var serviceRecord = repository.GetById(id);
        return serviceRecord is null ? NotFound() : View("Form", BuildFormModel(serviceRecord));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, ServiceRecordFormViewModel formModel)
    {
        if (id != formModel.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View("Form", BuildFormModel(formModel));
        }

        var serviceRecord = new ServiceRecord
        {
            Id = formModel.Id,
            ServiceType = formModel.ServiceType,
            Description = formModel.Description,
            Cost = formModel.Cost,
            ServiceDate = formModel.ServiceDate,
            Mileage = formModel.Mileage,
            CarId = formModel.CarId
        };

        if (!repository.Update(serviceRecord))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var serviceRecord = repository.GetById(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
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

    private ServiceRecordFormViewModel BuildFormModel(ServiceRecord? serviceRecord = null)
    {
        return new ServiceRecordFormViewModel
        {
            Id = serviceRecord?.Id ?? 0,
            ServiceType = serviceRecord?.ServiceType ?? string.Empty,
            Description = serviceRecord?.Description ?? string.Empty,
            Cost = serviceRecord?.Cost ?? 0,
            ServiceDate = serviceRecord?.ServiceDate ?? DateTime.Today,
            Mileage = serviceRecord?.Mileage ?? 0,
            CarId = serviceRecord?.CarId ?? 0,
            CarOptions = carRepository.GetAll()
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private ServiceRecordFormViewModel BuildFormModel(ServiceRecordFormViewModel formModel)
    {
        formModel.CarOptions = carRepository.GetAll()
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


