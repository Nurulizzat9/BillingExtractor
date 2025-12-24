using BillingExtractor.Application.Abstractions;
using BillingExtractor.Application.DTOs;
using BillingExtractor.Application.UseCases;
using BillingExtractor.Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace BillingExtractor.Application.Tests;

public class ExtractedInvoiceUseCaseTests
{
    [Fact]
    public async Task Should_save_invoice_when_not_duplicate()
    {
        // Arrange
        var extractor = new Mock<IDocumentExtractor>();
        var repository = new Mock<IInvoiceRepository>();
        var validator = new Mock<IInvoiceValidator>();

        extractor.Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
            .ReturnsAsync(new ExtractedInvoiceDto
            {
                InvoiceNumber = "INV-001",
                VendorName = "Test Vendor",
                InvoiceDate = DateTime.UtcNow,
                Currency = "USD",
                Tax = 0,
                LineItems =
                {
                    new InvoiceLineItemDto
                    {
                        Description = "Item A",
                        Quantity = 1,
                        UnitPrice = 10
                    }
                }
            });

        repository.Setup(r =>
            r.ExistsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<decimal>()))
            .ReturnsAsync(false);

        validator.Setup(v => v.Validate(It.IsAny<BillingExtractor.Domain.Entities.Invoice>()))
            .Returns(Array.Empty<string>());

        var useCase = new ExtractedInvoiceUseCase(
            extractor.Object,
            repository.Object,
            validator.Object);

        // Act
        var result = await useCase.ExecuteAsync(new byte[] { 1, 2, 3 }, "invoice.png");

        // Assert
        result.IsDuplicate.Should().BeFalse();

        repository.Verify(r => r.SaveAsync(It.IsAny<BillingExtractor.Domain.Entities.Invoice>()), Times.Once);
    }

    [Fact]
    public async Task Should_not_save_invoice_when_duplicate()
    {
        var extractor = new Mock<IDocumentExtractor>();
        var repository = new Mock<IInvoiceRepository>();
        var validator = new Mock<IInvoiceValidator>();

        extractor.Setup(x => x.ExtractAsync(It.IsAny<byte[]>(), It.IsAny<string>()))
            .ReturnsAsync(new ExtractedInvoiceDto
            {
                InvoiceNumber = "INV-002",
                VendorName = "Duplicate Vendor",
                InvoiceDate = DateTime.UtcNow,
                Currency = "USD",
                LineItems =
                {
                    new InvoiceLineItemDto
                    {
                        Description = "Item",
                        Quantity = 1,
                        UnitPrice = 5
                    }
                }
            });

        repository.Setup(r =>
            r.ExistsAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime>(),
                It.IsAny<decimal>()))
            .ReturnsAsync(true);

        validator.Setup(v => v.Validate(It.IsAny<BillingExtractor.Domain.Entities.Invoice>()))
            .Returns(new[] { "DUPLICATE_INVOICE" });

        var useCase = new ExtractedInvoiceUseCase(
            extractor.Object,
            repository.Object,
            validator.Object);

        var result = await useCase.ExecuteAsync(new byte[] { 1 }, "invoice.png");

        result.IsDuplicate.Should().BeTrue();

        repository.Verify(r => r.SaveAsync(It.IsAny<BillingExtractor.Domain.Entities.Invoice>()), Times.Never);
    }
}
