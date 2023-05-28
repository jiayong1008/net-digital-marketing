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
        public List<Discussion>? Discussions { get; set; }
        //public List<Enrollment>? Enrollments { get; set;}

    }

    public class ProfileModuleSummary
    {
        public int ModuleId { get; set; }

        [DisplayName("Module Name")]
        public string ModuleName { get; set; }

        public List<ProfileQuizScore>? ProfileQuizScores { get; set; }
    }

    public class ModuleDetailModel
    {
        public Module Module { get; set; }
        public DiscussionFormModel DiscussionForm { get; set; }
    }
}
