using Commons.Utilities;

namespace UnitTest.CommonTest
{
    public class MinioUtilityUnitTest
    {

        [Fact]
        public async Task Send()
        {
            string objectName = "Setting VPN v2.jpeg";
            string contentType = "application/JPEG";

            string filePath = "C:\\Users\\Hendry Priyatno\\Documents\\Project Ongoing\\BitHealth\\Setting VPN v2.jpeg";

            var bs = File.ReadAllBytes(filePath);

            using var ms = new MemoryStream(bs);
            var status = await MinioUtility.SendAsync(objectName, ms, contentType);
            Assert.True(status);

        }
    }
}
