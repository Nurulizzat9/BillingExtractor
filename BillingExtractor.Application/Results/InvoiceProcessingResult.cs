using BillingExtractor.Domain.Entities;

namespace BillingExtractor.Application.Results;

public class InvoiceProcessingResult
{
    public Invoice?  Invoice { get; init; }
    public bool IsDuplicate { get; init; }
    public IReadOnlyCollection<string> ValidationWarnings { get; init; } = [];
}
