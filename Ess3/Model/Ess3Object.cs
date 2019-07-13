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
        public Ess3Bucket Bucket { get; } = null;
        public string Key { get; } = string.Empty;
        public string Prefix { get; protected set; }
        public bool IsDirectory { get; } = false;
        public bool IsAtBucketRoot => String.IsNullOrEmpty(Prefix);
        public string Etag { get; } = string.Empty;
        public DateTime LastModified { get; } = DateTime.MinValue;
        public Owner Owner { get; } = null;
        public Int64 Size { get; protected set; } = 0L;
        public S3StorageClass StorageClass { get; } = S3StorageClass.Standard;
        public S3AccessControlList ACL { get; } = null;
        #endregion

        protected Ess3Object(S3Object s3Object, Ess3Bucket ess3Bucket, bool isDirectory)
        {
            if (s3Object is null) { throw new ArgumentNullException(nameof(s3Object)); }
            if (ess3Bucket is null) { throw new ArgumentNullException(nameof(ess3Bucket)); }

            Etag = s3Object.ETag;
            Key = s3Object.Key;
            LastModified = s3Object.LastModified;
            Owner = s3Object.Owner;
            Size = s3Object.Size;
            StorageClass = s3Object.StorageClass;

            IsDirectory = isDirectory;

            Bucket = ess3Bucket;
        }

        public abstract string GetPrefix(string p);
        
        public bool Equals(Ess3Object other)
        {
            if (other is null) { return false; }

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
            if (other is null) { throw new ArgumentNullException(nameof(other)); }

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
