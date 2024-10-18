using FPTAlumniConnect.BusinessTier.Payload;
using FPTAlumniConnect.BusinessTier.Payload.GroupChat;
using FPTAlumniConnect.DataTier.Paginate;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IGroupChatService
    {
        Task<int> CreateGroupChat(GroupChatInfo request);
        Task<GroupChatInfo> GetGroupChatById(int id);
        Task<bool> UpdateGroupChat(int id, GroupChatInfo request);
        Task<IPaginate<GroupChatInfo>> ViewAllMessagesInGroupChat(GroupChatFilter filter, PagingModel pagingModel);
    }
}
