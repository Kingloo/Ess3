using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public class Ess3Directory : Ess3Object
    {
        private readonly ObservableCollection<Ess3Object> _ess3Objects = new ObservableCollection<Ess3Object>();
        public IReadOnlyCollection<Ess3Object> Ess3Objects => _ess3Objects;

        public override long Size => _ess3Objects.Sum(o => o.Size);

        public Ess3Directory()
            : base()
        { }

        public Ess3Directory(S3Object s3Object)
            : base(s3Object) { }

        public void Add(Ess3Object ess3Object)
        {
            if (!_ess3Objects.Contains(ess3Object))
            {
                _ess3Objects.Add(ess3Object);
            }
        }

        public void Clear()
        {
            _ess3Objects.Clear();
        }
    }
}
