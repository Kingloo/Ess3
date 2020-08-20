using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amazon.S3.Model;
using Ess3.Library.Common;

namespace Ess3.Library.Model
{
    public class Ess3Bucket : BindableBase
    {
        public string BucketName { get; } = string.Empty;
        public DateTime CreationDate { get; } = DateTime.Now;

        public Int64 Size => _directories.Sum(d => d.Size) + _files.Sum(f => f.Size);

        private readonly ObservableCollection<Ess3Directory> _directories = new ObservableCollection<Ess3Directory>();
        public IReadOnlyCollection<Ess3Directory> Directories => _directories;

        private readonly ObservableCollection<Ess3File> _files = new ObservableCollection<Ess3File>();
        public IReadOnlyCollection<Ess3File> Files => _files;

        public Ess3Bucket(S3Bucket bucket)
            : this(bucket.BucketName, bucket.CreationDate)
        { }

        public Ess3Bucket(string bucketName)
            : this(bucketName, DateTime.Now)
        { }

        public Ess3Bucket(string bucketName, DateTime creationDate)
        {
            if (String.IsNullOrEmpty(bucketName))
            {
                throw new ArgumentNullException(nameof(bucketName));
            }

            BucketName = bucketName;
            CreationDate = creationDate;
        }

        public void AddDirectory(Ess3Directory directory)
        {
            if (!_directories.Contains(directory))
            {
                _directories.Add(directory);
            }
        }

        public void RemoveDirectory(Ess3Directory directory)
        {
            if (_directories.Contains(directory))
            {
                _directories.Remove(directory);
            }
        }

        public void ClearDirectories()
        {
            _directories.Clear();

            RaisePropertyChanged(nameof(Size));
        }

        public void AddFile(Ess3File file)
        {
            if (!_files.Contains(file))
            {
                _files.Add(file);

                RaisePropertyChanged(nameof(Size));
            }
        }

        public void RemoveFile(Ess3File file)
        {
            if (_files.Contains(file))
            {
                _files.Remove(file);

                RaisePropertyChanged(nameof(Size));
            }
        }

        public void ClearFiles()
        {
            _files.Clear();

            RaisePropertyChanged(nameof(Size));
        }

        public void ClearAll()
        {
            ClearDirectories();
            ClearFiles();
        }
    }
}
