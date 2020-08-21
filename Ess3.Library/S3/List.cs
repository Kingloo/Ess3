using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Ess3.Library.Interfaces;
using Ess3.Library.Model;

namespace Ess3.Library.S3
{
    public static class List
    {
        public static Task<Ess3Bucket[]> BucketsAsync(IAccount account, RegionEndpoint regionEndpoint)
            => BucketsAsync(account, regionEndpoint, CancellationToken.None);

        public static async Task<Ess3Bucket[]> BucketsAsync(IAccount account, RegionEndpoint regionEndpoint, CancellationToken token)
        {
            ListBucketsRequest request = new ListBucketsRequest();

            using IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), regionEndpoint);

            ListBucketsResponse? response = await Helpers
                .RunWithCatchAsync<ListBucketsRequest, ListBucketsResponse>(client.ListBucketsAsync, request, token)
                .ConfigureAwait(false);

            if (response is null)
            {
                return Array.Empty<Ess3Bucket>();
            }

            return response.Buckets.Select(b => new Ess3Bucket(b)).ToArray();
        }
        
        public static Task<ListObjectsV2Response?> ObjectsAsync(IAccount account, Ess3Bucket bucket)
            => ObjectsAsync(account, bucket, CancellationToken.None);

        public static async Task<ListObjectsV2Response?> ObjectsAsync(IAccount account, Ess3Bucket bucket, CancellationToken token)
        {
            ListObjectsV2Request request = new ListObjectsV2Request
            {
                BucketName = bucket.BucketName,
            };

            using (IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), bucket.RegionEndpoint))
            {
                ListObjectsV2Response? response = await Helpers
                    .RunWithCatchAsync<ListObjectsV2Request, ListObjectsV2Response>(client.ListObjectsV2Async, request, token)
                    .ConfigureAwait(false);

                return response;
            }
        }
    }
}
