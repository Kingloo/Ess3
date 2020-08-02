using System.Threading.Tasks;

namespace Ess3.Gui.Interfaces
{
    public interface IAddAccountWindowViewModel
    {
        string AWSAccessKey { get; set; }
        string AWSSecretKey { get; set; }

        Task<bool> ValidateAsync(string awsAccessKey, string awsSecretKey);
    }
}
