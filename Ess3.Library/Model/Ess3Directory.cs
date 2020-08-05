using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public class Ess3Directory : Ess3Object
    {
        public Ess3Directory(S3Object s3Object)
            : base(s3Object) { }
    }
}
