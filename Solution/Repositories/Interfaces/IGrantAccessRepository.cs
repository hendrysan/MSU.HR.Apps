using Discord;
using Models.Requests;
using Models.Responses;
using static Models.Entities.EnumEntities;

namespace Repositories.Interfaces
{
    public interface IGrantAccessRepository
    {
        public Task<DefaultResponse> InititalAdminAsync();
        public Task<GrandAccessListResponse> ListAccess(string roleCode, EnumSource source);
        public Task<DataTableResponse> DataTableAsync(DataTableRequest request);
    }
}
