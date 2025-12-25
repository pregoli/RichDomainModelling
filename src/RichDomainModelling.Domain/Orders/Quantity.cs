using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Value object representing a positive quantity.
/// Ensures quantity is always greater than zero.
/// </summary>
public sealed class Quantity : ValueObject
{
    public int Value { get; }

    private Quantity(int value) => Value = value;

    public static Quantity Create(int value)
    {
        if (value <= 0)
            throw new DomainException("Quantity must be positive");

        return new Quantity(value);
    }

    public Quantity Add(Quantity other) => new(Value + other.Value);

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value.ToString();
}
