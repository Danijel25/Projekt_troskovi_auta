using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
using CarExpenses.Model.Enums;
using CarExpenses.Model.Models;
using CarExpenses.Model.Security;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/cars")]
public sealed class CarsApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly ICurrentUserService currentUserService;
    private readonly ICarRepository carRepository;

    public CarsApiController(
        CarExpesesDbContext dbContext,
        ICurrentUserService currentUserService,
        ICarRepository carRepository)
    {
        this.dbContext = dbContext;
        this.currentUserService = currentUserService;
        this.carRepository = carRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CarListItemDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] int? userId,
        [FromQuery] FuelType? fuelType,
        [FromQuery] int? minYear,
        [FromQuery] int? maxYear)
    {
        var cars = await carRepository
            .Query(new CarFilter
            {
                Search = search,
                UserId = userId,
                FuelType = fuelType,
                MinYear = minYear,
                MaxYear = maxYear
            })
            .Include(car => car.User)
            .ToListAsync();
        var result = cars.Select(DtoMapping.ToListItemDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CarDetailDto>> GetById(int id)
    {
        var car = await GetCarDetailsAsync(id);
        if (car is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(car));
    }

    [HttpPost]
    public async Task<ActionResult<CarDetailDto>> Create(CarCreateDto dto)
    {
        var userId = dto.UserId;
        if (!User.IsInRole(AppRoles.Admin))
        {
            if (!currentUserService.UserId.HasValue)
            {
                return Forbid();
            }

            userId = currentUserService.UserId.Value;
        }

        if (!await dbContext.Users.AnyAsync(user => user.Id == userId))
        {
            ModelState.AddModelError(nameof(dto.UserId), "User not found.");
            return ValidationProblem(ModelState);
        }

        var car = new Car
        {
            UserId = userId,
            Brand = dto.Brand,
            Model = dto.Model,
            Year = dto.Year,
            EngineVolume = dto.EngineVolume,
            CurrentMilage = dto.CurrentMilage,
            PurchasePrice = dto.PurchasePrice,
            PurchaseDate = dto.PurchaseDate,
            FuelType = dto.FuelType
        };

        dbContext.Cars.Add(car);
        await dbContext.SaveChangesAsync();

        var created = await GetCarDetailsAsync(car.Id);
        if (created is null)
        {
            return NotFound();
        }

        return CreatedAtAction(nameof(GetById), new { id = car.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CarUpdateDto dto)
    {
        var car = await dbContext.Cars.FirstOrDefaultAsync(item => item.Id == id);
        if (car is null)
        {
            return NotFound();
        }

        if (User.IsInRole(AppRoles.Admin))
        {
            if (!await dbContext.Users.AnyAsync(user => user.Id == dto.UserId))
            {
                ModelState.AddModelError(nameof(dto.UserId), "User not found.");
                return ValidationProblem(ModelState);
            }

            car.UserId = dto.UserId;
        }
        car.Brand = dto.Brand;
        car.Model = dto.Model;
        car.Year = dto.Year;
        car.EngineVolume = dto.EngineVolume;
        car.CurrentMilage = dto.CurrentMilage;
        car.PurchasePrice = dto.PurchasePrice;
        car.PurchaseDate = dto.PurchaseDate;
        car.FuelType = dto.FuelType;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var car = await dbContext.Cars
            .Include(item => item.FuelExpenses)
            .Include(item => item.ServiceRecords)
            .Include(item => item.Insurances)
            .Include(item => item.CarTires)!
                .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Expenses)!
                .ThenInclude(expense => expense.Category)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (car is null)
        {
            return NotFound();
        }

        dbContext.Cars.Remove(car);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    private Task<Car?> GetCarDetailsAsync(int id)
    {
        return dbContext.Cars
            .Include(item => item.User)
            .Include(item => item.FuelExpenses)
            .Include(item => item.ServiceRecords)
            .Include(item => item.Insurances)
            .Include(item => item.CarTires)!
                .ThenInclude(carTire => carTire.Tire)
            .Include(item => item.Expenses)!
                .ThenInclude(expense => expense.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);
    }
}
