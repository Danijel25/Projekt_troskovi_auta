using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers;

public class ServiceRecordsController(IServiceRecordRepository repository, ICarRepository carRepository, CarExpesesDbContext dbContext) : Controller
{
    public IActionResult Index() => View(repository.GetAll());

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

        dbContext.ServiceRecords.Add(new ServiceRecord
        {
            ServiceType = formModel.ServiceType,
            Description = formModel.Description,
            Cost = formModel.Cost,
            ServiceDate = formModel.ServiceDate,
            Mileage = formModel.Mileage,
            CarId = formModel.CarId
        });

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var serviceRecord = dbContext.ServiceRecords.AsNoTracking().FirstOrDefault(item => item.Id == id);
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

        var serviceRecord = dbContext.ServiceRecords.FirstOrDefault(item => item.Id == id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        serviceRecord.ServiceType = formModel.ServiceType;
        serviceRecord.Description = formModel.Description;
        serviceRecord.Cost = formModel.Cost;
        serviceRecord.ServiceDate = formModel.ServiceDate;
        serviceRecord.Mileage = formModel.Mileage;
        serviceRecord.CarId = formModel.CarId;

        dbContext.SaveChanges();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var serviceRecord = dbContext.ServiceRecords.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var serviceRecord = dbContext.ServiceRecords.Include(item => item.Car).FirstOrDefault(item => item.Id == id);
        if (serviceRecord is null)
        {
            return NotFound();
        }

        dbContext.ServiceRecords.Remove(serviceRecord);
        dbContext.SaveChanges();
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


