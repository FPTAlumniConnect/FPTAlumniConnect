using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.JobPost;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using System.Linq.Expressions;

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
            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.UserId)) ??
                throw new BadHttpRequestException("UserNotFound");

            MajorCode majorId = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(request.MajorId)) ??
                throw new BadHttpRequestException("MajorCodeNotFound");

            // Validate Job Title
            if (string.IsNullOrWhiteSpace(request.JobTitle))
                throw new BadHttpRequestException("Job title is required.");

            if (request.JobTitle.Length < 5 || request.JobTitle.Length > 100)
                throw new BadHttpRequestException("Job title must be between 5 and 100 characters.");

            // Validate Job Description
            if (string.IsNullOrWhiteSpace(request.JobDescription))
                throw new BadHttpRequestException("Job description is required.");

            if (request.JobDescription.Length < 20 || request.JobDescription.Length > 2000)
                throw new BadHttpRequestException("Job description must be between 20 and 2000 characters.");

            // Validate Salary
            if (request.MinSalary.HasValue && request.MinSalary.Value <= 0)
                throw new BadHttpRequestException("Minimum salary must be greater than 0.");

            if (request.MinSalary.HasValue && request.MaxSalary.HasValue &&
                request.MaxSalary.Value <= request.MinSalary.Value)
                throw new BadHttpRequestException("Maximum salary must be greater than minimum salary.");

            // Validate Email
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new BadHttpRequestException("Email is required.");

            if (!IsValidEmail(request.Email))
                throw new BadHttpRequestException("Invalid email format.");

            // Validate Status
            if (string.IsNullOrWhiteSpace(request.Status))
                throw new BadHttpRequestException("Status is required.");

            // Validate Time
            if (request.Time == default)
                throw new BadHttpRequestException("Time is required.");

            // Perform mapping and database operations
            JobPost newJobPost = _mapper.Map<JobPost>(request);
            await _unitOfWork.GetRepository<JobPost>().InsertAsync(newJobPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful)
                throw new BadHttpRequestException("Create job post failed.");

            return newJobPost.JobPostId;
        }

        // Helper method for email validation
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
            // Find the existing job post
            JobPost jobPost = await _unitOfWork.GetRepository<JobPost>().SingleOrDefaultAsync(
                predicate: x => x.JobPostId.Equals(id)) ??
                throw new BadHttpRequestException("JobPostNotFound");

            // Validate Job Title (if provided)
            if (!string.IsNullOrEmpty(request.JobTitle))
            {
                if (request.JobTitle.Length < 5 || request.JobTitle.Length > 100)
                    throw new BadHttpRequestException("Job title must be between 5 and 100 characters.");
                jobPost.JobTitle = request.JobTitle;
            }

            // Validate Job Description (if provided)
            if (!string.IsNullOrEmpty(request.JobDescription))
            {
                if (request.JobDescription.Length < 20 || request.JobDescription.Length > 2000)
                    throw new BadHttpRequestException("Job description must be between 20 and 2000 characters.");
                jobPost.JobDescription = request.JobDescription;
            }

            // Validate Requirements (if provided)
            if (!string.IsNullOrEmpty(request.Requirements))
            {
                if (request.Requirements.Length > 1000)
                    throw new BadHttpRequestException("Requirements cannot exceed 1000 characters.");
                jobPost.Requirements = request.Requirements;
            }

            // Validate Location (if provided)
            if (!string.IsNullOrEmpty(request.Location))
            {
                if (request.Location.Length > 100)
                    throw new BadHttpRequestException("Location cannot exceed 100 characters.");
                jobPost.Location = request.Location;
            }

            // Validate Benefits (if provided)
            if (!string.IsNullOrEmpty(request.Benefits))
            {
                if (request.Benefits.Length > 1000)
                    throw new BadHttpRequestException("Benefits cannot exceed 1000 characters.");
                jobPost.Benefits = request.Benefits;
            }

            // Validate Salary
            if (request.MinSalary.HasValue)
            {
                if (request.MinSalary.Value <= 0)
                    throw new BadHttpRequestException("Minimum salary must be greater than 0.");
                jobPost.MinSalary = request.MinSalary;
            }

            if (request.MaxSalary.HasValue)
            {
                if (request.MinSalary.HasValue && request.MaxSalary.Value <= request.MinSalary.Value)
                    throw new BadHttpRequestException("Maximum salary must be greater than minimum salary.");
                jobPost.MaxSalary = request.MaxSalary;
            }

            // Validate Time
            if (request.Time != default)
            {
                if (request.Time < DateTime.Now.AddYears(-1) || request.Time > DateTime.Now.AddYears(2))
                    throw new BadHttpRequestException("Invalid job post time.");
                jobPost.Time = request.Time;
            }

            // Validate Status
            if (!string.IsNullOrEmpty(request.Status))
            {
                // Add your specific status validation if needed
                jobPost.Status = request.Status;
            }

            // Validate Email (if provided)
            if (!string.IsNullOrEmpty(request.Email))
            {
                if (!IsValidEmail(request.Email))
                    throw new BadHttpRequestException("Invalid email format.");
                jobPost.Email = request.Email;
            }

            // Update other fields
            jobPost.MajorId = request.MajorId;
            jobPost.UpdatedAt = DateTime.Now;
            jobPost.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Perform update
            _unitOfWork.GetRepository<JobPost>().UpdateAsync(jobPost);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<JobPostResponse>> ViewAllJobPosts(JobPostFilter filter, PagingModel pagingModel)
        {
            // Validate salary range
            if (filter.MinSalary.HasValue && filter.MaxSalary.HasValue)
            {
                if (filter.MinSalary.Value < 0)
                {
                    throw new BadHttpRequestException("Minimum salary cannot be negative.");
                }

                if (filter.MaxSalary.Value < 0)
                {
                    throw new BadHttpRequestException("Maximum salary cannot be negative.");
                }

                if (filter.MinSalary.Value > filter.MaxSalary.Value)
                {
                    throw new BadHttpRequestException("Minimum salary cannot be greater than maximum salary.");
                }
            }

            // Build filter expression
            Expression<Func<JobPost, bool>> predicate = x =>
                (filter.UserId == null || x.UserId == filter.UserId) &&
                (filter.MajorId == null || x.MajorId == filter.MajorId) &&
                (filter.MinSalary == null || x.MaxSalary >= filter.MinSalary) &&
                (filter.MaxSalary == null || x.MinSalary <= filter.MaxSalary) &&
                (filter.IsDeal == null || x.IsDeal == filter.IsDeal) &&
                (string.IsNullOrEmpty(filter.Location) || x.Location.Contains(filter.Location)) &&
                (string.IsNullOrEmpty(filter.Status) || x.Status == filter.Status) &&
                (filter.Time == null || x.CreatedAt == filter.Time.Value.Date);
                //(filter.CreatedAt == null || x.CreatedAt == filter.CreatedAt.Value.Date);
                //(filter.UpdatedAt == null || x.UpdatedAt == filter.UpdatedAt.Value.Date);

            IPaginate<JobPostResponse> response = await _unitOfWork.GetRepository<JobPost>().GetPagingListAsync(
                selector: x => _mapper.Map<JobPostResponse>(x),
                predicate: predicate,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }

}
