using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Model.Models;
public class Tire
{
    [Key]
    public int Id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required string Season { get; set; }
    public decimal Price { get; set; } 
    public virtual ICollection<CarTire>? CarTires { get; set; } = new List<CarTire>();
}
