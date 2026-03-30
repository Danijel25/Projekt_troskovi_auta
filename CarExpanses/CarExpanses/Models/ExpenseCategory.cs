namespace CarExpanses.Models;

public class ExpenseCategory
{
    public int Id { get; set; }

    public required string Name { get; set; }

    public List<Expense>? Expenses { get; set; }
}