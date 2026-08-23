using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class ExpenseRepository(CarExpesesDbContext dbContext) : IExpenseRepository
{
    public async Task<IReadOnlyList<Expense>> GetListAsync(ExpenseFilter filter)
    {
        var query = dbContext.Expenses
            .Include(expense => expense.Category)
            .AsNoTracking()
            .AsQueryable();

        if (filter.CategoryId.HasValue)
        {
            query = query.Where(expense => expense.CategoryId == filter.CategoryId.Value);
        }

        if (filter.CarId.HasValue)
        {
            query = query.Where(expense => expense.CarId == filter.CarId.Value);
        }

        if (filter.FromDate.HasValue)
        {
            query = query.Where(expense => expense.Date >= filter.FromDate.Value);
        }

        if (filter.ToDate.HasValue)
        {
            query = query.Where(expense => expense.Date <= filter.ToDate.Value);
        }

        if (filter.MinAmount.HasValue)
        {
            query = query.Where(expense => expense.Amount >= filter.MinAmount.Value);
        }

        if (filter.MaxAmount.HasValue)
        {
            query = query.Where(expense => expense.Amount <= filter.MaxAmount.Value);
        }

        var term = filter.Search?.Trim();
        if (!string.IsNullOrWhiteSpace(term))
        {
            var hasId = int.TryParse(term, NumberStyles.Integer, CultureInfo.CurrentCulture, out var idValue);
            var hasAmount = decimal.TryParse(term, NumberStyles.Any, CultureInfo.CurrentCulture, out var amountValue);
            var hasExactDate = DateTime.TryParseExact(term, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dateValue)
                || DateTime.TryParse(term, CultureInfo.CurrentCulture, DateTimeStyles.AssumeLocal, out dateValue);
            var hasMonthDay = DateTime.TryParseExact(term, "MM/dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var monthDayValue);

            query = query.Where(expense =>
                expense.Description.Contains(term)
                || (expense.Category != null && expense.Category.Name.Contains(term))
                || (hasId && expense.Id == idValue)
                || (hasAmount && expense.Amount == amountValue)
                || (hasExactDate && expense.Date.Date == dateValue.Date)
                || (hasMonthDay && expense.Date.Month == monthDayValue.Month && expense.Date.Day == monthDayValue.Day));
        }

        return await query.OrderByDescending(expense => expense.Date).ToListAsync();
    }

    public async Task<IReadOnlyList<Expense>> GetAllAsync() => await GetListAsync(new ExpenseFilter());

    public async Task<Expense?> GetByIdAsync(int id) => await dbContext.Expenses
        .Include(expense => expense.Category)
        .Include(expense => expense.Car)
        .AsNoTracking()
        .FirstOrDefaultAsync(expense => expense.Id == id);

    public async Task<int> AddAsync(Expense expense)
    {        
        dbContext.Expenses.Add(expense);
        dbContext.SaveChanges();
        return expense.Id;
    }

    public async Task<bool> UpdateAsync(Expense expense)
    {
        var existing = dbContext.Expenses.FirstOrDefault(item => item.Id == expense.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Description = expense.Description;
        existing.Amount = expense.Amount;
        existing.Date = expense.Date;
        existing.CategoryId = expense.CategoryId;
        existing.Category = await dbContext.ExpenseCategories.FirstAsync(category => category.Id == expense.CategoryId);

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var expense = await dbContext.Expenses
            .Include(item => item.Category)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (expense is null)
        {
            return false;
        }

        dbContext.Expenses.Remove(expense);
        await dbContext.SaveChangesAsync();
        return true;
    }
}