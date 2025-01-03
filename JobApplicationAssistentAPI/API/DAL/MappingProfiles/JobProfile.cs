using AutoMapper;
using API.DAL.Dto;
using API.DAL.Models;

namespace API.DAL.MappingProfiles
{
    public class ApplicationProfile : Profile
    {
        public ApplicationProfile()
        {
            CreateMap<ApplicationRequest, Application>();
            CreateMap<Application, ApplicationRequest>();
            CreateMap<Application, ApplicationResponse>();
        }
    }
}
