public class FuelExpense
{
    //properties: id, date, liters, price per liters, total cost, milage, car id , car
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public double Liters { get; set; }
    public decimal PricePerLiter { get; set; }
    public decimal TotalCost { get; set; }
    public int Milage { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }

}