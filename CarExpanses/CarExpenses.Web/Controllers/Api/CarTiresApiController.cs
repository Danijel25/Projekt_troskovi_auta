using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/car-tires")]
public sealed class CarTiresApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;

    public CarTiresApiController(CarExpesesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarTireDto>>> GetAll(
        [FromQuery] int? carId,
        [FromQuery] int? tireId,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var query = dbContext.CarTires
            .Include(item => item.Car)
            .Include(item => item.Tire)
            .AsNoTracking()
            .AsQueryable();

        if (carId.HasValue)
        {
            query = query.Where(item => item.CarId == carId.Value);
        }

        if (tireId.HasValue)
        {
            query = query.Where(item => item.TireId == tireId.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(item => item.InstalledDate >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(item => item.InstalledDate <= toDate.Value);
        }

        var carTires = await query.OrderByDescending(item => item.InstalledDate).ToListAsync();
        var result = carTires.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarTireDto>> GetById(int id)
    {
        var carTire = await dbContext.CarTires
            .Include(item => item.Car)
            .Include(item => item.Tire)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (carTire is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDto(carTire));
    }

    [HttpPost]
    public async Task<ActionResult<CarTireDto>> Create(CarTireCreateDto dto)
    {
        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        if (!await dbContext.Tires.AnyAsync(tire => tire.Id == dto.TireId))
        {
            ModelState.AddModelError(nameof(dto.TireId), "Tire not found.");
            return ValidationProblem(ModelState);
        }

        var carTire = new CarTire
        {
            CarId = dto.CarId,
            TireId = dto.TireId,
            InstalledDate = dto.InstalledDate
        };

        dbContext.CarTires.Add(carTire);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.CarTires
            .Include(item => item.Car)
            .Include(item => item.Tire)
            .AsNoTracking()
            .FirstAsync(item => item.Id == carTire.Id);

        return CreatedAtAction(nameof(GetById), new { id = carTire.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CarTireUpdateDto dto)
    {
        var carTire = await dbContext.CarTires.FirstOrDefaultAsync(item => item.Id == id);
        if (carTire is null)
        {
            return NotFound();
        }

        if (!await dbContext.Cars.AnyAsync(car => car.Id == dto.CarId))
        {
            ModelState.AddModelError(nameof(dto.CarId), "Car not found.");
            return ValidationProblem(ModelState);
        }

        if (!await dbContext.Tires.AnyAsync(tire => tire.Id == dto.TireId))
        {
            ModelState.AddModelError(nameof(dto.TireId), "Tire not found.");
            return ValidationProblem(ModelState);
        }

        carTire.CarId = dto.CarId;
        carTire.TireId = dto.TireId;
        carTire.InstalledDate = dto.InstalledDate;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var carTire = await dbContext.CarTires.FirstOrDefaultAsync(item => item.Id == id);
        if (carTire is null)
        {
            return NotFound();
        }

        dbContext.CarTires.Remove(carTire);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
