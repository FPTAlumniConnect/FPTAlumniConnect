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
            ValidateCommentInfo(request);

            Comment newComment = _mapper.Map<Comment>(request);
            await _unitOfWork.GetRepository<Comment>().InsertAsync(newComment);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");
            return newComment.CommentId;
        }

        private void ValidateCommentInfo(CommentInfo request)
        {
            List<string> errors = new List<string>();

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Comment information cannot be null");
            }

            if (request.PostId == null)
            {
                errors.Add("PostId is required");
            }

            if (request.AuthorId == null)
            {
                errors.Add("AuthorId is required");
            }

            if (string.IsNullOrWhiteSpace(request.Content))
            {
                errors.Add("Content is required");
            }

            // Nếu Type được cung cấp, kiểm tra nó không được trống
            if (request.Type != null && string.IsNullOrWhiteSpace(request.Type))
            {
                errors.Add("Type cannot be empty if provided");
            }

            // ParentCommentId có thể null nên không cần validate

            if (errors.Any())
            {
                throw new BadHttpRequestException($"Validation failed: {string.Join(", ", errors)}");
            }
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

            comment.Content = string.IsNullOrEmpty(request.Content) ? comment.Content : request.Content;
            //comment.Type = string.IsNullOrEmpty(request.Type) ? comment.Type : request.Type;
            comment.UpdatedAt = DateTime.Now;
            comment.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Comment>().UpdateAsync(comment);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<CommentReponse>> ViewAllComment(CommentFilter filter, PagingModel pagingModel)
        {
            // Lấy danh sách comment từ repository
            var comments = await _unitOfWork.GetRepository<Comment>().GetPagingListAsync(
                selector: x => _mapper.Map<CommentReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
            );

            foreach (var commentResponse in comments.Items)
            {
                // Kiểm tra parentCommentId cho mỗi comment
                if (commentResponse.ParentCommentId.HasValue)
                {
                    // Tìm comment cha dựa trên parentCommentId
                    var parentComment = await _unitOfWork.GetRepository<Comment>()
                        .FindAsync(x => x.CommentId == commentResponse.ParentCommentId.Value);

                    // Kiểm tra xem comment cha có tồn tại không
                    if (parentComment == null)
                    {
                        throw new Exception($"Không tìm thấy comment [{commentResponse.CommentId}] có CommentId = {commentResponse.ParentCommentId}");
                    }

                    // Kiểm tra postId của comment cha và con
                    if (parentComment.PostId != commentResponse.PostId)
                    {
                        throw new Exception($"postId của [CommentId = {commentResponse.CommentId}](postId: {commentResponse.PostId}) không khớp với postId của comment cha [CommentId = {parentComment.CommentId}](postId: {parentComment.PostId})");

                    }
                }
            }
            return comments;
        }
    }
}
