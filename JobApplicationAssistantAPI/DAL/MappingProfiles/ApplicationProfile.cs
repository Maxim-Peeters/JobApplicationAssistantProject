using AutoMapper;
using DAL.Dto;
using DAL.Models;

namespace DAL.MappingProfiles
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
