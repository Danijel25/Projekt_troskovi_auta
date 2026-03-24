public class ExpenseCategory
{
    public int Id { get; set; }

    public string Name { get; set; }

    public List<Expense> Expenses { get; set; }
}