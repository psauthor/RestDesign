using Microsoft.AspNetCore.Mvc.ActionConstraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesigningApis.Constraints
{
  public class RequiredFromQueryActionConstraint : IActionConstraint
  {
    private readonly string _parameter;

    public RequiredFromQueryActionConstraint(string parameter)
    {
      _parameter = parameter;
    }

    public int Order => 999;

    public bool Accept(ActionConstraintContext context)
    {
      if (!context.RouteContext.HttpContext.Request.Query.ContainsKey(_parameter))
      {
        return false;
      }

      return true;
    }
  }
}