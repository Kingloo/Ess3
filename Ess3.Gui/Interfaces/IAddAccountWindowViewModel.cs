using System.Threading.Tasks;
using Ess3.Library.Interfaces;

namespace Ess3.Gui.Interfaces
{
    public interface IAddAccountWindowViewModel
    {
        IAccount? Account { get; }

        Task<bool> ValidateAsync(string awsAccessKey, string awsSecretKey);
    }
}
