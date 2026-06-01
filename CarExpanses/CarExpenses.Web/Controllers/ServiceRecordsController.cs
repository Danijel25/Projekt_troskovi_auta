using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarExpenses.Web.Controllers;

[Authorize]
public class ServiceRecordsController(IServiceRecordRepository repository, ICarRepository carRepository) : Controller
{
    public async Task<IActionResult> Index() => View(await repository.GetAllAsync());

    [HttpGet]
    public async Task<IActionResult> Search(string? query)
    {
        var serviceRecords = await repository.GetListAsync(new ServiceRecordFilter { Search = query });

        return PartialView("_ServiceRecordList", serviceRecords);
    }

    public async Task<IActionResult> Details(int id)
    {
        var serviceRecord = await repository.GetByIdAsync(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
    }

    [HttpGet]
    public async Task<IActionResult> Create() => View("Form", await BuildFormModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceRecordFormViewModel formModel)
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

        await repository.AddAsync(new ServiceRecord
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
    public async Task<IActionResult> Edit(int id)
    {
        var serviceRecord = await repository.GetByIdAsync(id);
        return serviceRecord is null ? NotFound() : View("Form", await BuildFormModel(serviceRecord));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceRecordFormViewModel formModel)
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

        if (!await repository.UpdateAsync(serviceRecord))
        {
            return NotFound();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var serviceRecord = await repository.GetByIdAsync(id);
        return serviceRecord is null ? NotFound() : View(serviceRecord);
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

    private async Task<ServiceRecordFormViewModel> BuildFormModel(ServiceRecord? serviceRecord = null)
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
            CarOptions = (await carRepository.GetAllAsync())
                .OrderBy(car => car.Brand)
                .ThenBy(car => car.Model)
                .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
                .ToList()
        };
    }

    private async Task<ServiceRecordFormViewModel> BuildFormModel(ServiceRecordFormViewModel formModel)
    {
        formModel.CarOptions = (await carRepository.GetAllAsync())
            .OrderBy(car => car.Brand)
            .ThenBy(car => car.Model)
            .Select(car => new SelectListItem($"{car.Brand} {car.Model} ({car.Year})", car.Id.ToString()))
            .ToList();
        return formModel;
    }
}


