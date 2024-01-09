namespace RestDesign.Validators;

public class CustomValidator<T> : AbstractValidator<T>
{ 
  protected bool IsEmpty<T>(T val) => val is null || (val is string str && string.IsNullOrWhiteSpace(str));
}