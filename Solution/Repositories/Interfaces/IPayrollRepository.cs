using Models.Entities;
using Models.Responses;

namespace Repositories.Interfaces
{
    public interface IPayrollRepository
    {
        public Task<DefaultResponse> PayrollInfo(MasterUser masterUser);
        public Task<DefaultResponse> PayslipPeriod(MasterUser masterUser, int year);
        public Task<DefaultResponse> GeneratePayslip(MasterUser masterUser, int yearMonth, Guid payslipId);
    }
}
