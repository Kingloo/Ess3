using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Ess3.Library.Interfaces;
using Ess3.Library.Model;

namespace Ess3.Library.S3
{
    public static class Delete
    {
        public static Task<bool> FileAsync(IAccount account, Ess3Bucket bucket, Ess3File file)
            => FileAsync(account, bucket, file, CancellationToken.None);

        public static async Task<bool> FileAsync(IAccount account, Ess3Bucket bucket, Ess3File file, CancellationToken token)
        {
            DeleteObjectRequest request = new DeleteObjectRequest
            {
                BucketName = bucket.BucketName,
                Key = file.Key
            };

            using IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), bucket.RegionEndpoint);

            DeleteObjectResponse? response = await Helpers
                .RunWithCatchAsync<DeleteObjectRequest, DeleteObjectResponse>(client.DeleteObjectAsync, request, token)
                .ConfigureAwait(false);

            return (response?.HttpStatusCode ?? HttpStatusCode.BadRequest) == HttpStatusCode.OK;
            // I don't know what status code translates to success
        }

        public static Task<bool> DirectoryAsync(IAccount account, Ess3Bucket bucket, Ess3Directory directory, bool failOnHasItems)
            => DirectoryAsync(account, bucket, directory, failOnHasItems, CancellationToken.None);

        public static async Task<bool> DirectoryAsync(IAccount account, Ess3Bucket bucket, Ess3Directory directory, bool failOnHasItems, CancellationToken token)
        {
            if (BucketHasItems(bucket) && failOnHasItems)
            {
                return false;
            }

            DeleteObjectsRequest request = CreateDirectoryDeletionRequest(bucket, directory);

            using IAmazonS3 client = new AmazonS3Client(account.GetCredentials(), bucket.RegionEndpoint);

            DeleteObjectsResponse? response = await Helpers
                .RunWithCatchAsync<DeleteObjectsRequest, DeleteObjectsResponse>(client.DeleteObjectsAsync, request, token)
                .ConfigureAwait(false);

            return (response?.HttpStatusCode ?? HttpStatusCode.BadRequest) == HttpStatusCode.OK;
            // I don't know which status code translates to success
        }

        private static bool BucketHasItems(Ess3Bucket bucket)
        {
            //return bucket.Directories.Count + bucket.Files.Count > 0;

            return bucket.Ess3Objects.Count > 0;
        }

        private static DeleteObjectsRequest CreateDirectoryDeletionRequest(Ess3Bucket bucket, Ess3Directory directory)
        {
            DeleteObjectsRequest request = new DeleteObjectsRequest
            {
                BucketName = bucket.BucketName
            };

            // delete the directory key itself
            request.AddKey(directory.Key);

            // delete every subdirectory key
            //foreach (Ess3Directory eachDirectory in bucket.Directories)
            //{
            //    request.AddKey(eachDirectory.Key);
            //}

            // delete every file key on the directory
            //foreach (Ess3File eachFile in bucket.Files)
            //{
            //    request.AddKey(eachFile.Key);
            //}

            foreach (Ess3Object each in bucket.Ess3Objects)
            {
                request.AddKey(each.Key);
            }

            return request;
        }
    }
}
