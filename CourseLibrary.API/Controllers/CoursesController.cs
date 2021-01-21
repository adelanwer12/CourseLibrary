using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors/{authorId}/courses")]
    public class CoursesController: ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public CoursesController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseForReturn>>> GetCourses(Guid authorId)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound("Author Not Found");
            }
            var coursesFromRepo =await _repository.GetCourses(authorId);
            if (coursesFromRepo == null)
            {
                return NotFound("this author has no courses");
            }

            var courses = _mapper.Map<IEnumerable<CourseForReturn>>(coursesFromRepo);
            return Ok(courses);
        }
        /// <summary>
        /// get a course by id of specific author 
        /// </summary>
        /// <param name="authorId">the id of course author</param>
        /// <param name="courseId">the id o the course</param>
        /// <returns>An ActionResult of type Course</returns>
        /// <response code="200">Returns the requested course</response>
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{courseId}",Name = "getCourse")]
        public async Task<ActionResult<CourseForReturn>> GetCourse(Guid authorId, Guid courseId)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound("Author Not Found");
            }

            var courseFromRepo =await _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound("this Course Not Found");
            }

            var course = _mapper.Map<CourseForReturn>(courseFromRepo);
            return Ok(course);
        }

        [HttpPost]
        public async Task<ActionResult<CourseForReturn>> CreateCourseForAuthor(Guid authorId, CourseForCreation course)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAdd = _mapper.Map<Course>(course);
            _repository.AddCourse(authorId,courseForAdd);
            if (!await _repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            var courseToReturn = _mapper.Map<CourseForReturn>(courseForAdd);

            return CreatedAtRoute("getCourse", new {authorId = authorId, courseId = courseForAdd.Id}, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public async Task<ActionResult<CourseForReturn>> UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdate course)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = await _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                _repository.AddCourse(authorId, courseToAdd);
                if (!await _repository.Save())
                {
                    return BadRequest("Error Happens when saving in Data Base");
                }

                var courseToReturn = _mapper.Map<CourseForReturn>(courseToAdd);
                return CreatedAtRoute("getCourse", new { authorId = authorId, courseId = courseToAdd.Id }, courseToReturn);

            }

            _mapper.Map(course, courseFromRepo);
            _repository.UpdateCourse(courseFromRepo);
            if (!await _repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();
        }
        /// <summary>
        /// Partially update an author
        /// </summary>
        /// <param name="authorId">the id of the author that you want to update course for him </param>
        /// <param name="courseId">the id of the course you want to update</param>
        /// <param name="pathDocument"> the set of operations to apply to the course</param>
        /// <returns>An action result of type course</returns>
        /// <remarks>
        /// Sample request (this request update the author's course title)  \
        /// PATCH /authors/{authorId}/courses/{courseId}    \
        /// [   \
        ///     {   \
        ///         "op":"replace", \
        ///         "path":"/title",    \
        ///         "value":"new title" \
        ///     }   \
        /// ]   
        /// </remarks>
        [HttpPatch("{courseId}")]
        public async Task<ActionResult> PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId,
            JsonPatchDocument<CourseForUpdate> pathDocument)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo =await _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound();
            }

            var courseToPatch = _mapper.Map<CourseForUpdate>(courseFromRepo);
            pathDocument.ApplyTo(courseToPatch,ModelState);
            if (!TryValidateModel(courseToPatch))
            {
                return ValidationProblem(ModelState);
            }
            _mapper.Map(courseToPatch, courseFromRepo);
            _repository.UpdateCourse(courseFromRepo);
            if (!await _repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();

        }

        [HttpDelete("{courseId}")]
        public async Task<ActionResult> DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!await _repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo =await _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCourse(courseFromRepo);
            if (! await _repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }
            return NoContent();
        }
        
    }
}
