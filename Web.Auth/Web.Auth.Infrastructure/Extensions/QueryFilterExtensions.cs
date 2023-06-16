using Web.Auth.Core.Dtos;
using Web.Auth.Core.Entities;

namespace Web.Auth.Infrastructure.Extensions;

public static class QueryFilterExtensions
{
    public static IQueryable<User> SetUsersFilters(this IQueryable<User> query, UsersFilter? filter)
    {
        if (filter is not null)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                query = query.Where(u => u.NormalizedUserName != null && u.NormalizedUserName.Contains(filter.Name.ToUpperInvariant()));
            }

            if (filter.PageFilter is not null)
            {
                var skip = (filter.PageFilter.Page - 1) * filter.PageFilter.Size;
                var take = filter.PageFilter.Size;

                query = query.Skip(skip).Take(take);
            }
        }

        return query;
    }
}
