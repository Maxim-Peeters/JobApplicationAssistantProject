using AutoMapper;
using DAL.Dto;
using DAL.Models;

namespace DAL.MappingProfiles
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
