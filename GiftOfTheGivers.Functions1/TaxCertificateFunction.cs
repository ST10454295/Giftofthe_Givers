using GiftOfTheGivers.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GiftOfTheGivers.Functions1;

public sealed class TaxCertificateFunction(ILogger<TaxCertificateFunction> logger)
{
    public sealed class DonationRequest
    {
        [Required, StringLength(200)]
        public string? DonorName { get; set; }
        [Range(typeof(decimal), "0.01", "1000000000")]
        public decimal Amount { get; set; }
        [Required, StringLength(64)]
        public string? DonationReference { get; set; }
        public DateTime DonationDate { get; set; }
        [Required, StringLength(3, MinimumLength = 3)]
        public string? Currency { get; set; }
    }

    public sealed record TaxCertificateResponse(string CertificateNumber, string DonorName, decimal DonationAmount,
        string Currency, string DonationReference, DateTime DonationDate, string Message);

    [Function("GenerateTaxCertificate")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "generate-certificate")] HttpRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Tax certificate request received.");
        try
        {
            var donation = await JsonSerializer.DeserializeAsync<DonationRequest>(request.Body,
                new JsonSerializerOptions(JsonSerializerDefaults.Web) { PropertyNameCaseInsensitive = true }, cancellationToken);
            if (donation is null)
            {
                return new BadRequestObjectResult(new { error = "A donation request is required." });
            }

            var validationResults = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(donation, new ValidationContext(donation), validationResults, true);
            if (donation.DonationDate == default)
            {
                validationResults.Add(new ValidationResult("DonationDate is required."));
                valid = false;
            }
            if (!valid)
            {
                return new BadRequestObjectResult(new { error = string.Join(" ", validationResults.Select(x => x.ErrorMessage)) });
            }

            var certificateNumber = DonationFormatting.FormatCertificateNumber($"TAX-{Guid.NewGuid():N}"[..12]);
            var result = new TaxCertificateResponse(certificateNumber, donation.DonorName!.Trim(), donation.Amount,
                donation.Currency!.Trim().ToUpperInvariant(), donation.DonationReference!.Trim(), donation.DonationDate,
                "Tax certificate generated successfully.");
            logger.LogInformation("Generated tax certificate {CertificateNumber} for donation {DonationReference}.",
                certificateNumber, result.DonationReference);
            return new OkObjectResult(result);
        }
        catch (JsonException)
        {
            return new BadRequestObjectResult(new { error = "Request body must contain valid donation JSON." });
        }
    }
}
