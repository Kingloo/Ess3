using Ess3.Gui.Common;
using Ess3.Gui.Interfaces;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.ViewModels
{
    public class FileControlViewModel : BindableBase, IFileControlViewModel
    {
        private IAccount? _account = null;
        public IAccount? Account
        {
            get => _account;
            set => SetProperty(ref _account, value, nameof(Account));
        }

        public FileControlViewModel() { }
    }
}
