using Bogus;

namespace DesigningApis.Data;

public static class Fakers
{
  static Random _seed = new Random(1337);
  static int _customerIds = 0;
  static int _ticketIds = 0;
  static int _employeeIds = 0;
  static int _projectIds = 0;

  public static Faker<Customer> CustomerFaker =>
    new Faker<Customer>()
      .RuleFor(c => c.Id, f => ++_customerIds)
      .RuleFor(c => c.CompanyName, f => f.Company.CompanyName(0))
      .RuleFor(c => c.Contact, f => f.Name.FullName())
      .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
      .RuleFor(c => c.AddressLine1, f => f.Address.StreetAddress())
      .RuleFor(c => c.City, f => f.Address.City())
      .RuleFor(c => c.StateProvince, f => f.Address.StateAbbr())
      .RuleFor(c => c.PostalCode, f => f.Address.ZipCode("#####-####"));

  public static Faker<Employee> EmployeeFaker =>
    new Faker<Employee>()
      .RuleFor(e => e.Id, f => ++_employeeIds)
      .RuleFor(e => e.Name, f => f.Name.FullName())
      .RuleFor(e => e.BillingRate, f => 100.0 + _seed.Next(10) * 25);

  public static Faker<Project> ProjectFaker =>
    new Faker<Project>()
      .RuleFor(p => p.Id, f => ++_projectIds)
    .RuleFor(p => p.ProjectName, f => f.Internet.DomainName())
    .RuleFor(p => p.StartDate, f => f.Date.Past())
    .RuleFor(p => p.CustomerId, f => CustomerFaker.Generate(10).ElementAt(f.Random.Number(0, 9)).Id);

  public static Faker<Ticket> TicketFaker =>
    new Faker<Ticket>()
    .RuleFor(t => t.Id, f => ++_ticketIds)
    .RuleFor(t => t.Hours, f => f.Random.Number(0, 80) / 10)
    .RuleFor(t => t.WorkPerformed, f => f.Hacker.Phrase())
    .RuleFor(t => t.EmployeeId, f => EmployeeFaker.Generate(10).ElementAt(f.Random.Number(0, 9)).Id)
    .RuleFor(t => t.ProjectId, f => ProjectFaker.Generate(25).ElementAt(f.Random.Number(0, 24)).Id);


}
