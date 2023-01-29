using BillSplit.Domain.Exceptions;

namespace BillSplit.Services.Extensions
{
    public static class ValidationExtensions
    {
        public static T ThrowIfNull<T>(this T? entity, params long[]? id) where T : class
        {
            if (entity is not null)
            {
                return entity;
            }

            if (id is not null)
            {
                throw new NotFoundException(typeof(T), id);
            }

            throw new NotFoundException(typeof(T));
        }
    }
}