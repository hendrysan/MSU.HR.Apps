using Discord;
using Microsoft.AspNetCore.Http;
using Models.Entities;
using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<DefaultResponse> UploadAsync(MasterUser user, IFormFile file, DateTime documentDate, string? remarks = null);
        Task<DefaultResponse> ProocessDocumentAsync(MasterUser user, Guid documentId);
    }
}
