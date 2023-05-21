
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string UserId { get; set;}
        public User User { get; set; }

        [Required]
        [ForeignKey("Module")]
        public int ModuleId { get; set; }
        public Module Module { get; set; }
    }

    public class DiscussionFormModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Comment { get; set; }
    }
}
