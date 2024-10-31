using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.SkillJob;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface ISkillService
    {
        Task<int> CreateNewSkill(SkillJobFilter request);
        Task<bool> UpdateSkillInfo(int id, SkillJobInfo request);
        Task<IPaginate<SkillJobReponse>> ViewAllSkill(SkillJobFilter filter, PagingModel pagingModel);
        Task<SkillJobReponse> GetSkillById(int id);
    }
}
