using Amazon.S3.Model;
using Ess3.Library.Model;
using Ess3.Library.S3;
using NUnit.Framework;

namespace Ess3.Test.Ess3LibraryTests
{
    [TestFixture]
    public class Ess3FactoryTests
    {
        [Test]
        public void Create_Directory_ReturnsDirectory()
        {
            var s3 = new S3Object
            {
                Key = "fred/",
                Size = 0L
            };

            var ess3 = Ess3Factory.Create(s3);

            Assert.IsInstanceOf<Ess3Directory>(ess3);
        }

        [Test]
        public void Create_File_ReturnsFile()
        {
            var s3 = new S3Object
            {
                Key = "fred",
                Size = 1L
            };

            var ess3 = Ess3Factory.Create(s3);

            Assert.IsInstanceOf<Ess3File>(ess3);
        }

        [Test]
        public void WhenIsBucketLevel_ReturnsTrue()
        {
            var ess3 = new Ess3File { Key = "fred/" };

            bool expected = true;
            bool actual = Ess3Factory.IsBucketLevel(ess3);

            Assert.AreEqual(expected, actual, "key: {0}, prefix: {1}", ess3.Key, ess3.Prefix);
        }

        [Test]
        public void WhenIsNotBucketLevel_ReturnsFalse()
        {
            var ess3 = new Ess3File { Key = "fred/james.jpg" };

            bool expected = false;
            bool actual = Ess3Factory.IsBucketLevel(ess3);

            Assert.AreEqual(expected, actual, "key: {0}, prefix: {1}", ess3.Key, ess3.Prefix);
        }
    }
}
