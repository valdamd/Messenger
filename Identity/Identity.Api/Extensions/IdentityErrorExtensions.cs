using Identity.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Extensions;

public static class IdentityErrorExtensions
{
    public static ObjectResult ToProblemResult(this IdentityError error, int statusCode) =>
        new(new ProblemDetails
        {
            Detail = error.Detail,
            Status = statusCode,
            Extensions =
            {
                {
                "code", error.Code
                },
            },
        })
        {
            StatusCode = statusCode,
        };
}
