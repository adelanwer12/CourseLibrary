using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    /// <summary>
    /// class for create course
    /// </summary>
    [CourseTitleMustBeDifferentFromDescription]
    public class CourseForCreation 
    {
        /// <summary>
        /// course title is required and max length is 100
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        /// <summary>
        /// course description is max length is 1500
        /// </summary>
        [MaxLength(1500)]
        public string Description { get; set; }
    }
}
