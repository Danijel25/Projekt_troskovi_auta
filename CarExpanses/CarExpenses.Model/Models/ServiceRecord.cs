using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;

public class ServiceRecord
{
    [Key]
    public int Id { get; set; }
    public required string ServiceType { get; set; }
    public required string Description { get; set; }
    public decimal Cost { get; set; }
    public DateTime ServiceDate { get; set; }
    public int Mileage { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public virtual Car? Car { get; set; }
}
