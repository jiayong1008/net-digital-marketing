
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        [DisplayName("Enrollment Date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        [Required]
        [DisplayName("Enrollment Completion Status")]
        [StringLength(15)]
        public string Status { get; set; } = "Incomplete";

        // RELATIONSHIPS
        [Required]
        public User User { get; set; }

        [Required]
        public Module Module { get; set; }

    }
}
