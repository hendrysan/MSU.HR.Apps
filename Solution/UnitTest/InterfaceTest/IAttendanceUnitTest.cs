using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Implements;
using Repositories.Interfaces;
using System.IO;

namespace UnitTest.InterfaceTest
{
    public class IAttendanceUnitTest
    {
        //private readonly IUserRepository userRepository;
        private readonly IAttendanceRepository attendanceRepository;
        //private readonly IMailRepository mailRepository;
        private readonly IWorkDayRepository workDayRepository;
        private readonly ConnectionContext _context;

        public IAttendanceUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



            _context = new ConnectionContext(dbOption, configuration);
            //mailRepository = new MailRepository(configuration, _context);
            //userRepository = new UserRepository(_context, mailRepository);
            workDayRepository = new WorkDayRepository(_context);
            attendanceRepository = new AttendanceRepository(_context, workDayRepository);
        }

        [Fact]
        public async Task UploadFingerPrint()
        {
            var user = await _context.MasterUsers.FirstOrDefaultAsync();

            string filePath = "C:\\Users\\Hendry Priyatno\\Documents\\Project Ongoing\\MitraSolutechUtama\\20171205.Txt";

            using var stream = File.OpenRead(filePath);

            FileInfo fileInfo = new(filePath);
            string fileName = Path.GetFileName(filePath);
            var mime = MimeMapping.KnownMimeTypes.Text;


            var file = new FormFile(stream, 0, stream.Length, fileInfo.Name, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = mime
            };

            //IFormFile formFile = file;

            var result = await attendanceRepository.UploadAsync(user ?? new Models.Entities.MasterUser(), file, DateTime.Now, "Unit Test");

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }

        [Fact]
        public async Task ProccessDocumentFingerPrint()
        {
            var staging = await _context.StagingDocumentAttendances.FirstAsync();
            var user = await _context.MasterUsers.FirstAsync();
            Guid id = staging.Id;
            var result = await attendanceRepository.ProocessDocumentAsync(user, id);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);

        }
    }
}
