using Microsoft.AspNetCore.Http;
using Models.Entities;
using Models.Requests;
using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IWorkDayRepository
    {
        //public Task<FileTemplateResponse> GenerateTemplateAsync();
        //public Task<DefaultResponse> UploadAsync(MasterUser masterUser, IFormFile file, string? remarks = null);
        public Task<DataTableResponse> DataTableAsync(DataTableRequest request);
        public Task<SearchWorkDayResponse> SearchAsync(string period);
        public Task<SearchWorkDayResponse> SearchAsync(DateTime date);
        public Task<SearchWorkDayResponse> SearchAsync(List<string> periods);
        public Task<SearchWorkDayResponse> SearchAsync(List<DateTime> dates);
        public Task<SearchWorkDayResponse> SearchAsync(DateTime start, DateTime end);
        public Task<DefaultResponse> GenerateDate(MasterUser masterUser, int year);
        public Task<DefaultResponse> UpdateDate(MasterUser masterUser, DateTime date, string value);
    }
}
