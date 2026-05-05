using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;

public class FuelExpense
{
    [Key]
    public int Id { get; set; }
    public DateTime FuelExpenseDate { get; set; }
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost => Liters * PricePerLiter;
    public int Kilometars { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public virtual Car? Car { get; set; }
}
