using Bogus;

namespace RestDesign.Data.Fakers;

public class CustomerFaker : Faker<Customer>
{
  int _ids = 0;

  protected CustomerFaker()
  {
    UseSeed(1337)
      .RuleFor(c => c.Id, f => ++_ids)
      .RuleFor(c => c.CompanyName, f => f.Company.CompanyName(0))
      .RuleFor(c => c.Contact, f => f.Name.FullName())
      .RuleFor(c => c.PhoneNumber, f => f.Phone.PhoneNumber())
      .RuleFor(c => c.AddressLine1, f => f.Address.StreetAddress())
      .RuleFor(c => c.City, f => f.Address.City())
      .RuleFor(c => c.StateProvince, f => f.Address.StateAbbr())
      .RuleFor(c => c.PostalCode, f => f.Address.ZipCode("#####-####"));
  }

  public static List<Customer> Generate() => new CustomerFaker().Generate(25);
}
