using System;
using Amazon.S3.Model;
using Ess3.Library.Model;

namespace Ess3.Library.S3
{
    internal static class Ess3Factory
    {
        internal static Ess3Object Create(S3Object s3Object)
            => IsDirectory(s3Object) switch
            {
                true => new Ess3Directory(s3Object),
                false => new Ess3File(s3Object)
            };

        private static bool IsDirectory(S3Object s3Object)
            => s3Object.Size == 0L
            && s3Object.Key.EndsWith("/", StringComparison.OrdinalIgnoreCase);

        internal static bool IsBucketLevel(S3Object s3Object)
        {
            return true;
        }

        internal static bool IsBucketLevel(Ess3Object ess3Object)
        {
            return true;
        }
    }
}
