using Discord;
using Models.Responses;
using static Models.Entities.EnumEntities;

namespace Repositories.Interfaces
{
    public interface IGrandAccessRepository
    {
        public Task<DefaultResponse> InititalAdminAsync();
        public Task<GrandAccessListResponse> ListAccess(string roleCode, EnumSource source);
    }
}
