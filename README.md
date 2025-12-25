# Rich Domain Modelling Demo

A complete, buildable .NET 10 solution demonstrating Rich Domain Model patterns from the article [Rich Domain Modelling: Escaping the Anaemic Model Trap](https://devocrazia.com/articles/rich-domain-modelling-escape-anaemic-models).

## Project Structure

```
RichDomainModelling/
├── src/
│   └── RichDomainModelling.Domain/
│       ├── Common/           # Base classes (Entity, ValueObject, Specification, etc.)
│       ├── Orders/           # Order aggregate with value objects
│       └── Pricing/          # Domain service example
└── tests/
    └── RichDomainModelling.Domain.Tests/
        ├── OrderTests.cs         # Aggregate behaviour tests
        └── ValueObjectTests.cs   # Value object tests
```

## Key Concepts Demonstrated

- **Value Objects**: `Email`, `Money`, `Quantity`, `Address` — immutable, self-validating
- **Strongly-Typed IDs**: `OrderId`, `ProductId`, `CustomerId` — prevent primitive obsession
- **Entities**: `Order`, `OrderLine` — identity-based equality
- **Aggregates**: `Order` as aggregate root controlling `OrderLine` children
- **Domain Events**: `OrderCreatedEvent`, `OrderSubmittedEvent`, etc.
- **Specifications**: `OrderReadyToShipSpecification`, `HighValueOrderSpecification`
- **Domain Services**: `IPricingService` for cross-aggregate behaviour
- **Factory Methods**: Controlled object creation via `Order.Create()`

## Requirements

- .NET 10.0 SDK or later
- Visual Studio 2022, Rider, or VS Code with C# extension

## Getting Started

```bash
# Clone the repository
git clone https://github.com/pregoli/RichDomainModelling.git
cd RichDomainModelling

# Build
dotnet build

# Run tests
dotnet test
```

## Running Tests

```bash
dotnet test --verbosity normal
```

All 35+ NUnit tests should pass, demonstrating:
- Order lifecycle (Draft → Submitted → Paid → Shipped)
- Invariant enforcement (can't ship unpaid orders, can't modify submitted orders)
- Value object validation and equality
- Domain event raising

## Example Usage

```csharp
// Create an order using factory method
var order = Order.Create(
    Email.Create("customer@example.com"),
    Address.Create("123 Main St", "London", "SW1A 1AA", "UK"));

// Add lines — strongly typed, validated inputs
order.AddLine(
    ProductId.New(),
    "Mechanical Keyboard",
    Quantity.Create(2),
    Money.Create(149.99m, Currency.GBP));

// Submit — enforces minimum order value
order.Submit();

// Pay
order.MarkAsPaid("PAY-123456");

// Ship — only works for paid orders
order.Ship("TRK-789");

// Check domain events
foreach (var evt in order.DomainEvents)
{
    Console.WriteLine(evt.GetType().Name);
}
```

## Related Article

This code accompanies the article on [devocrazia.com](https://devocrazia.com/articles/rich-domain-modelling-escape-anaemic-models) which covers:

- Why anaemic models are problematic
- Value objects and their benefits
- Entity vs Value Object design decisions
- Aggregate boundaries and consistency
- The Specification pattern
- Domain services for cross-cutting behaviour
- Testing rich domain models

## License

MIT
