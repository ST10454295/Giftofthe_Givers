using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class TaxCertificate
    {
        [Key]
        public int CertificateId { get; set; }

        [Required]
        public int DonationId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CertificateNumber { get; set; } = string.Empty;

        public DateTime GeneratedDate { get; set; } = DateTime.Now;

        [MaxLength(500)]
        public string? CertificateUrl { get; set; }

        [ForeignKey(nameof(DonationId))]
        public virtual Donation? Donation { get; set; }
    }
}