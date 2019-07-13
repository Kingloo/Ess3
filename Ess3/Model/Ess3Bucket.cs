using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Amazon.S3.Model;
using Ess3.Extensions;

namespace Ess3.Model
{
    public class Ess3Bucket : S3Bucket
    {
        #region Properties
        public Int64 TotalSize { get; set; } = 0L;

        private readonly ObservableCollection<Ess3Object> _ess3Objects = new ObservableCollection<Ess3Object>();
        public IReadOnlyCollection<Ess3Object> Ess3Objects => _ess3Objects;
        #endregion

        public Ess3Bucket(S3Bucket s3Bucket)
        {
            if (s3Bucket is null) { throw new ArgumentNullException(nameof(s3Bucket)); }

            BucketName = s3Bucket.BucketName;
            CreationDate = s3Bucket.CreationDate;
        }

        public void AssembleObjectsIntoTree(IEnumerable<Ess3Object> objects)
        {
            _ess3Objects.Clear();

            var allDirectories = objects.Where(x => x.IsDirectory).Cast<Ess3Directory>().ToList();
            var allFiles = objects.Where(x => !x.IsDirectory).Cast<Ess3File>().ToList();

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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            var cc = CultureInfo.CurrentCulture;

            sb.AppendLine(GetType().Name);
            sb.AppendLine(string.Format(cc, "Name: {0}", BucketName));
            sb.AppendLine(string.Format(cc, "Created: {0}", CreationDate));
            sb.AppendLine(string.Format(cc, "This bucket has {0} objects", Ess3Objects.Count));

            return sb.ToString();
        }
    }
}
