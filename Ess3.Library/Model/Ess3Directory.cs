using System.Collections.Generic;
using System.Collections.ObjectModel;
using Amazon.S3.Model;

namespace Ess3.Library.Model
{
    public class Ess3Directory : Ess3Object
    {
        private ObservableCollection<Ess3Directory> _directories = new ObservableCollection<Ess3Directory>();
        public IReadOnlyCollection<Ess3Directory> Directories => _directories;

        private ObservableCollection<Ess3File> _files = new ObservableCollection<Ess3File>();
        public IReadOnlyCollection<Ess3File> Files => _files;

        public Ess3Directory(S3Object s3Object)
            : base(s3Object) { }

        public void Add(Ess3Object ess3Object)
        {
            switch (ess3Object)
            {
                case Ess3Directory directory:
                    AddDirectory(directory);
                    break;
                case Ess3File file:
                    AddFile(file);
                    break;
                default:
                    break;
            }
        }

        public void AddDirectory(Ess3Directory directory)
        {
            if (!_directories.Contains(directory))
            {
                _directories.Add(directory);
            }
        }

        public void AddFile(Ess3File file)
        {
            if (!_files.Contains(file))
            {
                _files.Add(file);
            }
        }

        public void Clear()
        {
            _directories.Clear();
            _files.Clear();
        }
    }
}
