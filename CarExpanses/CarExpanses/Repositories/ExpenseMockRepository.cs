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

        _expenses =
        [
            new Expense
            {
                Id = 8001,
                Description = "Annual vehicle registration and road tax",
                Amount = 362m,
                Date = new DateTime(2026, 1, 14),
                CategoryId = registrationCategory.Id,
                Category = registrationCategory
            },
            new Expense
            {
                Id = 8002,
                Description = "City center parking pass (monthly)",
                Amount = 58m,
                Date = new DateTime(2026, 2, 1),
                CategoryId = parkingCategory.Id,
                Category = parkingCategory
            },
            new Expense
            {
                Id = 8003,
                Description = "Air conditioning disinfection and filter service",
                Amount = 79m,
                Date = new DateTime(2026, 2, 10),
                CategoryId = maintenanceCategory.Id,
                Category = maintenanceCategory
            },
            new Expense
            {
                Id = 8004,
                Description = "Public parking garage fees",
                Amount = 34m,
                Date = new DateTime(2026, 2, 26),
                CategoryId = parkingCategory.Id,
                Category = parkingCategory
            },
            new Expense
            {
                Id = 8005,
                Description = "Wheel alignment and suspension check",
                Amount = 96m,
                Date = new DateTime(2026, 1, 22),
                CategoryId = maintenanceCategory.Id,
                Category = maintenanceCategory
            },
            new Expense
            {
                Id = 8006,
                Description = "Annual EV registration renewal",
                Amount = 290m,
                Date = new DateTime(2026, 3, 3),
                CategoryId = registrationCategory.Id,
                Category = registrationCategory
            }
        ];

        maintenanceCategory.Expenses!.Add(_expenses[2]);
        maintenanceCategory.Expenses!.Add(_expenses[4]);
        registrationCategory.Expenses!.Add(_expenses[0]);
        registrationCategory.Expenses!.Add(_expenses[5]);
        parkingCategory.Expenses!.Add(_expenses[1]);
        parkingCategory.Expenses!.Add(_expenses[3]);
    }

    public IReadOnlyList<Expense> GetAll() => _expenses;

    public Expense? GetById(int id) => _expenses.FirstOrDefault(expense => expense.Id == id);
}
