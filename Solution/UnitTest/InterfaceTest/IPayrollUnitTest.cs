using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IPayrollUnitTest
    {
        private readonly IPayrollRepository payrollRepository;
        private readonly IUserRepository userRepository;
        private readonly IMailRepository mailRepository;
        private readonly ConnectionContext? _context;
        private readonly int timeSleep = 1000;

        public IPayrollUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



            _context = new ConnectionContext(dbOption, configuration);
            mailRepository = new MailRepository(configuration, _context);
            userRepository = new UserRepository(_context, mailRepository);

            payrollRepository = new PayrollRepository(_context, mailRepository);
        }

        [Fact]
        public async Task GeneratePayslip()
        {
            var masterUser = await _context.MasterUsers.FirstOrDefaultAsync();
            var result = await payrollRepository.GeneratePayslip(masterUser, 202312, Guid.NewGuid());

            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }



    }
}
