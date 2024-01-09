using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RestDesign.Services
{
  // Adapted from https://github.com/Microsoft/aspnet-api-versioning/issues/42
  public class AcceptHeaderApiVersionReader : IApiVersionReader
  {
    // looking for application/vnd.wilderminds.arest-v2+json
    private const string Pattern = @".wilderminds.arest-v(\d+(\.\d+)?)\+\S+$";

    public void AddParameters(IApiVersionParameterDescriptionContext context)
    {
    }

    IReadOnlyList<string> IApiVersionReader.Read(HttpRequest request)
    {
      var list = new List<string>();

      var mediaType = request.Headers["Accept"].Single();
      if (mediaType is not null &&
          Regex.IsMatch(mediaType, Pattern, RegexOptions.RightToLeft))
      {
        var match = Regex.Match(mediaType, Pattern, RegexOptions.RightToLeft);
        if (match.Success)
        {
          list.Add(match.Groups[1].Value);
        }
      }

      return list.AsReadOnly();
    }
  }
}
