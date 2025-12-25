using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Value object representing a validated email address.
/// Ensures email format is valid and normalises the value.
/// </summary>
public sealed class Email : ValueObject
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Email cannot be empty");

        if (!value.Contains('@') || !value.Contains('.'))
            throw new DomainException("Invalid email format");

        if (value.Length > 255)
            throw new DomainException("Email too long");

        return new Email(value.ToLowerInvariant().Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;

    // Implicit conversion for convenience
    public static implicit operator string(Email email) => email.Value;
}
