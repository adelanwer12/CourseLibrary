using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseLibrary.API.Models
{
    /// <summary>
    /// An Author With id, Full Name , age and Main Category
    /// </summary>
    public class AuthorForReturn
    {
        /// <summary>
        /// the id of the author
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// the full name of the author
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// the age of the author
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// the Main Category of the author
        /// </summary>
        public string MainCategory { get; set; }
    }
}
