using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.CV;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class CVService : BaseService<CVService>, ICVService
    {

        public CVService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<CVService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewCV(CVInfo request)
        {
            Cv cV = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(request.UserId)) ??
                throw new BadHttpRequestException("UserNotFound");

            Cv newCV = _mapper.Map<Cv>(request);

            await _unitOfWork.GetRepository<Cv>().InsertAsync(newCV);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newCV.Id;
        }

        public async Task<CVReponse> GetCVById(int id)
        {
            Cv cV = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("CVNotFound");

            CVReponse result = _mapper.Map<CVReponse>(cV);
            return result;
        }

        public async Task<CVReponse> GetCVByUserId(int id)
        {
            Cv cV = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id)) ??
                throw new BadHttpRequestException("CVNotFound");

            CVReponse result = _mapper.Map<CVReponse>(cV);
            return result;
        }

        public async Task<bool> UpdateCVInfo(int id, CVInfo request)
        {
            Cv cV = await _unitOfWork.GetRepository<Cv>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ??
                throw new BadHttpRequestException("CVNotFound");

            cV.FullName = string.IsNullOrEmpty(request.FullName) ? cV.FullName : request.FullName;
            cV.Address = string.IsNullOrEmpty(request.Address) ? cV.Address : request.Address;
            if (request.Birthday.HasValue)
            {
                cV.Birthday = request.Birthday.Value;
            }
            cV.Gender = string.IsNullOrEmpty(request.Gender) ? cV.Gender : request.Gender;
            cV.Email = string.IsNullOrEmpty(request.Email) ? cV.Email : request.Email;
            cV.Phone = string.IsNullOrEmpty(request.Phone) ? cV.Phone : request.Phone;
            cV.City = string.IsNullOrEmpty(request.City) ? cV.City : request.City;
            cV.Company = string.IsNullOrEmpty(request.Company) ? cV.Company : request.Company;
            cV.PrimaryDuties = string.IsNullOrEmpty(request.PrimaryDuties) ? cV.PrimaryDuties : request.PrimaryDuties;
            cV.JobLevel = string.IsNullOrEmpty(request.JobLevel) ? cV.JobLevel : request.JobLevel;
            if (request.StartAt.HasValue)
            {
                cV.StartAt = request.StartAt.Value;
            }
            if (request.EndAt.HasValue)
            {
                cV.EndAt = request.EndAt.Value;
            }
            cV.Language = string.IsNullOrEmpty(request.Language) ? cV.Language : request.Language;
            cV.LanguageLevel = string.IsNullOrEmpty(request.LanguageLevel) ? cV.LanguageLevel : request.LanguageLevel;
            cV.MinSalary = request.MinSalary;
            cV.MaxSalary = request.MaxSalary;
            if (request.IsDeal.HasValue)
            {
                cV.IsDeal = request.IsDeal.Value;
            }
            cV.UpdatedAt = DateTime.Now;
            cV.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Cv>().UpdateAsync(cV);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<CVReponse>> ViewAllCV(CVFilter filter, PagingModel pagingModel)
        {
            // Kiểm tra điều kiện thời gian
            if (filter.StartAt.HasValue)
            {
                if (filter.StartAt.Value > DateTime.UtcNow)
                {
                    throw new BadHttpRequestException("StartAt cannot be in the future.");
                }
            }

            if (filter.EndAt.HasValue)
            {
                if (filter.StartAt.HasValue && filter.EndAt.Value < filter.StartAt.Value)
                {
                    throw new BadHttpRequestException("EndAt cannot be earlier than StartAt.");
                }
            }

            // Thực hiện truy vấn sau khi kiểm tra hợp lệ
            IPaginate<CVReponse> response = await _unitOfWork.GetRepository<Cv>().GetPagingListAsync(
                selector: x => _mapper.Map<CVReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );
            return response;
        }
    }
}
