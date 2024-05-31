using System.Text;
using S3Service;
using S3Service.Helpers;

namespace TestS3
{
    class Program
    {
        static void Main()
        {
            MainAsync().Wait();
        }

        public static async Task MainAsync()
        {
            var awsConfig = await ConfigurationHelper.LoadAwsConfigurationAsync();
            var uploadUtils = new UploadUtils(awsConfig);

            var bucketName = "yuri.dev.s3";
            var key = "test-upload.txt";

            //Upload from buffer
            var buffer = Encoding.UTF8.GetBytes("Hello World!, S3");
            await uploadUtils.UploadFromBufferAsync(buffer, bucketName, key);
            Console.WriteLine("Uploading Completed");

            // Upload from stream
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes("Hello, S3 again!")))
            {
                await uploadUtils.UploadFromStreamAsync(stream, bucketName, key);
            }

            Console.WriteLine("Upload from stream completed.");

            // Generate presigned URL
            var presignedUrl = uploadUtils.GeneratePreSignedURL(bucketName, key, TimeSpan.FromMinutes(15));
            Console.WriteLine($"Presigned URL: {presignedUrl}");
        }
    }
}
