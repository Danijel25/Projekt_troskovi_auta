using CarExpanses.Models;

namespace CarExpanses.Repositories;

public sealed class ExpenseMockRepository
{
    private readonly List<Expense> _expenses;

    public ExpenseMockRepository()
    {
        var maintenanceCategory = new ExpenseCategory { Id = 1, Name = "Maintenance", Expenses = new List<Expense>() };
        var registrationCategory = new ExpenseCategory { Id = 2, Name = "Registration", Expenses = new List<Expense>() };
        var parkingCategory = new ExpenseCategory { Id = 3, Name = "Parking", Expenses = new List<Expense>() };

        var expense1 = new Expense
        {
            Id = 8001,
            Description = "Annual vehicle registration and road tax",
            Amount = 362m,
            Date = new DateTime(2026, 1, 14),
            CategoryId = registrationCategory.Id,
            Category = registrationCategory
        };

        var expense2 = new Expense
        {
            Id = 8002,
            Description = "City center parking pass (monthly)",
            Amount = 58m,
            Date = new DateTime(2026, 2, 1),
            CategoryId = parkingCategory.Id,
            Category = parkingCategory
        };

        var expense3 = new Expense
        {
            Id = 8003,
            Description = "Air conditioning disinfection and filter service",
            Amount = 79m,
            Date = new DateTime(2026, 2, 10),
            CategoryId = maintenanceCategory.Id,
            Category = maintenanceCategory
        };

        var expense4 = new Expense
        {
            Id = 8004,
            Description = "Public parking garage fees",
            Amount = 34m,
            Date = new DateTime(2026, 2, 26),
            CategoryId = parkingCategory.Id,
            Category = parkingCategory
        };

        var expense5 = new Expense
        {
            Id = 8005,
            Description = "Wheel alignment and suspension check",
            Amount = 96m,
            Date = new DateTime(2026, 1, 22),
            CategoryId = maintenanceCategory.Id,
            Category = maintenanceCategory
        };

        var expense6 = new Expense
        {
            Id = 8006,
            Description = "Annual EV registration renewal",
            Amount = 290m,
            Date = new DateTime(2026, 3, 3),
            CategoryId = registrationCategory.Id,
            Category = registrationCategory
        };

        _expenses = new List<Expense> { expense1, expense2, expense3, expense4, expense5, expense6 };

        maintenanceCategory.Expenses!.Add(expense3);
        maintenanceCategory.Expenses!.Add(expense5);
        registrationCategory.Expenses!.Add(expense1);
        registrationCategory.Expenses!.Add(expense6);
        parkingCategory.Expenses!.Add(expense2);
        parkingCategory.Expenses!.Add(expense4);
    }

    public IReadOnlyList<Expense> GetAll() => _expenses;

    public Expense? GetById(int id) => _expenses.FirstOrDefault(expense => expense.Id == id);
}
