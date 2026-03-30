namespace CarExpanses.Models;

public class FuelExpense
{
    public int Id { get; set; }
    public DateTime FuelExpenseDate { get; set; }
    public decimal Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost => Liters * PricePerLiter;
    public int Kilometars { get; set; }
    public int CarId { get; set; }
    public Car? Car { get; set; }

}