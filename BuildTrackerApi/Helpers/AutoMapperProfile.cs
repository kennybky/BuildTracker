using AutoMapper;
using BuildTrackerApi.Models;
using BuildTrackerApi.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BuildTrackerApi.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();

            CreateMap<BuildDto, Build>();
            CreateMap<Build, BuildDto>();

            CreateMap<TestDto, Test>();
            CreateMap<Test, TestDto>();

            CreateMap<ProductDto, Product>();
            CreateMap<Product, ProductDto>();

            CreateMap<ProductDeveloperDto, ProductDeveloper>();
            CreateMap<ProductDeveloper, ProductDeveloperDto>();

        }
    }
}
