using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/fuel-expenses")]
public sealed class FuelExpensesApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IFuelExpenseRepository fuelExpenseRepository;

    public FuelExpensesApiController(CarExpesesDbContext dbContext, IFuelExpenseRepository fuelExpenseRepository)
    {
        this.dbContext = dbContext;
        this.fuelExpenseRepository = fuelExpenseRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FuelExpenseDto>>> GetAll(
        [FromQuery] FuelExpenseFilter filter)
    {
        var fuelExpenses = await fuelExpenseRepository
            .GetListAsync(filter);
        var result = fuelExpenses.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FuelExpenseDto>> GetById(int id)
    {
        var fuelExpense = await fuelExpenseRepository.GetByIdAsync(id);

        if (fuelExpense is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDto(fuelExpense));
    }

    [HttpPost]
    public async Task<ActionResult<FuelExpenseDto>> Create(FuelExpenseCreateDto dto)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        var fuelExpense = new FuelExpense
        {
            FuelExpenseDate = dto.FuelExpenseDate,
            Liters = dto.Liters,
            PricePerLiter = dto.PricePerLiter,
            Kilometars = dto.Kilometars,
            CarId = dto.CarId
        };

        var fuelExpenseId = await fuelExpenseRepository.AddAsync(fuelExpense);

        var created = await fuelExpenseRepository.GetByIdAsync(fuelExpenseId);

        return CreatedAtAction(nameof(GetById), new { id = fuelExpense.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FuelExpenseUpdateDto dto)
    {
        var fuelExpense = await fuelExpenseRepository.GetByIdAsync(id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        await fuelExpenseRepository.UpdateAsync(new()
        {
            Id = id,
            FuelExpenseDate = dto.FuelExpenseDate,
            Liters = dto.Liters,
            PricePerLiter = dto.PricePerLiter,
            Kilometars = dto.Kilometars,
            CarId = dto.CarId
        });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fuelExpense = await fuelExpenseRepository.GetByIdAsync(id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        await fuelExpenseRepository.DeleteAsync(id);
        return NoContent();
    }
}
