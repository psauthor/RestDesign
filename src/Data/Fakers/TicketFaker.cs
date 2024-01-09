using Bogus;

namespace RestDesign.Data.Fakers;

public class TicketFaker : Faker<Ticket>
{
  int _ids = 0;
  List<Employee> _employees = EmployeeFaker.Generate();
  List<Project> _projects = ProjectFaker.Generate();


  protected TicketFaker()
  {
    UseSeed(1337)
    .RuleFor(t => t.Id, f => ++_ids)
    .RuleFor(t => t.Hours, f => f.Random.Number(0, 80) / 10)
    .RuleFor(t => t.WorkPerformed, f => f.Hacker.Phrase())
    .RuleFor(t => t.EmployeeId, f => _employees.ElementAt(f.Random.Number(0, 9)).Id)
    .RuleFor(t => t.ProjectId, f => _projects.ElementAt(f.Random.Number(0, 24)).Id);
  }

  public static List<Ticket> Generate() => new TicketFaker().Generate(100);

}
