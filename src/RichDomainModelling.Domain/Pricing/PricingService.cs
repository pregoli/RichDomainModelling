using RichDomainModelling.Domain.Orders;

namespace RichDomainModelling.Domain.Pricing;

/// <summary>
/// Domain service for calculating order discounts.
/// Used when behaviour doesn't naturally belong to a single entity.
/// </summary>
public interface IPricingService
{
    Money CalculateDiscount(Order order, Customer customer);
}

/// <summary>
/// Implementation of pricing rules for order discounts.
/// </summary>
public class PricingService : IPricingService
{
    public Money CalculateDiscount(Order order, Customer customer)
    {
        var discount = Money.Zero(order.TotalAmount.Currency);

        // Loyal customer discount: 10%
        if (customer.TotalPurchases.Amount > 1000)
        {
            discount = discount.Add(order.TotalAmount.Percentage(10));
        }

        // Bulk order discount: 5%
        if (order.Lines.Sum(l => l.Quantity.Value) > 10)
        {
            discount = discount.Add(order.TotalAmount.Percentage(5));
        }

        return discount;
    }
}
