using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Entity representing a line item within an order.
/// Part of the Order aggregate - can only be modified through the Order aggregate root.
/// </summary>
public class OrderLine : Entity<OrderLineId>
{
    public ProductId ProductId { get; }
    public string ProductName { get; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice.Multiply(Quantity.Value);

    internal OrderLine(ProductId productId, string productName, Quantity quantity, Money unitPrice)
        : base(OrderLineId.New())
    {
        ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
        ProductName = !string.IsNullOrWhiteSpace(productName)
            ? productName
            : throw new DomainException("Product name is required");
        Quantity = quantity ?? throw new ArgumentNullException(nameof(quantity));
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
    }

    internal void IncreaseQuantity(Quantity additionalQuantity)
    {
        Quantity = Quantity.Add(additionalQuantity);
    }
}
