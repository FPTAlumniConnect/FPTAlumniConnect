using AutoMapper;
using Azure.Messaging;
using FirebaseAdmin.Auth;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.User;
using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class UserService : BaseService<UserService>, IUserService
    {
        private readonly IFirebaseService _firebaseService;

        public UserService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<UserService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _firebaseService = firebaseService;
        }

        public async Task<int> CreateNewUser(UserInfo request)
        {
            User newUser = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Email.Equals(request.Email)
                );

            if (newUser != null) throw new BadHttpRequestException("UserExisted");

            newUser = _mapper.Map<User>(request);

            await _unitOfWork.GetRepository<User>().InsertAsync(newUser);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newUser.UserId;
        }

        public async Task<GetUserResponse> GetUserById(int id)
        {
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id)) ??
                throw new BadHttpRequestException("UserNotFound");

            GetUserResponse result = _mapper.Map<GetUserResponse>(user);
            return result;
        }


        public async Task<LoginResponse> LoginUser(LoginFirebaseRequest request)
        {
            var cred = await _firebaseService.VerifyIdToken(request.Token);
            if (cred == null) throw new BadHttpRequestException("Firebase token không hợp lệ");
            var firebaseClaims = cred.Claims;

            var email = firebaseClaims.FirstOrDefault(c => c.Key == "email").Value.ToString();
            var uid = firebaseClaims.FirstOrDefault(c => c.Key == "user_id").Value.ToString();

            User userLogin = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.Email.Equals(email)
                );

            string accessToken;
            if (userLogin != null)
            {
                userLogin.GoogleId = uid;
                _unitOfWork.GetRepository<User>().UpdateAsync(userLogin);
                accessToken = JwtUtil.GenerateJwtToken(userLogin);
                await _unitOfWork.CommitAsync();
            }
            else throw new BadHttpRequestException("UserNotFound");

            return new LoginResponse
            {
                Message = "Login success",
                AccessToken = accessToken,
                UserInfo = new UserResponse()
                {
                    UserId = userLogin.UserId,
                    FirstName = userLogin.FirstName,
                    LastName = userLogin.LastName,
                    Email = userLogin.Email,
                    GoogleId = userLogin.GoogleId,
                }
            };
        }

        public async Task<bool> UpdateUserInfo(int id, UserInfo request)
        {
            // Fetch the user based on the provided ID.
            User user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(id)) ??
                throw new BadHttpRequestException("UserNotFound");

            // Check if the logged-in user is an admin or staff
            var loggedInUserRole = _httpContextAccessor.HttpContext?.User.FindFirst("Role")?.Value; // Assuming Role is stored as a claim
            if (loggedInUserRole != null && (loggedInUserRole == "admin" || loggedInUserRole == "staff"))
            {
                user.IsMentor = request.IsMentor;

            }

            // Update other fields
            user.FirstName = string.IsNullOrEmpty(request.FirstName) ? user.FirstName : request.FirstName;
            user.Email = string.IsNullOrEmpty(request.Email) ? user.Email : request.Email;
            user.LastName = string.IsNullOrEmpty(request.LastName) ? user.LastName : request.LastName;

            user.UpdatedAt = DateTime.Now;
            user.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // Perform the update in the repository
            _unitOfWork.GetRepository<User>().UpdateAsync(user);

            // Commit the changes
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;

            return isSuccesful;
        }

        public async Task<IPaginate<GetUserResponse>> ViewAllUser(UserFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetUserResponse> response = await _unitOfWork.GetRepository<User>().GetPagingListAsync(
                selector: x => _mapper.Map<GetUserResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.Email),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
