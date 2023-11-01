using Commons.Utilities;

namespace UnitTest.CommonTest
{
    public class MailUtilityUnitTest
    {

        [Fact]
        public async Task Send()
        {
            var status = await MailUtility.SendAsync();
            Assert.True(status);
        }   
    }
}
