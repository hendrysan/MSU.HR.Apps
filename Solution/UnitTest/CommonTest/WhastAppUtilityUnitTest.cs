using Commons.Utilities;

namespace UnitTest.CommonTest
{
    public class WhastAppUtilityUnitTest
    {
        [Fact]
        public async Task Send()
        {
            var status = await WhatsAppUtility.SendAsync("6281281101180", "UnitTest");
            Assert.True(status == System.Net.HttpStatusCode.OK);
        }
    }
}
