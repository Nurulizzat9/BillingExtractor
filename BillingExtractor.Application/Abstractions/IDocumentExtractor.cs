using BillingExtractor.Application.DTOs;

namespace BillingExtractor.Application.Abstractions;

public interface IDocumentExtractor
{
    Task<ExtractedInvoiceDto> ExtractAsync(byte[] fileContent, string fileName);
}
