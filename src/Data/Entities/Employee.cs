namespace DesigningApis.Data.Entities;


public class Employee
{
  public int Id { get; set; }
  public required string Name { get; set; }
  public double BillingRate { get; set; }
}