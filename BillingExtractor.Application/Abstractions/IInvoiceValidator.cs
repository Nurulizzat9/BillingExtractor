using BillingExtractor.Domain.Entities;

namespace BillingExtractor.Application.Abstractions;

public interface IInvoiceValidator
{
    IReadOnlyCollection<string> Validate(Invoice invoice);
}
