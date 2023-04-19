using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketing2.Models
{
    public class QuizQuestion
    {
        [Key]
        public int QuizQuestionId { get; set; }

        [Required]
        [DisplayName("Question")]
        public string Question { get; set; }

        [Required]
        [Range(0, 100000)]
        [DisplayName("Quiz Order")]
        public int QuizOrder { get; set; }

        // RELATIONSHIPS
        [Required]
        [DisplayName("Lesson Name")]
        public Lesson Lesson { get; set; }

        // Navigation property to the collection of associated QuestionOptions
        [DisplayName("Answer Options")]
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new HashSet<QuestionOption>();

        [DisplayName("Answer")]
        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public QuestionOption? Answer { get; set; }

        [DisplayName("Student Score")]
        public List<StudentScore>? StudentScores { get; set; }

        public int NumOptions => QuestionOptions.Count;
    }

    public class QuizFormModel
    {
        [HiddenInput]
        [DisplayName("Quiz Question Id")]
        public int QuizQuestionId { get; set; }

        [Required]
        [DisplayName("Lesson")]
        public int LessonId { get; set; }

        [Required]
        [DisplayName("Question")]
        public string Question { get; set; }

        [Required]
        [Range(0, 100000)]
        [DisplayName("Quiz Order")]
        public int QuizOrder { get; set; }

        [MinLength(2, ErrorMessage = "At least two options are required.")]
        [DisplayName("Answer Options")]
        public List<QuestionOption> QuestionOptions { get; set; }

        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public QuestionOption? Answer { get; set; }
    }

}
