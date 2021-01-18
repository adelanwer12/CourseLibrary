using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
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
        public ActionResult<IEnumerable<CourseForReturn>> GetCourses(Guid authorId)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound("Author Not Found");
            }
            var coursesFromRepo = _repository.GetCourses(authorId);
            if (coursesFromRepo == null)
            {
                return NotFound("this author has no courses");
            }

            var courses = _mapper.Map<IEnumerable<CourseForReturn>>(coursesFromRepo);
            return Ok(courses);
        }

        [HttpGet("{courseId}",Name = "getCourse")]
        public ActionResult<CourseForReturn> GetCourse(Guid authorId, Guid courseId)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound("Author Not Found");
            }

            var courseFromRepo = _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound("this Course Not Found");
            }

            var course = _mapper.Map<CourseForReturn>(courseFromRepo);
            return Ok(course);
        }

        [HttpPost]
        public ActionResult<CourseForReturn> CreateCourseForAuthor(Guid authorId, CourseForCreation course)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }
            var courseForAdd = _mapper.Map<Course>(course);
            _repository.AddCourse(authorId,courseForAdd);
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            var courseToReturn = _mapper.Map<CourseForReturn>(courseForAdd);

            return CreatedAtRoute("getCourse", new {authorId = authorId, courseId = courseForAdd.Id}, courseToReturn);
        }

        [HttpPut("{courseId}")]
        public ActionResult<CourseForReturn> UpdateCourseForAuthor(Guid authorId, Guid courseId, CourseForUpdate course)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                var courseToAdd = _mapper.Map<Course>(course);
                _repository.AddCourse(authorId, courseToAdd);
                if (!_repository.Save())
                {
                    return BadRequest("Error Happens when saving in Data Base");
                }

                var courseToReturn = _mapper.Map<CourseForReturn>(courseToAdd);
                return CreatedAtRoute("getCourse", new { authorId = authorId, courseId = courseToAdd.Id }, courseToReturn);

            }

            _mapper.Map(course, courseFromRepo);
            _repository.UpdateCourse(courseFromRepo);
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();
        }

        [HttpPatch("{courseId}")]
        public ActionResult PartiallyUpdateCourseForAuthor(Guid authorId, Guid courseId,
            JsonPatchDocument<CourseForUpdate> pathDocument)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _repository.GetCourse(authorId, courseId);
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
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();

        }

        [HttpDelete("{courseId}")]
        public ActionResult DeleteCourseForAuthor(Guid authorId, Guid courseId)
        {
            if (!_repository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var courseFromRepo = _repository.GetCourse(authorId, courseId);
            if (courseFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCourse(courseFromRepo);
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }
            return NoContent();
        }
        
    }
}
