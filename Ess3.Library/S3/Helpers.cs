﻿using System;
using System.Collections.Generic;
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
using Ess3.Library.Model;

namespace Ess3.Library.S3
{
    public static class Helpers
    {
        public static Task<bool> ValidateAccountAsync(IAccount account, RegionEndpoint regionEndpoint)
            => ValidateAccountAsync(account, regionEndpoint, CancellationToken.None);

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
                    ListBucketsRequest request = new ListBucketsRequest();

                    var response = await RunWithoutCatchAsync<ListBucketsRequest, ListBucketsResponse>(client.ListBucketsAsync, request, token).ConfigureAwait(false);

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

        public static Task UpdateBucketAsync(IAccount account, Ess3Bucket bucket)
            => UpdateBucketAsync(account, bucket, CancellationToken.None);

        public static async Task UpdateBucketAsync(IAccount account, Ess3Bucket bucket, CancellationToken token)
        {
            ListObjectsV2Response? response = await List.ObjectsAsync(account, bucket, token).ConfigureAwait(true); // ConfigAwait must remain true

            if (response is null) { return; }
            if (response.HttpStatusCode != HttpStatusCode.OK) { return; }

            var ess3Objects = response.S3Objects.Select(o => Ess3Factory.Create(o)).ToList();

            var files = ess3Objects.Where(o => o is Ess3File).Cast<Ess3File>();
            var directories = ess3Objects.Where(o => o is Ess3Directory).Cast<Ess3Directory>();

            PutEveryDirectoryIntoParentDirectory(directories);
            PutEveryFileIntoParentDirectory(files, directories);

            // adding dirs then files in this way "sorts" the dirs first
            // this means we don't need to sort in the GUI
            foreach (Ess3Directory each in directories.Where(d => Ess3Factory.IsBucketLevel(d)))
            {
                bucket.Add(each);
            }

            foreach (Ess3File each in files.Where(f => Ess3Factory.IsBucketLevel(f)))
            {
                bucket.Add(each);
            }
        }

        private static void PutEveryDirectoryIntoParentDirectory(IEnumerable<Ess3Directory> directories)
        {
            foreach (Ess3Directory each in directories)
            {
                var immediateSubdirectories = directories.Where(d => d.Prefix == each.Key);

                foreach (Ess3Directory subdirectory in immediateSubdirectories)
                {
                    //if (!each.Directories.Contains(subdirectory))
                    //{
                    //    each.AddDirectory(subdirectory);
                    //}

                    if (!each.Ess3Objects.Contains(subdirectory))
                    {
                        each.Add(subdirectory);
                    }
                }
            }
        }

        private static void PutEveryFileIntoParentDirectory(IEnumerable<Ess3File> files, IEnumerable<Ess3Directory> directories)
        {
            foreach (Ess3Directory each in directories)
            {
                var immediateSubfiles = files.Where(f => f.Prefix == each.Key);

                foreach (Ess3File file in immediateSubfiles)
                {
                    //if (!each.Files.Contains(file))
                    //{
                    //    each.AddFile(file);
                    //}

                    if (!each.Ess3Objects.Contains(file))
                    {
                        each.Add(file);
                    }
                }
            }
        }

        internal static async Task<TResponse> RunWithoutCatchAsync<TRequest, TResponse>(
            Func<TRequest, CancellationToken, Task<TResponse>> s3Call,
            TRequest request,
            CancellationToken token)
            where TRequest : AmazonWebServiceRequest
            where TResponse : AmazonWebServiceResponse
        {
            return await s3Call.Invoke(request, token).ConfigureAwait(false);
        }

        internal static async Task<TResponse?> RunWithCatchAsync<TRequest, TResponse>(
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

        internal static async Task<TResponse?> RunWithCatchAndLogAsync<TRequest, TResponse>(
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
