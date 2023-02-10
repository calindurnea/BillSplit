using Microsoft.EntityFrameworkCore;

namespace BillSplit.Persistence.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<TEntity> WithNoTracking<TEntity>(this IQueryable<TEntity> source, bool withNoTracking = true) where TEntity : class
    {
        return withNoTracking ? source.AsNoTracking() : source;
    }
}