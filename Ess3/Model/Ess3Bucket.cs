using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Amazon.S3.Model;
using Ess3.Extensions;

namespace Ess3.Model
{
    public class Ess3Bucket : S3Bucket
    {
        #region Properties
        private long _totalSize = 0L;
        public long TotalSize
        {
            get => _totalSize;
            set => _totalSize = value;
        }

        private readonly ObservableCollection<Ess3Object> _ess3Objects
            = new ObservableCollection<Ess3Object>();
        public IReadOnlyCollection<Ess3Object> Ess3Objects => _ess3Objects;
        #endregion

        public Ess3Bucket(S3Bucket s3Bucket)
        {
            if (s3Bucket == null) { throw new ArgumentNullException(nameof(s3Bucket)); }

            BucketName = s3Bucket.BucketName;
            CreationDate = s3Bucket.CreationDate;
        }

        public void AssembleObjectsIntoTree(IEnumerable<Ess3Object> objects)
        {
            ClearData();

            // these break without .ToList()
            var allDirectories = objects
                .Where(x => x is Ess3Directory)
                .Select(x => (Ess3Directory)x)
                .ToList();

            var allFiles = objects
                .Where(x => x is Ess3File)
                .Select(x => (Ess3File)x)
                .ToList();

            // retaining this order sorts directories before files in the TreeView
            PutEveryDirectoryIntoDirectory(allDirectories);
            PutEveryFileIntoDirectory(allDirectories, allFiles);
            
            _ess3Objects.AddMissing(allDirectories.Where(x => x.IsAtBucketRoot));
            _ess3Objects.AddMissing(allFiles.Where(x => x.IsAtBucketRoot));

            TotalSize = objects.Sum(x => x.Size);
        }

        private static void PutEveryDirectoryIntoDirectory(IEnumerable<Ess3Directory> allDirectories)
        {
            foreach (Ess3Directory eachDir in allDirectories)
            {
                eachDir.Ess3Objects.AddMissing(allDirectories.Where(x => x.Prefix.Equals(eachDir.Key)));
            }
        }

        private static void PutEveryFileIntoDirectory(IEnumerable<Ess3Directory> allDirectories, IEnumerable<Ess3File> allFiles)
        {
            foreach (Ess3Directory eachDir in allDirectories)
            {
                eachDir.Ess3Objects.AddMissing(allFiles.Where(x => x.Prefix.Equals(eachDir.Key)));
            }
        }
        
        public void ClearData() => _ess3Objects.Clear();

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
            sb.AppendLine($"Name: {BucketName}");
            sb.AppendLine($"Created: {CreationDate}");
            sb.AppendLine($"This bucket has {Ess3Objects.Count} objects");

            return sb.ToString();
        }
    }
}
