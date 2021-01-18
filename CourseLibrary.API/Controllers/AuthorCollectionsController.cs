using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    [Route("api/authorCollections")]
    public class AuthorCollectionsController: ControllerBase
    {
        private readonly ICourseLibraryRepository _repository;
        private readonly IMapper _mapper;

        public AuthorCollectionsController(ICourseLibraryRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet("({ids})", Name = "getAuthorCollection")]
        public IActionResult GetAuthorsCollections([FromRoute] [ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            if (ids == null)
            {
                return BadRequest();
            }

            var authorFromRepo = _repository.GetAuthors(ids);
            if (ids.Count() != authorFromRepo.Count())
            {
                return NotFound();
            }

            var authorToReturn = _mapper.Map<IEnumerable<AuthorForReturn>>(authorFromRepo);
            return Ok(authorToReturn);
        }
        
        [HttpPost]
        public ActionResult<IEnumerable<AuthorForReturn>> CreateAuthorCollection(
            IEnumerable<AuthorForCreation> authorCollection)
        {
            var authorsToAdd = _mapper.Map<IEnumerable<Author>>(authorCollection);
            foreach (var author in authorsToAdd)
            {
                _repository.AddAuthor(author);
            }

            if (!_repository.Save())
            {
                return BadRequest("Error Happens when saving in Data Base");
            }

            var authorCollectionToReturn = _mapper.Map<IEnumerable<AuthorForReturn>>(authorsToAdd);
            var idsAsString = string.Join(",", authorCollectionToReturn.Select(a => a.Id));
            return CreatedAtRoute("getAuthorCollection", new {ids = idsAsString}, authorCollectionToReturn);
        }
    }
}
