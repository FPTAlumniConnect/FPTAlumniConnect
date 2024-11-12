using AutoMapper;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;

namespace FPTAlumniConnect.API.Mappers
{
    public class JobPostModule : Profile
    {
        public JobPostModule()
        {
            CreateMap<JobPost, JobPostResponse>();
            CreateMap<JobPostInfo, JobPost>();
        }
    }
}
