using AutoMapper;
using DAL.Dto;
using DAL.Models;

namespace DAL.MappingProfiles
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
