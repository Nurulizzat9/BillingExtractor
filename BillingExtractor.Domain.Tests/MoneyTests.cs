using BillingExtractor.Domain.Exceptions;
using BillingExtractor.Domain.ValueObjects;
using FluentAssertions;

namespace BillingExtractor.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Should_create_money_when_amount_is_positive ()
    {
        var money = new Money(10, "usd");

        money.Amount.Should().Be(10);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Should_throw_exception_when_amount_is_negative()
    {
        var action = () => new Money(-5, "USD");

        action.Should().Throw<DomainException>()
            .WithMessage("Amount cannot be negative");
    }

    [Fact]
    public void Should_add_money_with_same_currency()
    {
        var a = new Money(10, "USD");
        var b = new Money(5, "USD");

        var result = a + b;

        result.Amount.Should().Be(15);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Should_throw_when_currency_is_different()
    {
        var a = new Money(10, "USD");
        var b = new Money(5, "MYR");

        var action = () => { var _ = a + b; };

        action.Should().Throw<DomainException>()
            .WithMessage("Currency mismatch");
    }
}