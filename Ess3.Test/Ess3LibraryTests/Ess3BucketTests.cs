using System;
using Amazon.S3.Model;
using Ess3.Library.Model;
using NUnit.Framework;

namespace Ess3.Test.Ess3LibraryTests
{
    [TestFixture]
    public class Ess3BucketTests
    {
        [Test]
        public void BucketSize_Calculation()
        {
            Random random = new Random(Environment.TickCount % 17);

            Int64 fileSize1 = (Int64)random.Next(10, Int32.MaxValue - 1);
            Int64 fileSize2 = (Int64)random.Next(10, Int32.MaxValue - 1);
            Int64 fileSize3 = (Int64)random.Next(10, Int32.MaxValue - 1);
            Int64 fileSize4 = (Int64)random.Next(10, Int32.MaxValue - 1);

            var file1 = new Ess3File(new S3Object { Size = fileSize1, Key = "file1" });
            var file2 = new Ess3File(new S3Object { Size = fileSize2, Key = "file2" });
            var file3 = new Ess3File(new S3Object { Size = fileSize3, Key = "file3" });
            var file4 = new Ess3File(new S3Object { Size = fileSize4, Key = "file4" });

            var directory1 = new Ess3Directory { Key = "dir1" };
            var directory2 = new Ess3Directory { Key = "dir2" };
            var directory3 = new Ess3Directory { Key = "dir3" };

            directory1.Add(file1);
            directory2.Add(file2);
            directory3.Add(file3);

            directory2.Add(directory3);
            directory1.Add(directory2);

            var bucket = new Ess3Bucket("bucket", DateTime.Now);

            bucket.Add(directory1);
            bucket.Add(file4);

            Int64 expected = fileSize1 + fileSize2 + fileSize3 + fileSize4;
            Int64 actual = bucket.Size;

            Assert.AreEqual(expected, actual, "bucket has {0} objects", bucket.Ess3Objects.Count);
        }
    }
}
