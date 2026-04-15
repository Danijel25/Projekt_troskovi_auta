using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class ExpenseCategoryMockRepository
{
    private readonly List<ExpenseCategory> _categories;

    public ExpenseCategoryMockRepository()
    {
        var category1 = new ExpenseCategory
        {
            Id = 1,
            Name = "Maintenance",
            Expenses = new List<Expense>()
        };

        var category2 = new ExpenseCategory
        {
            Id = 2,
            Name = "Registration",
            Expenses = new List<Expense>()
        };

        var category3 = new ExpenseCategory
        {
            Id = 3,
            Name = "Parking",
            Expenses = new List<Expense>()
        };

        _categories = new List<ExpenseCategory> { category1, category2, category3 };
    }

    public IReadOnlyList<ExpenseCategory> GetAll() => _categories;

    public ExpenseCategory? GetById(int id) => _categories.FirstOrDefault(category => category.Id == id);
}
