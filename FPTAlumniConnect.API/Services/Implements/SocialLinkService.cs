using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SocialLink;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class SocialLinkService : BaseService<SocialLinkService>, ISocialLinkService
    {
        public SocialLinkService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<SocialLinkService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateSocialLink(SocialLinkInfo request)
        {
            var newSocialLink = _mapper.Map<SoicalLink>(request);
            newSocialLink.CreatedAt = DateTime.Now;
            newSocialLink.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            await _unitOfWork.GetRepository<SoicalLink>().InsertAsync(newSocialLink);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newSocialLink.Slid;
        }

        public async Task<GetSocialLinkResponse> GetSocialLinkById(int id)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            GetSocialLinkResponse result = _mapper.Map<GetSocialLinkResponse>(socialLink);
            return result;
        }

        public async Task<bool> UpdateSocialLink(int id, SocialLinkInfo request)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            socialLink.UserId = request.UserId ?? socialLink.UserId;
            socialLink.Link = request.Link ?? socialLink.Link;
            socialLink.UpdatedAt = DateTime.Now;
            socialLink.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<SoicalLink>().UpdateAsync(socialLink);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<IPaginate<GetSocialLinkResponse>> ViewAllSocialLinks(SocialLinkFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetSocialLinkResponse> response = await _unitOfWork.GetRepository<SoicalLink>().GetPagingListAsync(
                selector: x => _mapper.Map<GetSocialLinkResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }

        public async Task<bool> DeleteSocialLink(int id)
        {
            SoicalLink socialLink = await _unitOfWork.GetRepository<SoicalLink>().SingleOrDefaultAsync(
                predicate: x => x.Slid.Equals(id)) ??
                throw new BadHttpRequestException("SocialLinkNotFound");

            _unitOfWork.GetRepository<SoicalLink>().DeleteAsync(socialLink);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}