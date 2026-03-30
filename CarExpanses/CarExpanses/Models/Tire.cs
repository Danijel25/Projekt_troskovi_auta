namespace CarExpanses.Models;
public class Tire
{
    public int Id { get; set; }
    public required string Brand { get; set; }
    public required string Model { get; set; }
    public required string Season { get; set; }
    public decimal Price { get; set; } 
    public List<CarTire>? CarTires { get; set; }
}