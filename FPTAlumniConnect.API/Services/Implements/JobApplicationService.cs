using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobApplication;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class JobApplicationService : BaseService<JobApplicationService>, IJobApplicationService
    {
        public JobApplicationService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<JobApplicationService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewJobApplication(JobApplicationInfo request)
        {
            // Check if the user already apply this job
            JobApplication existingJobApply = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: s => s.JobPostId == request.JobPostId && s.Cvid == request.Cvid);

            if (existingJobApply != null)
            {
                throw new BadHttpRequestException("Bạn đã nộp CV vào đây rồi!");
            }

            JobApplication newJobApplication = _mapper.Map<JobApplication>(request);
            await _unitOfWork.GetRepository<JobApplication>().InsertAsync(newJobApplication);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJobApplication.ApplicationId;
        }

        public async Task<JobApplicationResponse> GetJobApplicationById(int id)
        {
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            JobApplicationResponse result = _mapper.Map<JobApplicationResponse>(jobApplication);
            return result;
        }

        public async Task<bool> UpdateJobApplicationInfo(int id, JobApplicationInfo request)
        {
            JobApplication jobApplication = await _unitOfWork.GetRepository<JobApplication>().SingleOrDefaultAsync(
                predicate: x => x.ApplicationId.Equals(id)) ??
                throw new BadHttpRequestException("JobApplicationNotFound");

            jobApplication.JobPostId = request.JobPostId;
            jobApplication.Cvid = request.Cvid;
            jobApplication.LetterCover = request.LetterCover;
            jobApplication.Status = request.Status;
            jobApplication.Type = request.Type;
            jobApplication.UpdatedAt = DateTime.Now;
            jobApplication.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<JobApplication>().UpdateAsync(jobApplication);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<JobApplicationResponse>> ViewAllJobApplications(JobApplicationFilter filter, PagingModel pagingModel)
        {
            IPaginate<JobApplicationResponse> response = await _unitOfWork.GetRepository<JobApplication>().GetPagingListAsync(
                selector: x => _mapper.Map<JobApplicationResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }

}
