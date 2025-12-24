namespace BillingExtractor.Application.DTOs;

public class ExtractedInvoiceDto
{
    public string? InvoiceNumber { get; set; }
    public string? VendorName { get; set; }
    public DateTime? InvoiceDate { get; set; }
    public string? Currency {  get; set; }
    public decimal? Subtotal { get; set; }
    public decimal? Tax { get; set; }
    public decimal? Total { get; set; }

    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
}
