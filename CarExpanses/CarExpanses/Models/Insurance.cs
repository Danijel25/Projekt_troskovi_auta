public class Insurance
{
    //properties: id, Company, insurance type, price, start date, end date, car id ,car
    public int Id { get; set; }
    public string Company { get; set; }
    public string InsuranceType { get; set; }
    public decimal Price { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; }

}