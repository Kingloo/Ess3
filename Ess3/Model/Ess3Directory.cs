using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using Amazon.S3.Model;

namespace Ess3.Model
{
    public sealed class Ess3Directory : Ess3Object
    {
        #region Properties
        public ObservableCollection<Ess3Object> Ess3Objects { get; } = new ObservableCollection<Ess3Object>();
        #endregion

        public Ess3Directory(S3Object s3Object, Ess3Bucket ess3Bucket)
            : base(s3Object, ess3Bucket, true)
        {
            Prefix = GetPrefix(s3Object.Key);
        }

        public override string GetPrefix(string p)
        {
            int slashCount = 0;

            foreach (char c in p)
            {
                if (c.ToString().Equals(@"/"))
                {
                    slashCount++;
                }
            }

            if (slashCount < 2) // only one slash means bucket root directory
            {
                return string.Empty;
            }
            else
            {
                string keyWithoutTrailingSlash = p.Substring(0, p.Length - 1);

                int indexOfLastSlash = keyWithoutTrailingSlash.LastIndexOf(@"/");

                return p.Substring(0, indexOfLastSlash + 1);
            }
        }

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
