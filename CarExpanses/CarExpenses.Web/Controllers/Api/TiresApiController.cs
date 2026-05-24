using CarExpenses.DAL;
using CarExpenses.Model.Models;
using CarExpenses.Web.Api.Dtos;
using CarExpenses.Web.Api.Mapping;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.Web.Controllers.Api;

[ApiController]
[Route("api/tires")]
public sealed class TiresApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;

    public TiresApiController(CarExpesesDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TireSummaryDto>>> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? season,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice)
    {
        var query = dbContext.Tires.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(tire => tire.Brand.Contains(term) || tire.Model.Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(season))
        {
            var term = season.Trim();
            query = query.Where(tire => tire.Season.Contains(term));
        }

        if (minPrice.HasValue)
        {
            query = query.Where(tire => tire.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(tire => tire.Price <= maxPrice.Value);
        }

        var tires = await query.OrderBy(tire => tire.Id).ToListAsync();
        var result = tires.Select(DtoMapping.ToSummaryDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TireDetailDto>> GetById(int id)
    {
        var tire = await dbContext.Tires
            .Include(item => item.CarTires)!
                .ThenInclude(carTire => carTire.Car)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (tire is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(tire));
    }

    [HttpPost]
    public async Task<ActionResult<TireDetailDto>> Create(TireCreateDto dto)
    {
        var tire = new Tire
        {
            Brand = dto.Brand,
            Model = dto.Model,
            Season = dto.Season,
            Price = dto.Price
        };

        dbContext.Tires.Add(tire);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.Tires
            .Include(item => item.CarTires)!
                .ThenInclude(carTire => carTire.Car)
            .AsNoTracking()
            .FirstAsync(item => item.Id == tire.Id);

        return CreatedAtAction(nameof(GetById), new { id = tire.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TireUpdateDto dto)
    {
        var tire = await dbContext.Tires.FirstOrDefaultAsync(item => item.Id == id);
        if (tire is null)
        {
            return NotFound();
        }

        tire.Brand = dto.Brand;
        tire.Model = dto.Model;
        tire.Season = dto.Season;
        tire.Price = dto.Price;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tire = await dbContext.Tires
            .Include(item => item.CarTires)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (tire is null)
        {
            return NotFound();
        }

        dbContext.Tires.Remove(tire);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
