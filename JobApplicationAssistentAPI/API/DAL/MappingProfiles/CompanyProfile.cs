using AutoMapper;
using API.DAL.Dto;
using API.DAL.Models;

namespace API.DAL.MappingProfiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CompanyRequest, Company>();
            CreateMap<Company, CompanyRequest>();
            CreateMap<Company, CompanyResponse>();
        }
    }
}
