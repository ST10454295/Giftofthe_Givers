using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class ReliefProject
    {
        [Key]
        public int ProjectId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProjectName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Planned";

        [Required]
        public int CreatedByEmployeeId { get; set; }

        [ForeignKey(nameof(CreatedByEmployeeId))]
        public virtual Employee? CreatedByEmployee { get; set; }

        public virtual ICollection<ProjectUpdate> ProjectUpdates { get; set; }
            = new List<ProjectUpdate>();
    }
}