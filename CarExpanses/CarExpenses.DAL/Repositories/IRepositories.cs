using CarExpenses.Model.Models;
using CarExpenses.Model.Security;

namespace CarExpenses.DAL.Repositories;

public interface ICarRepository
{
    Task<IReadOnlyList<Car>> GetListAsync(CarFilter filter);
    Task<IReadOnlyList<Car>> GetAllAsync();
    Task<Car?> GetByIdAsync(int id);
    Task<int> AddAsync(Car car);
    Task<bool> UpdateAsync(Car car);
    Task<bool> DeleteAsync(int id);
}

public interface ITireRepository
{
    Task<IReadOnlyList<Tire>> GetListAsync(TireFilter filter);
    Task<IReadOnlyList<Tire>> GetAllAsync();
    Task<Tire?> GetByIdAsync(int id);
    Task<int> AddAsync(Tire tire);
    Task<bool> UpdateAsync(Tire tire);
    Task<bool> DeleteAsync(int id);
}

public interface ICarTireRepository
{
    Task<IReadOnlyList<CarTire>> GetListAsync(CarTireFilter filter);
    Task<IReadOnlyList<CarTire>> GetAllAsync();
    Task<CarTire?> GetByIdAsync(int id);
    Task<int> AddAsync(CarTire carTire);
    Task<bool> UpdateAsync(CarTire carTire);
    Task<bool> DeleteAsync(int id);
}

public interface IFuelExpenseRepository
{
    Task<IReadOnlyList<FuelExpense>> GetListAsync(FuelExpenseFilter filter);
    Task<IReadOnlyList<FuelExpense>> GetAllAsync();
    Task<FuelExpense?> GetByIdAsync(int id);
    Task<int> AddAsync(FuelExpense fuelExpense);
    Task<bool> UpdateAsync(FuelExpense fuelExpense);
    Task<bool> DeleteAsync(int id);
}

public interface IServiceRecordRepository
{
    Task<IReadOnlyList<ServiceRecord>> GetListAsync(ServiceRecordFilter filter);
    Task<IReadOnlyList<ServiceRecord>> GetAllAsync();
    Task<ServiceRecord?> GetByIdAsync(int id);
    Task<int> AddAsync(ServiceRecord serviceRecord);
    Task<bool> UpdateAsync(ServiceRecord serviceRecord);
    Task<bool> DeleteAsync(int id);
}

public interface IInsuranceRepository
{
    Task<IReadOnlyList<Insurance>> GetListAsync(InsuranceFilter filter);
    Task<IReadOnlyList<Insurance>> GetAllAsync();
    Task<Insurance?> GetByIdAsync(int id);
    Task<int> AddAsync(Insurance insurance);
    Task<bool> UpdateAsync(Insurance insurance);
    Task<bool> DeleteAsync(int id);
}

public interface IExpenseCategoryRepository
{
    Task<IReadOnlyList<ExpenseCategory>> GetListAsync(ExpenseCategoryFilter filter);
    Task<IReadOnlyList<ExpenseCategory>> GetAllAsync();
    Task<ExpenseCategory?> GetByIdAsync(int id);
    Task<int> AddAsync(ExpenseCategory category);
    Task<bool> UpdateAsync(ExpenseCategory category);
    Task<bool> DeleteAsync(int id);
}

public interface IExpenseRepository
{
    Task<IReadOnlyList<Expense>> GetListAsync(ExpenseFilter filter);
    Task<IReadOnlyList<Expense>> GetAllAsync();
    Task<Expense?> GetByIdAsync(int id);
    Task<int> AddAsync(Expense expense);
    Task<bool> UpdateAsync(Expense expense);
    Task<bool> DeleteAsync(int id);
}

public interface IUserRepository
{
    Task<IReadOnlyList<User>> GetListAsync(UserFilter filter);
    Task<User?> GetByIdAsync(int id);
}