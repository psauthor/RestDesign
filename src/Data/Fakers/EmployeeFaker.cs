using Bogus;

namespace RestDesign.Data.Fakers;

public class EmployeeFaker : Faker<Employee>
{
  int _ids = 0;

  protected EmployeeFaker()
  {
    UseSeed(1337)
      .RuleFor(e => e.Id, f => ++_ids)
      .RuleFor(e => e.Name, f => f.Name.FullName())
      .RuleFor(e => e.BillingRate, f => 100.0 + f.Random.Number(1, 10) * 25);
  }

  public static List<Employee> Generate() => new EmployeeFaker().Generate(10);

}
