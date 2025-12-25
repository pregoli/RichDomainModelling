using RichDomainModelling.Domain.Orders;

namespace RichDomainModelling.Domain.Pricing;

/// <summary>
/// Simplified Customer class for demonstrating domain services.
/// In a real application, this would be a full aggregate in its own bounded context.
/// </summary>
public class Customer
{
    public CustomerId Id { get; }
    public Email Email { get; }
    public Money TotalPurchases { get; private set; }

    private Customer(CustomerId id, Email email, Money totalPurchases)
    {
        Id = id;
        Email = email;
        TotalPurchases = totalPurchases;
    }

    public static Customer Create(Email email)
    {
        return new Customer(
            CustomerId.New(),
            email,
            Money.Zero(Currency.GBP));
    }

    public static Customer CreateWithHistory(Email email, Money totalPurchases)
    {
        return new Customer(CustomerId.New(), email, totalPurchases);
    }
}
