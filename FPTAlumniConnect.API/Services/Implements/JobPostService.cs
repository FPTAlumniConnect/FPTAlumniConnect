﻿using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobPostService : BaseService<JobPostService>, IJobPostService
    {
        public JobPostService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<JobPostService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewJobPost(JobPostInfo request)
        {
            JobPost newJobPost = _mapper.Map<JobPost>(request);
            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJobPost.JobPostId;
        }

        public async Task<JobPostResponse> GetJobPostById(int id)
        {
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            JobPostResponse result = _mapper.Map<JobPostResponse>(jobPost);
            return result;
        }

        public async Task<bool> UpdateJobPostInfo(int id, JobPostInfo request)
        {
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            jobPost.JobDescription = string.IsNullOrEmpty(request.JobDescription) ? jobPost.JobDescription : request.JobDescription;
            jobPost.Location = request.Location;
            jobPost.Requirements = request.Requirements;
            jobPost.Benefits = request.Benefits;
            jobPost.Time = request.Time;
            jobPost.Status = request.Status;
            jobPost.Email = request.Email;
            jobPost.MajorId = request.MajorId;
            jobPost.UpdatedAt = DateTime.Now;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            IPaginate<JobPostResponse> response = await _unitOfWork.GetRepository<JobPost>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }

}
