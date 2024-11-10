using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.EducationHistory;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EducationHistoryService : BaseService<EducationHistoryService>, IEducationHistoryService
    {
        public EducationHistoryService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EducationHistoryService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewEducationHistory(EducationHistoryInfo request)
        {
            // Kiểm tra xem Iduser có tồn tại trong bảng User không
            bool userExists = await _unitOfWork.GetRepository<User>().AsQueryableAsync(u => u.UserId == request.Iduser) != null;

            if (!userExists)
            {
                throw new BadHttpRequestException("User with the specified Id does not exist.");
            }

            // Ánh xạ request sang EducationHistory
            var newEducationHistory = _mapper.Map<EducationHistory>(request);

            // Thêm EducationHistory vào database
            await _unitOfWork.GetRepository<EducationHistory>().InsertAsync(newEducationHistory);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEducationHistory.EduHistoryId;
        }


        public async Task<GetEducationHistoryResponse> GetEducationHistoryById(int id)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id)) ??
                throw new BadHttpRequestException("EducationHistoryNotFound");

            GetEducationHistoryResponse result = _mapper.Map<GetEducationHistoryResponse>(educationHistory);
            return result;
        }

        public async Task<bool> UpdateEducationHistory(int id, EducationHistoryInfo request)
        {
            EducationHistory educationHistory = await _unitOfWork.GetRepository<EducationHistory>().SingleOrDefaultAsync(
                predicate: x => x.EduHistoryId.Equals(id)) ??
                throw new BadHttpRequestException("EducationHistoryNotFound");

            educationHistory.Name = string.IsNullOrEmpty(request.Name) ? educationHistory.Name : request.Name;
            educationHistory.ReceivedAt = request.ReceivedAt;
            educationHistory.UpdatedAt = DateTime.Now;
            educationHistory.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<EducationHistory>().UpdateAsync(educationHistory);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetEducationHistoryResponse>> ViewAllEducationHistory(EducationHistoryFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetEducationHistoryResponse> response = await _unitOfWork.GetRepository<EducationHistory>().GetPagingListAsync(
                selector: x => _mapper.Map<GetEducationHistoryResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.ReceivedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
