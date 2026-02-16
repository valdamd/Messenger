namespace Identity.Api.DTOs.Common;

public sealed record AcceptHeaderDto
{
    public string? Accept { get; init; }

    public bool IncludeLinks => !string.IsNullOrEmpty(Accept) &&
                                Accept.Contains("hateoas", StringComparison.OrdinalIgnoreCase);
}
