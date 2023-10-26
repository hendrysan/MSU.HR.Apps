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
        private readonly ConnectionContext _context;
        private int timeSleep = 1000;

        public IUserUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;

            _context = new ConnectionContext(dbOption, configuration);
            _service = new UserRepository(_context);
        }

        [Fact]
        public async Task Register()
        {
            var request = new RegisterRequest
            {
                Email = "test@gmail.com",
                Password = "123456",
                Name = "test"
            };

            var result = await _service.Register(request);
            Assert.True(result);
            Thread.Sleep(timeSleep);
        }

    }
}
