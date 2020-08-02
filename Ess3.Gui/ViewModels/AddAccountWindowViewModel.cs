using System;
using System.Threading.Tasks;
using Ess3.Gui.Interfaces;

namespace Ess3.Gui.ViewModels
{
    public class AddAccountWindowViewModel : IAddAccountWindowViewModel
    {
        public string AWSAccessKey { get; set; } = string.Empty;
        public string AWSSecretKey { get; set; } = string.Empty;

        public AddAccountWindowViewModel() { }

        public async Task<bool> ValidateAsync(string awsAccessKey, string awsSecretKey)
        {
            await Task.Delay(TimeSpan.FromSeconds(0.5d)).ConfigureAwait(false);

            return true;
        }
    }
}
