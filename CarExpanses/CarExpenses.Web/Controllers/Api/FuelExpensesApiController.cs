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
        [FromQuery] int? carId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] decimal? minLiters,
        [FromQuery] decimal? maxLiters)
    {
        var fuelExpenses = await fuelExpenseRepository
            .Query(new FuelExpenseFilter
            {
                CarId = carId,
                FromDate = fromDate,
                ToDate = toDate,
                MinLiters = minLiters,
                MaxLiters = maxLiters
            })
            .ToListAsync();
        var result = fuelExpenses.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<FuelExpenseDto>> GetById(int id)
    {
        var fuelExpense = await dbContext.FuelExpenses
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

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

        dbContext.FuelExpenses.Add(fuelExpense);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.FuelExpenses
            .Include(item => item.Car)
            .AsNoTracking()
            .FirstAsync(item => item.Id == fuelExpense.Id);

        return CreatedAtAction(nameof(GetById), new { id = fuelExpense.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, FuelExpenseUpdateDto dto)
    {
        var fuelExpense = await dbContext.FuelExpenses.FirstOrDefaultAsync(item => item.Id == id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        fuelExpense.FuelExpenseDate = dto.FuelExpenseDate;
        fuelExpense.Liters = dto.Liters;
        fuelExpense.PricePerLiter = dto.PricePerLiter;
        fuelExpense.Kilometars = dto.Kilometars;
        fuelExpense.CarId = dto.CarId;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var fuelExpense = await dbContext.FuelExpenses.FirstOrDefaultAsync(item => item.Id == id);
        if (fuelExpense is null)
        {
            return NotFound();
        }

        dbContext.FuelExpenses.Remove(fuelExpense);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
