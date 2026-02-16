using Identity.Api.DTOs.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;

namespace Identity.Api.Services;

public sealed class LinkService
{
    private readonly IUrlHelper _urlHelper;

    public LinkService(IUrlHelperFactory urlHelperFactory, IActionContextAccessor actionContextAccessor)
    {
        _urlHelper = urlHelperFactory.GetUrlHelper(actionContextAccessor.ActionContext!);
    }

    public IReadOnlyList<LinkDto> GenerateUserLinks(Guid userId)
    {
        return
        [
            new LinkDto
            {
                Href = _urlHelper.Link("GetUserById", new
                    {
                        id = userId,
                    }) ?? string.Empty,
                Rel = "self",
                Method = "GET",
            },
            new LinkDto
            {
                Href = _urlHelper.Link("UpdateUser", new
                    {
                        id = userId,
                    }) ?? string.Empty,
                Rel = "update_user",
                Method = "PUT",
            },
        ];
    }
}
