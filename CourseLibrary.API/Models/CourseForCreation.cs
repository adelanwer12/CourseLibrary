using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseLibrary.API.ValidationAttributes;

namespace CourseLibrary.API.Models
{
    [CourseTitleMustBeDifferentFromDescription]
    public class CourseForCreation //:IValidatableObject
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }
        [MaxLength(1500)]
        public string Description { get; set; }

        //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        //{
        //    if (Title == Description)
        //    {
        //        yield return new ValidationResult("the provided description should be different from title",
        //            new[] { "TitleAndDescription" });
        //    }
        //}
    }
}
