using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Request;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IDiscordUnitTest
    {
        private readonly IDiscordRepository _service;
        //private readonly ConnectionContext _context;
        private int timeSleep = 1000;

        public IDiscordUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;

            //_context = new ConnectionContext(dbOption, configuration);
            _service = new DiscordRepository();
        }

        [Fact]
        public async Task SendMessage()
        {
            
            var result = await _service.SendText("test");
            Assert.True(result);
            Thread.Sleep(timeSleep);
        }
    }
}
