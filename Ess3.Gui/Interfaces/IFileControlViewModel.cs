using Ess3.Library.Interfaces;
using Ess3.Library.Model;

namespace Ess3.Gui.Interfaces
{
    public interface IFileControlViewModel
    {
        IAccount? Account { get; set; }
        Ess3Bucket? SelectedBucket { get; set; }
    }
}
