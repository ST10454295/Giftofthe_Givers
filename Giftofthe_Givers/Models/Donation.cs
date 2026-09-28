using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class Donation
    {
        [Key]
        public int DonationId { get; set; }

        public int? DonorId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [MaxLength(3)]
        public string Currency { get; set; } = "ZAR";

        [Required]
        [MaxLength(20)]
        public string DonationType { get; set; } = "Once-Off";

        public DateTime DonationDate { get; set; } = DateTime.Now;

        public bool IsAnonymous { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Completed";

        [ForeignKey(nameof(DonorId))]
        public virtual Donor? Donor { get; set; }

        public virtual ICollection<DonationSchedule> DonationSchedules { get; set; }
            = new List<DonationSchedule>();

        public virtual ICollection<TaxCertificate> TaxCertificates { get; set; }
            = new List<TaxCertificate>();
    }
}