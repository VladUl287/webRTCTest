namespace Web.Hubs.Core.Dtos.Filters;

public class PageFilter
{
    public required int Size { get; init; }

    public required int Page { get; init; }
}