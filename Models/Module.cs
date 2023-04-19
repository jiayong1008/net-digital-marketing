using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.Models
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required]
        [DisplayName("Module Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} should not be longer than {1} characters.")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Module Description")]
        public string Description { get; set; }

        [DisplayName("Module Order")]
        [Range(1, 500, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int ModuleOrder { get; set; }

        // RELATIONSHIPS
        public List<Lesson>? Lessons { get; set;}        
        public List<Enrollment>? Enrollments { get; set;}

    }
}
