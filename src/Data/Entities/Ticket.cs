﻿namespace DesigningApis.Data.Entities;

public class Ticket
{
  public int Id { get; set; }
  public Employee? Employee { get; set; }
  public int EmployeeId { get; set; }
  public double Hours { get; set; }
  public double BillingRate { get; set; }
  public DateTime? Date { get; set; }
  public required Project Project { get; set; }
  public int ProjectId { get; set; }
  public string? WorkPerformed { get; set; }
}