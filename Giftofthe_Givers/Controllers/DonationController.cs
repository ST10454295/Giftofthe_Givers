using GiftOfTheGivers.Helpers;
using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json;
using GiftOfTheGivers.Data;
using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GiftOfTheGivers.Controllers;

public sealed class DonationController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DonationController> _logger;

    public DonationController(ApplicationDbContext context, IHttpClientFactory httpClientFactory,
        IConfiguration configuration, ILogger<DonationController> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(Donation donation, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("~/Views/Home/Donate.cshtml", donation);
        }

        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userId, out var donorId))
            {
                ModelState.AddModelError(string.Empty, "Your account could not be linked to this donation.");
                return View("~/Views/Home/Donate.cshtml", donation);
            }
            donation.DonorId = donorId;
        }

        donation.DonationDate = DateTime.UtcNow;
        _context.Donations.Add(donation);
        await _context.SaveChangesAsync(cancellationToken);
        await TryGenerateTaxCertificateAsync(donation, cancellationToken);
        return RedirectToAction(nameof(Confirmation));
    }

    [HttpGet]
    public IActionResult Confirmation() => View();

    private async Task TryGenerateTaxCertificateAsync(Donation donation, CancellationToken cancellationToken)
    {
        var endpoint = _configuration["TaxCertificateFunctionUrl"];
        if (!Uri.TryCreate(endpoint, UriKind.Absolute, out var functionUri) ||
            (functionUri.Scheme != Uri.UriSchemeHttps && !functionUri.IsLoopback))
        {
            _logger.LogWarning("Tax certificate generation is not configured; donation {DonationId} was saved.", donation.DonationId);
            TempData["CertificatePending"] = true;
            return;
        }

        try
        {
            var donorName = "Anonymous Donor";
            if (!donation.IsAnonymous && donation.DonorId is int donorId)
            {
                var donor = await _context.Donors.AsNoTracking()
                    .FirstOrDefaultAsync(item => item.DonorId == donorId, cancellationToken);
                if (donor is not null)
                {
                    donorName = $"{donor.FirstName} {donor.LastName}".Trim();
                }
            }

            var request = new TaxCertificateRequest(donorName, donation.Amount,
                donation.DonationId.ToString(CultureInfo.InvariantCulture), donation.DonationDate, donation.Currency);
            using var response = await _httpClientFactory.CreateClient("TaxCertificateFunction")
                .PostAsJsonAsync(functionUri, request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Tax certificate function returned HTTP {StatusCode} for donation {DonationId}.",
                    (int)response.StatusCode, donation.DonationId);
                TempData["CertificatePending"] = true;
                return;
            }

            var certificate = await response.Content.ReadFromJsonAsync<TaxCertificateResponse>(cancellationToken: cancellationToken);
            if (string.IsNullOrWhiteSpace(certificate?.CertificateNumber))
            {
                _logger.LogWarning("Tax certificate function returned an invalid response for donation {DonationId}.", donation.DonationId);
                TempData["CertificatePending"] = true;
                return;
            }

            var certificateNumber = DonationFormatting.FormatCertificateNumber(certificate.CertificateNumber);
            _context.TaxCertificates.Add(new TaxCertificate
            {
                DonationId = donation.DonationId,
                CertificateNumber = certificateNumber,
                GeneratedDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync(cancellationToken);
            TempData["CertificateNumber"] = certificateNumber;
            TempData["DonationAmount"] = DonationFormatting.FormatDonationAmount(donation.Amount, donation.Currency);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(exception, "Tax certificate service was unavailable for donation {DonationId}.", donation.DonationId);
            TempData["CertificatePending"] = true;
        }
        catch (TaskCanceledException exception) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(exception, "Tax certificate request timed out for donation {DonationId}.", donation.DonationId);
            TempData["CertificatePending"] = true;
        }
        catch (JsonException exception)
        {
            _logger.LogWarning(exception, "Tax certificate response could not be read for donation {DonationId}.", donation.DonationId);
            TempData["CertificatePending"] = true;
        }
    }

    private sealed record TaxCertificateRequest(string DonorName, decimal Amount, string DonationReference,
        DateTime DonationDate, string Currency);
    private sealed record TaxCertificateResponse(string? CertificateNumber);
}
