using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace CarExpenses.DAL.Repositories;

public sealed class ExpenseCategoryRepository(CarExpesesDbContext dbContext) : IExpenseCategoryRepository
{
    public async Task<IReadOnlyList<ExpenseCategory>> GetListAsync(ExpenseCategoryFilter filter)
    {
        var query = dbContext.ExpenseCategories
            .Include(category => category.Expenses)
            .AsNoTracking()
            .AsQueryable();

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasId = int.TryParse(term, out var idValue);
            query = query.Where(category =>
                category.Name.Contains(term)
                || (hasId && category.Id == idValue));
        }

        return await query.OrderBy(category => category.Id)
                            .ToListAsync();
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetAllAsync() => await GetListAsync(new ExpenseCategoryFilter());

    public async Task<ExpenseCategory?> GetByIdAsync(int id) => await dbContext.ExpenseCategories
        .Include(category => category.Expenses)
        .AsNoTracking()
        .FirstOrDefaultAsync(category => category.Id == id);

    public async Task<int> AddAsync(ExpenseCategory category)
    {
        dbContext.ExpenseCategories.Add(category);
        await dbContext.SaveChangesAsync();
        return category.Id;
    }

    public async Task<bool> UpdateAsync(ExpenseCategory category)
    {
        var existing = await dbContext.ExpenseCategories.FirstOrDefaultAsync(item => item.Id == category.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = category.Name;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await dbContext.ExpenseCategories
            .Include(item => item.Expenses)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (category is null)
        {
            return false;
        }

        dbContext.ExpenseCategories.Remove(category);
        await dbContext.SaveChangesAsync();
        return true;
    }
}