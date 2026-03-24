public class Expense
{
    public int Id { get; set; }

    public string Description { get; set; }

    public decimal Amount { get; set; }

    public DateTime Date { get; set; }

    public int CategoryId { get; set; }

    public ExpenseCategory Category { get; set; }
}