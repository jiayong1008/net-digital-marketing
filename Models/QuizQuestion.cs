using DigitalMarketing2.CustomValidations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

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
        public int LessonId { get; set; }
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

    public class AttemptQuizIndexModel
    {
        public int LessonId { get; set; }
        public List<QuizQuestionViewModel> QuizQuestionViewModels { get; set; }
    }

    public class ViewQuizIndexModel
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public List<QuizQuestion> QuizQuestions { get; set; }
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

    public class QuizQuestionViewModel
    {
        public int QuizQuestionId { get; set; }
        public string Question { get; set; }
        public int LessonId { get; set; }

        [BindNever]
        public List<QuestionOptionViewModel> Options { get; set; }
        public int AnswerId { get; set; }
        [Required]
        [RequiredAnswer(ErrorMessage = "Please select an answer.")]
        public int AttemptedAnswerId { get; set; }
    }

    public class QuizAttemptViewModel
    {
        public int QuizQuestionId { get; set; }
        public int AttemptedAnswerId { get; set; }
    }

    public class QuizAttemptModel
    {
        [HiddenInput]
        public int QuizQuestionId { get; set; }

        [Required]
        [DisplayName("Question")]
        public string Question { get; set; }

        [HiddenInput]
        [Required]
        [DisplayName("Lesson")]
        public int LessonId { get; set; }

        [DisplayName("Answer Options")]
        public virtual ICollection<QuestionOption> QuestionOptions { get; set; } = new HashSet<QuestionOption>();

        [HiddenInput]
        [Required]
        [ForeignKey("Answer")]
        public int? AnswerId { get; set; }
        public QuestionOption? Answer { get; set; }

        [Required]
        public int? AttemptedAnswerId { get; set; }
        public QuestionOption? AttemptedAnswer { get; set; }
    }

    public class ProfileQuizScore
    {
        public int LessonId { get; set; }

        [DisplayName("Lesson Name")]
        public string LessonName { get; set; }

        //public int QuizQuestionId { get; set; }

        [DisplayName("Correct Questions")]
        public int CorrectQuestions { get; set; }

        [DisplayName("Total Questions")]
        public int TotalQuestions { get; set; }

        [DisplayName("Quiz Score")]
        public double QuizScore { get; set; }
    }

}
