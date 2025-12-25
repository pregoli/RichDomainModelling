using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Order aggregate root. Controls all modifications to order lines
/// and enforces business invariants.
/// </summary>
public class Order : Entity<OrderId>, IAggregateRoot
{
    private readonly List<OrderLine> _lines = new();

    public Email CustomerEmail { get; }
    public OrderStatus Status { get; private set; }
    public Money TotalAmount { get; private set; }
    public Address ShippingAddress { get; private set; }
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();

    private Order(OrderId id, Email customerEmail, Address shippingAddress)
        : base(id)
    {
        CustomerEmail = customerEmail;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Draft;
        TotalAmount = Money.Zero(Currency.GBP);
    }

    /// <summary>
    /// Creates a new order with the specified customer email and shipping address.
    /// </summary>
    public static Order Create(Email customerEmail, Address shippingAddress)
    {
        if (customerEmail is null) throw new ArgumentNullException(nameof(customerEmail));
        if (shippingAddress is null) throw new ArgumentNullException(nameof(shippingAddress));

        var order = new Order(OrderId.New(), customerEmail, shippingAddress);
        order.AddDomainEvent(new OrderCreatedEvent(order.Id, customerEmail));
        return order;
    }

    /// <summary>
    /// Adds a product line to the order. If the product already exists, increases the quantity.
    /// </summary>
    public void AddLine(ProductId productId, string productName, Quantity quantity, Money unitPrice)
    {
        EnsureCanModify();

        var existingLine = _lines.FirstOrDefault(l => l.ProductId == productId);
        if (existingLine != null)
        {
            existingLine.IncreaseQuantity(quantity);
        }
        else
        {
            _lines.Add(new OrderLine(productId, productName, quantity, unitPrice));
        }

        RecalculateTotal();
    }

    /// <summary>
    /// Removes a product line from the order.
    /// </summary>
    public void RemoveLine(ProductId productId)
    {
        EnsureCanModify();

        var line = _lines.FirstOrDefault(l => l.ProductId == productId)
            ?? throw new DomainException($"Product {productId} not found in order");

        _lines.Remove(line);
        RecalculateTotal();
    }

    /// <summary>
    /// Submits the order for processing. Order must have at least one line
    /// and a minimum total of £1.
    /// </summary>
    public void Submit()
    {
        EnsureCanModify();

        if (!_lines.Any())
            throw new DomainException("Cannot submit an empty order");

        if (TotalAmount.Amount < 1)
            throw new DomainException("Order total must be at least £1");

        Status = OrderStatus.Submitted;

        AddDomainEvent(new OrderSubmittedEvent(Id, TotalAmount, Lines.Count));
    }

    /// <summary>
    /// Marks the order as paid with the given payment reference.
    /// </summary>
    public void MarkAsPaid(string paymentReference)
    {
        if (string.IsNullOrWhiteSpace(paymentReference))
            throw new DomainException("Payment reference is required");

        if (Status != OrderStatus.Submitted)
            throw new DomainException("Only submitted orders can be marked as paid");

        Status = OrderStatus.Paid;

        AddDomainEvent(new OrderPaidEvent(Id, paymentReference));
    }

    /// <summary>
    /// Ships the order with the given tracking number.
    /// </summary>
    public void Ship(string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(trackingNumber))
            throw new DomainException("Tracking number is required");

        if (Status != OrderStatus.Paid)
            throw new DomainException("Only paid orders can be shipped");

        Status = OrderStatus.Shipped;

        AddDomainEvent(new OrderShippedEvent(Id, trackingNumber, DateTime.UtcNow));
    }

    /// <summary>
    /// Cancels the order with the given reason. Shipped orders cannot be cancelled.
    /// </summary>
    public void Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new DomainException("Cancellation reason is required");

        if (Status == OrderStatus.Shipped)
            throw new DomainException("Cannot cancel a shipped order");

        Status = OrderStatus.Cancelled;

        AddDomainEvent(new OrderCancelledEvent(Id, reason));
    }

    private void EnsureCanModify()
    {
        if (Status != OrderStatus.Draft)
            throw new DomainException($"Cannot modify order in {Status} status");
    }

    private void RecalculateTotal()
    {
        TotalAmount = _lines.Aggregate(
            Money.Zero(Currency.GBP),
            (total, line) => total.Add(line.LineTotal));
    }
}
