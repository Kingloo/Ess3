using System;
using Ess3.Library.Model;
using NUnit.Framework;

namespace Ess3.Test.Ess3LibraryTests
{
    [TestFixture]
    public class Ess3DirectoryTests
    {
        private static readonly string exampleKey = "fred/james.jpg";

        [Test]
        public void Add_AddsWhenDoesNotAlreadyContain()
        {
            var directory = new Ess3Directory();
            
            var file = new Ess3File
            {
                Key = exampleKey
            };

            directory.Add(file);

            int expected = 1;
            int actual = directory.Ess3Objects.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Add_DoesNotAddWhenAlreadyContains()
        {
            var directory = new Ess3Directory();

            var file1 = new Ess3File
            {
                Key = exampleKey
            };

            var file2 = new Ess3File
            {
                Key = exampleKey
            };

            directory.Add(file1);
            directory.Add(file2);

            int expected = 1;
            int actual = directory.Ess3Objects.Count;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Clear_RemovesAllObjects()
        {
            var directory = new Ess3Directory();

            var file1 = new Ess3File
            {
                Key = exampleKey
            };

            var file2 = new Ess3File
            {
                Key = exampleKey + "blabla"
            };

            directory.Add(file1);
            directory.Add(file2);

            if (directory.Ess3Objects.Count != 2)
            {
                throw new Exception($"there were supposed to be 2 objects, there were actually {directory.Ess3Objects.Count}");
            }

            directory.Clear();

            int expected = 0;
            int actual = directory.Ess3Objects.Count;

            Assert.AreEqual(expected, actual);
        }
    }
}
