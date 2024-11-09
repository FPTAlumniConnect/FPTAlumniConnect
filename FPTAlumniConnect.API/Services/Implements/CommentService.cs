using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Comment;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class CommentService : BaseService<CommentService>, ICommentService
    {

        public CommentService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<CommentService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewComment(CommentInfo request)
        {
            Comment newComment = _mapper.Map<Comment>(request);

            await _unitOfWork.GetRepository<Comment>().InsertAsync(newComment);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newComment.CommentId;
        }

        public async Task<CommentReponse> GetCommentById(int id)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id)) ??
                throw new BadHttpRequestException("CommentNotFound");

            CommentReponse result = _mapper.Map<CommentReponse>(comment);
            return result;
        }

        public async Task<bool> UpdateCommentInfo(int id, CommentInfo request)
        {
            Comment comment = await _unitOfWork.GetRepository<Comment>().SingleOrDefaultAsync(
                predicate: x => x.CommentId.Equals(id)) ??
                throw new BadHttpRequestException("CommentNotFound");
            if (request.PostId.HasValue)
            {
                comment.PostId = request.PostId.Value;
            }
            if (request.AuthorId.HasValue)
            {
                comment.AuthorId = request.AuthorId.Value;
            }
            if (request.ParentCommentId.HasValue)
            {
                comment.ParentCommentId = request.ParentCommentId.Value;
            }
            comment.Content = string.IsNullOrEmpty(request.Content) ? comment.Content : request.Content;
            comment.Type = string.IsNullOrEmpty(request.Type) ? comment.Type : request.Type;
            comment.UpdatedAt = DateTime.Now;
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            IPaginate<CommentReponse> response = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
                selector: x => _mapper.Map<CommentReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
