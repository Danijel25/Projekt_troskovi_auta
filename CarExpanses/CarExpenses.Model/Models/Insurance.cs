using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;

public class Insurance
{
    [Key]
    public int Id { get; set; }
    public required string Company { get; set; }
    public required string InsuranceType { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public virtual Car? Car { get; set; }
}
