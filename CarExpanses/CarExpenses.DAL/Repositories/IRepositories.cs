using CarExpenses.Model.Models;
using CarExpenses.Model.Security;

namespace CarExpenses.DAL.Repositories;

public interface ICarRepository
{
    IQueryable<Car> Query(CarFilter filter);
    IReadOnlyList<Car> GetAll();
    Car? GetById(int id);
    void Add(Car car);
    bool Update(Car car);
    bool Delete(int id);
}

public interface ITireRepository
{
    IQueryable<Tire> Query(TireFilter filter);
    IReadOnlyList<Tire> GetAll();
    Tire? GetById(int id);
    void Add(Tire tire);
    bool Update(Tire tire);
    bool Delete(int id);
}

public interface ICarTireRepository
{
    IQueryable<CarTire> Query(CarTireFilter filter);
    IReadOnlyList<CarTire> GetAll();
    CarTire? GetById(int id);
    void Add(CarTire carTire);
    bool Update(CarTire carTire);
    bool Delete(int id);
}

public interface IFuelExpenseRepository
{
    IQueryable<FuelExpense> Query(FuelExpenseFilter filter);
    IReadOnlyList<FuelExpense> GetAll();
    FuelExpense? GetById(int id);
    void Add(FuelExpense fuelExpense);
    bool Update(FuelExpense fuelExpense);
    bool Delete(int id);
}

public interface IServiceRecordRepository
{
    IQueryable<ServiceRecord> Query(ServiceRecordFilter filter);
    IReadOnlyList<ServiceRecord> GetAll();
    ServiceRecord? GetById(int id);
    void Add(ServiceRecord serviceRecord);
    bool Update(ServiceRecord serviceRecord);
    bool Delete(int id);
}

public interface IInsuranceRepository
{
    IQueryable<Insurance> Query(InsuranceFilter filter);
    IReadOnlyList<Insurance> GetAll();
    Insurance? GetById(int id);
    void Add(Insurance insurance);
    bool Update(Insurance insurance);
    bool Delete(int id);
}

public interface IExpenseCategoryRepository
{
    IQueryable<ExpenseCategory> Query(ExpenseCategoryFilter filter);
    IReadOnlyList<ExpenseCategory> GetAll();
    ExpenseCategory? GetById(int id);
    void Add(ExpenseCategory category);
    bool Update(ExpenseCategory category);
    bool Delete(int id);
}

public interface IExpenseRepository
{
    IQueryable<Expense> Query(ExpenseFilter filter);
    IReadOnlyList<Expense> GetAll();
    Expense? GetById(int id);
    void Add(Expense expense);
    bool Update(Expense expense);
    bool Delete(int id);
}

public interface IUserRepository
{
    IQueryable<User> Query(UserFilter filter);
    Task<User?> GetByIdAsync(int id);
}