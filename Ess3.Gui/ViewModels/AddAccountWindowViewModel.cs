using System.Threading.Tasks;
using Amazon;
using Ess3.Gui.Common;
using Ess3.Gui.Interfaces;
using Ess3.Library;
using Ess3.Library.Interfaces;
using Ess3.Library.S3;

namespace Ess3.Gui.ViewModels
{
    public class AddAccountWindowViewModel : BindableBase, IAddAccountWindowViewModel
    {
        public IAccount? Account { get; private set; }

        public AddAccountWindowViewModel() { }

        public Task<bool> ValidateAsync(string awsAccessKey, string awsSecretKey, RegionEndpoint regionEndpoint)
        {
            Account = new Account
            {
                AWSAccessKey = awsAccessKey,
                AWSSecretKey = awsSecretKey
            };

            return Helpers.ValidateAccountAsync(Account, regionEndpoint);
        }
    }
}
