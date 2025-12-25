namespace RichDomainModelling.Domain.Orders;

public enum Currency
{
    GBP,
    USD,
    EUR
}

public static class CurrencyExtensions
{
    public static string Symbol(this Currency currency) => currency switch
    {
        Currency.GBP => "£",
        Currency.USD => "$",
        Currency.EUR => "€",
        _ => currency.ToString()
    };
}
