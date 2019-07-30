using System;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ess3.Model
{
    public class Ess3Settings
    {
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

            credentials = new BasicAWSCredentials(awsAccessKey, awsSecretKey);

            S3Config = new AmazonS3Config
            {
                RegionEndpoint = endpoint
            };
        }

        public static bool TryParse(string rawJson, out Ess3Settings settings)
        {
            try
            {
                JObject json = JObject.Parse(rawJson);

                string accessKey = (string)json["AWSAccessKey"];
                string secretKey = (string)json["AWSSecretKey"];

                RegionEndpoint endpoint = RegionEndpoint.GetBySystemName((string)json["EndpointSystemName"]);

                settings = new Ess3Settings(accessKey, secretKey, endpoint);

                return true;
            }
            catch (JsonReaderException)
            {
                settings = null;

                return false;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().FullName);
            sb.AppendLine($"Region Endpoint: {S3Config.RegionEndpoint.DisplayName}");

            return sb.ToString();
        }
    }
}
