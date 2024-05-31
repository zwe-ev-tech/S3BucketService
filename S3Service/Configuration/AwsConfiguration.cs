using System;
using System.Collections.Generic;
using System.Text;

namespace S3Service.Configuration
{
    public class AwsConfiguration
    {
        public string AccessKeyId { get; set; }
        public string SecretAccessKey { get; set; }
        public string  Region { get; set; }
    }
}
