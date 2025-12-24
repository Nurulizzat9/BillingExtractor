using BillingExtractor.Domain.ValueObjects;

namespace BillingExtractor.Domain.Entities;

public class InvoiceLineItem
{
    public string Description { get; }
    public decimal Quantity { get; }
    public Money UnitPrice {  get; }
    public Money LineTotal { get; }

    public InvoiceLineItem (string description, decimal quantity, Money unitPrice)
    {
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        LineTotal = unitPrice * quantity;
    }
}
