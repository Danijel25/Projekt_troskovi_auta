using CarExpenses.Model.Models;

namespace CarExpenses.DAL.Repositories;

public interface IUserRepository
{
    IReadOnlyList<User> GetAll();
    User? GetById(int id);
    User? GetByIdWithDetails(int id);
    void Add(User user);
    bool Update(User user);
    bool Delete(int id);
}

public interface ICarRepository
{
    IReadOnlyList<Car> GetAll();
    Car? GetById(int id);
    void Add(Car car);
    bool Update(Car car);
    bool Delete(int id);
}

public interface ITireRepository
{
    IReadOnlyList<Tire> GetAll();
    Tire? GetById(int id);
    void Add(Tire tire);
    bool Update(Tire tire);
    bool Delete(int id);
}

public interface ICarTireRepository
{
    IReadOnlyList<CarTire> GetAll();
    CarTire? GetById(int id);
    void Add(CarTire carTire);
    bool Update(CarTire carTire);
    bool Delete(int id);
}

public interface IFuelExpenseRepository
{
    IReadOnlyList<FuelExpense> GetAll();
    FuelExpense? GetById(int id);
    void Add(FuelExpense fuelExpense);
    bool Update(FuelExpense fuelExpense);
    bool Delete(int id);
}

public interface IServiceRecordRepository
{
    IReadOnlyList<ServiceRecord> GetAll();
    ServiceRecord? GetById(int id);
    void Add(ServiceRecord serviceRecord);
    bool Update(ServiceRecord serviceRecord);
    bool Delete(int id);
}

public interface IInsuranceRepository
{
    IReadOnlyList<Insurance> GetAll();
    Insurance? GetById(int id);
    void Add(Insurance insurance);
    bool Update(Insurance insurance);
    bool Delete(int id);
}

public interface IExpenseCategoryRepository
{
    IReadOnlyList<ExpenseCategory> GetAll();
    ExpenseCategory? GetById(int id);
    void Add(ExpenseCategory category);
    bool Update(ExpenseCategory category);
    bool Delete(int id);
}

public interface IExpenseRepository
{
    IReadOnlyList<Expense> GetAll();
    Expense? GetById(int id);
    void Add(Expense expense);
    bool Update(Expense expense);
    bool Delete(int id);
}