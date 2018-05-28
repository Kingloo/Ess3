using System;
using System.Text;
using Amazon.S3.Model;

namespace Ess3.Model
{
    public sealed class Ess3File : Ess3Object, IEquatable<Ess3File>
    {
        public Ess3File(S3Object s3Object, Ess3Bucket ess3Bucket)
            : base(s3Object, ess3Bucket, false)
        {
            Prefix = GetPrefix(s3Object.Key);
        }

        public override string GetPrefix(string p)
        {
            if (String.IsNullOrWhiteSpace(p)) { throw new ArgumentNullException(nameof(p)); }

            int indexOfLastSlash = p.LastIndexOf(@"/");

            if (indexOfLastSlash > 0)
            {
                string raw = p.Substring(0, indexOfLastSlash);

                return raw.EndsWith(@"/") ? raw : $"{raw}/";
            }
            
            return string.Empty;
        }

        public bool Equals(Ess3File other)
        {
            if (other == null) { return false; }

            return Key.Equals(other.Key);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
            sb.Append(base.ToString());

            return sb.ToString();
        }
    }
}
