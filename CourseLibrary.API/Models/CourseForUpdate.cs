using System.ComponentModel.DataAnnotations;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    /// <summary>
    /// this is update class for course updates only title and description
    /// </summary>
    public class CourseForUpdate
    {
        /// <summary>
        /// course title is required
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// course description
        /// </summary>
        [MaxLength(1500)]
        public string Description { get; set; }
    }
}