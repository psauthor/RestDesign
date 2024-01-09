namespace RestDesign.Data.Entities;

public class Customer
{
  public int Id { get; set; }
  public required string CompanyName { get; set; }
  public string? Contact { get; set; }
  public string? PhoneNumber { get; set; }
  public string? Email { get; set; }
  public string? AddressLine1 { get; set; }
  public string? AddressLine2 { get; set; }
  public string? AddressLine3 { get; set; }
  public string? City { get; set; }
  public string? StateProvince { get; set; }
  public string? PostalCode { get; set; }
  public string? Country { get; set; }
}