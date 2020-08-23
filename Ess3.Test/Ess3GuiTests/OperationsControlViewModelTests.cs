using Ess3.Gui.Interfaces;
using Ess3.Gui.ViewModels;
using NUnit.Framework;

namespace Ess3.Test.Ess3GuiTests
{
    [TestFixture]
    public class OperationsControlViewModelTests
    {
        [Test]
        public void Operations_ShouldStartEmpty()
        {
            IOperationsControlViewModel vm = new OperationsControlViewModel();

            Assert.Zero(vm.Operations.Count, "expected: {0}, actual: {1}", 0, vm.Operations.Count);
        }

        //[Test]
        //public void Add_ShouldAddOperation()
        //{
        //    IOperationsControlViewModel vm = new OperationsControlViewModel();

        //    vm.Add(new Operation { OperationType = OperationType.Delete });

        //    int expected = 1;
        //    int actual = vm.Operations.Count;

        //    Assert.AreEqual(expected, actual);
        //}

        //[Test]
        //public void Clear_ShouldEmptyOperations()
        //{
        //    IOperationsControlViewModel vm = new OperationsControlViewModel();

        //    vm.Add(new Operation { OperationType = OperationType.Delete });
        //    vm.Add(new Operation { OperationType = OperationType.Download });
        //    vm.Add(new Operation { OperationType = OperationType.Upload });
        //    vm.Add(new Operation { OperationType = OperationType.SetStorageClass });

        //    if (vm.Operations.Count != 4)
        //    {
        //        throw new Exception($"adding operations failed, count should have been 4, it was {vm.Operations.Count}");
        //    }

        //    vm.Clear();

        //    Assert.Zero(vm.Operations.Count);
        //}
    }
}
