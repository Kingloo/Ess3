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

        //private static Ess3Directory CreateDirectory(S3Object s3Object)
        //    => new Ess3Directory(s3Object);

        //private static Ess3File CreateFile(S3Object s3Object)
        //    => new Ess3File(s3Object);
    }
}
