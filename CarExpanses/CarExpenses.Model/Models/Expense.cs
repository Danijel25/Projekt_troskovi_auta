using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;
using CarExpenses.Model;

public class Expense : ISoftDeleate
{
    [Key]
    public int Id { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    [ForeignKey("ExpenseCategory")]
    public int CategoryId { get; set; }
    public required virtual ExpenseCategory Category { get; set; }
    [ForeignKey("Car")]
    public int CarId { get; set; }
    public virtual Car Car { get; set; } = null!;
    public DateTime? DeleatedAt { get; set; }
}
