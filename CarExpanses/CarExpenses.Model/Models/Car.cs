using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CarExpenses.Model.Enums;

namespace CarExpenses.Model.Models;

public class Car
{    
    [Key]
    public int Id { get; set; }
    [ForeignKey("User")]
    public int UserId { get; set; }
    public virtual User? User { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public double EngineVolume { get; set; }
    public int CurrentMilage { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public FuelType FuelType { get; set; }
    public virtual ICollection<FuelExpense>? FuelExpenses { get; set; } = new List<FuelExpense>();
    public virtual ICollection<ServiceRecord>? ServiceRecords { get; set; } = new List<ServiceRecord>();
    public virtual ICollection<Insurance>? Insurances { get; set; } = new List<Insurance>();
    public virtual ICollection<CarTire>? CarTires { get; set; } = new List<CarTire>();
    public virtual ICollection<Expense>? Expenses { get; set; } = new List<Expense>();
}
