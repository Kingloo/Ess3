using System;
using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public class Ess3File : Ess3Object
    {
        private readonly Int64? _size = 0L;
        public override Int64? Size { get => _size; }

        public Ess3File() { }

        public Ess3File(S3Object s3Object)
            : base(s3Object)
        {
            _size = s3Object.Size;
        }
    }
}
