using CarExpenses.DAL;
using CarExpenses.DAL.Repositories;
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
[Route("api/expense-categories")]
public sealed class ExpenseCategoriesApiController : ControllerBase
{
    private readonly CarExpesesDbContext dbContext;
    private readonly IExpenseCategoryRepository expenseCategoryRepository;

    public ExpenseCategoriesApiController(
        CarExpesesDbContext dbContext,
        IExpenseCategoryRepository expenseCategoryRepository)
    {
        this.dbContext = dbContext;
        this.expenseCategoryRepository = expenseCategoryRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ExpenseCategoryDto>>> GetAll([FromQuery] string? search)
    {
        var categories = await expenseCategoryRepository
            .Query(new ExpenseCategoryFilter { Search = search })
            .ToListAsync();
        var result = categories.Select(DtoMapping.ToDto).ToList();
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ExpenseCategoryDetailDto>> GetById(int id)
    {
        var category = await dbContext.ExpenseCategories
            .Include(item => item.Expenses)
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        return Ok(DtoMapping.ToDetailDto(category));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ExpenseCategoryDetailDto>> Create(ExpenseCategoryCreateDto dto)
    {
        var category = new ExpenseCategory
        {
            Name = dto.Name
        };

        dbContext.ExpenseCategories.Add(category);
        await dbContext.SaveChangesAsync();

        var created = await dbContext.ExpenseCategories
            .Include(item => item.Expenses)
            .AsNoTracking()
            .FirstAsync(item => item.Id == category.Id);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, DtoMapping.ToDetailDto(created));
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ExpenseCategoryUpdateDto dto)
    {
        var category = await dbContext.ExpenseCategories.FirstOrDefaultAsync(item => item.Id == id);
        if (category is null)
        {
            return NotFound();
        }

        category.Name = dto.Name;

        await dbContext.SaveChangesAsync();
        return NoContent();
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await dbContext.ExpenseCategories
            .Include(item => item.Expenses)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (category is null)
        {
            return NotFound();
        }

        dbContext.ExpenseCategories.Remove(category);
        await dbContext.SaveChangesAsync();
        return NoContent();
    }
}
