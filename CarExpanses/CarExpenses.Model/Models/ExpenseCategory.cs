using System.ComponentModel.DataAnnotations;

namespace CarExpenses.Model.Models;

public class ExpenseCategory
{
    [Key]
    public int Id { get; set; }

    public required string Name { get; set; }

    public virtual ICollection<Expense>? Expenses { get; set; } = new List<Expense>();
}
