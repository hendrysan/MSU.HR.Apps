using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IPayrollUnitTest
    {
        //private readonly IPayrollRepository payrollRepository;
        //private readonly IMailRepository mailRepository;
        //private readonly ConnectionContext _context;

        //public IPayrollUnitTest()
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



        //    _context = new ConnectionContext(dbOption, configuration);
        //    mailRepository = new MailRepository(configuration, _context);
        //    payrollRepository = new PayrollRepository(_context, mailRepository);
        //}

        //[Fact]
        //public async Task GeneratePayslip()
        //{
        //    var masterUser = await _context.MasterUsers.FirstOrDefaultAsync() ?? new();
        //    var result = await payrollRepository.GeneratePayslip(masterUser, 202312, Guid.NewGuid());

        //    Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        //}



    }
}
