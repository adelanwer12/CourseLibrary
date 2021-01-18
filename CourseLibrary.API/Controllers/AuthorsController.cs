using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CourseLibrary.API.Controllers
{
    [ApiController]
    [Route("api/authors")]
    public class AuthorsController: ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public AuthorsController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public  ActionResult<IEnumerable<AuthorForReturn>> GetAuthors([FromQuery] AuthorsResourceParameters parameters)
        {
            var authorsFromRepo =  _repository.GetAuthors(parameters);
            var authors = _mapper.Map<IEnumerable<AuthorForReturn>>(authorsFromRepo);
            var paginationMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages
            };
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetaData));
            return Ok(authors);
        }

        [HttpGet("{authorId}",Name = "getAuthor")]
        public ActionResult<AuthorForReturn> GetAuthor(Guid authorId)
        {
            var authorFromRepo = _repository.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = _mapper.Map<AuthorForReturn>(authorFromRepo);
            
            return Ok(author);
        }

        [HttpPost]
        public ActionResult<AuthorForReturn> CreateAuthor(AuthorForCreation author)
        {
            var authorToAdd = _mapper.Map<Author>(author);
            _repository.AddAuthor(authorToAdd);
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            var authorToReturn = _mapper.Map<AuthorForReturn>(authorToAdd);
            return CreatedAtRoute("getAuthor", new {authorId = authorToAdd.Id}, authorToReturn);
        }

        [HttpOptions]
        public IActionResult GetAuthorOptions()
        {
            Response.Headers.Add("Allow","GET,OPTIONS,POST");
            return Ok();
        }

        [HttpDelete("{authorId}")]
        public ActionResult DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = _repository.GetAuthor(authorId);
            if (authorFromRepo ==null)
            {
                return NotFound();
            }
            _repository.DeleteAuthor(authorFromRepo);
            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();
        }
    }
}
