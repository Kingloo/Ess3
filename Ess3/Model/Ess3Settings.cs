using System;
using System.Text;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;

namespace Ess3.Model
{
    public class Ess3Settings
    {
        private readonly string _awsAccessKey = string.Empty;
        public string AWSAccessKey => _awsAccessKey;

        private readonly string _awsSecretKey = string.Empty;
        public string AWSSecretKey => _awsSecretKey;
        
        private readonly RegionEndpoint _endpoint = RegionEndpoint.EUWest1;
        public RegionEndpoint Endpoint => _endpoint;

        private BasicAWSCredentials _credentials = null;
        public BasicAWSCredentials Credentials
        {
            get
            {
                if (_credentials == null)
                {
                    _credentials = new BasicAWSCredentials(AWSAccessKey, AWSSecretKey);
                }

                return _credentials;
            }
        }

        private AmazonS3Config _s3Config = null;
        public AmazonS3Config S3Config
        {
            get
            {
                if (_s3Config == null)
                {
                    _s3Config = new AmazonS3Config
                    {
                        RegionEndpoint = Endpoint
                    };
                }

                return _s3Config;
            }
        }

        // no field-caching to ensure we get a new one each time
        // because we "using" it, if cached it would throw ObjectDisposedException
        public IAmazonS3 Client => new AmazonS3Client(Credentials, S3Config);

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
            
            _awsAccessKey = awsAccessKey;
            _awsSecretKey = awsSecretKey;

            _endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
            sb.AppendLine($"Access Key: {AWSAccessKey}");
            sb.AppendLine($"Secret Key: {AWSSecretKey}");
            sb.AppendLine($"Region Endpoint: {Endpoint.DisplayName}");

            return sb.ToString();
        }
    }
}
