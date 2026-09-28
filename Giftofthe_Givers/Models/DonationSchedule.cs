using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class DonationSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        public int DonationId { get; set; }

        [Required]
        [MaxLength(20)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime NextDonationDate { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(DonationId))]
        public virtual Donation? Donation { get; set; }
    }
}