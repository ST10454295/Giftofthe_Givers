using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiftOfTheGivers.Models
{
    public class ProjectUpdate
    {
        public int UpdateId { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string UpdateDescription { get; set; } = string.Empty;  
        public DateTime PostedDate { get; set; } = DateTime.Now;       
        public virtual ReliefProject? Project { get; set; }
        public virtual Employee? Employee { get; set; }
    }
}