using Identity.Api.DTOs.Common;
using Microsoft.AspNetCore.Routing;

namespace Identity.Api.Services;

public sealed class LinkService(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
{
    public LinkDto Create(string routeName, string rel, string method, object? values = null) => new()
    {
        Href = linkGenerator.GetUriByRouteValues(
            httpContextAccessor.HttpContext!,
            routeName,
            values) ?? string.Empty,
        Rel = rel,
        Method = method,
    };
}
