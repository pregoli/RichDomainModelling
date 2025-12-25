using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Event raised when a new order is created.
/// </summary>
public sealed record OrderCreatedEvent(OrderId OrderId, Email CustomerEmail) : DomainEvent;

/// <summary>
/// Event raised when an order is submitted for processing.
/// </summary>
public sealed record OrderSubmittedEvent(OrderId OrderId, Money Total, int LineCount) : DomainEvent;

/// <summary>
/// Event raised when payment for an order is confirmed.
/// </summary>
public sealed record OrderPaidEvent(OrderId OrderId, string PaymentReference) : DomainEvent;

/// <summary>
/// Event raised when an order is shipped.
/// </summary>
public sealed record OrderShippedEvent(OrderId OrderId, string TrackingNumber, DateTime ShippedAt) : DomainEvent;

/// <summary>
/// Event raised when an order is cancelled.
/// </summary>
public sealed record OrderCancelledEvent(OrderId OrderId, string Reason) : DomainEvent;
