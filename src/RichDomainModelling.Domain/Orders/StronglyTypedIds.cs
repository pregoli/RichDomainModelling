using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Strongly-typed identifier for Order entities.
/// Prevents mixing up different ID types at compile time.
/// </summary>
public sealed record OrderId
{
    public Guid Value { get; }

    private OrderId(Guid value) => Value = value;

    public static OrderId New() => new(Guid.NewGuid());
    
    public static OrderId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("OrderId cannot be empty");
        return new(value);
    }

    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for OrderLine entities.
/// </summary>
public sealed record OrderLineId
{
    public Guid Value { get; }

    private OrderLineId(Guid value) => Value = value;

    public static OrderLineId New() => new(Guid.NewGuid());
    
    public static OrderLineId From(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for Product entities.
/// </summary>
public sealed record ProductId
{
    public Guid Value { get; }

    private ProductId(Guid value) => Value = value;

    public static ProductId New() => new(Guid.NewGuid());
    
    public static ProductId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("ProductId cannot be empty");
        return new(value);
    }

    public override string ToString() => Value.ToString();
}

/// <summary>
/// Strongly-typed identifier for Customer entities.
/// </summary>
public sealed record CustomerId
{
    public Guid Value { get; }

    private CustomerId(Guid value) => Value = value;

    public static CustomerId New() => new(Guid.NewGuid());
    
    public static CustomerId From(Guid value)
    {
        if (value == Guid.Empty)
            throw new DomainException("CustomerId cannot be empty");
        return new(value);
    }

    public override string ToString() => Value.ToString();
}
