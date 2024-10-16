using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.MajorCode;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class MajorCodeService : BaseService<MajorCodeService>, IMajorCodeService
    {

        public MajorCodeService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<MajorCodeService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewMajorCode(MajorCodeInfo request)
        {
            MajorCode newMajorCode = _mapper.Map<MajorCode>(request);

            await _unitOfWork.GetRepository<MajorCode>().InsertAsync(newMajorCode);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newMajorCode.MajorId;
        }

        public async Task<MajorCodeReponse> GetMajorCodeById(int id)
        {
            MajorCode post = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException("MajorCodeNotFound");

            MajorCodeReponse result = _mapper.Map<MajorCodeReponse>(post);
            return result;
        }

        public async Task<bool> UpdateMajorCodeInfo(int id, MajorCodeInfo request)
        {
            MajorCode post = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
                predicate: x => x.MajorId.Equals(id)) ??
                throw new BadHttpRequestException("MajorCodeNotFound");

            post.MajorName = string.IsNullOrEmpty(request.MajorName) ? post.MajorName : request.MajorName;
            post.UpdatedAt = DateTime.Now;
            post.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<MajorCode>().UpdateAsync(post);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<MajorCodeReponse>> ViewAllMajorCode(MajorCodeFilter filter, PagingModel pagingModel)
        {
            IPaginate<MajorCodeReponse> response = await _unitOfWork.GetRepository<MajorCode>().GetPagingListAsync(
                selector: x => _mapper.Map<MajorCodeReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
