using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using S3Service.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace S3Service
{
    public class UploadUtils
    {
        private readonly AmazonS3Client _s3Client;

        public UploadUtils(AwsConfiguration awsConfig)
        {
            var config = new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(awsConfig.Region)
            };
            _s3Client = new AmazonS3Client(awsConfig.AccessKeyId, awsConfig.SecretAccessKey, config);
        }

        // Upload to S3 using Buffer
        public async Task UploadFromBufferAsync(byte[] buffer, string bucketName, string key)
        {
            using (var stream = new MemoryStream(buffer))
            {
                var request = new TransferUtilityUploadRequest
                {
                    InputStream = stream,
                    BucketName = bucketName,
                    Key = key
                };

                var transferUtility = new TransferUtility(_s3Client);
                await transferUtility.UploadAsync(request);
            }
        }

        // Stream to S3bucket directly
        public async Task UploadFromStreamAsync(Stream stream, string bucketName, string key)
        {
            var request = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                BucketName = bucketName,
                Key = key
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);
        }

        // Get Pre-signed URL to download
        public string GeneratePreSignedURL(string bucketName, string key, TimeSpan expiryDuration)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = key,
                Expires = DateTime.UtcNow.Add(expiryDuration)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
