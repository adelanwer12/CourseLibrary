using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    public class AuthorsResourceParameters
    {
        public string mainCategory { get; set; }
        public string searchQuery { get; set; }
    }
}
