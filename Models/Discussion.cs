
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.Models
{
    public class Discussion
    {
        [Key]
        public int DiscussionId { get; set; }

        [Required]
        public string Comment { get; set;}

        [Required]
        [DisplayName("Dicusssion Created At")]
        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set;} = DateTime.Now;

        // RELATIONSHIPS
        [Required]
        public User User { get; set; }

        [Required]
        public Lesson Lesson { get; set; }
    }
}
