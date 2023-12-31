using Microsoft.AspNetCore.Http;
using Models.Entities;
using Models.Requests;
using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IWorkDayRepository
    {
        public Task<DataTableResponse> DataTableAsync(DataTableRequest request);
        public Task<SearchWorkDayResponse> SearchAsync(string period);
        public Task<SearchWorkDayResponse> SearchAsync(DateTime date);
        public Task<SearchWorkDayResponse> SearchAsync(List<string> periods);
        public Task<SearchWorkDayResponse> SearchAsync(List<DateTime> dates);
        public Task<SearchWorkDayResponse> SearchAsync(DateTime start, DateTime end);
        public Task<DefaultResponse> GenerateDate(MasterUser masterUser, int year);
        public Task<DefaultResponse> UpdateDate(MasterUser masterUser, DateTime date, string remarks);
        public Task<DefaultResponse> DeleteDate(MasterUser masterUser, int id);
    }
}
