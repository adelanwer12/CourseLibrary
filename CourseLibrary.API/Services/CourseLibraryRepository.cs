using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CourseLibrary.API.Helpers;
using CourseLibrary.API.Models;
using Microsoft.EntityFrameworkCore;

namespace CourseLibrary.API.Services
{
    public class CourseLibraryRepository : ICourseLibraryRepository, IDisposable
    {
        private readonly CourseLibraryContext _context;

        public CourseLibraryRepository(CourseLibraryContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async void AddCourse(Guid authorId, Course course)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (course == null)
            {
                throw new ArgumentNullException(nameof(course));
            }
            // always set the AuthorId to the passed-in authorId
            course.AuthorId = authorId;
            await _context.Courses.AddAsync(course);
        }

        public  void DeleteCourse(Course course)
        {
             _context.Courses.Remove(course);
        }

        public async Task<Course> GetCourse(Guid authorId, Guid courseId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(courseId));
            }

            return await _context.Courses
              .Where(c => c.AuthorId == authorId && c.Id == courseId).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Course>> GetCourses(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Courses
                        .Where(c => c.AuthorId == authorId)
                        .OrderBy(c => c.Title).ToListAsync();
        }

        public void UpdateCourse(Course course)
        {
            // no code in this implementation
        }

        public async void AddAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            //// the repository fills the id (instead of using identity columns)
            //author.Id = Guid.NewGuid();

            //foreach (var course in author.Courses)
            //{
            //    course.Id = Guid.NewGuid();
            //}

            await _context.Authors.AddAsync(author);
        }

        public async Task<bool> AuthorExists(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Authors.AnyAsync(a => a.Id == authorId);
        }

        public void DeleteAuthor(Author author)
        {
            if (author == null)
            {
                throw new ArgumentNullException(nameof(author));
            }

            _context.Authors.Remove(author);
        }

        public async Task<Author> GetAuthor(Guid authorId)
        {
            if (authorId == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(authorId));
            }

            return await _context.Authors.FirstOrDefaultAsync(a => a.Id == authorId);
        }

        public async Task<IEnumerable<Author>> GetAuthors()
        {
            return await _context.Authors.ToListAsync<Author>();
        }

        public async Task<PagedList<Author>> GetAuthors(AuthorsResourceParameters parameters)
        {
            IQueryable<Author> collection =  _context.Authors;
            if (!string.IsNullOrWhiteSpace(parameters.MainCategory))
            {
                parameters.MainCategory = parameters.MainCategory.Trim();
                collection = collection.Where(x => x.MainCategory == parameters.MainCategory);
            }

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                parameters.SearchQuery = parameters.SearchQuery.Trim();
                collection = collection.Where(a =>
                    a.MainCategory.Contains(parameters.SearchQuery) 
                    || a.FirstName.Contains(parameters.SearchQuery) 
                    || a.LastName.Contains(parameters.SearchQuery));
            }

            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
            {
                if (parameters.OrderBy.ToLowerInvariant()=="name")
                {
                    collection = collection.OrderBy(a => a.FirstName).ThenBy(a => a.LastName);
                }
            }
            return await PagedList<Author>.CreateAsync(collection, parameters.PageNumber, parameters.PageSize);
        }
        public async Task<IEnumerable<Author>> GetAuthors(IEnumerable<Guid> authorIds)
        {
            if (authorIds == null)
            {
                throw new ArgumentNullException(nameof(authorIds));
            }

            return await _context.Authors.Where(a => authorIds.Contains(a.Id))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .ToListAsync();
        }

        public void UpdateAuthor(Author author)
        {
            // no code in this implementation
        }

        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // dispose resources when needed
            }
        }
    }
}
