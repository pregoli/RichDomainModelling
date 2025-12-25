using RichDomainModelling.Domain.Common;

namespace RichDomainModelling.Domain.Orders;

/// <summary>
/// Value object representing a postal address.
/// Validates and normalises address components.
/// </summary>
public sealed class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostCode { get; }
    public string Country { get; }

    private Address(string street, string city, string postCode, string country)
    {
        Street = street;
        City = city;
        PostCode = postCode;
        Country = country;
    }

    public static Address Create(string street, string city, string postCode, string country)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new DomainException("Street is required");
        if (string.IsNullOrWhiteSpace(city))
            throw new DomainException("City is required");
        if (string.IsNullOrWhiteSpace(postCode))
            throw new DomainException("Post code is required");
        if (string.IsNullOrWhiteSpace(country))
            throw new DomainException("Country is required");

        return new Address(
            street.Trim(),
            city.Trim(),
            postCode.Trim().ToUpper(),
            country.Trim());
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostCode;
        yield return Country;
    }

    public override string ToString() => $"{Street}, {City}, {PostCode}, {Country}";
}
