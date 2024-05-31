using System;
using Microsoft.Extensions.Configuration;
using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon;
using S3Service.Configuration;
using S3Service.Constants;

namespace S3Service.Helpers
{
    public static class ConfigurationHelper
    {
        public static IConfigurationRoot LoadConfiguration()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        /** Load Aws Configuration based on the environment
         **** Remember to set up environment variables in launchSettings.json Or use cmd 'set ASPNETCORE_ENVIRONMENT=Staging'
         */
        public static async Task<AwsConfiguration> LoadAwsConfigurationAsync()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            if (environment == "Production")
            {
                return await LoadAwsConfigurationFromSecretsManagerAsync();
            }

            AwsConfiguration awsConfig = GetAwsConfiguration();
            return awsConfig;
        }

        /** Load Aws Configuration from the AWS Secret Manager
         **** Remember to set secret name in 'SecretManagerConstants'
         */
        private static async Task<AwsConfiguration> LoadAwsConfigurationFromSecretsManagerAsync()
        {
            AwsConfiguration awsConfig = GetAwsConfiguration();
            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(awsConfig.Region));

            var request = new GetSecretValueRequest
            {
                SecretId = SecretManagerConstants.AWS_SECRET_NAME
            };

            var response = await client.GetSecretValueAsync(request);

            if (response.SecretString != null)
            {
                return JsonSerializer.Deserialize<AwsConfiguration>(response.SecretString);
            }

            throw new Exception("Unable to load AWS configuration from Secrets Manager.");
        }

        // Get Aws configuration from configuration aka (appsettings.{env}.json)
        private static AwsConfiguration GetAwsConfiguration()
        {
            var configuration = LoadConfiguration();
            var awsConfig = new AwsConfiguration();
            configuration.GetSection("AWS").Bind(awsConfig);
            return awsConfig;
        }

    }
}
