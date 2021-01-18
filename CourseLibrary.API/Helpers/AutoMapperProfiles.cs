using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CourseLibrary.API.Entities;
using CourseLibrary.API.Models;

namespace CourseLibrary.API.Helpers
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            // AuthorProfiles
            CreateMap<Author, AuthorForReturn>()
                .ForMember(
                    dest=>dest.Name,
                    opt=>opt.MapFrom(src=>$"{src.FirstName} {src.LastName}"))
                .ForMember(
                    dest=>dest.Age, 
                    opt=>opt.MapFrom(src=>src.DateOfBirth.GetCurrentAge()));
            CreateMap<AuthorForCreation, Author>();
            
            //Course Profiles
            CreateMap<Course, CourseForReturn>();
            CreateMap<Course, CourseForUpdate>();
            CreateMap<CourseForCreation, Course>();
            CreateMap<CourseForUpdate, Course>();
        }
    }
}
