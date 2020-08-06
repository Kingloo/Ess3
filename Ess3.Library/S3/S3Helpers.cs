using System;
using System.Linq;
using System.Net;
using System.Threading;
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

        public static Task<bool> ValidateAccountAsync(IAccount account, CancellationToken token)
            => ValidateAccountAsync(account, DefaultEndpoint, token);

        /// <summary>
        /// Validates whether an account's keys are valid credentials.
        /// If successful, it sets DisplayName and Id, and marks it as validated.
        /// </summary>
        /// <param name="account">The account object to validate.</param>
        /// <returns>An account is considered validated if the keys can list its buckets.</returns>
        public static async Task<bool> ValidateAccountAsync(IAccount account, RegionEndpoint regionEndpoint, CancellationToken token)
        {
            if (account is null) { throw new ArgumentNullException(nameof(account)); }

            bool isValidated = false;

            using (IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), regionEndpoint))
            {
                try
                {
                    var response = await RunWithoutCatchAsync<ListBucketsRequest, ListBucketsResponse>(client.ListBucketsAsync, new ListBucketsRequest(), token).ConfigureAwait(false);

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

        public static Task<Int64> GetBucketSizeAsync(IAccount account, string bucketName, CancellationToken token)
            => GetBucketSizeAsync(account, bucketName, DefaultEndpoint, token);

        /// <summary>
        /// Gets the total size of the objects in a bucket.
        /// </summary>
        /// <param name="account">The AWS account to query.</param>
        /// <param name="bucketName">The bucket for which to calculate total size.</param>
        /// <returns>-1 means an AmazonS3Exception was thrown.</returns>
        public static async Task<Int64> GetBucketSizeAsync(IAccount account, string bucketName, RegionEndpoint regionEndpoint, CancellationToken token)
        {
            if (account is null) { throw new ArgumentNullException(nameof(account)); }
            if (String.IsNullOrWhiteSpace(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            using IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), regionEndpoint);

            var request = new ListObjectsV2Request { BucketName = bucketName };

            ListObjectsV2Response? response = await RunWithCatchAsync<ListObjectsV2Request, ListObjectsV2Response>(client.ListObjectsV2Async, request, token).ConfigureAwait(false);

            return response?.S3Objects.Sum(o => o.Size) ?? -1L;
        }

        public static Task<string[]> GetBucketKeysAsync(IAccount account, string bucketName, CancellationToken token)
            => GetBucketKeysAsync(account, bucketName, DefaultEndpoint, token);

        /// <summary>
        /// Gets an array of all the keys in an S3 bucket.
        /// </summary>
        /// <param name="account">Account that owns the bucket.</param>
        /// <param name="bucketName">Bucket name for which to get the keys.</param>
        /// <param name="regionEndpoint">Endpoint the bucket resides in.</param>
        /// <param name="token">A cancellation token.</param>
        /// <returns>An array of all the S3 object keys, or an empty array if the call failed.</returns>
        public static async Task<string[]> GetBucketKeysAsync(IAccount account, string bucketName, RegionEndpoint regionEndpoint, CancellationToken token)
        {
            if (account is null) { throw new ArgumentNullException(nameof(account)); }
            if (String.IsNullOrWhiteSpace(bucketName)) { throw new ArgumentNullException(nameof(bucketName)); }

            using IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), regionEndpoint);

            var request = new ListObjectsV2Request { BucketName = bucketName };

            ListObjectsV2Response? response = await RunWithCatchAsync<ListObjectsV2Request, ListObjectsV2Response>(client.ListObjectsV2Async, request, token).ConfigureAwait(false);

            return response?.S3Objects.Select(o => o.Key).ToArray() ?? Array.Empty<string>();
        }

        private static async Task<TResponse> RunWithoutCatchAsync<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> s3Call,
            TRequest request,
            CancellationToken token)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse
        {
            return await s3Call.Invoke(request, token).ConfigureAwait(false);
        }

        private static async Task<TResponse?> RunWithCatchAsync<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> s3Call,
            TRequest request,
            CancellationToken token)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse
        {
            try
            {
                return await s3Call.Invoke(request, token).ConfigureAwait(false);
            }
            catch (AmazonS3Exception)
            {
                return null;
            }
        }

        private static async Task<TResponse?> RunWithCatchAndLogAsync<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> s3Call,
            TRequest request,
            Func<Exception, Task> logWriter,
            CancellationToken token)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse
        {
            try
            {
                return await s3Call.Invoke(request, token).ConfigureAwait(false);
            }
            catch (AmazonS3Exception ex)
            {
                await logWriter.Invoke(ex).ConfigureAwait(false);

                return null;
            }
        }
    }
}
