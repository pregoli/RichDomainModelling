using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Specification that checks if an order is ready to be shipped.
/// </summary>
public class OrderReadyToShipSpecification : Specification<Order>
{
    public override bool IsSatisfiedBy(Order order)
        => order.Status == OrderStatus.Paid && order.Lines.Any();
}

/// <summary>
/// Specification that checks if an order exceeds a value threshold.
/// </summary>
public class HighValueOrderSpecification : Specification<Order>
{
    private readonly Money _threshold;

    public HighValueOrderSpecification(Money threshold) => _threshold = threshold;

    public override bool IsSatisfiedBy(Order order)
        => order.TotalAmount.Amount >= _threshold.Amount;
}
