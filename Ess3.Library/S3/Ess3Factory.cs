using System;
using System.Diagnostics;
using Amazon.S3.Model;
using Ess3.Library.Model;

namespace Ess3.Library.S3
{
    public static class Ess3Factory
    {
        public static Ess3Object Create(S3Object s3Object)
            => IsDirectory(s3Object) switch
            {
                true => new Ess3Directory(s3Object),
                false => new Ess3File(s3Object)
            };

        private static bool IsDirectory(S3Object s3Object)
            => s3Object.Size == 0L
            && s3Object.Key.EndsWith("/", StringComparison.OrdinalIgnoreCase);

        public static bool IsBucketLevel(Ess3Object ess3Object)
        {
            bool isAtBucketLevel = String.IsNullOrEmpty(ess3Object.Prefix);

            Debug.WriteLine($"{ess3Object.Key} - bucket?: {isAtBucketLevel}");

            return isAtBucketLevel;
        }
    }
}
