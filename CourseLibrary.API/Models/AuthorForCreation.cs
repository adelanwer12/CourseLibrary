using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseLibrary.API.Entities;

namespace CourseLibrary.API.Models
{
    public class AuthorForCreation
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTimeOffset DateOfBirth { get; set; }
        public string MainCategory { get; set; }
        public ICollection<CourseForCreation> Courses { get; set; }
            = new List<CourseForCreation>();
    }
}
