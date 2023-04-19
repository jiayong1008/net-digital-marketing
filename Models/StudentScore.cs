using NuGet.Protocol.Core.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DigitalMarketing2.Models
{
    public class StudentScore
    {
        [Key]
        public int StudentScoreId { get; set; }

        [Required]
        [DisplayName("Status")] // correct, incorrect
        [StringLength(10, ErrorMessage = "Student's score status must be either 'correct' or 'incorrect'.")]
        public string Status { get; set; }

        // RELATIONSHIPS
        [Required]
        public Enrollment Enrollment { get; set; }

        [Required]
        [ForeignKey("QuizQuestion")]
        [DisplayName("Quiz Question")]
        public int StudentQuizQuestionId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }
    }
}
