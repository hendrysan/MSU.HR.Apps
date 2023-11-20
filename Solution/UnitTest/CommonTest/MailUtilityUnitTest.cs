using Commons.Utilities;

namespace UnitTest.CommonTest
{
    public class MailUtilityUnitTest
    {

        [Fact]
        public async Task Send()
        {
            List<string> recipients = new List<string>();
            recipients.Add("hendry.priyatno@gmail.com");
            string subject = "test kirim email " + DateTime.Now.ToString();
            string body = "body content email";

            var status = await MailUtility.SendAsync(recipients, subject, body);
            Assert.True(status);
        }
    }
}
