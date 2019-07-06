using System;
using System.Globalization;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;
using Ess3.Common;

namespace Ess3.Model
{
    public abstract class Ess3Object : BindableBase, IEquatable<Ess3Object>, IComparable<Ess3Object>
    {
        #region Properties
        private readonly Ess3Bucket _bucket = null;
        public Ess3Bucket Bucket => _bucket;

        private readonly string _key = string.Empty;
        public string Key => _key;

        private string _prefix = string.Empty;
        public string Prefix
        {
            get => _prefix;
            set => _prefix = value;
        }

        private bool _isDirectory = false;
        public bool IsDirectory => _isDirectory;

        public bool IsAtBucketRoot
            => String.IsNullOrEmpty(Prefix);

        private readonly string _etag = string.Empty;
        public string Etag => _etag;

        private readonly DateTime _lastModified = DateTime.MinValue;
        public DateTime LastModified => _lastModified;

        private readonly Owner _owner = null;
        public Owner Owner => _owner;

        private readonly long _size = 0L;
        public long Size => _size;

        private readonly S3StorageClass _storageClass = S3StorageClass.Standard;
        public S3StorageClass StorageClass => _storageClass;

        private S3AccessControlList _acl = null;
        public S3AccessControlList ACL
        {
            get
            {
                return _acl;
            }
            set
            {
                _acl = value;

                RaisePropertyChanged(nameof(ACL));
            }
        }
        #endregion

        protected Ess3Object(S3Object s3Object, Ess3Bucket ess3Bucket, bool isDirectory)
        {
            _etag = s3Object.ETag;
            _key = s3Object.Key;
            _lastModified = s3Object.LastModified;
            _owner = s3Object.Owner;
            _size = s3Object.Size;
            _storageClass = s3Object.StorageClass;

            _isDirectory = isDirectory;

            _bucket = ess3Bucket;
        }

        public abstract string GetPrefix(string p);
        
        public bool Equals(Ess3Object other)
        {
            if (other == null) { return false; }

            if (IsDirectory != other.IsDirectory)
            {
                return false;
            }

            if (!Prefix.Equals(other.Prefix))
            {
                return false;
            }

            if (!Bucket.BucketName.Equals(other.Bucket.BucketName))
            {
                return false;
            }

            if (!Etag.Equals(other.Etag))
            {
                return false;
            }

            if (!Key.Equals(other.Key))
            {
                return false;
            }

            if (!LastModified.Equals(other.LastModified))
            {
                return false;
            }

            if (!Owner.Equals(other.Owner))
            {
                return false;
            }

            if (Size != other.Size)
            {
                return false;
            }

            if (StorageClass != other.StorageClass)
            {
                return false;
            }

            return true;
        }

        public int CompareTo(Ess3Object other)
        {
            if (other == null) { throw new ArgumentNullException(nameof(other)); }

            Type t = other.GetType();

            if (t == typeof(Ess3Directory))
            {
                return 1;
            }
            else if (t == typeof(Ess3File))
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            var cc = CultureInfo.CurrentCulture;

            sb.AppendLine(string.Format(cc, "Bucket name: {0}", Bucket.BucketName));
            sb.AppendLine(string.Format(cc, "Key: {0}", Key));
            sb.AppendLine(string.Format(cc, "Prefix: {0}", Prefix));
            sb.AppendLine(string.Format(cc, "IsDirectory: {0}", IsDirectory));
            sb.AppendLine(string.Format(cc, "IsAtBucketRoot: {0}", IsAtBucketRoot));
            sb.AppendLine(string.Format(cc, "Etag: {0}", Etag));
            sb.AppendLine(string.Format(cc, "LastModified: {0}", LastModified));
            sb.AppendLine(string.Format(cc, "Owner: {0}", Owner));
            sb.AppendLine(string.Format(cc, "Size: {0}", Size));
            sb.AppendLine(string.Format(cc, "Storage class: {0}", StorageClass.Value));

            return sb.ToString();
        }
    }
}
