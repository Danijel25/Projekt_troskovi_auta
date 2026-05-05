using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;


public class CarTire
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
}
