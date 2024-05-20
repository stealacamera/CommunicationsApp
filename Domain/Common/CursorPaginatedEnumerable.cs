using Microsoft.EntityFrameworkCore;

namespace CommunicationsApp.Domain.Common;

public class CursorPaginatedEnumerable<TKey, TEntity>
    where TEntity : BaseEntity
    where TKey : struct, IComparable<TKey>
{
    public TKey NextCursor { get; private set; }
    public IEnumerable<TEntity> Values { get; private set; }

    private CursorPaginatedEnumerable(TKey cursor, IEnumerable<TEntity> values)
    {
        NextCursor = cursor;
        Values = values;
    }

    public static async Task<CursorPaginatedEnumerable<TKey, TEntity>> CreateAsync(
        TKey? cursor,
        int pageSize,
        string sortingPropertyName,
        IQueryable<TEntity> query,
        CancellationToken cancellationToken,
        bool getOlderValues = true)
    {
        if (cursor.HasValue)
            query = query.Where(e => getOlderValues
                                     ? EF.Property<TKey>(e, sortingPropertyName).CompareTo(cursor.Value) >= 0
                                     : EF.Property<TKey>(e, sortingPropertyName).CompareTo(cursor.Value) <= 0);

        query = query.Take(pageSize + 1);
        var values = await query.ToListAsync(cancellationToken);
        TKey newCursor = default;

        if(values.Skip(1).Any() && values.Count >= pageSize)
        {
            newCursor = (TKey)values[^1].GetType().GetProperty(sortingPropertyName).GetValue(values[^1]);
            values.RemoveAt(values.Count - 1);
        }

        return new CursorPaginatedEnumerable<TKey, TEntity>(newCursor, values);
    }
}
