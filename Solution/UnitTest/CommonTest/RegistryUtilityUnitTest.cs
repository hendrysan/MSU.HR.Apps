using Commons.Loggers;
using Microsoft.Win32;

namespace UnitTest.CommonTest
{
    public class RegistryUtilityUnitTest
    {

        [Fact]
        public async Task GetEnvironmentOnWindows()
        {
            // Arrange
            var serviceName = "UnitTest";
            var message = "UnitTest with file";

            var env = Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine);

            var val = env["MSU_HRIS_PostgreSQLConnection1"];
        }


        [Fact]
        public async Task GetRegistryOnWindows()
        {
            // Arrange
            var serviceName = "UnitTest";
            var message = "UnitTest with file";
            //var query = JsonSerializer.Deserialize<object>(Body());

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\MSU_HRIS"))
            {
                if (key != null)
                {
                    var val = key.GetValue("PostgreSQLConnection");
                    Console.WriteLine(val);
                }
            }

            //Environment.SetEnvironmentVariable("PostgreSQLConnection", "Host=103.171.164.79;Database=dbhris;Username=postgres;Password=Awasekagi123");

            //var data = Environment.GetEnvironmentVariable("PostgreSQLConnection");

            //// Act
            //var status = await DiscordLogger.SendAsync(serviceName, message, query);

            // Assert
            //Assert.True(status == System.Net.HttpStatusCode.OK || status == System.Net.HttpStatusCode.NoContent);
        }
    }
}
