using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Model.Models;
using CarExpenses.Model;

public class ExpenseCategory : ISoftDeleate
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Expense>? Expenses { get; set; } = new List<Expense>();
    public DateTime? DeleatedAt { get; set; }
}
