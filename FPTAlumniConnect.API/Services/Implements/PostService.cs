using AutoMapper;
using FPTAlumniConnect.API.Services.Implements.FPTAlumniConnect.API.Services.Implements;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Post;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PostService : BaseService<PostService>, IPostService
    {
        private readonly IMajorCodeService _majorCodeService;
        private readonly IUserService _userService;

        public PostService(
            IUnitOfWork<AlumniConnectContext> unitOfWork, 
            ILogger<PostService> logger, 
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IMajorCodeService majorCodeService,
            IUserService userService) : 
            base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _majorCodeService = majorCodeService;
            _userService = userService;
        }

        public async Task<int> CreateNewPost(PostInfo request)
        {
            // Check MajorId
            MajorCode checkMajorId = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
            predicate: s => s.MajorId == request.MajorId);
            if (checkMajorId == null)
            {
                throw new BadHttpRequestException("MajorIdNotFound");
            }

            // Check AuthorId
            User checkAuthorId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.AuthorId);
            if (checkAuthorId == null)
            {
                throw new BadHttpRequestException("AuthorIdNotFound");
            }

            Post newPost = _mapper.Map<Post>(request);

            await _unitOfWork.GetRepository<Post>().InsertAsync(newPost);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newPost.PostId;
        }

        public async Task<PostReponse> GetPostById(int id)
        {
            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id)) ??
                throw new BadHttpRequestException("PostNotFound");

            PostReponse result = _mapper.Map<PostReponse>(post);
            return result;
        }

        public async Task<bool> UpdatePostInfo(int id, PostInfo request)
        {
            Post post = await _unitOfWork.GetRepository<Post>().SingleOrDefaultAsync(
                predicate: x => x.PostId.Equals(id)) ??
                throw new BadHttpRequestException("PostNotFound");

            // Check MajorId
            MajorCode checkMajorId = await _unitOfWork.GetRepository<MajorCode>().SingleOrDefaultAsync(
            predicate: s => s.MajorId == request.MajorId);
            if (checkMajorId == null)
            {
                throw new BadHttpRequestException("MajorIdNotFound");
            }

            // Check AuthorId
            User checkAuthorId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: s => s.UserId == request.AuthorId);
            if (checkAuthorId == null)
            {
                throw new BadHttpRequestException("AuthorIdNotFound");
            }

            post.Title = string.IsNullOrEmpty(request.Title) ? post.Title : request.Title;
            post.Content = string.IsNullOrEmpty(request.Content) ? post.Content : request.Content;
            if (request.IsPrivate.HasValue)
            {
                post.IsPrivate = request.IsPrivate.Value;
            }
            if (request.MajorId.HasValue)
            {
                post.MajorId = request.MajorId.Value;
            }
            if (request.AuthorId.HasValue)
            {
                post.AuthorId = request.AuthorId.Value;
            }
            post.UpdatedAt = DateTime.Now;
            post.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Post>().UpdateAsync(post);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<PostReponse>> ViewAllPost(PostFilter filter, PagingModel pagingModel)
        {
            IPaginate<PostReponse> response = await _unitOfWork.GetRepository<Post>().GetPagingListAsync(
                selector: x => _mapper.Map<PostReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
