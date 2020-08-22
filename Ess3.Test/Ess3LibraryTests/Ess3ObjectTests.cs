using Ess3.Library.Model;
using NUnit.Framework;

namespace Ess3.Test.Ess3LibraryTests
{
    [TestFixture]
    public class Ess3ObjectTests
    {
        [TestCase("fred/", "")]
        [TestCase("fred/james/", "fred/")]
        [TestCase("fred/james/mary/albert/claudia", "fred/james/mary/albert/")]
        public void SetPrefixCorrectly_ForDirectories(string key, string prefix)
        {
            var directory = new Ess3Directory
            {
                Key = key
            };

            Assert.AreEqual(prefix, directory.Prefix, "directory's prefix: {0}, correct prefix: {1}", directory.Prefix, prefix);
        }

        [TestCase("fred", "")]
        [TestCase("fred.jpg", "")]
        [TestCase("fred/james.jpg", "fred/")]
        [TestCase("fred/james/mary.jpg", "fred/james/")]
        [TestCase("fred/james/mary", "fred/james/")]
        public void SetPrefixCorrectly_ForFiles(string key, string prefix)
        {
            var file = new Ess3File
            {
                Key = key
            };

            Assert.AreEqual(prefix, file.Prefix, "file's prefix: {0}, correct prefix: {1}", file.Prefix, prefix);
        }
    }
}
