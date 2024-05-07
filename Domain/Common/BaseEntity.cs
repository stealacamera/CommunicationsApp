namespace CommunicationsApp.Domain.Common;

public abstract class BaseStrongEntity<T>
{
    public T Id { get; set; }
}

public abstract class BaseStrongEntity : BaseStrongEntity<int> { }

public abstract class BaseSoftDeleteEntity<TKey> : BaseStrongEntity<TKey>
{
    public DateTime? DeletedAt { get; set; }
}

public abstract class BaseSoftDeleteEntity : BaseSoftDeleteEntity<int> { }