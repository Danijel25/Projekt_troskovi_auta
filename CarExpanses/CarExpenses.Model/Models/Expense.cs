using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarExpenses.Model.Models;

public class Expense
{
    [Key]
    public int Id { get; set; }
    public required string Description { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    [ForeignKey("ExpenseCategory")]
    public int CategoryId { get; set; }
    public required virtual ExpenseCategory Category { get; set; }
}
