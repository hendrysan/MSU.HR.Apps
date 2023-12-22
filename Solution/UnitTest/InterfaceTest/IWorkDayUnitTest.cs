using DocumentFormat.OpenXml.Vml;
using Infrastructures;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repositories.Implements;
using Repositories.Interfaces;
using System.IO;

namespace UnitTest.InterfaceTest
{
    public class IWorkDayUnitTest
    {
        private readonly IWorkDayRepository workDayRepository;
        private readonly ConnectionContext _context;
        private readonly int timeSleep = 1000;

        public IWorkDayUnitTest()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            var dbOption = new DbContextOptionsBuilder<ConnectionContext>().Options;



            _context = new ConnectionContext(dbOption, configuration);
            workDayRepository = new WorkDayRepository(_context);
        }


        [Fact]
        public async Task UploadAsync()
        {

            var masterUser = await _context.MasterUsers.FirstAsync();

            string filePath = $"D:\\TemplateWorkDay.xlsx";

            FileInfo fileInfo = new FileInfo(filePath);
            string fileName = System.IO.Path.GetFileName(filePath);
            var mime = MimeMapping.MimeUtility.GetMimeMapping(fileName);
           
            var stream = File.OpenRead(path: filePath);
            var formFile = new FormFile(stream, 0, stream.Length, fileInfo.Name, System.IO.Path.GetFileName(filePath))
            {
               Headers =  new HeaderDictionary(),
               ContentType = mime
            };

            var result = await workDayRepository.UploadAsync(masterUser, formFile);
            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task GenerateFileTemplate()
        {
            var result = await workDayRepository.GenerateTemplateAsync();


            if (result.FileBytes == null)
                Assert.True(false);


            string location = $"D:\\{result.FileName}";

            if (File.Exists(location))
                File.Delete(location);

            using var writer = new BinaryWriter(File.OpenWrite(location));
            writer.Write(result.FileBytes);

            Assert.True(result.StatusCode == System.Net.HttpStatusCode.OK);
            Assert.True(File.Exists(location));
        }
    }
}
