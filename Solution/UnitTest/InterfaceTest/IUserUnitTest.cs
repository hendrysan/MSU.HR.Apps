using Infrastructures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Requests;
using Repositories.Implements;
using Repositories.Interfaces;

namespace UnitTest.InterfaceTest
{
    public class IUserUnitTest
    {
        private readonly IUserRepository service;
        private readonly IMailRepository mailRepository;

        //public IUserUnitTest(IUserRepository service, IMailRepository mailRepository)
        //{
        //    service = service;
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
            mailRepository = new MailRepository(configuration, _context);
            service = new UserRepository(_context, mailRepository);
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

            var result = await service.Register(request);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Created);

        }

        [Fact]
        public async Task VerifyEmail()
        {
            string tokenSecure = @"s6ueGkK3gK1HJlIzM6LgRJN%2b1I6w0tnq6h6THLoN6GkGpUqq3XpVEWJAi2XffaFR";
            string requester = "hendry.priyatno@gmail.com";
            var result = await service.EmailVerify(tokenSecure, requester);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task VerifyPhoneNumber()
        {
            string tokenSecure = "A7AF";
            string requester = "6281281101180";
            string idNumber = "4267506";
            var result = await service.PhoneNumberVerify(tokenSecure, requester, idNumber);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterWithPhoneNumber()
        {
            var request = new RegisterRequest
            {
                RegisterVerify = RegisterVerify.PhoneNumber,
                IdNumber = "123",
                UserInput = "6281281101180",
                Password = "123456",
                FullName = "test"
            };

            var result = await service.Register(request);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task LoginWithIdNumber()
        {
            await Task.Delay(timeSleep * 10);
            var data = new LoginRequest
            {
                //LoginMethod = LoginMethod.IdNumber,
                Password = "123456",
                UserInput = "123"
            };

            var result = await service.Login(data);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoginWithEmail()
        {
            await Task.Delay(timeSleep * 10);
            var data = new LoginRequest
            {
                //LoginMethod = LoginMethod.Email,
                Password = "123456",
                UserInput = "hendry.priyatno@gmail.com"
            };

            var result = await service.Login(data);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task LoginWithPhoneNumber()
        {
            await Task.Delay(timeSleep * 10);
            var data = new LoginRequest
            {
                //LoginMethod = LoginMethod.PhoneNumber,
                Password = "123456",
                UserInput = "6281281101180"
            };

            var result = await service.Login(data);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task AllowLogin()
        {
            Guid userId = Guid.Parse("27263d2c-e82c-4f55-a070-f9cf6ba546b2");
            var result = await service.AllowLogin(userId, "123", true);

            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task CountDown()
        {
            string requester = "6281281101180";
            string idNumber = "4267506";

            
            var result = await service.CheckExpiredToken(requester, idNumber);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
        }
    }
}
