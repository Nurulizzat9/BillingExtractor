using BillingExtractor.Application.Abstractions;
using BillingExtractor.Application.Results;
using BillingExtractor.Domain.Entities;
using BillingExtractor.Domain.ValueObjects;

namespace BillingExtractor.Application.UseCases;

public class ExtractedInvoiceUseCase
{
    private readonly IDocumentExtractor _documentExtractor;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceValidator _validator;

    public ExtractedInvoiceUseCase(IDocumentExtractor documentExtractor, IInvoiceRepository invoiceRepository, IInvoiceValidator validator)
    {
        _documentExtractor = documentExtractor;
        _invoiceRepository = invoiceRepository;
        _validator = validator;
    }

    public async Task<InvoiceProcessingResult> ExecuteAsync(byte[] fileContent, string fileName)
    {
        var extracted = await _documentExtractor.ExtractAsync(fileContent, fileName);

        if (extracted.Currency is null)
            throw new InvalidOperationException("Currency missing from extracted data");

        var currency = extracted.Currency;

        var lineItems = extracted.LineItems.Select(li =>
            new InvoiceLineItem(
                li.Description ?? "Unknown",
                li.Quantity ?? 1,
                new Money(li.UnitPrice ?? 0, currency)
            )
        ).ToList();

        var tax = new Money(extracted.Tax ?? 0, currency);

        var invoice = new Invoice(
            extracted.InvoiceNumber ?? Guid.NewGuid().ToString(),
            extracted.VendorName ?? "Unknown Vendor",
            extracted.InvoiceDate ?? DateTime.UtcNow,
            tax,
            lineItems
        );

        var warnings = _validator.Validate(invoice);

        var isDuplicate = await _invoiceRepository.ExistsAsync(
            invoice.InvoiceNumber,
            invoice.VendorName,
            invoice.InvoiceDate,
            invoice.Total.Amount
        );

        if (!isDuplicate )
            await _invoiceRepository.SaveAsync( invoice );

        return new InvoiceProcessingResult
        {
            Invoice = invoice,
            IsDuplicate = isDuplicate,
            ValidationWarnings = warnings
        };
    }
}
