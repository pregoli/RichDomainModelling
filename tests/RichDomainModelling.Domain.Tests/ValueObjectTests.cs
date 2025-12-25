using FluentAssertions;
using NUnit.Framework;
using RichDomainModelling.Domain.Common;
using RichDomainModelling.Domain.Orders;

namespace RichDomainModelling.Domain.Tests;

[TestFixture]
public class MoneyTests
{
    [Test]
    public void Create_WithValidAmount_ReturnsMoney()
    {
        var money = Money.Create(100.50m, Currency.GBP);

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be(Currency.GBP);
    }

    [Test]
    public void Create_WithNegativeAmount_ThrowsDomainException()
    {
        var act = () => Money.Create(-10, Currency.GBP);

        act.Should().Throw<DomainException>()
            .WithMessage("Money amount cannot be negative");
    }

    [Test]
    public void Create_RoundsToTwoDecimalPlaces()
    {
        var money = Money.Create(10.555m, Currency.GBP);

        money.Amount.Should().Be(10.56m); // Banker's rounding
    }

    [Test]
    public void Add_SameCurrency_ReturnsSum()
    {
        var money1 = Money.Create(10, Currency.GBP);
        var money2 = Money.Create(5, Currency.GBP);

        var result = money1.Add(money2);

        result.Amount.Should().Be(15);
    }

    [Test]
    public void Add_DifferentCurrency_ThrowsDomainException()
    {
        var gbp = Money.Create(10, Currency.GBP);
        var usd = Money.Create(10, Currency.USD);

        var act = () => gbp.Add(usd);

        act.Should().Throw<DomainException>()
            .WithMessage("Cannot combine GBP with USD");
    }

    [Test]
    public void Subtract_SufficientFunds_ReturnsDifference()
    {
        var money = Money.Create(100, Currency.GBP);
        var toSubtract = Money.Create(30, Currency.GBP);

        var result = money.Subtract(toSubtract);

        result.Amount.Should().Be(70);
    }

    [Test]
    public void Subtract_InsufficientFunds_ThrowsDomainException()
    {
        var money = Money.Create(10, Currency.GBP);
        var toSubtract = Money.Create(20, Currency.GBP);

        var act = () => money.Subtract(toSubtract);

        act.Should().Throw<DomainException>()
            .WithMessage("Insufficient funds");
    }

    [Test]
    public void Multiply_ReturnsProduct()
    {
        var money = Money.Create(10, Currency.GBP);

        var result = money.Multiply(3);

        result.Amount.Should().Be(30);
    }

    [Test]
    public void Percentage_CalculatesCorrectly()
    {
        var money = Money.Create(200, Currency.GBP);

        var result = money.Percentage(15);

        result.Amount.Should().Be(30);
    }

    [Test]
    public void Equality_SameAmountAndCurrency_AreEqual()
    {
        var money1 = Money.Create(50, Currency.GBP);
        var money2 = Money.Create(50, Currency.GBP);

        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Test]
    public void Equality_DifferentCurrency_AreNotEqual()
    {
        var gbp = Money.Create(50, Currency.GBP);
        var usd = Money.Create(50, Currency.USD);

        gbp.Should().NotBe(usd);
    }
}

[TestFixture]
public class EmailTests
{
    [Test]
    public void Create_WithValidEmail_ReturnsEmail()
    {
        var email = Email.Create("TEST@Example.COM");

        email.Value.Should().Be("test@example.com"); // Normalised
    }

    [Test]
    public void Create_WithEmptyString_ThrowsDomainException()
    {
        var act = () => Email.Create("");

        act.Should().Throw<DomainException>()
            .WithMessage("Email cannot be empty");
    }

    [Test]
    public void Create_WithInvalidFormat_ThrowsDomainException()
    {
        var act = () => Email.Create("notanemail");

        act.Should().Throw<DomainException>()
            .WithMessage("Invalid email format");
    }

    [Test]
    public void Equality_SameEmail_AreEqual()
    {
        var email1 = Email.Create("test@example.com");
        var email2 = Email.Create("TEST@EXAMPLE.COM");

        email1.Should().Be(email2);
    }

    [Test]
    public void ImplicitConversion_ToString_Works()
    {
        var email = Email.Create("test@example.com");
        string value = email;

        value.Should().Be("test@example.com");
    }
}

[TestFixture]
public class QuantityTests
{
    [Test]
    public void Create_WithPositiveValue_ReturnsQuantity()
    {
        var qty = Quantity.Create(5);

        qty.Value.Should().Be(5);
    }

    [Test]
    public void Create_WithZero_ThrowsDomainException()
    {
        var act = () => Quantity.Create(0);

        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be positive");
    }

    [Test]
    public void Create_WithNegative_ThrowsDomainException()
    {
        var act = () => Quantity.Create(-1);

        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be positive");
    }

    [Test]
    public void Add_ReturnsSum()
    {
        var qty1 = Quantity.Create(3);
        var qty2 = Quantity.Create(2);

        var result = qty1.Add(qty2);

        result.Value.Should().Be(5);
    }
}

[TestFixture]
public class AddressTests
{
    [Test]
    public void Create_WithValidInputs_ReturnsAddress()
    {
        var address = Address.Create("123 Main St", "London", "sw1a 1aa", "UK");

        address.Street.Should().Be("123 Main St");
        address.City.Should().Be("London");
        address.PostCode.Should().Be("SW1A 1AA"); // Normalised to uppercase
        address.Country.Should().Be("UK");
    }

    [Test]
    public void Create_WithEmptyStreet_ThrowsDomainException()
    {
        var act = () => Address.Create("", "London", "SW1A 1AA", "UK");

        act.Should().Throw<DomainException>()
            .WithMessage("Street is required");
    }

    [Test]
    public void Equality_SameValues_AreEqual()
    {
        var address1 = Address.Create("123 Main St", "London", "SW1A 1AA", "UK");
        var address2 = Address.Create("123 Main St", "London", "sw1a 1aa", "UK");

        address1.Should().Be(address2);
    }
}
