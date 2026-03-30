namespace CarExpanses.Models;


public class CarTire
{
    public int CarId { get; set; }
    public Car? Car { get; set; }
    public int TireId { get; set; }
    public Tire? Tire { get; set; }
    public DateTime InstalledDate { get; set; }

}