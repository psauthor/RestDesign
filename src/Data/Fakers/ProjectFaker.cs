using Bogus;

namespace RestDesign.Data.Fakers;

public class ProjectFaker : Faker<Project>
{
  int _ids = 0;
  List<Customer> _customers = CustomerFaker.Generate();


  protected ProjectFaker()
  {
    UseSeed(1337)
      .RuleFor(p => p.Id, f => ++_ids)
      .RuleFor(p => p.ProjectName, f => f.Internet.DomainName())
      .RuleFor(p => p.StartDate, f => f.Date.Past())
      .RuleFor(p => p.CustomerId, f => _customers.ElementAt(f.Random.Number(0, 24)).Id);
  }

  public static List<Project> Generate() => new ProjectFaker().Generate(25);

}
