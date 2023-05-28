using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices;

namespace DigitalMarketing2.Models
{
    public class User : IdentityUser
    {
        [Required]
        public Gender Gender { get; set; }

        // RELATIONSHIPS
        //public List<Enrollment>? Enrollments { get; set; }

        [DisplayName("Quiz Question")]
        public List<Discussion>? Discussions { get; set; }
    }

    public enum Gender { male, female }

    public class RegisterUserModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password Confirmation")]
        [Compare("Password")]
        public string PasswordConfirmation { get; set; }
    }

    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? ReturnUrl { get; set; }

        [DisplayName("Remeber Me")]
        public bool Remember { get; set; }
    }

    public class UpdateUserModel
    {
        [Required]
        [DisplayName("User ID")]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        //[Required]
        [DataType(DataType.Password)]
        [DisplayName("Password Confirmation")]
        [Compare("Password")]
        public string? PasswordConfirmation { get; set; }
    }

    public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<User> Members { get; set; }
        public IEnumerable<User> NonMembers { get; set; }
    }

    public class RoleModification
    {
        [Required]
        public string RoleName { get; set; }

        public string RoleId { get; set; }

        public string[]? AddIds { get; set; }

        public string[]? DeleteIds { get; set; }
    }

    public class ProfileUserModel
    {
        [DisplayName("User ID")]
        public string Id { get; set; }

        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Gender")]
        public Gender Gender { get; set; }

        public List<ProfileModuleSummary> profileModuleSummaries { get; set; }
    }
}
