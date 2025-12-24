using BillingExtractor.Domain.Exceptions;
using BillingExtractor.Domain.ValueObjects;

namespace BillingExtractor.Domain.Entities;

public class Invoice
{
    public string InvoiceNumber { get; }
    public string VendorName { get; }
    public DateTime InvoiceDate { get; }
    public Money Subtotal { get;}
    public Money Tax { get;}
    public Money Total { get;}
    public IReadOnlyCollection<InvoiceLineItem> LineItems => _lineItems.AsReadOnly();

    private readonly List<InvoiceLineItem> _lineItems = new();

    public Invoice (string invoiceNumber, string vendorName, DateTime invoiceDate, Money tax, IEnumerable<InvoiceLineItem> lineItems)
    {
        if (string.IsNullOrWhiteSpace(invoiceNumber))
            throw new DomainException("Invoice number is required");

        if (string.IsNullOrWhiteSpace(vendorName))
            throw new DomainException("Vendor name is required");

        InvoiceNumber = invoiceNumber;
        VendorName = vendorName;
        InvoiceDate = invoiceDate;
        Tax = tax;

        _lineItems.AddRange(lineItems);

        Subtotal = CalculateSubtotal();
        Total = Subtotal + Tax;
    }

    private Money CalculateSubtotal()
    {
        if (!_lineItems.Any())
            throw new DomainException("Invoice must contain at least one line item");

        var currency = _lineItems.First().LineTotal.Currency;

        return _lineItems.Select(li => li.LineTotal).Aggregate(Money.Zero(currency), (current, next) => current + next);
    }
}
