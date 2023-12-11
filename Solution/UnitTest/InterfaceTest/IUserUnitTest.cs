using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Request;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IUserUnitTest
    {
        private readonly IUserRepository _service;
        private readonly IMailRepository _mailRepository;

        //public IUserUnitTest(IUserRepository service, IMailRepository mailRepository)
        //{
        //    _service = service;
        //    _mailRepository = mailRepository;
        //}

        private readonly ConnectionContext? _context;
        private readonly int timeSleep = 1000;

        public IUserUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



            _context = new ConnectionContext(dbOption, configuration);
            _mailRepository = new MailRepository(_context);
            _service = new UserRepository(_context, _mailRepository);
        }

        [Fact]
        public async Task RegisterWithEmail()
        {
            var request = new RegisterRequest
            {
                RegisterVerify = RegisterVerify.Email,
                IdNumber = "123",
                UserInput = "hendry.priyatno@gmail.com",
                Password = "123456",
                FullName = "test"
            };

            var result = await _service.Register(request);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Created);
            Thread.Sleep(timeSleep);
        }

    }
}
