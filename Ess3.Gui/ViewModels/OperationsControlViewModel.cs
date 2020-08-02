using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Ess3.Gui.Interfaces;
using Ess3.Library;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.ViewModels
{
    public class OperationsControlViewModel : IOperationsControlViewModel
    {
        private readonly ObservableCollection<IOperation> _operations = new ObservableCollection<IOperation>();
        public IReadOnlyCollection<IOperation> Operations => _operations;

        public OperationsControlViewModel()
        {
            //AddFakeData();
        }

        private void AddFakeData()
        {
            _operations.Add(new Operation { OperationType = OperationType.Upload });
            _operations.Add(new Operation { OperationType = OperationType.Download });
            _operations.Add(new Operation { OperationType = OperationType.SetStorageClass });
            _operations.Add(new Operation { OperationType = OperationType.Delete });
        }

        public void Add(IOperation operation)
        {
            _operations.Add(operation);
        }

        public void Clear()
        {
            foreach (IOperation each in _operations)
            {
                each.Cancel();
            }

            _operations.Clear();
        }
    }
}
