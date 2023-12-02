using Commons.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.CommonTest
{
    public class SecureUtilityUnitTest
    {
        [Fact]
        public async Task EncryptValue()
        {
            var encrypt = await SecureUtility.AesEncryptAsync("My name is Devon Johnathan Miller");
            Assert.NotNull(encrypt);
        }

        [Fact]
        public async Task DecryptValue()
        {
            var decrypt = await SecureUtility.AesDecryptAsync(@"D3TEVS/1kIzmakAxIYkL4zd8uQCIzmGfdabJRpPeikMkeGrJxKGYB5ctfAdUJnyu");
            Assert.NotNull(decrypt);
        }

    }
}
