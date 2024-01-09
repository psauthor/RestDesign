namespace DesigningApis.Data.Entities;


public class Project
{
  public int Id { get; set; }
  public required string ProjectName { get; set; }
  public Customer? Customer { get; set; }
  public DateTime? StartDate { get; set; }
  public DateTime? EndDate { get; set; }
  public int CustomerId { get; set; }
}