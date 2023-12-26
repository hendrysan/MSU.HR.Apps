using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IGrandAccessUnitTest
    {
        private readonly ConnectionContext _context;
        private readonly IGrandAccessRepository _grandAccessRepository;

        public IGrandAccessUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json")
              .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;

            _context = new ConnectionContext(dbOption, configuration);
            _grandAccessRepository = new GrandAccessRepository(_context);
        }

        [Fact]
        public async Task GenerateIntitalAdministrator()
        {
            var result = await _grandAccessRepository.InititalAdminAsync();
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task GetList()
        {
            var result = await _grandAccessRepository.ListAccess("Administrator", Models.Entities.EnumEntities.EnumSource.WebClient);
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }
    }
}
