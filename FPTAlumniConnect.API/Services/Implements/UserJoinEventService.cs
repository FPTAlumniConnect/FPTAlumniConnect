using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.BusinessTier.Payload.UserJoinEvent;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class UserJoinEventService : BaseService<UserJoinEventService>, IUserJoinEventService
    {
        public UserJoinEventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<UserJoinEventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<int> CreateNewUserJoinEvent(UserJoinEventInfo request)
        {
            User userId = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
                predicate: x => x.UserId.Equals(request.UserId)) ??
                throw new BadHttpRequestException("UserNotFound");

            Event eventId = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(request.EventId)) ??
                throw new BadHttpRequestException("EventNotFound");

            if (GetUserJoinEventById(request.UserId) != null)
            {
                throw new BadHttpRequestException("This user already join!");
            }

            UserJoinEvent newJoinEvent = _mapper.Map<UserJoinEvent>(request);

            await _unitOfWork.GetRepository<UserJoinEvent>().InsertAsync(newJoinEvent);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newJoinEvent.Id;
        }

        public async Task<GetUserJoinEventResponse> GetUserJoinEventById(int id)
        {
            UserJoinEvent userJoinEvent = await _unitOfWork.GetRepository<UserJoinEvent>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            GetUserJoinEventResponse result = _mapper.Map<GetUserJoinEventResponse>(userJoinEvent);
            return result;
        }

        public async Task<bool> UpdateUserJoinEvent(int id, UserJoinEventInfo request)
        {
            UserJoinEvent userJoinEventToUpdate = await _unitOfWork.GetRepository<UserJoinEvent>().SingleOrDefaultAsync(
                predicate: x => x.Id.Equals(id)) ?? throw new BadHttpRequestException("UserJoinEventNotFound");

            userJoinEventToUpdate.Content = string.IsNullOrEmpty(request.Content) ? userJoinEventToUpdate.Content : request.Content;
            if (request.Rating.HasValue)
            {
                userJoinEventToUpdate.Rating = request.Rating.Value;
            }
            userJoinEventToUpdate.UpdatedAt = DateTime.Now;
            userJoinEventToUpdate.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;
            _unitOfWork.GetRepository<UserJoinEvent>().UpdateAsync(userJoinEventToUpdate);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            return isSuccessful;
        }

        public async Task<IPaginate<GetUserJoinEventResponse>> ViewAllUserJoinEvents(UserJoinEventFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetUserJoinEventResponse> response = await _unitOfWork.GetRepository<UserJoinEvent>().GetPagingListAsync(
                selector: x => _mapper.Map<GetUserJoinEventResponse>(x),
                  filter: filter,
                orderBy: x => x.OrderBy(x => x.Id),
                page: pagingModel.page,
                size: pagingModel.size
            );

            return response;
        }
    }
}
