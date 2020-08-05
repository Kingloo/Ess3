using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Amazon.S3;
using Amazon.S3.Model;
using Ess3.Library.Interfaces;

namespace Ess3.Library.S3
{
    public static class S3Helpers
    {
        public static RegionEndpoint DefaultEndpoint { get; set; } = RegionEndpoint.EUWest1;

        public static Task<bool> ValidateAccountAsync(IAccount account)
            => ValidateAccountAsync(account, DefaultEndpoint);

        /// <summary>
        /// Validates whether an account's keys are valid credentials.
        /// If successful, it sets DisplayName and Id, and marks it as validated.
        /// </summary>
        /// <param name="account">The account object to validate.</param>
        /// <returns>An account is considered validated if the keys can list its buckets.</returns>
        public static async Task<bool> ValidateAccountAsync(IAccount account, RegionEndpoint endpoint)
        {
            if (account is null) { throw new ArgumentNullException(nameof(account)); }

            bool isValidated = false;

            var basicCreds = new BasicAWSCredentials(account.AWSAccessKey, account.AWSSecretKey);
            
            using (IAmazonS3 client = new AmazonS3Client(basicCreds, endpoint))
            {
                try
                {
                    ListBucketsResponse response = await client.ListBucketsAsync().ConfigureAwait(false);

                    isValidated = true;

                    account.DisplayName = response.Owner.DisplayName;
                    account.Id = response.Owner.Id;
                    account.IsValidated = true;
                }
                catch (AmazonS3Exception ex)
                    when (ex.InnerException is HttpErrorResponseException inner
                        && inner.Response.StatusCode == HttpStatusCode.Forbidden)
                { }
            }

            return isValidated;
        }

        public static Task<Int64> GetBucketSizeAsync(IAccount account, string bucketName)
            => GetBucketSizeAsync(account, bucketName, DefaultEndpoint);

        /// <summary>
        /// Gets the total size of the objects in a bucket.
        /// </summary>
        /// <param name="account">The AWS account to query.</param>
        /// <param name="bucketName">The bucket for which to calculate total size.</param>
        /// <returns>-1 means bucket name was null or empty. -2 means bucket does not exist.</returns>
        public static async Task<Int64> GetBucketSizeAsync(IAccount account, string bucketName, RegionEndpoint endpoint)
        {
            if (account is null) { throw new ArgumentNullException(nameof(account)); }

            if (String.IsNullOrWhiteSpace(bucketName))
            {
                return -1L;
            }

            Int64 size = 0L;

            var basicCreds = new BasicAWSCredentials(account.AWSAccessKey, account.AWSSecretKey);

            using (IAmazonS3 client = new AmazonS3Client(basicCreds, endpoint))
            {
                var request = new ListObjectsV2Request { BucketName = bucketName };

                try
                {
                    ListObjectsV2Response response = await client.ListObjectsV2Async(request).ConfigureAwait(false);

                    size = response.S3Objects.Sum(o => o.Size);
                }
                catch (AmazonS3Exception ex)
                    when (ex.InnerException is HttpErrorResponseException inner
                        && inner.Response.StatusCode == HttpStatusCode.NotFound)
                {
                    size = -2L;
                    // S3 answers NotFound even if the account's credentials are wrong
                    // it does NOT answer Forbidden like above
                }
            }

            return size;
        }
    }
}
