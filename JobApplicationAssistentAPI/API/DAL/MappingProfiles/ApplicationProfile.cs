using AutoMapper;
using API.DAL.Dto;
using API.DAL.Models;

namespace API.DAL.MappingProfiles
{
    public class JobProfile : Profile
    {
        public JobProfile()
        {
            CreateMap<JobRequest, Job>();
            CreateMap<Job, JobRequest>();
            CreateMap<Job, JobResponse>();
        }
    }
}
