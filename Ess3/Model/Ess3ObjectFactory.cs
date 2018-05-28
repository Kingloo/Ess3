using System;
using Amazon.S3.Model;

namespace Ess3.Model
{
    public static class Ess3ObjectFactory
    {
        public static Ess3Object Create(S3Object s3Object, Ess3Bucket ess3Bucket)
        {
            if (IsDirectory(s3Object))
            {
                return new Ess3Directory(s3Object, ess3Bucket);
            }
            else
            {
                return new Ess3File(s3Object, ess3Bucket);
            }
        }

        private static bool IsDirectory(S3Object s3Object)
            => s3Object.Size == 0L
            && s3Object.Key.EndsWith(@"/", StringComparison.OrdinalIgnoreCase);
    }
}
