﻿using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.PostReport;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PostReportService : BaseService<PostReportService>, IPostReportService
    {

        public PostReportService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<PostReportService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewReport(PostReportFilter request)
        {
            PostReport newRp = _mapper.Map<PostReport>(request);

            await _unitOfWork.GetRepository<PostReport>().InsertAsync(newRp);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newRp.RpId;
        }

        public async Task<PostReportReponse> GetReportById(int id)
        {
            PostReport rp = await _unitOfWork.GetRepository<PostReport>().SingleOrDefaultAsync(
                predicate: x => x.RpId.Equals(id)) ??
                throw new BadHttpRequestException("PostNotFound");

            PostReportReponse result = _mapper.Map<PostReportReponse>(rp);
            return result;
        }

        public async Task<IPaginate<PostReportReponse>> ViewAllReport(PostReportFilter filter, PagingModel pagingModel)
        {
            IPaginate<PostReportReponse> response = await _unitOfWork.GetRepository<Post>().GetPagingListAsync(
                selector: x => _mapper.Map<PostReportReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
