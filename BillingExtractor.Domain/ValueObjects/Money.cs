using BillingExtractor.Domain.Exceptions;

namespace BillingExtractor.Domain.ValueObjects;

public sealed class Money
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency) 
    {
        if (amount < 0)
            throw new DomainException("Amount cannot be negative");

        if (string.IsNullOrWhiteSpace(currency))
            throw new DomainException("Currency is required");

        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Zero(string currency) => new(0, currency);

    public static Money operator +(Money a, Money b)
    {
        EnsureSameCurrency(a, b);
        return new Money(a.Amount + b.Amount, a.Currency);
    }

    public static Money operator *(Money money, decimal multiplier)
    {
        if (multiplier < 0)
            throw new DomainException("Multiplier cannot be negative");

        return new Money(money.Amount * multiplier, money.Currency);
    }

    private static void EnsureSameCurrency(Money a, Money b)
    {
        if (a.Currency != b.Currency)
            throw new DomainException("Currency mismatch");
    }
}