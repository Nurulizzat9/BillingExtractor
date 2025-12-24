using BillingExtractor.Domain.Entities;

namespace BillingExtractor.Application.Abstractions;

public interface IInvoiceRepository
{
    Task<bool> ExistsAsync(string invoiceNumber, string vendorName, DateTime invoiceDate, decimal totalAmount);
    Task SaveAsync(Invoice invoice);
}
