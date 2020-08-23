using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amazon;
using Amazon.S3.Model;
using Ess3.Library.Common;

namespace Ess3.Library.Model
{
    public class Ess3Bucket : BindableBase
    {
        public string BucketName { get; } = string.Empty;
        public DateTime CreationDate { get; } = DateTime.MinValue;
        public RegionEndpoint RegionEndpoint { get; set; } = RegionEndpoint.EUWest1;

        public Int64 Size => _ess3Objects.Sum(o => o.Size);

        private readonly ObservableCollection<Ess3Object> _ess3Objects = new ObservableCollection<Ess3Object>();
        public IReadOnlyCollection<Ess3Object> Ess3Objects => _ess3Objects;

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

        public void Add(Ess3Object ess3Object)
        {
            if (!_ess3Objects.Contains(ess3Object))
            {
                _ess3Objects.Add(ess3Object);

                RaisePropertyChanged(nameof(Size));
            }
        }

        public void Clear() => _ess3Objects.Clear();
    }
}
