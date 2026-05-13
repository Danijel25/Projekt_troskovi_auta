using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;
using CarExpenses.Model;

public class CarTire : ISoftDeleate
{
    [Key]
    public int Id { get; set;}
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public virtual Car? Car { get; set; }
    [ForeignKey("Tire")]
    public int TireId { get; set; }
    public virtual Tire? Tire { get; set; }
    public DateTime InstalledDate { get; set; }
    public DateTime? DeleatedAt { get; set; }
}
