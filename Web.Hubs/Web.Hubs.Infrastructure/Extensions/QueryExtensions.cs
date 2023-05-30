using Web.Hubs.Core.Dtos.Filters;

namespace Web.Hubs.Infrastructure.Extensions;

internal static class QueryExtensions
{
    public static IQueryable<T> PageFilter<T>(this IQueryable<T> queryable, PageFilter? pageFilter)
    {
        ArgumentNullException.ThrowIfNull(queryable);

        if (pageFilter is not null)
        {
            var skip = (pageFilter.Page - 1) * pageFilter.Size;
            var take = pageFilter.Size;

            queryable = queryable.Skip(skip).Take(take);
        }

        return queryable;
    }
}
