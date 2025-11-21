using System;
using System.Linq;
using System.Text;
using Amazon.S3;
using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public abstract class Ess3Object : IEquatable<Ess3Object>, IComparable<Ess3Object>
    {
        private readonly S3Object? original;

        public string ETag { get; set; } = string.Empty;
        public string BucketName { get; set; } = string.Empty;
        public string Key { get; set; } = string.Empty;
        public string Prefix => GetPrefix(Key);
        public DateTime? LastModified { get; set; } = DateTime.MinValue;
        public Owner Owner { get; set; } = new Owner { DisplayName = "uninitialized display name", Id = "uninitialized id" };
        public abstract Int64? Size { get; }
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
            StorageClass = s3Object.StorageClass;
        }

        private static string GetPrefix(string key)
        {
            if (String.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "key was null or whitespace");
            }

            Char slash = Char.Parse(@"/");

            if (!key.Contains(slash))
            {
                return string.Empty;
            }

            string[] segments = key.Split(new char[] { slash }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < segments.Length - 1; i++)
            {
                sb.Append(segments[i]);
                sb.Append(slash);
            }

            return sb.ToString();
        }

        public bool Equals(Ess3Object? other)
        {
            return (other is not null) && EqualsInternal(this, other);
        }
            

        public override bool Equals(object? obj)
            => (obj is Ess3Object ess3Object) && EqualsInternal(this, ess3Object);

        public static bool operator ==(Ess3Object lhs, Ess3Object rhs)
            => EqualsInternal(lhs, rhs);

        public static bool operator !=(Ess3Object lhs, Ess3Object rhs)
            => EqualsInternal(lhs, rhs);

        private static bool EqualsInternal(Ess3Object thisOne, Ess3Object otherOne)
            => thisOne.Key.Equals(otherOne.Key, StringComparison.OrdinalIgnoreCase);

        public override int GetHashCode()
            => original?.GetHashCode() ?? Key.GetHashCode();

        public int CompareTo(Ess3Object? other)
        {
            return other switch
            {
                Ess3Directory _ => 1,
                Ess3File _ => -1,
                _ => 0
            };
        }

        public static bool operator <(Ess3Object left, Ess3Object right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(Ess3Object left, Ess3Object right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(Ess3Object left, Ess3Object right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(Ess3Object left, Ess3Object right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
