using DigitalMarketing2.CustomHandlers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DigitalMarketing2.Models
{
    public class LessonSection
    {
        [Key]
        public int LessonSectionId { get; set; }

        [DisplayName("Lesson Section Order")]
        public int LessonSectionOrder { get; set; }

        [DisplayName("Text")]
        public string? Text { get; set; }

        [DisplayName("Image")]
        public byte[]? ImageData { get; set; }

        [DisplayName("Image Type")]
        public string? ImageType { get; set; }

        // RELATIONSHIPS
        [Required]
        [DisplayName("Lesson")]
        public int LessonId { get; set; }
        public Lesson Lesson { get; set; }

        // Only one of Text and Image can be filled at a time
        public bool IsText => !string.IsNullOrEmpty(Text);
        public bool IsImage => ImageData != null;
        public bool IsEmpty => !IsText && !IsImage;
    }

    public class LessonSectionFormModel
    {
        [HiddenInput]
        [DisplayName("Lesson Section ID")]
        public int LessonSectionId { get; set; }

        [DisplayName("Lesson Section Order")]
        [Range(1, 500)]
        public int LessonSectionOrder { get; set; }

        [DisplayName("Text")]
        public string? Text { get; set; }

        [DisplayName("Image")]
        [DataType(DataType.Upload)]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" }, ErrorMessage = "Only image files (.jpg, .jpeg, .png, .gif) are allowed.")]
        public IFormFile? Image { get; set; }

        public bool RemoveImage { get; set; }

        [Required]
        [DisplayName("Lesson")]
        public int LessonId { get; set; }
    }

}
