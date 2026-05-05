using CarExpenses.Model.Models;

namespace CarExpenses.DAL.Repositories;

public interface IUserRepository
{
    IReadOnlyList<User> GetAll();
    User? GetById(int id);
}

public interface ICarRepository
{
    IReadOnlyList<Car> GetAll();
    Car? GetById(int id);
}

public interface ITireRepository
{
    IReadOnlyList<Tire> GetAll();
    Tire? GetById(int id);
}

public interface ICarTireRepository
{
    IReadOnlyList<CarTire> GetAll();
    CarTire? GetById(int id);
}

public interface IFuelExpenseRepository
{
    IReadOnlyList<FuelExpense> GetAll();
    FuelExpense? GetById(int id);
}

public interface IServiceRecordRepository
{
    IReadOnlyList<ServiceRecord> GetAll();
    ServiceRecord? GetById(int id);
}

public interface IInsuranceRepository
{
    IReadOnlyList<Insurance> GetAll();
    Insurance? GetById(int id);
}

public interface IExpenseCategoryRepository
{
    IReadOnlyList<ExpenseCategory> GetAll();
    ExpenseCategory? GetById(int id);
}

public interface IExpenseRepository
{
    IReadOnlyList<Expense> GetAll();
    Expense? GetById(int id);
}