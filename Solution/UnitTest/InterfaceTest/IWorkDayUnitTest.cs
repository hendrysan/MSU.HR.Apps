using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Repositories.Implements;
using Repositories.Interfaces;
using System.Globalization;

namespace UnitTest.InterfaceTest
{
    public class IWorkDayUnitTest
    {
        private readonly IWorkDayRepository _workDayRepository;
        private readonly ConnectionContext _context;

        public IWorkDayUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



            _context = new ConnectionContext(dbOption, configuration);
            _workDayRepository = new WorkDayRepository(_context);
        }


        [Fact]
        public async Task UploadAsync()
        {

            var masterUser = await _context.MasterUsers.FirstAsync();

            string filePath = $"D:\\TemplateWorkDay2018.xlsx";

            FileInfo fileInfo = new(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);
            var mime = MimeMapping.MimeUtility.GetMimeMapping(fileName);

            var stream = File.OpenRead(path: filePath);
            var formFile = new FormFile(stream, 0, stream.Length, fileInfo.Name, System.IO.Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = mime
            };

            var result = await _workDayRepository.UploadAsync(masterUser, formFile);
            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);

        }

        [Fact]
        public async Task GenerateFileTemplate()
        {
            var result = await _workDayRepository.GenerateTemplateAsync();


            if (result.FileBytes == null)
                Assert.True(false);


            string location = $"D:\\{result.FileName}";

            if (File.Exists(location))
                File.Delete(location);

            using var writer = new BinaryWriter(File.OpenWrite(location));
            writer.Write(result.FileBytes);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(File.Exists(location));
        }

        [Fact]
        public async Task SearchByPeriod()
        {
            string period = "202302";
            var result = await _workDayRepository.SearchAsync(period);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.DetailWorkDays?.Count > 0);
        }

        [Fact]
        public async Task SearchByDate()
        {
            DateTime date = DateTime.ParseExact("20230206", "yyyyMMdd", CultureInfo.InvariantCulture);
            var result = await _workDayRepository.SearchAsync(date);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
            Assert.True(result.DetailWorkDays?.Count > 0);
        }

        [Fact]
        public async Task GenerateDate()
        {
            int year = 2018;
            MasterUser masterUser = await _context.MasterUsers.FirstAsync();

            var result = await _workDayRepository.GenerateDate(masterUser, year);

            Assert.Equal(System.Net.HttpStatusCode.OK, result.StatusCode);
        }
    }
}
