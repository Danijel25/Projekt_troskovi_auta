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
[Route("api/tires")]
public sealed class TiresApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly ITireRepository tireRepository;

    public TiresApiController(CarExpesesDbContext dbContext, ITireRepository tireRepository)
    {
        this.dbContext = dbContext;
        this.tireRepository = tireRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TireSummaryDto>>> GetAll(
        [FromQuery] TireFilter filter)
    {
        var tires = await tireRepository
            .GetListAsync(filter);
        var result = tires.Select(DtoMapping.ToSummaryDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TireDetailDto>> GetById(int id)
    {
        var tire = await tireRepository .GetByIdAsync(id);

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

        var tireId = await tireRepository.AddAsync(tire);

        var created = await tireRepository.GetByIdAsync(tireId);

        return CreatedAtAction(nameof(GetById), new { id = tire.Id }, DtoMapping.ToDetailDto(created));
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TireUpdateDto dto)
    {
        var tire = await tireRepository.GetByIdAsync(id);
        if (tire is null)
        {
            return NotFound();
        }

        await tireRepository.UpdateAsync(new()
        {
            Id = id,
            Brand = dto.Brand,
            Model = dto.Model,
            Season = dto.Season,
            Price = dto.Price
        });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var tire = await tireRepository.GetByIdAsync(id);

        if (tire is null)
        {
            return NotFound();
        }

        await tireRepository.DeleteAsync(id);
        return NoContent();
    }
}
