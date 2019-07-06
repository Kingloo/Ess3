using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3.Util;
using Ess3.Common;

namespace Ess3.Model
{
    public static class S3
    {
        // AWS S3 enforced limit, correct as of April 2017
        private static readonly Int64 singleCopyMaximumAllowableSize = Convert.ToInt64(Math.Pow(2d, 30d) * 5d); // 2^30 => giga, (* 5) => 5 GiB

        // chose a smaller size anyway, for performance reasons
        private static readonly Int64 multipartCopyWhenGreaterThanSize = Convert.ToInt64(Math.Pow(2d, 20d) * 250d); // 2^20 => mega, (* 250) => 250 MiB


        public static Task<IEnumerable<Ess3Bucket>> ListAllBucketsAsync(Ess3Settings settings)
            => ListAllBucketsAsync(settings, CancellationToken.None);

        public static async Task<IEnumerable<Ess3Bucket>> ListAllBucketsAsync(Ess3Settings settings, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (token == null) { token = CancellationToken.None; }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    ListBucketsResponse resp = await client.ListBucketsAsync(new ListBucketsRequest(), token).ConfigureAwait(false);

                    if (resp.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return resp.Buckets.Select(x => new Ess3Bucket(x));
                    }
                    else
                    {
                        await Log.MessageAsync($"List all buckets failed ({resp.HttpStatusCode.ToString()})").ConfigureAwait(false);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return Enumerable.Empty<Ess3Bucket>();
        }
        
        public static async Task<IEnumerable<Ess3Object>> ListAllObjectsForBucketAsync(Ess3Settings settings, Ess3Bucket bucket, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (bucket is null) { throw new ArgumentNullException(nameof(bucket)); }
            if (token == null) { token = CancellationToken.None; }

            ListObjectsRequest req = new ListObjectsRequest
            {
                BucketName = bucket.BucketName
            };

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    ListObjectsResponse resp = await client.ListObjectsAsync(req, token).ConfigureAwait(false);

                    if (resp.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return resp.S3Objects.Select(x => Ess3ObjectFactory.Create(x, bucket));
                    }
                    else
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "List all objects in {0} failed: {1}", bucket.BucketName, resp.HttpStatusCode.ToString());

                        await Log.MessageAsync(message).ConfigureAwait(false);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return Enumerable.Empty<Ess3Object>();
        }

        public static async Task<IEnumerable<Ess3Object>> ListAllObjectsForDirectoryAsync(Ess3Settings settings, Ess3Directory directory, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }
            if (token == null) { token = CancellationToken.None; }

            ListObjectsRequest req = new ListObjectsRequest
            {
                BucketName = directory.Bucket.BucketName,
                Prefix = directory.Key
            };

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    ListObjectsResponse resp = await client.ListObjectsAsync(req, token).ConfigureAwait(false);

                    if (resp.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return resp.S3Objects.Select(x => Ess3ObjectFactory.Create(x, directory.Bucket));
                    }
                    else
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "list objects in {0} failed: {1}", directory.Key, resp.HttpStatusCode.ToString());

                        await Log.MessageAsync(message).ConfigureAwait(false);
                    }
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return Enumerable.Empty<Ess3Object>();
        }

        public static async Task<HttpStatusCode> CreateDirectoryAsync(Ess3Settings settings, PutObjectRequest request, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            if (token == null) { token = CancellationToken.None; }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    PutObjectResponse resp = await client.PutObjectAsync(request, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }
            
            return HttpStatusCode.NotImplemented;
        }

        //public static async Task<HttpStatusCode> DeleteDirectoryAsync(Ess3Settings settings, Ess3Directory directory, bool isDirectoryEmpty, CancellationToken token)
        //{
        //    if (settings == null) { throw new ArgumentNullException(nameof(settings)); }
        //    if (directory == null) { throw new ArgumentNullException(nameof(directory)); }
        //    if (token == null) { token = CancellationToken.None; }

        //    if (isDirectoryEmpty)
        //    {
        //        DeleteObjectRequest req = new DeleteObjectRequest
        //        {
        //            BucketName = directory.Bucket.BucketName,
        //            Key = directory.Key
        //        };

        //        return await DeleteObjectAsync(settings, req, token).ConfigureAwait(false);
        //    }
        //    else
        //    {
        //        List<KeyVersion> toDelete = directory.Ess3Objects
        //            .Select(x => new KeyVersion { Key = x.Key })
        //            .ToList();

        //        toDelete.Add(new KeyVersion { Key = directory.Key });

        //        DeleteObjectsRequest req = new DeleteObjectsRequest
        //        {
        //            BucketName = directory.Bucket.BucketName,
        //            Objects = toDelete
        //        };

        //        return await DeleteObjectsAsync(settings, req, token).ConfigureAwait(false);
        //    }
        //}

        public static async Task<HttpStatusCode> DeleteDirectoryAsync(Ess3Settings settings, Ess3Directory directory, bool failOnDirectoryNotEmpty, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (directory is null) { throw new ArgumentNullException(nameof(directory)); }
            if (token == null) { token = CancellationToken.None; }

            if (directory.Ess3Objects.Count > 0)
            {
                if (failOnDirectoryNotEmpty)
                {
                    return HttpStatusCode.NotModified;
                }
            }

            DeleteObjectsRequest request = new DeleteObjectsRequest
            {
                BucketName = directory.Bucket.BucketName
            };

            request.AddKey(directory.Key);

            foreach (Ess3Object each in directory.Ess3Objects)
            {
                request.AddKey(each.Key);
            }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    DeleteObjectsResponse resp = await client.DeleteObjectsAsync(request, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);

                    return HttpStatusCode.NotImplemented;
                }
            }
        }

        public static Task<HttpStatusCode> DeleteFileAsync(Ess3Settings settings, Ess3File file, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (file is null) { throw new ArgumentNullException(nameof(file)); }
            if (token == null) { token = CancellationToken.None; }

            DeleteObjectRequest req = new DeleteObjectRequest
            {
                BucketName = file.Bucket.BucketName,
                Key = file.Key
            };

            return DeleteObjectAsync(settings, req, token);
        }

        public static async Task<HttpStatusCode> DownloadFileAsync(Ess3Settings settings, Ess3File file, string localFilePath, CancellationToken token, EventHandler<WriteObjectProgressArgs> progressHandler)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (file is null) { throw new ArgumentNullException(nameof(file)); }
            if (token == null) { token = CancellationToken.None; }

            GetObjectRequest req = new GetObjectRequest
            {
                BucketName = file.Bucket.BucketName,
                Key = file.Key
            };

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    GetObjectResponse response = await client.GetObjectAsync(req, token).ConfigureAwait(false);

                    if (response.HttpStatusCode == HttpStatusCode.OK)
                    {
                        if (progressHandler != null) { response.WriteObjectProgressEvent += progressHandler; }

                        await response.WriteResponseStreamToFileAsync(localFilePath, false, token).ConfigureAwait(false);
                    }
                    else
                    {
                        string message = string.Format(CultureInfo.CurrentCulture, "getting object {0} failed: {1}", file.Key, response.HttpStatusCode.ToString());

                        await Log.MessageAsync(message).ConfigureAwait(false);
                    }

                    return response.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
                catch (IOException ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);

                    return HttpStatusCode.InternalServerError;
                }
            }
            
            return HttpStatusCode.NotImplemented;
        }

        public static async Task<HttpStatusCode> UploadFileInPartsAsync(Ess3Settings settings, string localFilePath, string bucketName, string prefix, CancellationToken token, EventHandler<UploadProgressArgs> progressHandler)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (token == null) { token = CancellationToken.None; }

            TransferUtilityUploadRequest req = new TransferUtilityUploadRequest
            {
                AutoCloseStream = true,
                BucketName = bucketName,
                Key = string.Concat(prefix, GetFileName(localFilePath)),
                FilePath = localFilePath,
                StorageClass = S3StorageClass.Standard
            };

            if (progressHandler != null) { req.UploadProgressEvent += progressHandler; }

            TransferUtilityConfig transferUtilConfig = new TransferUtilityConfig
            {
                MinSizeBeforePartUpload = 1024 * 1024 * 50, // 50 mebibytes
                ConcurrentServiceRequests = 1
            };

            using (TransferUtility util = new TransferUtility(settings.Client, transferUtilConfig))
            {
                try
                {
                    await util.UploadAsync(req, token).ConfigureAwait(false);
                    
                    return HttpStatusCode.OK;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);

                    return HttpStatusCode.NotImplemented;
                }
                catch (IOException ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);

                    return HttpStatusCode.InternalServerError;
                }
            }
        }

        public static async Task<S3AccessControlList> GetACLAsync(Ess3Settings settings, Ess3Object obj, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (obj is null) { throw new ArgumentNullException(nameof(obj)); }
            if (token == null) { token = CancellationToken.None; }

            GetACLRequest req = new GetACLRequest
            {
                BucketName = obj.Bucket.BucketName,
                Key = obj.Key
            };

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    GetACLResponse resp = await client.GetACLAsync(req, token).ConfigureAwait(false);

                    return resp.AccessControlList;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return null;
        }

        public static async Task<HttpStatusCode> SetACLAsync(Ess3Settings settings, Ess3Object obj, S3CannedACL cannedACL, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (obj is null) { throw new ArgumentNullException(nameof(obj)); }
            if (cannedACL is null) { throw new ArgumentNullException(nameof(cannedACL)); }
            if (token == null) { token = CancellationToken.None; }

            PutACLRequest req = new PutACLRequest
            {
                BucketName = obj.Bucket.BucketName,
                Key = obj.Key,
                CannedACL = cannedACL
            };

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    PutACLResponse resp = await client.PutACLAsync(req, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }
            
            return HttpStatusCode.NotImplemented;
        }

        public static void SetS3StorageClass(Ess3Settings settings, Ess3File file, S3StorageClass storageClass)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (file is null) { throw new ArgumentNullException(nameof(file)); }
            if (storageClass is null) { throw new ArgumentNullException(nameof(storageClass)); }

            using (IAmazonS3 client = settings.Client)
            {
                AmazonS3Util.SetObjectStorageClass(client, file.Bucket.BucketName, file.Key, storageClass);
            }
        }


        private static async Task<HttpStatusCode> DeleteObjectAsync(Ess3Settings settings, DeleteObjectRequest deleteRequest, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (deleteRequest is null) { throw new ArgumentNullException(nameof(deleteRequest)); }
            if (token == null) { token = CancellationToken.None; }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    DeleteObjectResponse resp = await client.DeleteObjectAsync(deleteRequest, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }
            
            return HttpStatusCode.NotImplemented;
        }

        private static async Task<HttpStatusCode> DeleteObjectsAsync(Ess3Settings settings, DeleteObjectsRequest deleteRequest, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (deleteRequest is null) { throw new ArgumentNullException(nameof(deleteRequest)); }
            if (token == null) { token = CancellationToken.None; }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    DeleteObjectsResponse resp = await client.DeleteObjectsAsync(deleteRequest, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }
            
            return HttpStatusCode.NotImplemented;
        }

        private static string GetFileName(string localFilePath)
        {
            string[] segments = localFilePath.Split(new string[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);

            return segments.Last();
        }


        private static async Task<HttpStatusCode> CopyObjectAsync(Ess3Settings settings, CopyObjectRequest copyRequest, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (copyRequest is null) { throw new ArgumentNullException(nameof(copyRequest)); }
            if (token == null) { token = CancellationToken.None; }

            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    CopyObjectResponse resp = await client.CopyObjectAsync(copyRequest, token).ConfigureAwait(false);

                    return resp.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return HttpStatusCode.NotImplemented;
        }

        private static async Task<HttpStatusCode> CopyObjectInPartsAsync(Ess3Settings settings, InitiateMultipartUploadRequest initiateRequest, Int64 fileSize, CancellationToken token)
        {
            if (settings is null) { throw new ArgumentNullException(nameof(settings)); }
            if (initiateRequest is null) { throw new ArgumentNullException(nameof(initiateRequest)); }
            if (token == null) { token = CancellationToken.None; }

            Debug.WriteLine($"copying {initiateRequest.Key} in parts...");

            var uploadResponses = new List<UploadPartResponse>();
            
            using (IAmazonS3 client = settings.Client)
            {
                try
                {
                    var initiateResponse = await client.InitiateMultipartUploadAsync(initiateRequest, token).ConfigureAwait(false);

                    if (initiateResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        return initiateResponse.HttpStatusCode;
                    }

                    IEnumerable<CopyPartRequest> copyRequests = CreateCopyPartRequests(initiateRequest, initiateResponse.UploadId, fileSize);

                    var copyPartTasks = copyRequests
                        .Select(x => client.CopyPartAsync(x, token))
                        .ToList();

                    Debug.WriteLine($"copying in {copyPartTasks.Count} parts");

                    CopyPartResponse[] copyResponses = await Task.WhenAll(copyPartTasks).ConfigureAwait(false);

                    var completeRequest = new CompleteMultipartUploadRequest
                    {
                        BucketName = initiateRequest.BucketName,
                        Key = initiateRequest.Key,
                        UploadId = initiateResponse.UploadId
                    };

                    completeRequest.AddPartETags(copyResponses);

                    CompleteMultipartUploadResponse completeResponse = await client.CompleteMultipartUploadAsync(completeRequest, token).ConfigureAwait(false);

                    return completeResponse.HttpStatusCode;
                }
                catch (AmazonS3Exception ex)
                {
                    await Log.ExceptionAsync(ex).ConfigureAwait(false);
                }
            }

            return HttpStatusCode.NotImplemented;
        }

        private static IEnumerable<CopyPartRequest> CreateCopyPartRequests(InitiateMultipartUploadRequest uploadRequest, string uploadId, Int64 size)
        {
            var requests = new List<CopyPartRequest>();

            Int64 lastByte = 0L;
            
            // 2^20 => mega, (* 25) => 150 MiB
            Int64 partSize = Convert.ToInt64(Math.Pow(2d, 20d) * 150d);

            Int64 bytePosition = 0L;

            for (int i = 1; bytePosition < size; i++)
            {
                lastByte = (bytePosition + partSize - 1) >= size
                    ? size - 1
                    : bytePosition + partSize - 1;

                CopyPartRequest request = new CopyPartRequest
                {
                    DestinationBucket = uploadRequest.BucketName,
                    DestinationKey = uploadRequest.Key,
                    SourceBucket = uploadRequest.BucketName,
                    SourceKey = uploadRequest.Key,
                    UploadId = uploadId,
                    PartNumber = i,
                    FirstByte = bytePosition,
                    LastByte = lastByte
                };

                requests.Add(request);

                bytePosition += partSize;
            }

            return requests;
        }
    }
}
