namespace CommunicationsApp.Domain.Common;

public abstract class BaseStrongEntity<T>
{
    public T Id { get; set; }
}

public abstract class BaseCompositeEntity<TKey1, TKey2>
{
    public TKey1 Id1 { get; set; }
    public TKey2 Id2 { get; set; }
}

public abstract class BaseSoftDeleteEntity<TKey> : BaseStrongEntity<TKey>
{
    public DateTime? DeletedAt { get; set; }
}