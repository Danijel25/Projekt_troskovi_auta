using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class ExpenseCategoryMockRepository
{
    private readonly List<ExpenseCategory> _categories =
    [
        new ExpenseCategory
        {
            Id = 1,
            Name = "Maintenance",
            Expenses = new List<Expense>()
        },
        new ExpenseCategory
        {
            Id = 2,
            Name = "Registration",
            Expenses = new List<Expense>()
        },
        new ExpenseCategory
        {
            Id = 3,
            Name = "Parking",
            Expenses = new List<Expense>()
        }
    ];

    public IReadOnlyList<ExpenseCategory> GetAll() => _categories;

    public ExpenseCategory? GetById(int id) => _categories.FirstOrDefault(category => category.Id == id);
}
