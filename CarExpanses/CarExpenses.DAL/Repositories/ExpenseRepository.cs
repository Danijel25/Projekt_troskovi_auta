using CarExpenses.Model.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace CarExpenses.DAL.Repositories;

public sealed class ExpenseRepository(CarExpesesDbContext dbContext) : IExpenseRepository
{
    public IQueryable<Expense> Query(ExpenseFilter filter)
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

        return query.OrderByDescending(expense => expense.Date);
    }

    public IReadOnlyList<Expense> GetAll() => Query(new ExpenseFilter()).ToList();

    public Expense? GetById(int id) => dbContext.Expenses
        .Include(expense => expense.Category)
        .AsNoTracking()
        .FirstOrDefault(expense => expense.Id == id);

    public void Add(Expense expense)
    {
        expense.Category ??= dbContext.ExpenseCategories.First(category => category.Id == expense.CategoryId);
        dbContext.Expenses.Add(expense);
        dbContext.SaveChanges();
    }

    public bool Update(Expense expense)
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
        existing.Category = dbContext.ExpenseCategories.First(category => category.Id == expense.CategoryId);

        dbContext.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var expense = dbContext.Expenses
            .Include(item => item.Category)
            .FirstOrDefault(item => item.Id == id);

        if (expense is null)
        {
            return false;
        }

        dbContext.Expenses.Remove(expense);
        dbContext.SaveChanges();
        return true;
    }
}