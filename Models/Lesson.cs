using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.Models
{
    public class Lesson
    {
        [Key]
        public int LessonId { get; set; }

        [Required]
        [DisplayName("Lesson Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} should not be longer than {1} characters.")]
        public string Name { get; set; }

        [Required]
        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Duration { get; set; }

        [Required]
        [Range(1, 500, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int LessonOrder { get; set; }

        // RELATIONSHIPS
        public int ModuleId { get; set; }
        public Module Module { get; set; }

        public List<LessonSection>? LessonSections { get; set; }
        public List<QuizQuestion>? QuizQuestions { get; set; }
    }

    public class LessonCreateFormModel
    {
        [HiddenInput]
        [DisplayName("Lesson ID")]
        public int LessonId { get; set; }

        [Required]
        [DisplayName("Lesson Name")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "{0} should not be longer than {1} characters.")]
        public string Name { get; set; }

        [DisplayName("Lesson Duration (Minutes)")]
        [Range(1, 1000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int Duration { get; set; }

        [DisplayName("Lesson Order")]
        [Range(1, 500, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public int LessonOrder { get; set; }

        [Required]
        [DisplayName("Module")]
        public int ModuleId { get; set; }
    }
}
