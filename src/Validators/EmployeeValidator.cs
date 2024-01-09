using FluentValidation;
using FluentValidation.Validators;

namespace RestDesign.Validators;

public class EmployeeValidator : CustomValidator<Employee>
{
  public EmployeeValidator()
  {
    RuleFor(c => c.Name)
      .NotEmpty()
      .MinimumLength(5);

    RuleFor(e => e.BillingRate)
      .InclusiveBetween(50, 500)
      .WithMessage("Billing Rate is not valid.");
  }

}
