using FluentValidation;
using FluentValidation.Validators;

namespace RestDesign.Validators;

public class ProjectValidator : CustomValidator<Project>
{
  public ProjectValidator()
  {
    RuleFor(c => c.CustomerId)
      .NotEmpty();

    RuleFor(c => c.ProjectName)    
      .NotEmpty()
      .MinimumLength(5);

    RuleFor(c => c.StartDate)
      .NotNull();

  }

}
