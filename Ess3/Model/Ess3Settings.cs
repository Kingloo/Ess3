using System;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace Ess3.Model
{
    public class Ess3Settings
    {
        private readonly string awsAccessKey = string.Empty;
        private readonly string awsSecretKey = string.Empty;

        private readonly BasicAWSCredentials credentials = null;

        public AmazonS3Config S3Config { get; } = null;

        // no field-caching to ensure we get a new one each time
        // because we "using" it, if cached it would throw ObjectDisposedException
        public IAmazonS3 Client => new AmazonS3Client(credentials, S3Config);

        public Ess3Settings(string awsAccessKey, string awsSecretKey, RegionEndpoint endpoint)
        {
            if (String.IsNullOrWhiteSpace(awsAccessKey))
            {
                throw new ArgumentNullException(nameof(awsAccessKey));
            }

            if (String.IsNullOrWhiteSpace(awsSecretKey))
            {
                throw new ArgumentNullException(nameof(awsSecretKey));
            }

            if (endpoint is null)
            {
                throw new ArgumentNullException(nameof(endpoint));
            }

            this.awsAccessKey = awsAccessKey;
            this.awsSecretKey = awsSecretKey;

            credentials = new BasicAWSCredentials(this.awsAccessKey, this.awsSecretKey);
            S3Config = new AmazonS3Config { RegionEndpoint = endpoint };
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
            sb.AppendLine($"Access Key: {awsAccessKey}");
            sb.AppendLine($"Secret Key: {awsSecretKey}");
            sb.AppendLine($"Region Endpoint: {S3Config.RegionEndpoint.DisplayName}");

            return sb.ToString();
        }
    }
}
