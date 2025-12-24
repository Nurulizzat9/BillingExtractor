using BillingExtractor.Domain.Entities;
using BillingExtractor.Domain.Exceptions;
using BillingExtractor.Domain.ValueObjects;
using FluentAssertions;

namespace BillingExtractor.Domain.Tests;

public class InvoiceTests
{
    [Fact]
    public void Should_calculate_subtotal_and_total_correctly()
    {
        var items = new[]
        {
            new InvoiceLineItem("Apple", 2, new Money(3, "USD")),
            new InvoiceLineItem("Orange", 1, new Money(4, "USD"))
        };

        var tax = new Money(1, "USD");

        var invoice = new Invoice(
            invoiceNumber: "INV-001",
            vendorName: "Fresh Fruits Ltd",
            invoiceDate: DateTime.UtcNow,
            tax: tax,
            lineItems: items
        );

        invoice.Subtotal.Amount.Should().Be(10);
        invoice.Total.Amount.Should().Be(11);
    }

    [Fact]
    public void Should_throw_when_no_line_items()
    {
        var tax = new Money(0, "USD");

        var action = () =>
        {
            new Invoice(
                invoiceNumber: "INV-002",
                vendorName: "Vendor",
                invoiceDate: DateTime.UtcNow,
                tax: tax,
                lineItems: Array.Empty<InvoiceLineItem>()
            );
        };

        action.Should().Throw<DomainException>()
            .WithMessage("Invoice must contain at least one line item");
    }
}
