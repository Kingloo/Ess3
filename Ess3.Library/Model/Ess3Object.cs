using System;
using Amazon.S3;
using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public abstract class Ess3Object : IEquatable<Ess3Object>
    {
        private readonly S3Object? original;

        public string ETag { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public DateTime LastModified { get; set; } = DateTime.MinValue;
        public Owner Owner { get; set; } = new Owner { DisplayName = "uninitialized display name", Id = "uninitialized id" };
        public Int64 Size { get; set; } = 0L;
        public S3StorageClass StorageClass { get; set; } = S3StorageClass.Standard;

        protected Ess3Object() { }

        protected Ess3Object(S3Object s3Object)
        {
            original = s3Object;

            ETag = s3Object.ETag;
            BucketName = s3Object.BucketName;
            Key = s3Object.Key;
            LastModified = s3Object.LastModified;
            Owner = s3Object.Owner;
            Size = s3Object.Size;
            StorageClass = s3Object.StorageClass;
        }

        public bool Equals(Ess3Object other)
            => EqualsInternal(this, other);

        public override bool Equals(object obj)
            => (obj is Ess3Object ess3Object) && EqualsInternal(this, ess3Object);

        public static bool operator ==(Ess3Object lhs, Ess3Object rhs)
            => EqualsInternal(lhs, rhs);

        public static bool operator !=(Ess3Object lhs, Ess3Object rhs)
            => EqualsInternal(lhs, rhs);

        private static bool EqualsInternal(Ess3Object thisOne, Ess3Object otherOne)
            => thisOne.Key.Equals(otherOne.Key, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => original?.GetHashCode() ?? Key.GetHashCode();
    }
}
