using Commons.Loggers;
using System.Security.Cryptography;
using System.Text;

namespace Commons.Utilities
{
    public static class SecureUtility
    {
        private static readonly string key = "password";
        public static async Task<string> AesEncryptAsync(string value)
        {
            try
            {
                byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(value);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
                passwordBytes = SHA256.HashData(passwordBytes);
                byte[]? encryptedBytes = null;

                byte[] saltBytes = [2, 1, 7, 3, 6, 4, 8, 5];

                await Task.Run(() =>
                {
                    using MemoryStream ms = new();
#pragma warning disable SYSLIB0022 // Type or member is obsolete
                    using var AES = new RijndaelManaged();
#pragma warning restore SYSLIB0022 // Type or member is obsolete
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

#pragma warning disable SYSLIB0041 // Type or member is obsolete
                    Rfc2898DeriveBytes key = new(password: passwordBytes, salt: saltBytes, iterations: 1000);
#pragma warning restore SYSLIB0041 // Type or member is obsolete
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                });


                return Convert.ToBase64String(encryptedBytes);
            }
            catch (Exception ex)
            {
                await DiscordLogger.SendAsync("Common Encrypt", ex);
                throw;
            }
        }

        public static async Task<string> AesDecryptAsync(string value)
        {
            try
            {
                byte[] bytesToBeDecrypted = Convert.FromBase64String(value);
                byte[] passwordBytes = Encoding.UTF8.GetBytes(key);
                passwordBytes = SHA256.HashData(passwordBytes);

                byte[]? decryptedBytes = null;

                // Set your salt here, change it to meet your flavor:
                // The salt bytes must be at least 8 bytes.
                byte[] saltBytes = [2, 1, 7, 3, 6, 4, 8, 5];

                await Task.Run(() =>
                {
                    using MemoryStream ms = new();
#pragma warning disable SYSLIB0022 // Type or member is obsolete
                    using RijndaelManaged AES = new();
#pragma warning restore SYSLIB0022 // Type or member is obsolete
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

#pragma warning disable SYSLIB0041 // Type or member is obsolete
                    using (Rfc2898DeriveBytes key = new(passwordBytes, saltBytes, 1000))
                    {
                        AES.Key = key.GetBytes(AES.KeySize / 8);
                        AES.IV = key.GetBytes(AES.BlockSize / 8);
                    }
#pragma warning restore SYSLIB0041 // Type or member is obsolete

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                });

                return Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (Exception ex)
            {
                await DiscordLogger.SendAsync("Common Decrypt", ex);
                throw;
            }

        }
    }
}
