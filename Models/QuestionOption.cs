using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketing2.Models
{
    public class QuestionOption
    {
        [Key]
        public int QuestionOptionId { get; set; }

        [Required]
        [DisplayName("Option")]
        public string Option { get; set; }

        // RELATIONSHIPS
        [Required]
        [ValidateNever]
        [ForeignKey("QuizQuestion")]
        [DisplayName("Quiz Question")]
        public int QuizQuestionId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }
    }

    public class QuestionOptionViewModel
    {
        public int QuestionOptionId { get; set; }
        public string Option { get; set; }
    }
}
