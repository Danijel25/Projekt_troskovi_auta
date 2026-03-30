using CarExpanses.Enums;
using CarExpanses.Models;

namespace CarExpanses.Models;

public class Car
{    
    public int Id { get; set; }
    public int UserId { get; set; }
    public User? User { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public double EngineVolume { get; set; }
    public int CurrentMilage { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public FuelType FuelType { get; set; }
    public List<FuelExpense>? FuelExpenses { get; set; }
    public List<ServiceRecord>? ServiceRecords { get; set; }
    public List<Insurance>? Insurances { get; set; }
    public List<CarTire>? CarTires { get; set; }
    public List<Expense>? Expenses { get; set; }
}