using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using Amazon.S3.Model;

namespace Ess3.Model
{
    public sealed class Ess3Directory : Ess3Object
    {
        public ObservableCollection<Ess3Object> Ess3Objects { get; } = new ObservableCollection<Ess3Object>();

        public Ess3Directory(S3Object s3Object, Ess3Bucket ess3Bucket)
            : base(s3Object, ess3Bucket, true)
        {
            Prefix = GetPrefix(s3Object.Key);
        }

        public override string GetPrefix(string p)
        {
            Char slash = Char.Parse(@"/");

            int slashCount = p.Count(c => c == slash);

            if (slashCount == 1) // only one slash means bucket root "directory"
            {
                return string.Empty;
            }
            else
            {
                p = p.TrimEnd(slash);

                int indexOfLastSlash = p.LastIndexOf(slash);

                return p.Substring(0, indexOfLastSlash + 1);
            }
        }

        public void SetSize(Int64 size) => Size = size;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine(GetType().Name);
            sb.Append(base.ToString());
            sb.AppendLine(string.Format(CultureInfo.CurrentCulture, "Ess3Objects: {0}", Ess3Objects.Count));

            return sb.ToString();
        }
    }
}
