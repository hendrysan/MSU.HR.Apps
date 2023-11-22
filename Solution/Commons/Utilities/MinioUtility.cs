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
        private static async Task<IMinioClient> BuildMinioClinet(IConfigurationRoot config)
        {
            string endpoint = config["Minio:EndPoint"] ?? "";
            var accessKey = config["Minio:AccessKey"] ?? "";
            var secretKey = config["Minio:SecretKey"] ?? "";

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
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var bucketName = config["Minio:BucketName"] ?? "";//"test";
                var minio = await BuildMinioClinet(config);
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
                await Loggers.DiscordLogger.SendAsync("MinioUtility SendAsync", ex);
                return false;
            }
        }

        public static async Task<bool> RemoveAsync(string objectName)
        {
            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var bucketName = config["Minio:BucketName"] ?? "";//"test";
                var minio = await BuildMinioClinet(config);
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
                await Loggers.DiscordLogger.SendAsync("MinioUtility RemoveAsync", ex);
                return false;
            }
        }

        public static async Task<string> GetAsync(string objectName)
        {
            try
            {
                var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                var bucketName = config["Minio:BucketName"] ?? "";//"test";
                string endpoint = config["Minio:EndPoint"] ?? "";
                return $"http://{endpoint}/{bucketName}/{objectName}";
            }
            catch (MinioException ex)
            {
                await Loggers.DiscordLogger.SendAsync("MinioUtility GetAsync", ex);
                return string.Empty;
            }
        }

    }
}
