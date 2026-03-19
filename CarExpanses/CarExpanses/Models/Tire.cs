public class Tire
{
   //properties: id, brand, model, season, price
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string Season { get; set; }
    public decimal Price { get; set; } 
    public List<CarTire> CarTires { get; set; }
}