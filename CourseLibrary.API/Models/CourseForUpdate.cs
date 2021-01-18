using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    public class CourseForUpdate
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }
    }
}