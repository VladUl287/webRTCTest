namespace Web.Hubs.Core.Dtos.Filters;

public sealed class PageFilter
{
    public required int Size { get; init; }

    public required int Page { get; init; }
}