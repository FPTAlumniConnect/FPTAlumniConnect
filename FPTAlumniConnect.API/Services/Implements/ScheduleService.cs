using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.Schedule;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Paginate;
using FPTAlumniConnect.DataTier.Repository.Interfaces;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class ScheduleService : BaseService<ScheduleService>, IScheduleService
    {

        public ScheduleService(IUnitOfWork<AlumniConnectContext> unitOfWork, ILogger<ScheduleService> logger, IMapper mapper,
            IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {

        }

        public async Task<int> CreateNewSchedule(ScheduleInfo request)
        {
            Schedule newSchedule = _mapper.Map<Schedule>(request);

            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            if (!isSuccessful) throw new BadHttpRequestException("CreateFailed");

            return newSchedule.ScheduleId;
        }

        public async Task<ScheduleReponse> GetScheduleById(int id)
        {
            Schedule post = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId.Equals(id)) ??
                throw new BadHttpRequestException("ScheduleNotFound");

            ScheduleReponse result = _mapper.Map<ScheduleReponse>(post);
            return result;
        }

        public async Task<bool> UpdateScheduleInfo(int id, ScheduleInfo request)
        {
            Schedule schedule = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                predicate: x => x.ScheduleId.Equals(id)) ??
                throw new BadHttpRequestException("ScheduleNotFound");
            if (request.MentorId.HasValue)
            {
                schedule.MentorId = request.MentorId.Value;
            }
            if (request.MentorShipId.HasValue)
            {
                schedule.MentorShipId = request.MentorShipId.Value;
            }
            schedule.Content = string.IsNullOrEmpty(request.Content) ? schedule.Content : request.Content;
            if (request.StartTime.HasValue)
            {
                schedule.StartTime = request.StartTime.Value;
            }
            if (request.EndTime.HasValue)
            {
                schedule.EndTime = request.EndTime.Value;
            }
            schedule.Status = string.IsNullOrEmpty(request.Status) ? schedule.Status : request.Status;
            schedule.UpdatedAt = DateTime.Now;
            schedule.UpdatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            bool isSuccesful = await _unitOfWork.CommitAsync() > 0;
            return isSuccesful;
        }

        public async Task<IPaginate<ScheduleReponse>> ViewAllSchedule(ScheduleFilter filter, PagingModel pagingModel)
        {
            IPaginate<ScheduleReponse> response = await _unitOfWork.GetRepository<Schedule>().GetPagingListAsync(
                selector: x => _mapper.Map<ScheduleReponse>(x),
                filter: filter,
                orderBy: x => x.OrderBy(x => x.CreatedAt),
                page: pagingModel.page,
                size: pagingModel.size
                );
            return response;
        }
    }
}
