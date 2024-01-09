﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestDesign.Constraints
{
  public class RequiredFromQueryAttribute : FromQueryAttribute, IParameterModelConvention
  {
    public void Apply(ParameterModel parameter)
    {
      if (parameter.Action.Selectors != null && parameter.Action.Selectors.Any())
      {
        parameter.Action.Selectors.Last().ActionConstraints.Add(new RequiredFromQueryActionConstraint(parameter.BindingInfo?.BinderModelName ?? parameter.ParameterName));
      }
    }
  }
}
