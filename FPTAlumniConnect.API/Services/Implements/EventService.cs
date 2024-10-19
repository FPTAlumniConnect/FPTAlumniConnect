using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload.Event;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Utils;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class EventService : BaseService<EventService>, IEventService
    {

        public EventService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<EventService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor, IFirebaseService firebaseService) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            
        }

        public async Task<int> CreateNewEvent(EventInfo request)
        {
            Event newEvent = _mapper.Map<Event>(request);

            await _unitOfWork.GetRepository<Event>().InsertAsync(newEvent);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newEvent.EventId;
        }

        public async Task<GetEventResponse> GetEventById(int id)
        {
            Event Event = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(id)) ??
                throw new BadHttpRequestException("EventNotFound");

            GetEventResponse result = _mapper.Map<GetEventResponse>(Event);
            return result;
        }

        public async Task<bool> UpdateEventInfo(int id, EventInfo request)
        {
            Event eventToUpdate = await _unitOfWork.GetRepository<Event>().SingleOrDefaultAsync(
                predicate: x => x.EventId.Equals(id))
                ?? throw new BadHttpRequestException("EventNotFound");

            eventToUpdate.EventName = string.IsNullOrEmpty(request.EventName) ? eventToUpdate.EventName : request.EventName;
            eventToUpdate.Img = string.IsNullOrEmpty(request.Img) ? eventToUpdate.Img : request.Img;
            eventToUpdate.Description = string.IsNullOrEmpty(request.Description) ? eventToUpdate.Description : request.Description;
            if (request.StartDate.HasValue)
            {
                eventToUpdate.StartDate = request.StartDate.Value;
            }

            if (request.EndDate.HasValue)
            {
                eventToUpdate.EndDate = request.EndDate.Value;
            }
            eventToUpdate.Location = string.IsNullOrEmpty(request.Location) ? eventToUpdate.Location : request.Location;


            _unitOfWork.GetRepository<Event>().UpdateAsync(eventToUpdate);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;

            return isSuccesful;
        }
    public async Task<IPaginate<GetEventResponse>> ViewAllEvent(EventFilter filter, PagingModel pagingModel)
        {
            IPaginate<GetEventResponse> response = await _unitOfWork.GetRepository<Event>().GetPagingListAsync(
                selector: x => _mapper.Map<GetEventResponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.EventId),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
    }
