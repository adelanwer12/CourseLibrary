using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        /// <summary>
        /// get authors and add pagination and filtering 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>

        [HttpGet(Name = "GetAuthors")]
        public async  Task<ActionResult<IEnumerable<AuthorForReturn>>> GetAuthors([FromQuery] AuthorsResourceParameters parameters)
        {
            var authorsFromRepo =await  _repository.GetAuthors(parameters);
            var authors = _mapper.Map<IEnumerable<AuthorForReturn>>(authorsFromRepo);
            var previousPageLink = authorsFromRepo.HasPrevious
                ? CreateAuthorsResourceUri(parameters, ResourceUriType.PreviousPage)
                : null;
            var nextPageLink = authorsFromRepo.HasNext
                ? CreateAuthorsResourceUri(parameters, ResourceUriType.NextPage)
                : null;
            var paginationMetaData = new
            {
                totalCount = authorsFromRepo.TotalCount,
                pageSize = authorsFromRepo.PageSize,
                currentPage = authorsFromRepo.CurrentPage,
                totalPages = authorsFromRepo.TotalPages,
                previousPageLink,
                nextPageLink
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetaData));
            return Ok(authors);
        }
        /// <summary>
        /// get an author by his/her id
        /// </summary>
        /// <param name="authorId"></param>
        /// <returns>An ActionResult of type Author</returns>
        [HttpGet("{authorId}",Name = "getAuthor")]
        public async Task<ActionResult<AuthorForReturn>> GetAuthor(Guid authorId)
        {
            var authorFromRepo = await _repository.GetAuthor(authorId);
            if (authorFromRepo == null)
            {
                return NotFound();
            }

            var author = _mapper.Map<AuthorForReturn>(authorFromRepo);
            
            return Ok(author);
        }

        [HttpPost]
        public async Task<ActionResult<AuthorForReturn>> CreateAuthor(AuthorForCreation author)
        {
            var authorToAdd = _mapper.Map<Author>(author);
            _repository.AddAuthor(authorToAdd);
            if (! await _repository.Save())
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
        public async Task<ActionResult> DeleteAuthor(Guid authorId)
        {
            var authorFromRepo = await _repository.GetAuthor(authorId);
            if (authorFromRepo ==null)
            {
                return NotFound();
            }
            _repository.DeleteAuthor(authorFromRepo);
            if (!await _repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            return NoContent();
        }

        private string CreateAuthorsResourceUri(AuthorsResourceParameters parameters, ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviousPage:
                    return Url.Link("GetAuthors", new
                    {
                        pageNumber = parameters.PageNumber -1,
                        pageSize = parameters.PageSize,
                        mainCategory = parameters.MainCategory,
                        searchQuery = parameters.SearchQuery
                    });
                case ResourceUriType.NextPage:
                    return Url.Link("GetAuthors", new
                    {
                        pageNumber = parameters.PageNumber + 1,
                        pageSize = parameters.PageSize,
                        mainCategory = parameters.MainCategory,
                        searchQuery = parameters.SearchQuery
                    });
                default:
                    return Url.Link("GetAuthors", new
                    {
                        pageNumber = parameters.PageNumber,
                        pageSize = parameters.PageSize,
                        mainCategory = parameters.MainCategory,
                        searchQuery = parameters.SearchQuery
                    });
            }
        }
    }
}
