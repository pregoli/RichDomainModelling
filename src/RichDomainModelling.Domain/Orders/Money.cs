using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Value object representing a monetary amount with currency.
/// Prevents mixing currencies and handles precision correctly.
/// </summary>
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    private Money(decimal amount, Currency currency)
    {
        Amount = decimal.Round(amount, 2, MidpointRounding.ToEven);
        Currency = currency;
    }

    public static Money Create(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative");

        return new Money(amount, currency);
    }

    public static Money Zero(Currency currency) => new(0, currency);

    public Money Add(Money other)
    {
        EnsureSameCurrency(other);
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        EnsureSameCurrency(other);

        if (other.Amount > Amount)
            throw new DomainException("Insufficient funds");

        return new Money(Amount - other.Amount, Currency);
    }

    public Money Multiply(decimal factor)
    {
        if (factor < 0)
            throw new DomainException("Factor cannot be negative");

        return new Money(Amount * factor, Currency);
    }

    public Money Divide(decimal divisor)
    {
        if (divisor == 0)
            throw new DomainException("Cannot divide by zero");

        if (divisor < 0)
            throw new DomainException("Divisor cannot be negative");

        return new Money(Amount / divisor, Currency);
    }

    public Money Percentage(decimal percent)
    {
        return new Money(Amount * percent / 100, Currency);
    }

    private void EnsureSameCurrency(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot combine {Currency} with {other.Currency}");
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    public override string ToString() => $"{Currency.Symbol()}{Amount:N2}";
}
