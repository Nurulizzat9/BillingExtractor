namespace BillingExtractor.Application.DTOs;

public class InvoiceLineItemDto
{
    public string? Description {  get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? LineTotal { get; set; }
}
