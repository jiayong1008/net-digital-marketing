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
        [DisplayName("Status")]
        public ScoreStatus Status { get; set; }

        // RELATIONSHIPS
        [Required]
        // public string UserId { get; set; }
        public User User { get; set; }

        [Required]
        [ForeignKey("QuizQuestion")]
        [DisplayName("Quiz Question")]
        public int QuizQuestionId { get; set; }
        public QuizQuestion QuizQuestion { get; set; }

        [DisplayName("Answer")]
        [ForeignKey("Answer")]
        public int AnswerId { get; set; }
        public QuestionOption Answer { get; set; }
    }

    public enum ScoreStatus { correct, incorrect }
}
