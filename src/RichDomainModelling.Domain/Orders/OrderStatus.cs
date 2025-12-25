namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Represents the lifecycle states of an order.
/// </summary>
public enum OrderStatus
{
    Draft,
    Submitted,
    Paid,
    Shipped,
    Cancelled
}
