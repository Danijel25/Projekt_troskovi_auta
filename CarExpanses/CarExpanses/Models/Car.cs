public enum FuelType
{
    Petrol,
    Diesel,
    Electric,
    Hybrid
}

public class Car
{
    //id, brand, model, year, EngineVolume, currentMilage, PurchasePrice, PurchaseDate.
    
    public int Id { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public double EngineVolume { get; set; }
    public int CurrentMilage { get; set; }
    public decimal PurchasePrice { get; set; }
    public DateTime PurchaseDate { get; set; }
    public FuelType FuelType { get; set; }
    public List<FuelExpense> FuelExpenses { get; set; }
    public List<ServiceRecord> ServiceRecords { get; set; }
    public List<Insurance> Insurances { get; set; }
    public List<CarTire> CarTires { get; set; }
}