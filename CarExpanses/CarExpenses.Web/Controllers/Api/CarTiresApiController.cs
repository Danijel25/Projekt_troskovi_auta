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
[Route("api/car-tires")]
public sealed class CarTiresApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly ICarTireRepository carTireRepository;

    public CarTiresApiController(CarExpesesDbContext dbContext, ICarTireRepository carTireRepository)
    {
        this.dbContext = dbContext;
        this.carTireRepository = carTireRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarTireDto>>> GetAll(
        [FromQuery] CarTireFilter filter)
    {
        var carTires = await carTireRepository
            .GetListAsync(filter);
        var result = carTires.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarTireDto>> GetById(int id)
    {
        var carTire = await carTireRepository.GetByIdAsync(id);

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

        var carTireId = await carTireRepository.AddAsync(carTire);

        var created = await carTireRepository.GetByIdAsync(carTireId);

        return CreatedAtAction(nameof(GetById), new { id = carTire.Id }, DtoMapping.ToDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CarTireUpdateDto dto)
    {
        var carTire = await carTireRepository.GetByIdAsync(id);
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

        await carTireRepository.UpdateAsync(new()
        {
            Id = id,
            CarId = dto.CarId,
            TireId = dto.TireId,
            InstalledDate = dto.InstalledDate
        });
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var carTire = await carTireRepository.GetByIdAsync(id);
        if (carTire is null)
        {
            return NotFound();
        }

        await carTireRepository.DeleteAsync(id);
        return NoContent();
    }
}
