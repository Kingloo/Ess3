using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public class Ess3File : Ess3Object
    {
        public Ess3File() { }

        public Ess3File(S3Object s3Object)
            : base(s3Object)
        { }
    }
}
