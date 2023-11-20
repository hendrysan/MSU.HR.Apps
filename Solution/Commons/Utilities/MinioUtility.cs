using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace Commons.Utilities
{
    public class MinioUtility
    {

        private static async Task<IMinioClient> BuildMinioClinet()
        {
            string endpoint = "192.168.1.8:9100";
            var accessKey = "s7YywmlgTmvQENE959gu";
            var secretKey = "FFJgGUUrjvbAj97feOcVKnJ8SmBcySQo1c7iCLKK";

            IMinioClient minio = new MinioClient()
                                    .WithEndpoint(endpoint)
                                    .WithCredentials(accessKey, secretKey)
                                    .Build();

            return minio;
        }

        public static async Task<bool> SendAsync()
        {
            var bucketName = "test";
            var objectName = "Setting VPN v2.jpeg";
            var filePath = "C:\\Users\\Hendry Priyatno\\Documents\\Project Ongoing\\BitHealth\\Setting VPN v2.jpeg";
            var contentType = "application/JPEG";

            try
            {
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
                    .WithFileName(filePath)
                    .WithContentType(contentType);
                await minio.PutObjectAsync(putObjectArgs).ConfigureAwait(false);
                Console.WriteLine("Successfully uploaded " + objectName);

                return true;
            }
            catch (MinioException ex)
            {
                Console.WriteLine("File Upload Error: {0}", ex.Message);
                await Loggers.DiscordLogger.SendAsync("MinioUtility", ex);
                return false;
            }
        }
    }
}
