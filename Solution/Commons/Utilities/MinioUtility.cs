using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using System.Net.Http.Headers;
using System.Net.Mime;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Commons.Utilities
{
    public class MinioUtility
    {
        private static string GetConfig(string key)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var config = builder.Build();

            return config[key].ToString();
        }

        private static async Task<IMinioClient> BuildMinioClinet()
        {
            string endpoint = GetConfig("Minio:EndPoint");
            var accessKey = GetConfig("Minio:AccessKey");
            var secretKey = GetConfig("Minio:SecretKey");

            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
                                    .WithCredentials(accessKey, secretKey)
                                    .Build();

            return minio;
        }

        public static async Task<bool> SendAsync(string objectName, Stream streamObject, string contentType)
        {

            try
            {
                var bucketName = GetConfig("Minio:BucketName");//"test";
                var minio = await BuildMinioClinet();
                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                // Upload a file to bucket.
                var putObjectArgs = new PutObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName)
                    .WithStreamData(streamObject)
                    .WithObjectSize(streamObject.Length)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);

                return true;
            }
            catch (MinioException ex)
            {
                await Loggers.DiscordLogger.SendAsync("MinioUtility", ex);
                return false;
            }
        }

        public static async Task<bool> RemoveAsync(string objectName)
        {
            try
            {
                var bucketName = GetConfig("Minio:BucketName");//"test";
                var minio = await BuildMinioClinet();
                // Make a bucket on the server, if not already present.
                var beArgs = new BucketExistsArgs()
                    .WithBucket(bucketName);
                bool found = await minio.BucketExistsAsync(beArgs).ConfigureAwait(false);
                if (!found)
                {
                    var mbArgs = new MakeBucketArgs()
                        .WithBucket(bucketName);
                    await minio.MakeBucketAsync(mbArgs).ConfigureAwait(false);
                }
                var removeObjectArgs = new RemoveObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await minio.RemoveObjectAsync(removeObjectArgs);

                return true;
            }
            catch (MinioException ex)
            {
                await Loggers.DiscordLogger.SendAsync("MinioUtility", ex);
                return false;
            }
        }

        public static async Task<string> GetAsync(string objectName)
        {
            try
            {
                var bucketName = GetConfig("Minio:BucketName");//"test";
                string endpoint = GetConfig("Minio:EndPoint");
                return $"http://{endpoint}/{bucketName}/{objectName}";
            }
            catch (MinioException ex)
            {
                await Loggers.DiscordLogger.SendAsync("MinioUtility", ex);
                return string.Empty;
            }
        }

    }
}
