namespace CommunicationsApp.Domain.Common;


public abstract class BaseEntity { }

public abstract class BaseStrongEntity<T> : BaseEntity
{
    public T Id { get; set; }
}

public abstract class BaseStrongEntity : BaseStrongEntity<int> { }

public abstract class BaseSoftDeleteEntity<TKey> : BaseStrongEntity<TKey>
{
    public DateTime? DeletedAt { get; set; }
}

public abstract class BaseSoftDeleteEntity : BaseSoftDeleteEntity<int> { }