using FluentAssertions;
using NUnit.Framework;
using RichDomainModelling.Domain.Common;
using RichDomainModelling.Domain.Orders;

namespace RichDomainModelling.Domain.Tests;

[TestFixture]
public class OrderTests
{
    [Test]
    public void Create_WithValidInputs_ReturnsOrderInDraftStatus()
    {
        // Act
        var order = CreateDraftOrder();

        // Assert
        order.Status.Should().Be(OrderStatus.Draft);
        order.Lines.Should().BeEmpty();
        order.TotalAmount.Amount.Should().Be(0);
    }

    [Test]
    public void Create_RaisesOrderCreatedEvent()
    {
        // Act
        var order = CreateDraftOrder();

        // Assert
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCreatedEvent>();
    }

    [Test]
    public void AddLine_ToDraftOrder_IncreasesTotal()
    {
        // Arrange
        var order = CreateDraftOrder();
        var unitPrice = Money.Create(25.00m, Currency.GBP);

        // Act
        order.AddLine(ProductId.New(), "Widget", Quantity.Create(3), unitPrice);

        // Assert
        order.TotalAmount.Amount.Should().Be(75.00m);
        order.Lines.Should().HaveCount(1);
    }

    [Test]
    public void AddLine_SameProductTwice_IncreasesQuantity()
    {
        // Arrange
        var order = CreateDraftOrder();
        var productId = ProductId.New();
        var unitPrice = Money.Create(10.00m, Currency.GBP);

        // Act
        order.AddLine(productId, "Widget", Quantity.Create(2), unitPrice);
        order.AddLine(productId, "Widget", Quantity.Create(3), unitPrice);

        // Assert
        order.Lines.Should().HaveCount(1);
        order.Lines.First().Quantity.Value.Should().Be(5);
        order.TotalAmount.Amount.Should().Be(50.00m);
    }

    [Test]
    public void AddLine_ToSubmittedOrder_ThrowsDomainException()
    {
        // Arrange
        var order = CreateSubmittedOrder();

        // Act
        var act = () => order.AddLine(
            ProductId.New(),
            "Widget",
            Quantity.Create(1),
            Money.Create(10, Currency.GBP));

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot modify order in Submitted status");
    }

    [Test]
    public void RemoveLine_ExistingProduct_RemovesAndRecalculatesTotal()
    {
        // Arrange
        var order = CreateDraftOrder();
        var productId1 = ProductId.New();
        var productId2 = ProductId.New();
        order.AddLine(productId1, "Widget A", Quantity.Create(1), Money.Create(30, Currency.GBP));
        order.AddLine(productId2, "Widget B", Quantity.Create(1), Money.Create(20, Currency.GBP));

        // Act
        order.RemoveLine(productId1);

        // Assert
        order.Lines.Should().HaveCount(1);
        order.TotalAmount.Amount.Should().Be(20.00m);
    }

    [Test]
    public void RemoveLine_NonExistingProduct_ThrowsDomainException()
    {
        // Arrange
        var order = CreateDraftOrder();

        // Act
        var act = () => order.RemoveLine(ProductId.New());

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("*not found in order*");
    }

    [Test]
    public void Submit_EmptyOrder_ThrowsDomainException()
    {
        // Arrange
        var order = CreateDraftOrder();

        // Act
        var act = () => order.Submit();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot submit an empty order");
    }

    [Test]
    public void Submit_OrderBelowMinimum_ThrowsDomainException()
    {
        // Arrange
        var order = CreateDraftOrder();
        order.AddLine(ProductId.New(), "Cheap Item", Quantity.Create(1), Money.Create(0.50m, Currency.GBP));

        // Act
        var act = () => order.Submit();

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Order total must be at least £1");
    }

    [Test]
    public void Submit_ValidOrder_ChangesStatusAndRaisesEvent()
    {
        // Arrange
        var order = CreateDraftOrderWithLines();
        order.ClearDomainEvents();

        // Act
        order.Submit();

        // Assert
        order.Status.Should().Be(OrderStatus.Submitted);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderSubmittedEvent>();
    }

    [Test]
    public void MarkAsPaid_SubmittedOrder_ChangesStatusAndRaisesEvent()
    {
        // Arrange
        var order = CreateSubmittedOrder();
        order.ClearDomainEvents();

        // Act
        order.MarkAsPaid("PAY-123456");

        // Assert
        order.Status.Should().Be(OrderStatus.Paid);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderPaidEvent>();
    }

    [Test]
    public void MarkAsPaid_DraftOrder_ThrowsDomainException()
    {
        // Arrange
        var order = CreateDraftOrderWithLines();

        // Act
        var act = () => order.MarkAsPaid("PAY-123456");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Only submitted orders can be marked as paid");
    }

    [Test]
    public void Ship_PaidOrder_ChangesStatusAndRaisesEvent()
    {
        // Arrange
        var order = CreatePaidOrder();
        order.ClearDomainEvents();

        // Act
        order.Ship("TRK-789");

        // Assert
        order.Status.Should().Be(OrderStatus.Shipped);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderShippedEvent>();
    }

    [Test]
    public void Ship_UnpaidOrder_ThrowsDomainException()
    {
        // Arrange
        var order = CreateSubmittedOrder();

        // Act
        var act = () => order.Ship("TRK-789");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Only paid orders can be shipped");
    }

    [Test]
    public void Cancel_DraftOrder_ChangesStatusAndRaisesEvent()
    {
        // Arrange
        var order = CreateDraftOrderWithLines();
        order.ClearDomainEvents();

        // Act
        order.Cancel("Customer changed mind");

        // Assert
        order.Status.Should().Be(OrderStatus.Cancelled);
        order.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<OrderCancelledEvent>();
    }

    [Test]
    public void Cancel_ShippedOrder_ThrowsDomainException()
    {
        // Arrange
        var order = CreateShippedOrder();

        // Act
        var act = () => order.Cancel("Customer changed mind");

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot cancel a shipped order");
    }

    #region Test Helpers

    private static Order CreateDraftOrder()
        => Order.Create(Email.Create("test@example.com"), CreateAddress());

    private static Order CreateDraftOrderWithLines()
    {
        var order = CreateDraftOrder();
        order.AddLine(ProductId.New(), "Test Product", Quantity.Create(1), Money.Create(50, Currency.GBP));
        return order;
    }

    private static Order CreateSubmittedOrder()
    {
        var order = CreateDraftOrderWithLines();
        order.Submit();
        return order;
    }

    private static Order CreatePaidOrder()
    {
        var order = CreateSubmittedOrder();
        order.MarkAsPaid("PAY-123");
        return order;
    }

    private static Order CreateShippedOrder()
    {
        var order = CreatePaidOrder();
        order.Ship("TRK-456");
        return order;
    }

    private static Address CreateAddress()
        => Address.Create("123 Test St", "London", "SW1A 1AA", "UK");

    #endregion
}
