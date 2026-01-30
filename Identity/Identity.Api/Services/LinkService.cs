using Identity.Core.DTOs.Common;
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

    public List<LinkDto> GenerateUserLinks(Guid userId)
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

    public List<LinkDto> GeneratePaginationLinks(string routeName, int page, int pageSize, int totalPages)
    {
        var links = new List<LinkDto>
        {
            new LinkDto
            {
                Href = _urlHelper.Link(routeName, new
                    {
                        page, pageSize,
                    }) ?? string.Empty,
                Rel = "self",
                Method = "GET",
            },
        };

        if (page > 1)
        {
            links.Add(new LinkDto
            {
                Href = _urlHelper.Link(routeName, new
                {
                    page = page - 1, pageSize,
                }) ?? string.Empty,
                Rel = "previous_page",
                Method = "GET",
            });
        }

        if (page < totalPages)
        {
            links.Add(new LinkDto
            {
                Href = _urlHelper.Link(routeName, new
                {
                    page = page + 1, pageSize,
                }) ?? string.Empty,
                Rel = "next_page",
                Method = "GET",
            });
        }

        return links;
    }
}
