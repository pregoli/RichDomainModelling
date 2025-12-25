namespace RichDomainModelling.Domain.Common;

/// <summary>
/// Exception thrown when a domain rule is violated.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}
