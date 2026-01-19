using Identity.Core.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Services;

public sealed class LinkGenerator(IUrlHelper urlHelper)
{
    public List<LinkDto> GenerateUserLinks(Guid userId)
    {
        return new List<LinkDto>
        {
            new()
            {
                Href = urlHelper.Link("GetUserById", new
                {
                    id = userId,
                }) ?? string.Empty,
                Rel = "self",
                Method = "GET",
            },
            new()
            {
                Href = urlHelper.Link("UpdateUser", new
                {
                    id = userId,
                }) ?? string.Empty,
                Rel = "update_user",
                Method = "PUT",
            },
            new()
            {
                Href = urlHelper.Link("DeleteUser", new
                {
                    id = userId,
                }) ?? string.Empty,
                Rel = "delete_user",
                Method = "DELETE",
            },
        };
    }

    public List<LinkDto> GeneratePaginationLinks(string routeName, int page, int pageSize, int totalPages)
    {
        var links = new List<LinkDto>
        {
            new LinkDto
            {
                Href = urlHelper.Link(routeName, new
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
                Href = urlHelper.Link(routeName, new { page = page - 1, pageSize }) ?? string.Empty,
                Rel = "previous_page",
                Method = "GET",
            });
        }

        if (page < totalPages)
        {
            links.Add(new LinkDto
            {
                Href = urlHelper.Link(routeName, new { page = page + 1, pageSize }) ?? string.Empty,
                Rel = "next_page",
                Method = "GET",
            });
        }

        return links;
    }
}
