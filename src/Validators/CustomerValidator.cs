using FluentValidation;
using FluentValidation.Validators;

namespace RestDesign.Validators;

public class CustomerValidator : CustomValidator<Customer>
{
  public CustomerValidator()
  {
    RuleFor(c => c.CompanyName)
      .NotEmpty()
      .MinimumLength(5);

    RuleFor(c => c.PhoneNumber)
      .MinimumLength(6)
      .Unless(p => IsEmpty(p.PhoneNumber));

    RuleFor(c => c.Email)
      .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
      .Unless(p => IsEmpty(p.Email));

    RuleFor(c => c.StateProvince)
      .Length(2)
      .Unless(p => IsEmpty(p.StateProvince));
  }

}
